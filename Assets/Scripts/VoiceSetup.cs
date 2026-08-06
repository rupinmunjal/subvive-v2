using UnityEngine;
using Photon.Voice.PUN;
using Photon.Voice.Unity;

public class VoiceSetup : MonoBehaviour
{
    private Recorder recorder;

    void Awake()
    {
        recorder = GetComponent<Recorder>();
    }

    void Update()
    {
        if (PunVoiceClient.Instance != null && recorder != null && PunVoiceClient.Instance.PrimaryRecorder == null)
        {
            PunVoiceClient.Instance.PrimaryRecorder = recorder;
            enabled = false; // stop updating once set
        }
    }
}