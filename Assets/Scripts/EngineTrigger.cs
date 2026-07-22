using UnityEngine;
using Photon.Pun;

public class EngineTrigger : MonoBehaviour
{
    public EngineSystem engineSystem;
    public EngineMinigame engineMinigame;

    private bool playerNearby = false;
    private bool minigameActive = false;

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.Q) && !minigameActive)
        {
            if (engineSystem.isBroken)
            {
                minigameActive = true;
                LockMovement();
                engineMinigame.StartMinigame(
                    engineSystem,
                    OnSuccess,
                    OnFail
                );
            }
        }
    }

    private void OnSuccess()
    {
        minigameActive = false;
        engineSystem.FixEngine();
    }

    private void OnFail()
    {
        minigameActive = false;
    }

    private void LockMovement()
    {
        foreach (var pm in FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None))
        {
            var pv = pm.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                pm.enabled = false;
                break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerNearby = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            minigameActive = false;
        }
    }
}