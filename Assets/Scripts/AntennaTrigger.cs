using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Photon.Pun;

public class AntennaTrigger : MonoBehaviour
{
    public AntennaSystem antennaSystem;
    public int antennaIndex;
    public float repairTime = 4f;
    public Slider progressBar;

    private bool playerNearby = false;
    private float repairProgress = 0f;
    private bool repairRequested = false;

    void Update()
    {
        if (playerNearby && Keyboard.current.qKey.isPressed && antennaSystem.antennas[antennaIndex].isDamaged)
        {
            repairProgress += Time.deltaTime;

            if (progressBar != null)
            {
                progressBar.gameObject.SetActive(true);
                progressBar.value = repairProgress / repairTime;
            }

            if (repairProgress >= repairTime && !repairRequested)
            {
                repairProgress = 0f;
                repairRequested = true;

                if (progressBar != null)
                {
                    progressBar.value = 0f;
                    progressBar.gameObject.SetActive(false);
                }

                antennaSystem.RequestRepairAntenna(antennaIndex);
            }
        }
        else
        {
            repairProgress = 0f;
            repairRequested = false;

            if (progressBar != null)
            {
                progressBar.value = 0f;
                progressBar.gameObject.SetActive(false);
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
            playerNearby = false;
    }
}
