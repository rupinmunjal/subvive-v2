using UnityEngine;

public class HullManager : MonoBehaviour
{
    public static HullManager Instance;

    [Header("Hull Settings")]
    public float shipHP = 100f;
    public float hpLossPerLeak = 5f;

    [Header("Ballast Settings")]
    public float ballast = 0f; // 0-100%
    public float baseFillRate = 0.5f; // % per second
    public float leakFillRate = 0.3f; // additional % per second per active leak
    public float ballastPumpAmount = 5f; // % per press

    [Header("Depth Settings")]
    public float minDepth = -100f;
    public float maxDepth = -400f;
    public float currentDepth = -100f;
    public float depthFollowSpeed = 2f; // how fast depth follows ballast

    [Header("Destination Settings")]
    public float destinationTime = 600f; // 10 minutes in seconds
    public float currentTime = 600f;

    private int activeLeaks = 0;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        UpdateBallast();
        UpdateDepth();
        UpdateDestinationTimer();
    }

    private void UpdateBallast()
    {
        float fillRate = baseFillRate + (activeLeaks * leakFillRate);
        ballast += fillRate * Time.deltaTime;
        ballast = Mathf.Clamp(ballast, 0f, 100f);
    }

    private void UpdateDepth()
    {
        // depth follows ballast level
        float targetDepth = Mathf.Lerp(minDepth, maxDepth, ballast / 100f);
        currentDepth = Mathf.MoveTowards(currentDepth, targetDepth, depthFollowSpeed * Time.deltaTime);
    }

    private void UpdateDestinationTimer()
    {
        float speedMultiplier = GetSpeedMultiplier();
        currentTime -= Time.deltaTime * speedMultiplier;
        currentTime = Mathf.Clamp(currentTime, 0f, destinationTime);

        if (currentTime <= 0f)
            Debug.Log("DESTINATION REACHED");
    }

    public float GetSpeedMultiplier()
    {
        // 1.0x at -100m, 0.1x at -400m
        float t = (currentDepth - minDepth) / (maxDepth - minDepth);
        return Mathf.Lerp(1f, 0.1f, t);
    }

    public void PumpBallast()
    {
        ballast -= ballastPumpAmount;
        ballast = Mathf.Clamp(ballast, 0f, 100f);
    }

    public void RegisterLeak()
    {
        activeLeaks++;
    }

    public void UnregisterLeak()
    {
        activeLeaks = Mathf.Max(0, activeLeaks - 1);
    }

    public void TakeDamage(float amount)
    {
        shipHP -= amount;
        shipHP = Mathf.Clamp(shipHP, 0f, 100f);

        if (shipHP <= 0)
            Debug.Log("GAME OVER - Hull destroyed");
    }

    public void RepairHull(float amount)
    {
        shipHP += amount;
        shipHP = Mathf.Clamp(shipHP, 0f, 100f);
    }
}