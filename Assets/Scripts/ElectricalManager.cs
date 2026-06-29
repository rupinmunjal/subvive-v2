using UnityEngine;
using System.Collections.Generic;

public class ElectricalSystem : MonoBehaviour
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
    public float minTimeBetweenFailures = 15f;
    public float maxTimeBetweenFailures = 40f;
    public float flickerDuration = 5f;
    public float flickerInterval = 0.2f;
    public float gracePeriod = 10f;

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
                if (box.playerNearby && Input.GetKeyDown(KeyCode.Q))
                    FixBox(box);
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
        List<ElectricalBox> available = new List<ElectricalBox>();

        foreach (ElectricalBox box in boxes)
        {
            if (!box.isFlickering && !box.isBlackedOut)
                available.Add(box);
        }

        if (available.Count == 0) return;

        ElectricalBox selected = available[Random.Range(0, available.Count)];
        selected.isFlickering = true;
        selected.flickerTimer = 0f;
        selected.flickerCounter = 0f;
    }

    private void BlackoutBox(ElectricalBox box)
    {
        box.isFlickering = false;
        box.isBlackedOut = true;
        if (box.blackout != null)
            box.blackout.SetActive(true);
    }

    private void FixBox(ElectricalBox box)
    {
        box.isFlickering = false;
        box.isBlackedOut = false;
        box.flickerTimer = 0f;
        if (box.blackout != null)
            box.blackout.SetActive(false);
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