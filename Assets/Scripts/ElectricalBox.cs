using UnityEngine;

public class ElectricalBox : MonoBehaviour
{
    [Header("Timing Settings")]
    public float minTimeBetweenFailures = 15f;
    public float maxTimeBetweenFailures = 40f;
    public float flickerDuration = 5f;

    [Header("References")]
    public GameObject blackout;

    private float timer;
    private float nextFailureTime;
    private bool isFlickering = false;
    private bool isBlackedOut = false;
    private bool playerNearby = false;

    private float flickerTimer = 0f;
    private float flickerInterval = 0.2f;
    private float flickerCounter = 0f;

    void Start()
    {
        SetNextFailureTime();
        if (blackout != null)
            blackout.SetActive(false);
    }

    void Update()
    {
        if (isBlackedOut)
        {
            if (playerNearby && Input.GetKeyDown(KeyCode.E))
                FixElectrical();
            return;
        }

        timer += Time.deltaTime;

        if (!isFlickering && timer >= nextFailureTime)
        {
            StartFlicker();
        }

        if (isFlickering)
        {
            flickerTimer += Time.deltaTime;
            flickerCounter += Time.deltaTime;

            // toggle blackout on and off to create flicker
            if (flickerCounter >= flickerInterval)
            {
                flickerCounter = 0f;
                if (blackout != null)
                    blackout.SetActive(!blackout.activeSelf);
            }

            if (flickerTimer >= flickerDuration)
                TriggerBlackout();
        }
    }

    private void StartFlicker()
    {
        isFlickering = true;
        flickerTimer = 0f;
        flickerCounter = 0f;
    }

    private void TriggerBlackout()
    {
        isFlickering = false;
        isBlackedOut = true;
        if (blackout != null)
            blackout.SetActive(true);
    }

    private void FixElectrical()
    {
        isBlackedOut = false;
        isFlickering = false;
        flickerTimer = 0f;
        timer = 0f;
        SetNextFailureTime();
        if (blackout != null)
            blackout.SetActive(false);
    }

    private void SetNextFailureTime()
    {
        nextFailureTime = Random.Range(minTimeBetweenFailures, maxTimeBetweenFailures);
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