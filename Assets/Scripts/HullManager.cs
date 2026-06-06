using UnityEngine;

public class HullManager : MonoBehaviour
{
    public static HullManager Instance;

    public float shipHP = 100f;
    public float hpLossPerLeak = 5f;

    private void Awake()
    {
        Instance = this;
    }

    public void TakeDamage(float amount)
    {
        shipHP -= amount;
        shipHP = Mathf.Clamp(shipHP, 0f, 100f);

        if (shipHP <= 0)
        {
            Debug.Log("GAME OVER - Hull destroyed");
        }
    }

    public void RepairHull(float amount)
    {
        shipHP += amount;
        shipHP = Mathf.Clamp(shipHP, 0f, 100f);
    }
}