using UnityEngine;
using UnityEngine.UI;

public class OxygenStation : MonoBehaviour
{
    [Header("Settings")]
    public float refillTime = 2f;

    [Header("References")]
    public Slider progressBar;

    private bool playerNearby = false;
    private float holdTimer = 0f;
    private OxygenManager nearbyPlayerOxygen = null;

    void Start()
    {
        if (progressBar != null)
            progressBar.gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerNearby && nearbyPlayerOxygen != null && Input.GetKey(KeyCode.E))
        {
            holdTimer += Time.deltaTime;

            if (progressBar != null)
            {
                progressBar.gameObject.SetActive(true);
                progressBar.value = holdTimer / refillTime;
            }

            if (holdTimer >= refillTime)
            {
                holdTimer = 0f;
                nearbyPlayerOxygen.RefillOxygen();

                if (progressBar != null)
                {
                    progressBar.value = 0f;
                    progressBar.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            holdTimer = 0f;
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
        {
            OxygenManager om = other.GetComponent<OxygenManager>();
            if (om != null && om.photonView.IsMine)
            {
                playerNearby = true;
                nearbyPlayerOxygen = om;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            nearbyPlayerOxygen = null;
            holdTimer = 0f;
        }
    }
}