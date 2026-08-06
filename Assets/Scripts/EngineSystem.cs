using UnityEngine;
using Photon.Pun;

public class EngineSystem : MonoBehaviourPun
{
    [Header("Settings")]
    public float minTimeBetweenFailures = 90f;
    public float maxTimeBetweenFailures = 150f;
    public float flashSpeed = 0.4f;

    [Header("References")]
    public SpriteRenderer engineRenderer;
    public Color normalColor = Color.white;
    public Color flashColor = Color.red;

    public bool isBroken = false;
    private float timer = 0f;
    private float nextFailureTime;
    private float flashTimer = 0f;
    private bool flashState = false;

    void Start()
    {
        if (engineRenderer == null)
            engineRenderer = GetComponent<SpriteRenderer>();
        SetNextFailureTime();
    }

    void Update()
    {
        if (isBroken)
        {
            // flash red
            flashTimer += Time.deltaTime;
            if (flashTimer >= flashSpeed)
            {
                flashTimer = 0f;
                flashState = !flashState;
                engineRenderer.color = flashState ? flashColor : normalColor;
            }
            return;
        }

        if (!PhotonNetwork.IsMasterClient) return;

        timer += Time.deltaTime;
        if (timer >= nextFailureTime)
        {
            timer = 0f;
            TriggerFailure();
        }
    }

    public void TriggerFailure()
    {
        photonView.RPC("RPC_TriggerFailure", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_TriggerFailure()
    {
        isBroken = true;
        HullManager.Instance.PauseDestinationTimer(true);
        AlertManager.Instance.Register(transform, 3);
    }

    public void RequestFixEngine()
    {
        photonView.RPC("RPC_FixEngine", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_FixEngine()
    {
        isBroken = false;
        engineRenderer.color = normalColor;
        flashTimer = 0f;
        flashState = false;
        timer = 0f;
        SetNextFailureTime();
        HullManager.Instance.PauseDestinationTimer(false);
        AlertManager.Instance.Unregister(transform);
    }

    private void SetNextFailureTime()
    {
        nextFailureTime = Random.Range(minTimeBetweenFailures, maxTimeBetweenFailures);
    }
}