using UnityEngine;
using UnityEngine.UI;

public class LeakPoint : MonoBehaviour
{
    [Header("Repair Settings")]
    public float repairTime = 3f;
    public float hpRestoreAmount = 10f;
    public float hpDrainPerSecond = 2f;

    [Header("References")]
    public GameObject leakVisual;
    public Slider progressBar;

    public bool isLeaking = false;
    private bool playerNearby = false;
    private float repairProgress = 0f;

    void Update()
    {
        if (isLeaking)
        {
            // drain hull every second
            HullManager.Instance.TakeDamage(hpDrainPerSecond * Time.deltaTime);

            // player holding Q nearby
            if (playerNearby && Input.GetKey(KeyCode.Q))
            {
                repairProgress += Time.deltaTime;
                if (progressBar != null)
                    progressBar.value = repairProgress / repairTime;

                if (repairProgress >= repairTime)
                    CompleteRepair();
            }
            else
            {
                // reset progress if they let go
                repairProgress = 0f;
                if (progressBar != null)
                    progressBar.value = 0f;
            }
        }
    }

    public void TriggerLeak()
    {
        isLeaking = true;
        repairProgress = 0f;
        if (leakVisual != null)
            leakVisual.SetActive(true);
        if (progressBar != null)
            progressBar.gameObject.SetActive(true);
    }

    private void CompleteRepair()
    {
        isLeaking = false;
        repairProgress = 0f;
        if (leakVisual != null)
            leakVisual.SetActive(false);
        if (progressBar != null)
        {
            progressBar.value = 0f;
            progressBar.gameObject.SetActive(false);
        }
        HullManager.Instance.RepairHull(hpRestoreAmount);
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