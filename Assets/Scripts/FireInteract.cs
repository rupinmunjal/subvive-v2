using UnityEngine;
using UnityEngine.UI;

public class FireInteract : MonoBehaviour
{
    public FireSystem fireSystem;
    public float extinguishTime = 2f;
    public Slider progressBar;

    private bool playerNearby = false;
    private float holdTimer = 0f;
    private FireSystem.FirePoint myFirePoint;
    private int myFireIndex;

    void Start()
    {
        myFirePoint = fireSystem.GetFirePointByObject(gameObject);
        myFireIndex = fireSystem.GetFirePointIndex(myFirePoint);
        if (progressBar != null)
            progressBar.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!gameObject.activeSelf)
        {
            if (progressBar != null)
                progressBar.gameObject.SetActive(false);
            return;
        }

        if (playerNearby && Input.GetKey(KeyCode.Q))
        {
            holdTimer += Time.deltaTime;

            if (progressBar != null)
            {
                progressBar.gameObject.SetActive(true);
                progressBar.value = holdTimer / extinguishTime;
            }

            if (holdTimer >= extinguishTime)
            {
                holdTimer = 0f;
                if (progressBar != null)
                {
                    progressBar.value = 0f;
                    progressBar.gameObject.SetActive(false);
                }
                fireSystem.RequestExtinguishFire(myFireIndex);
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
            playerNearby = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerNearby = false;
    }
}