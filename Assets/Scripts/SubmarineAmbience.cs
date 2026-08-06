using UnityEngine;

public class SubmarineAmbience : MonoBehaviour
{
    public AudioClip ambienceClip;

    [Range(0f, 1f)]
    public float ambienceVolumeScale = 0.35f;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = ambienceClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = PlayerPrefs.GetFloat("MusicVolume", 0.5f) * ambienceVolumeScale;

        if (ambienceClip != null)
            audioSource.Play();
    }
}
