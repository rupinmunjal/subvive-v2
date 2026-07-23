using UnityEngine;
using Photon.Pun;

public class AntennaTrigger : MonoBehaviour
{
    public AntennaSystem antennaSystem;
    public int antennaIndex;
    public float repairTime = 3f;

    private bool playerNearby = false;
    private float repairProgress = 0f;
    private bool repairRequested = false;

    void Update()
    {
        if (playerNearby && Input.GetKey(KeyCode.Q))
        {
            if (!antennaSystem.antennas[antennaIndex].isDamaged) return;

            repairProgress += Time.deltaTime;
            if (repairProgress >= repairTime && !repairRequested)
            {
                repairProgress = 0f;
                repairRequested = true;
                antennaSystem.RequestRepairAntenna(antennaIndex);
            }
        }
        else
        {
            repairProgress = 0f;
            repairRequested = false;
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
            playerNearby = false;
    }
}
