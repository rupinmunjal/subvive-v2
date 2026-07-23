using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class OxygenManager : MonoBehaviourPun
{
    [Header("Oxygen Settings")]
    public float maxOxygen = 100f;
    public float drainRate = 2f;
    public float respawnTime = 5f;

    [Header("UI References")]
    public TextMeshProUGUI oxygenText;
    public GameObject deathPanel;
    public TextMeshProUGUI deathCountdownText;

    private float currentOxygen;
    private bool isDead = false;
    private float respawnTimer = 0f;

    private PlayerMovement movement;
    private SpriteRenderer spriteRenderer;
    private int lastDisplayedCountdown = -1;

    void Start()
    {
        if (!photonView.IsMine)
        {
            // this UI belongs to a remote player's prefab instance - hide it so it
            // doesn't sit on screen showing stale/default placeholder text forever
            if (oxygenText != null)
                oxygenText.gameObject.SetActive(false);
            if (deathPanel != null)
                deathPanel.SetActive(false);
            return;
        }

        currentOxygen = maxOxygen;
        movement = GetComponent<PlayerMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        if (isDead)
        {
            respawnTimer -= Time.deltaTime;

            int displayValue = Mathf.CeilToInt(respawnTimer);
            if (deathCountdownText != null && displayValue != lastDisplayedCountdown)
            {
                lastDisplayedCountdown = displayValue;
                deathCountdownText.text = "You died, respawning in " + displayValue;
            }

            if (respawnTimer <= 0f)
                Respawn();

            return;
        }

        // drain oxygen
        currentOxygen -= drainRate * Time.deltaTime;
        currentOxygen = Mathf.Clamp(currentOxygen, 0f, maxOxygen);

        // update UI
        if (oxygenText != null)
            oxygenText.text = "Oxygen: " + Mathf.FloorToInt(currentOxygen) + "%";

        if (currentOxygen <= 0f)
            Die();
    }

    private void Die()
    {
        isDead = true;
        respawnTimer = respawnTime;
        lastDisplayedCountdown = -1;

        // hide sprite
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        // disable movement
        if (movement != null)
            movement.enabled = false;

        // show death panel
        if (deathPanel != null)
            deathPanel.SetActive(true);
    }

    private void Respawn()
    {
        isDead = false;
        currentOxygen = maxOxygen;

        // get spawn point from room manager
        RoomManager roomManager = FindFirstObjectByType<RoomManager>();
        if (roomManager != null)
        {
            Transform spawnPoint = roomManager.GetSpawnPoint(PhotonNetwork.LocalPlayer.ActorNumber);
            if (spawnPoint != null)
                transform.position = spawnPoint.position;
        }

        // show sprite
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        // re-enable movement
        if (movement != null)
            movement.enabled = true;

        // hide death panel
        if (deathPanel != null)
            deathPanel.SetActive(false);

        if (oxygenText != null)
            oxygenText.text = "O2: 100%";
    }

    public void RefillOxygen()
    {
        currentOxygen = maxOxygen;
    }
}