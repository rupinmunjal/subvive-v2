using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HUDManager : MonoBehaviour
{
    [Header("Font (optional)")]
    public TMP_FontAsset hudFont;

    // ── Tweak drain rates here ────────────────────────────────────────────────
    // ⚠️ TESTING ONLY – These drain rates are used only for the passive test drain
    //    inside Update(). Once you move drain logic to your own SubmarineSystem
    //    (or similar) script, you can delete this entire [Header] block.
    [Header("Drain Rates (per second)")]
    public float oxygenDrain = 1.5f;
    public float fuelDrain   = 0.8f;
    public float powerDrain  = 0.5f;

        // ── Colors ────────────────────────────────────────────────────────────────
    static readonly Color PanelBg       = HexColor("#111820");
    static readonly Color PanelBorder   = HexColor("#4A3E2A");
    static readonly Color BrassGold     = HexColor("#C8A84B");
    static readonly Color LabelColor    = HexColor("#7A6830");
    static readonly Color ValueColor    = HexColor("#C8A84B");
    static readonly Color TrackBg       = HexColor("#0A0E14");
    static readonly Color FillO2        = HexColor("#1A5A8A");
    static readonly Color FillFuel      = HexColor("#3A6A14");
    static readonly Color FillPower     = HexColor("#8A6010");
    static readonly Color FillHull      = HexColor("#8A2010");
    static readonly Color CriticalRed   = HexColor("#C82020");
    static readonly Color WarnText      = HexColor("#C84040");

    // ── Internal refs ─────────────────────────────────────────────────────────
    Image[]    fillImages  = new Image[4];
    TMP_Text[] valueTexts  = new TMP_Text[4];
    TMP_Text   warningText;
    Canvas     hudCanvas;

    readonly string[] labels = { "OXYGEN", "FUEL", "POWER", "HULL" };
    readonly Color[]  fills  = { default, default, default, default };

    // ─────────────────────────────────────────────────────────────────────────
    void Awake()
    {
        fills[0] = FillO2;
        fills[1] = FillFuel;
        fills[2] = FillPower;
        fills[3] = FillHull;

        BuildHUD();
    }

    void Update()
{
    // ============================================================
    //    DELETE THE NEXT 4 LINES when your gameplay scripts 
    //    control the values directly (torpedo hits, leaks, etc.)
    // ============================================================
        SubmarineStats.oxygen = Mathf.Max(0, SubmarineStats.oxygen - oxygenDrain * Time.deltaTime);
        SubmarineStats.fuel   = Mathf.Max(0, SubmarineStats.fuel   - fuelDrain   * Time.deltaTime);
        SubmarineStats.power  = Mathf.Max(0, SubmarineStats.power  - powerDrain  * Time.deltaTime);
    // ============================================================

    UpdateBars();
}

    // ── Build the entire HUD ──────────────────────────────────────────────────
    void BuildHUD()
    {
        // Canvas
        var canvasGO = new GameObject("HUD_Canvas");
        hudCanvas = canvasGO.AddComponent<Canvas>();
        hudCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        hudCanvas.sortingOrder = 10;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();

        // Panel background
        var panel = MakeImage(canvasGO, "HUD_Panel", PanelBg);
        var panelRT = panel.rectTransform;
        panelRT.anchorMin = new Vector2(0, 1);
        panelRT.anchorMax = new Vector2(0, 1);
        panelRT.pivot     = new Vector2(0, 1);
        panelRT.anchoredPosition = new Vector2(20, -20);
        panelRT.sizeDelta = new Vector2(900, 400); // Update panel size here if you want to resize the whole HUD

        // Brass border (slightly larger image behind)
        var border = MakeImage(canvasGO, "HUD_Border", PanelBorder);
        var borderRT = border.rectTransform;
        borderRT.anchorMin = panelRT.anchorMin;
        borderRT.anchorMax = panelRT.anchorMax;
        borderRT.pivot     = panelRT.pivot;
        borderRT.anchoredPosition = new Vector2(18, -18);
        borderRT.sizeDelta = new Vector2(304, 214);
        border.transform.SetSiblingIndex(0); // push behind panel

        // Title text
        var title = MakeText(panel.gameObject, "Title", "◈  SYSTEMS STATUS  ◈", 14, LabelColor);
        var titleRT = title.rectTransform;
        titleRT.anchorMin = new Vector2(0, 1); titleRT.anchorMax = new Vector2(1, 1);
        titleRT.pivot = new Vector2(0.5f, 1);
        titleRT.anchoredPosition = new Vector2(0, -10);
        titleRT.sizeDelta = new Vector2(0, 20);
        title.alignment = TextAlignmentOptions.Center;
        title.characterSpacing = 8f;

        // Rivet corners
        AddRivet(panel.gameObject, new Vector2(10, -10),  new Vector2(0,1));
        AddRivet(panel.gameObject, new Vector2(-10, -10), new Vector2(1,1));
        AddRivet(panel.gameObject, new Vector2(10, 10),   new Vector2(0,0));
        AddRivet(panel.gameObject, new Vector2(-10, 10),  new Vector2(1,0));

        // Meter rows
        float rowStartY = -38f;
        float rowHeight = 38f;

        for (int i = 0; i < 4; i++)
        {
            BuildMeterRow(panel.gameObject, i, rowStartY - i * rowHeight);
        }

        // Separator lines
        for (int i = 0; i < 3; i++)
        {
            float y = rowStartY - (i + 1) * rowHeight + 4f;
            var sep = MakeImage(panel.gameObject, "Sep" + i, HexColor("#1A1A12"));
            sep.rectTransform.anchorMin = new Vector2(0.05f, 0.5f);
            sep.rectTransform.anchorMax = new Vector2(0.95f, 0.5f);
            sep.rectTransform.anchoredPosition = new Vector2(0, y);
            sep.rectTransform.sizeDelta = new Vector2(0, 1);
        }

        // Warning text at bottom
        warningText = MakeText(panel.gameObject, "Warning",
            "⚠  CRITICAL SYSTEM FAILURE  ⚠", 12, WarnText);
        var warnRT = warningText.rectTransform;
        warnRT.anchorMin = new Vector2(0, 0); warnRT.anchorMax = new Vector2(1, 0);
        warnRT.pivot = new Vector2(0.5f, 0);
        warnRT.anchoredPosition = new Vector2(0, 8);
        warnRT.sizeDelta = new Vector2(0, 18);
        warningText.alignment = TextAlignmentOptions.Center;
        warningText.characterSpacing = 4f;
        warningText.gameObject.SetActive(false);

        StartCoroutine(BlinkWarning());
    }

    void BuildMeterRow(GameObject parent, int index, float yPos)
    {
        // Row container
        var row = new GameObject("Row_" + labels[index]);
        row.transform.SetParent(parent.transform, false);
        var rowRT = row.AddComponent<RectTransform>();
        rowRT.anchorMin = new Vector2(0, 1); rowRT.anchorMax = new Vector2(1, 1);
        rowRT.pivot = new Vector2(0.5f, 1);
        rowRT.anchoredPosition = new Vector2(0, yPos);
        rowRT.sizeDelta = new Vector2(-20, 32);

        // Icon circle
        var icon = MakeImage(row, "Icon", HexColor("#1A1408"));
        icon.rectTransform.anchorMin = new Vector2(0, 0.5f);
        icon.rectTransform.anchorMax = new Vector2(0, 0.5f);
        icon.rectTransform.pivot = new Vector2(0, 0.5f);
        icon.rectTransform.anchoredPosition = new Vector2(0, 0);
        icon.rectTransform.sizeDelta = new Vector2(24, 24);

        // Icon border ring (brass)
        var ring = MakeImage(row, "Ring", PanelBorder);
        ring.rectTransform.anchorMin = icon.rectTransform.anchorMin;
        ring.rectTransform.anchorMax = icon.rectTransform.anchorMax;
        ring.rectTransform.pivot = icon.rectTransform.pivot;
        ring.rectTransform.anchoredPosition = new Vector2(-1, 0);
        ring.rectTransform.sizeDelta = new Vector2(26, 26);
        ring.transform.SetSiblingIndex(ring.transform.GetSiblingIndex() - 1);

        // Label
        var label = MakeText(row, "Label", labels[index], 12, LabelColor);
        label.rectTransform.anchorMin = new Vector2(0, 0.5f);
        label.rectTransform.anchorMax = new Vector2(0, 0.5f);
        label.rectTransform.pivot = new Vector2(0, 0.5f);
        label.rectTransform.anchoredPosition = new Vector2(30, 0);
        label.rectTransform.sizeDelta = new Vector2(55, 20);
        label.characterSpacing = 3f;

        // Track background
        var track = MakeImage(row, "Track", TrackBg);
        track.rectTransform.anchorMin = new Vector2(0, 0.5f);
        track.rectTransform.anchorMax = new Vector2(1, 0.5f);
        track.rectTransform.pivot = new Vector2(0, 0.5f);
        track.rectTransform.anchoredPosition = new Vector2(88, 0);
        track.rectTransform.sizeDelta = new Vector2(-130, 12);

        // Track border
        var outline = track.gameObject.AddComponent<Outline>();
        outline.effectColor = HexColor("#2E2514");
        outline.effectDistance = new Vector2(1, -1);

        // Fill bar
        var fillGO = new GameObject("Fill");
        fillGO.transform.SetParent(track.transform, false);
        var fillRT = fillGO.AddComponent<RectTransform>();
        fillRT.anchorMin = new Vector2(0, 0);
        fillRT.anchorMax = new Vector2(1, 1);
        fillRT.offsetMin = Vector2.zero;
        fillRT.offsetMax = Vector2.zero;

        // Mask on track so fill clips
        track.gameObject.AddComponent<Mask>().showMaskGraphic = true;

        var fillImg = fillGO.AddComponent<Image>();
        fillImg.color = fills[index];
        fillImages[index] = fillImg;

        // We drive width via anchorMax.x, so reset to full first
        fillRT.anchorMax = new Vector2(1f, 1f);

        // Value text
        var val = MakeText(row, "Value", "100%", 14, ValueColor);
        val.rectTransform.anchorMin = new Vector2(1, 0.5f);
        val.rectTransform.anchorMax = new Vector2(1, 0.5f);
        val.rectTransform.pivot = new Vector2(1, 0.5f);
        val.rectTransform.anchoredPosition = new Vector2(0, 0);
        val.rectTransform.sizeDelta = new Vector2(38, 20);
        val.alignment = TextAlignmentOptions.Right;
        valueTexts[index] = val;
    }

    // ── Runtime update ────────────────────────────────────────────────────────
    void UpdateBars()
    {
        float[] values = {
            SubmarineStats.oxygen,
            SubmarineStats.fuel,
            SubmarineStats.power,
            SubmarineStats.hull
        };

        bool anyCritical = false;

        for (int i = 0; i < 4; i++)
        {
            float t = Mathf.Clamp01(values[i] / 100f);
            bool critical = t < 0.25f;
            if (critical) anyCritical = true;

            // Shrink fill via anchorMax.x
            var rt = fillImages[i].rectTransform;
            rt.anchorMax = new Vector2(Mathf.Lerp(rt.anchorMax.x, t, Time.deltaTime * 5f), 1f);

            fillImages[i].color = critical
                ? Color.Lerp(CriticalRed, fills[i], t / 0.25f)
                : fills[i];

            valueTexts[i].text = Mathf.RoundToInt(values[i]) + "%";
            valueTexts[i].color = critical ? CriticalRed : ValueColor;
        }

        warningText?.gameObject.SetActive(anyCritical);
    }

    // ── Warning blink coroutine ───────────────────────────────────────────────
    IEnumerator BlinkWarning()
    {
        while (true)
        {
            if (warningText != null && warningText.gameObject.activeSelf)
            {
                var c = warningText.color;
                warningText.color = new Color(c.r, c.g, c.b, c.a < 0.5f ? 1f : 0.2f);
            }
            yield return new WaitForSeconds(0.6f);
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    void AddRivet(GameObject parent, Vector2 pos, Vector2 anchor)
    {
        var r = MakeImage(parent, "Rivet", BrassGold);
        r.rectTransform.anchorMin = anchor;
        r.rectTransform.anchorMax = anchor;
        r.rectTransform.pivot = anchor;
        r.rectTransform.anchoredPosition = pos;
        r.rectTransform.sizeDelta = new Vector2(7, 7);
    }

    Image MakeImage(GameObject parent, string name, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        go.AddComponent<RectTransform>();
        var img = go.AddComponent<Image>();
        img.color = color;
        return img;
    }

    TMP_Text MakeText(GameObject parent, string name, string text, float size, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        go.AddComponent<RectTransform>();
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        if (hudFont != null) tmp.font = hudFont;
        return tmp;
    }

    static Color HexColor(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color c);
        return c;
    }
}