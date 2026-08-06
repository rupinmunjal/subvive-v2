using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Photon.Pun;

public class ElectricalSystem : MonoBehaviourPun
{
    [System.Serializable]
    public class ElectricalBox
    {
        public GameObject blackout;
        public bool isFlickering = false;
        public bool isBlackedOut = false;
        public bool playerNearby = false;
        public float flickerTimer = 0f;
        public float flickerCounter = 0f;
    }

    [Header("Electrical Boxes")]
    public List<ElectricalBox> boxes = new List<ElectricalBox>();

    [Header("Timing Settings")]
    public float minTimeBetweenFailures = 50f;
    public float maxTimeBetweenFailures = 80f;
    public float flickerDuration = 6f;
    public float flickerInterval = 0.2f;
    public float gracePeriod = 15f;

    private float timer = 0f;
    private float nextFailureTime;
    private bool inGracePeriod = false;

    void Start()
    {
        SetNextFailureTime();
        foreach (ElectricalBox box in boxes)
        {
            if (box.blackout != null)
                box.blackout.SetActive(false);
        }
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
            HandleSpawning();
        HandleBoxes();
    }

    private void HandleSpawning()
    {
        timer += Time.deltaTime;

        if (inGracePeriod)
        {
            if (timer >= gracePeriod)
            {
                inGracePeriod = false;
                timer = 0f;
                SetNextFailureTime();
            }
            return;
        }

        if (timer >= nextFailureTime)
        {
            timer = 0f;
            TriggerRandomFailure();
            inGracePeriod = true;
        }
    }

    private void HandleBoxes()
    {
        foreach (ElectricalBox box in boxes)
        {
            if (box.isBlackedOut)
            {
                if (box.playerNearby && Keyboard.current.qKey.wasPressedThisFrame)
                    photonView.RPC("RPC_FixBox", RpcTarget.All, boxes.IndexOf(box));
                continue;
            }

            if (box.isFlickering)
            {
                box.flickerTimer += Time.deltaTime;
                box.flickerCounter += Time.deltaTime;

                if (box.flickerCounter >= flickerInterval)
                {
                    box.flickerCounter = 0f;
                    if (box.blackout != null)
                        box.blackout.SetActive(!box.blackout.activeSelf);
                }

                if (box.flickerTimer >= flickerDuration)
                    BlackoutBox(box);
            }
        }
    }

    private void TriggerRandomFailure()
    {
        List<int> available = new List<int>();

        for (int i = 0; i < boxes.Count; i++)
        {
            if (!boxes[i].isFlickering && !boxes[i].isBlackedOut)
                available.Add(i);
        }

        if (available.Count == 0) return;

        int selectedIndex = available[Random.Range(0, available.Count)];
        photonView.RPC("RPC_TriggerFailure", RpcTarget.All, selectedIndex);
    }

    [PunRPC]
    private void RPC_TriggerFailure(int index)
    {
        ElectricalBox selected = boxes[index];
        selected.isFlickering = true;
        selected.flickerTimer = 0f;
        selected.flickerCounter = 0f;
    }

    [PunRPC]
    private void RPC_FixBox(int index)
    {
        FixBox(boxes[index]);
    }

    private void BlackoutBox(ElectricalBox box)
    {
        box.isFlickering = false;
        box.isBlackedOut = true;
        if (box.blackout != null)
        {
            box.blackout.SetActive(true);
            AlertManager.Instance.Register(box.blackout.transform, 3);
        }
    }

    private void FixBox(ElectricalBox box)
    {
        box.isFlickering = false;
        box.isBlackedOut = false;
        box.flickerTimer = 0f;
        if (box.blackout != null)
        {
            box.blackout.SetActive(false);
            AlertManager.Instance.Unregister(box.blackout.transform);
        }
    }

    private void SetNextFailureTime()
    {
        nextFailureTime = Random.Range(minTimeBetweenFailures, maxTimeBetweenFailures);
    }

    public void SetPlayerNearby(int boxIndex, bool nearby)
    {
        if (boxIndex >= 0 && boxIndex < boxes.Count)
            boxes[boxIndex].playerNearby = nearby;
    }
}