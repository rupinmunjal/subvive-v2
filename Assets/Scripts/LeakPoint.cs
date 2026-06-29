using UnityEngine;
using UnityEngine.UI;

public class LeakPoint : MonoBehaviour
{
    [Header("Repair Settings")]
    public float repairTime = 3f;
    public float hpRestoreAmount = 10f;

    [Header("Stage Settings")]
    public float timeToStage2 = 15f;
    public float timeToStage3 = 30f;

    [Header("Stage Damage Per Second")]
    public float stage1Damage = 0.5f;
    public float stage2Damage = 1.5f;
    public float stage3Damage = 3f;

    [Header("Stage Visuals")]
    public GameObject stage0Visual;
    public GameObject stage1Visual;
    public GameObject stage2Visual;
    public GameObject stage3Visual;

    [Header("References")]
    public Slider progressBar;

    public bool isLeaking = false;
    private bool playerNearby = false;
    private float repairProgress = 0f;
    private float leakTimer = 0f;
    private int currentStage = 0;

    void Start()
    {
        SetStage(0);
    }

    void Update()
    {
        if (!isLeaking) return;

        leakTimer += Time.deltaTime;

        if (leakTimer >= timeToStage3 && currentStage < 3)
            SetStage(3);
        else if (leakTimer >= timeToStage2 && currentStage < 2)
            SetStage(2);

        float damage = currentStage == 1 ? stage1Damage :
                       currentStage == 2 ? stage2Damage : stage3Damage;
        HullManager.Instance.TakeDamage(damage * Time.deltaTime);

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
            repairProgress = 0f;
            if (progressBar != null)
                progressBar.value = 0f;
        }
    }

    public void TriggerLeak()
    {
        isLeaking = true;
        leakTimer = 0f;
        repairProgress = 0f;
        HullManager.Instance.RegisterLeak();
        SetStage(1);

        if (progressBar != null)
            progressBar.gameObject.SetActive(true);
    }

    private void CompleteRepair()
    {
        isLeaking = false;
        repairProgress = 0f;
        leakTimer = 0f;
        HullManager.Instance.UnregisterLeak();
        HullManager.Instance.RepairHull(hpRestoreAmount);
        SetStage(0);

        if (progressBar != null)
        {
            progressBar.value = 0f;
            progressBar.gameObject.SetActive(false);
        }
    }

    private void SetStage(int stage)
    {
        currentStage = stage;
        if (stage0Visual != null) stage0Visual.SetActive(stage == 0);
        if (stage1Visual != null) stage1Visual.SetActive(stage == 1);
        if (stage2Visual != null) stage2Visual.SetActive(stage == 2);
        if (stage3Visual != null) stage3Visual.SetActive(stage == 3);
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