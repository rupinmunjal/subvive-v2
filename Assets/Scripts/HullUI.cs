using UnityEngine;
using TMPro;

public class HullUI : MonoBehaviour
{
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI depthText;
    public TextMeshProUGUI ballastText;
    public TextMeshProUGUI timerText;

    void Update()
    {
        if (HullManager.Instance == null) return;

        hpText.text = "Hull:" + Mathf.RoundToInt(HullManager.Instance.shipHP) + "/100";

        depthText.text = "Depth:" + Mathf.RoundToInt(HullManager.Instance.currentDepth) + "m";

        ballastText.text = "Ballast:" + Mathf.RoundToInt(HullManager.Instance.ballast) + "%";

        float t = HullManager.Instance.currentTime;
        int minutes = Mathf.FloorToInt(t / 60f);
        int seconds = Mathf.FloorToInt(t % 60f);
        timerText.text = "Destination:" + minutes + ":" + seconds.ToString("D2");
    }
}