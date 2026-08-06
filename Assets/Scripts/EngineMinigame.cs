using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class EngineMinigame : MonoBehaviour
{
    [Header("Settings")]
    public float timeLimit = 15f;

    [Header("UI References")]
    public GameObject minigamePanel;
    public Slider timerBar;
    public Transform keyContainer;

    [Header("Key Images")]
    public Sprite q_sprite;
    public Sprite w_sprite;
    public Sprite e_sprite;
    public Sprite r_sprite;
    public Sprite a_sprite;
    public Sprite s_sprite;
    public Sprite d_sprite;
    public Sprite f_sprite;

    [Header("Key Image Prefab")]
    public GameObject keyImagePrefab;

    private char[] validChars = new char[] { 'q', 'w', 'e', 'r', 'a', 's', 'd', 'f' };
    private char[] sequence = new char[8];
    private int currentIndex = 0;
    private float timer = 0f;
    private bool isActive = false;
    private GameObject[] keyImages = new GameObject[8];

    private EngineSystem engineSystem;
    private System.Action onSuccess;
    private System.Action onFail;
    private bool skipInputThisFrame;

    void Update()
    {
        if (!isActive) return;

        if (skipInputThisFrame)
        {
            skipInputThisFrame = false;
            return;
        }

        timer -= Time.deltaTime;
        if (timerBar != null)
            timerBar.value = timer / timeLimit;

        if (timer <= 0f)
        {
            FailMinigame();
            return;
        }

        foreach (char c in validChars)
        {
            Key key = (Key)System.Enum.Parse(typeof(Key), c.ToString().ToUpperInvariant());
            if (Keyboard.current[key].wasPressedThisFrame)
            {
                HandleInput(c);
                break;
            }
        }
    }

    public void StartMinigame(EngineSystem engine, System.Action success, System.Action fail)
    {
        engineSystem = engine;
        onSuccess = success;
        onFail = fail;

        GenerateSequence();
        timer = timeLimit;
        currentIndex = 0;
        isActive = true;
        skipInputThisFrame = true;

        if (minigamePanel != null)
            minigamePanel.SetActive(true);

        if (timerBar != null)
        {
            timerBar.minValue = 0f;
            timerBar.maxValue = 1f;
            timerBar.value = 1f;
        }

        SpawnKeyImages();
    }

    private void GenerateSequence()
    {
        for (int i = 0; i < 8; i++)
            sequence[i] = validChars[Random.Range(0, validChars.Length)];
    }

    private void SpawnKeyImages()
    {
        // clear old keys
        foreach (Transform child in keyContainer)
            Destroy(child.gameObject);

        keyImages = new GameObject[8];

        for (int i = 0; i < 8; i++)
        {
            GameObject keyObj = Instantiate(keyImagePrefab, keyContainer);
            Image img = keyObj.GetComponent<Image>();
            if (img != null)
                img.sprite = GetSpriteForChar(sequence[i]);
            keyImages[i] = keyObj;
        }
    }

    private void HandleInput(char c)
    {
        if (c == sequence[currentIndex])
        {
            // correct � tint that key image grey
            if (keyImages[currentIndex] != null)
            {
                Image img = keyImages[currentIndex].GetComponent<Image>();
                if (img != null)
                    img.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
            }

            currentIndex++;

            if (currentIndex >= 8)
                CompleteMinigame();
        }
        else
        {
            // wrong key � fail
            FailMinigame();
        }
    }

    private void CompleteMinigame()
    {
        isActive = false;
        if (minigamePanel != null)
            minigamePanel.SetActive(false);
        onSuccess?.Invoke();
        UnlockMovement();
    }

    private void FailMinigame()
    {
        isActive = false;
        if (minigamePanel != null)
            minigamePanel.SetActive(false);
        onFail?.Invoke();
        UnlockMovement();
    }

    private void UnlockMovement()
    {
        foreach (var pm in FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None))
        {
            var pv = pm.GetComponent<Photon.Pun.PhotonView>();
            if (pv != null && pv.IsMine)
            {
                pm.enabled = true;
                break;
            }
        }
    }

    private Sprite GetSpriteForChar(char c)
    {
        switch (c)
        {
            case 'q': return q_sprite;
            case 'w': return w_sprite;
            case 'e': return e_sprite;
            case 'r': return r_sprite;
            case 'a': return a_sprite;
            case 's': return s_sprite;
            case 'd': return d_sprite;
            case 'f': return f_sprite;
            default: return null;
        }
    }
}