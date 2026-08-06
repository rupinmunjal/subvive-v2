using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic Instance;

    public AudioClip musicClip;
    private AudioSource audioSource;

    void Awake()
    {
        if (!PlayerPrefs.HasKey("VolumeResetV1"))
        {
            Debug.Log($"[BackgroundMusic] One-time volume reset: clearing MasterVolume={PlayerPrefs.GetFloat("MasterVolume", 1f)}, MusicVolume={PlayerPrefs.GetFloat("MusicVolume", 1f)}, SFXVolume={PlayerPrefs.GetFloat("SFXVolume", 1f)}");
            PlayerPrefs.DeleteKey("MasterVolume");
            PlayerPrefs.DeleteKey("MusicVolume");
            PlayerPrefs.DeleteKey("SFXVolume");
            PlayerPrefs.SetInt("VolumeResetV1", 1);
            PlayerPrefs.Save();
            AudioListener.volume = 1f;
        }

        if (Instance != null && Instance != this)
        {
            Debug.Log($"[BackgroundMusic] Duplicate on '{gameObject.scene.name}' destroying itself, existing Instance survives.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log($"[BackgroundMusic] Became the persistent Instance (originating scene: '{gameObject.scene.name}').");

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = musicClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        audioSource.mute = false;

        Debug.Log($"[BackgroundMusic] musicClip assigned: {(musicClip != null ? musicClip.name : "NULL")}");

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        audioSource.volume = savedVolume;

        Debug.Log($"[BackgroundMusic] Start() on scene '{gameObject.scene.name}': PlayerPrefs MusicVolume={savedVolume}, HasKey={PlayerPrefs.HasKey("MusicVolume")}, applied volume={audioSource.volume}, mute={audioSource.mute}, listeners in scene={Object.FindObjectsByType<AudioListener>(FindObjectsSortMode.None).Length}");

        if (musicClip != null && !audioSource.isPlaying)
        {
            audioSource.Play();
            Debug.Log($"[BackgroundMusic] Play() called. isPlaying={audioSource.isPlaying}");
        }
    }

    private static readonly string[] persistentScenes = { "HomeScene", "SettingsScene", "Tutorial1", "Tutorial2", "Tutorial3" };

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[BackgroundMusic] OnSceneLoaded fired for '{scene.name}' (mode={mode}). This instance is from '{gameObject.scene.name}'.");

        if (System.Array.IndexOf(persistentScenes, scene.name) < 0)
        {
            Debug.Log($"[BackgroundMusic] Leaving menu scenes, destroying self.");
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        Debug.Log($"[BackgroundMusic] OnDestroy called on instance from scene '{gameObject.scene.name}'.");
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void SetVolume(float volume)
    {
        Debug.Log($"[BackgroundMusic] SetVolume({volume}) called.");
        if (audioSource != null)
            audioSource.volume = volume;
    }
}
