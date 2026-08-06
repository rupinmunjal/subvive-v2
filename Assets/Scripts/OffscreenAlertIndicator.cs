using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class OffscreenAlertIndicator : MonoBehaviour
{
    [Header("References")]
    public RectTransform arrowIcon;
    public Canvas canvas;

    [Header("Settings")]
    public float edgeMargin = 200f;

    [Header("Stage Sprites")]
    public Sprite stage1Sprite;
    public Sprite stage2Sprite;
    public Sprite stage3Sprite;

    private Camera cam;
    private Transform player;
    private Image arrowImage;

    void Start()
    {
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
        if (arrowIcon != null)
        {
            arrowImage = arrowIcon.GetComponent<Image>();
            arrowIcon.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (arrowIcon == null || canvas == null) return;

        if (player == null)
        {
            player = FindLocalPlayer();
            if (player == null)
            {
                arrowIcon.gameObject.SetActive(false);
                return;
            }
        }

        if (cam == null)
            cam = Camera.main;
        if (cam == null)
        {
            arrowIcon.gameObject.SetActive(false);
            return;
        }

        Transform best = null;
        int bestSeverity = 1;
        float bestDistance = float.MaxValue;

        foreach (var kvp in AlertManager.Instance.GetAll())
        {
            Transform t = kvp.Key;
            if (t == null || IsOnScreen(t.position)) continue;

            float distance = (t.position - player.position).sqrMagnitude;
            if (distance < bestDistance)
            {
                bestDistance = distance;
                best = t;
                bestSeverity = kvp.Value;
            }
        }

        if (best == null)
        {
            arrowIcon.gameObject.SetActive(false);
            return;
        }

        UpdateArrow(best.position, bestSeverity);
    }

    private Transform FindLocalPlayer()
    {
        foreach (var pv in FindObjectsByType<PhotonView>(FindObjectsSortMode.None))
        {
            if (pv.IsMine && pv.GetComponent<PlayerMovement>() != null)
                return pv.transform;
        }
        return null;
    }

    private bool IsOnScreen(Vector3 worldPosition)
    {
        GetScreenOffset(worldPosition, out Vector2 fromCenter, out bool behindCamera);

        float halfWidth = Screen.width * 0.5f - edgeMargin;
        float halfHeight = Screen.height * 0.5f - edgeMargin;

        return !behindCamera &&
               Mathf.Abs(fromCenter.x) < halfWidth &&
               Mathf.Abs(fromCenter.y) < halfHeight;
    }

    private void GetScreenOffset(Vector3 worldPosition, out Vector2 fromCenter, out bool behindCamera)
    {
        Vector3 toTarget = worldPosition - cam.transform.position;
        behindCamera = Vector3.Dot(cam.transform.forward, toTarget) < 0f;

        Vector3 screenPos = cam.WorldToScreenPoint(worldPosition);
        if (behindCamera)
        {
            screenPos.x = Screen.width - screenPos.x;
            screenPos.y = Screen.height - screenPos.y;
        }

        Vector2 screenCenter = new Vector2(Screen.width, Screen.height) * 0.5f;
        fromCenter = (Vector2)screenPos - screenCenter;
    }

    private void UpdateArrow(Vector3 targetPosition, int severity)
    {
        ApplyStageVisual(severity);

        GetScreenOffset(targetPosition, out Vector2 fromCenter, out _);

        Vector2 screenCenter = new Vector2(Screen.width, Screen.height) * 0.5f;
        float halfWidth = screenCenter.x - edgeMargin;
        float halfHeight = screenCenter.y - edgeMargin;

        arrowIcon.gameObject.SetActive(true);

        // clamp the direction to the screen-edge box
        float slope = fromCenter.y / fromCenter.x;
        Vector2 clamped;

        if (fromCenter.x > 0f)
            clamped = new Vector2(halfWidth, halfWidth * slope);
        else
            clamped = new Vector2(-halfWidth, -halfWidth * slope);

        if (clamped.y > halfHeight)
            clamped = new Vector2(halfHeight / slope, halfHeight);
        else if (clamped.y < -halfHeight)
            clamped = new Vector2(-halfHeight / slope, -halfHeight);

        Vector2 clampedScreenPos = clamped + screenCenter;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            clampedScreenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : cam,
            out Vector2 localPoint
        );
        arrowIcon.localPosition = localPoint;

        float angle = Mathf.Atan2(fromCenter.y, fromCenter.x) * Mathf.Rad2Deg;
        arrowIcon.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void ApplyStageVisual(int severity)
    {
        if (arrowImage == null) return;

        switch (Mathf.Clamp(severity, 1, 3))
        {
            case 1:
                if (stage1Sprite != null) arrowImage.sprite = stage1Sprite;
                break;
            case 2:
                if (stage2Sprite != null) arrowImage.sprite = stage2Sprite;
                break;
            case 3:
                if (stage3Sprite != null) arrowImage.sprite = stage3Sprite;
                break;
        }
    }
}
