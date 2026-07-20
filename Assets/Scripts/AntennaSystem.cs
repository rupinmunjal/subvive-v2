using UnityEngine;
using Photon.Pun;

public class AntennaSystem : MonoBehaviourPun
{
    [System.Serializable]
    public class Antenna
    {
        public GameObject level0Visual;
        public GameObject level1Visual;
        public GameObject level2Visual;
        public GameObject level3Visual;
        public bool isDamaged = false;
        public int currentLevel = 0;
        public float damageTimer = 0f;
    }

    [Header("Antennas")]
    public Antenna[] antennas = new Antenna[2];

    [Header("Timing Settings")]
    public float timeBetweenDamage = 30f;
    public float timeToLevel2 = 20f;
    public float timeToLevel3 = 40f;

    [Header("Audio Clips")]
    public AudioClip staticClip;
    public AudioClip morseClip;

    [Header("Volume Settings")]
    public float level1Volume = 0.2f;
    public float level2Volume = 0.6f;
    public float level3Volume = 0.6f;

    [Header("Voice Mixer")]
    public UnityEngine.Audio.AudioMixer voiceMixer;

    private AudioSource staticSource;
    private AudioSource morseSource;
    private float spawnTimer = 0f;
    private bool antennaCurrentlyDamaged = false;

    void Start()
    {
        // grab the two audio sources on this object
        AudioSource[] sources = GetComponents<AudioSource>();
        staticSource = sources[0];
        morseSource = sources[1];

        staticSource.clip = staticClip;
        staticSource.loop = true;
        staticSource.volume = 0f;
        staticSource.Play();

        morseSource.clip = morseClip;
        morseSource.loop = true;
        morseSource.volume = 0f;
        morseSource.Play();

        foreach (Antenna a in antennas)
            SetAntennaLevel(a, 0);
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
            HandleSpawning();
        HandleAntennas();
    }

    private void HandleSpawning()
    {
        if (antennaCurrentlyDamaged) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= timeBetweenDamage)
        {
            spawnTimer = 0f;
            TriggerRandomAntenna();
        }
    }

    private void TriggerRandomAntenna()
    {
        System.Collections.Generic.List<int> available = new();
        for (int i = 0; i < antennas.Length; i++)
        {
            if (!antennas[i].isDamaged)
                available.Add(i);
        }

        if (available.Count == 0) return;

        int selectedIndex = available[Random.Range(0, available.Count)];
        photonView.RPC("RPC_TriggerAntenna", RpcTarget.All, selectedIndex);
    }

    [PunRPC]
    private void RPC_TriggerAntenna(int index)
    {
        Antenna selected = antennas[index];
        selected.isDamaged = true;
        selected.damageTimer = 0f;
        antennaCurrentlyDamaged = true;
        SetAntennaLevel(selected, 1);
    }

    private void HandleAntennas()
    {
        foreach (Antenna a in antennas)
        {
            if (!a.isDamaged) continue;

            a.damageTimer += Time.deltaTime;

            if (a.damageTimer >= timeToLevel3 && a.currentLevel < 3)
                SetAntennaLevel(a, 3);
            else if (a.damageTimer >= timeToLevel2 && a.currentLevel < 2)
                SetAntennaLevel(a, 2);
        }
    }

    private void SetAntennaLevel(Antenna a, int level)
    {
        a.currentLevel = level;

        if (a.level0Visual != null) a.level0Visual.SetActive(level == 0);
        if (a.level1Visual != null) a.level1Visual.SetActive(level == 1);
        if (a.level2Visual != null) a.level2Visual.SetActive(level == 2);
        if (a.level3Visual != null) a.level3Visual.SetActive(level == 3);

        switch (level)
        {
            case 0:
                staticSource.volume = 0f;
                morseSource.volume = 0f;
                SetVoiceVolume(0f);
                break;
            case 1:
                staticSource.volume = level1Volume;
                morseSource.volume = level1Volume;
                SetVoiceVolume(0f);
                break;
            case 2:
                staticSource.volume = level2Volume;
                morseSource.volume = level2Volume;
                SetVoiceVolume(-6f);
                break;
            case 3:
                staticSource.volume = level3Volume;
                morseSource.volume = level3Volume;
                SetVoiceVolume(-80f);
                break;
        }
    }

    public void RequestRepairAntenna(int index)
    {
        photonView.RPC("RPC_RepairAntenna", RpcTarget.All, index);
    }

    [PunRPC]
    private void RPC_RepairAntenna(int index)
    {
        Antenna antenna = antennas[index];
        antenna.isDamaged = false;
        antenna.damageTimer = 0f;
        antennaCurrentlyDamaged = false;
        SetAntennaLevel(antenna, 0);
    }

    private void SetVoiceVolume(float db)
    {
        if (voiceMixer != null)
            voiceMixer.SetFloat("VoiceVolume", db);
    }
}