using UnityEngine;
using Photon.Pun;

public class HullManager : MonoBehaviourPun, IPunObservable
{
    public static HullManager Instance;

    [Header("Hull Settings")]
    public float shipHP = 100f;
    public float hpLossPerLeak = 5f;

    [Header("Ballast Settings")]
    public float ballast = 0f;
    public float baseFillRate = 0.5f;
    public float leakFillRate = 0.3f;
    public float ballastPumpAmount = 5f;

    [Header("Depth Settings")]
    public float minDepth = -100f;
    public float maxDepth = -400f;
    public float currentDepth = -100f;
    public float depthFollowSpeed = 2f;

    [Header("Destination Settings")]
    public float destinationTime = 300f;
    public float currentTime = 300f;

    [Header("Destination Scene")]
    public string destinationSceneName = "SurfaceScene";

    private int activeLeaks = 0;
    private bool destinationPaused = false;
    private bool destinationReached = false;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        UpdateBallast();
        UpdateDepth();
        UpdateDestinationTimer();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(shipHP);
            stream.SendNext(ballast);
            stream.SendNext(currentDepth);
            stream.SendNext(currentTime);
        }
        else
        {
            shipHP = (float)stream.ReceiveNext();
            ballast = (float)stream.ReceiveNext();
            currentDepth = (float)stream.ReceiveNext();
            currentTime = (float)stream.ReceiveNext();
        }
    }

    private void UpdateBallast()
    {
        float fillRate = baseFillRate + (activeLeaks * leakFillRate);
        ballast += fillRate * Time.deltaTime;
        ballast = Mathf.Clamp(ballast, 0f, 100f);
    }

    private void UpdateDepth()
    {
        float targetDepth = Mathf.Lerp(minDepth, maxDepth, ballast / 100f);
        currentDepth = Mathf.MoveTowards(currentDepth, targetDepth, depthFollowSpeed * Time.deltaTime);
    }

    private void UpdateDestinationTimer()
    {
        if (destinationPaused || destinationReached) return;

        float speedMultiplier = GetSpeedMultiplier();
        currentTime -= Time.deltaTime * speedMultiplier;
        currentTime = Mathf.Clamp(currentTime, 0f, destinationTime);

        if (currentTime <= 0f)
        {
            destinationReached = true;
            Debug.Log("DESTINATION REACHED");
            PhotonNetwork.LoadLevel(destinationSceneName);
        }
    }

    public float GetSpeedMultiplier()
    {
        float t = (currentDepth - minDepth) / (maxDepth - minDepth);
        return Mathf.Lerp(1f, 0.1f, t);
    }

    public void PauseDestinationTimer(bool paused)
    {
        destinationPaused = paused;
    }

    public void PumpBallast()
    {
        ballast -= ballastPumpAmount;
        ballast = Mathf.Clamp(ballast, 0f, 100f);
    }

    [PunRPC]
    public void RPC_PumpBallast()
    {
        PumpBallast();
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