using UnityEngine;
using TMPro;

public class HullUI : MonoBehaviour
{
    public TextMeshProUGUI hpText;

    void Update()
    {
        if (HullManager.Instance != null)
            hpText.text = "Hull: " + Mathf.RoundToInt(HullManager.Instance.shipHP) + "/100";
    }
}