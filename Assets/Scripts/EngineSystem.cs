using UnityEngine;

public class EngineSystem : MonoBehaviour
{
    [Header("Settings")]
    public float minTimeBetweenFailures = 30f;
    public float maxTimeBetweenFailures = 60f;
    public float flashSpeed = 0.5f;

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

        timer += Time.deltaTime;
        if (timer >= nextFailureTime)
        {
            timer = 0f;
            TriggerFailure();
        }
    }

    public void TriggerFailure()
    {
        isBroken = true;
        HullManager.Instance.PauseDestinationTimer(true);
    }

    public void FixEngine()
    {
        isBroken = false;
        engineRenderer.color = normalColor;
        flashTimer = 0f;
        flashState = false;
        timer = 0f;
        SetNextFailureTime();
        HullManager.Instance.PauseDestinationTimer(false);
    }

    private void SetNextFailureTime()
    {
        nextFailureTime = Random.Range(minTimeBetweenFailures, maxTimeBetweenFailures);
    }
}