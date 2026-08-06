using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class BallastStation : MonoBehaviour
{
    public float interactCooldown = 0.2f; // min time between presses

    private bool playerNearby = false;
    private float cooldownTimer = 0f;

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (playerNearby && Keyboard.current.qKey.wasPressedThisFrame && cooldownTimer <= 0f)
        {
            HullManager.Instance.photonView.RPC("RPC_PumpBallast", RpcTarget.MasterClient);
            cooldownTimer = interactCooldown;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.GetComponentInParent<PhotonView>()?.IsMine == true)
            playerNearby = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.GetComponentInParent<PhotonView>()?.IsMine == true)
            playerNearby = false;
    }
}