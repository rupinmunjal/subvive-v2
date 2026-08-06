using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class FireSystem : MonoBehaviourPun
{
    [System.Serializable]
    public class FirePoint
    {
        public GameObject fireObject;
        public LadderBlock ladder;
        public bool isActive = false;
    }

    public FirePoint[] firePoints = new FirePoint[6];

    [Header("Ladder References")]
    public LadderBlock ladder1;
    public LadderBlock ladder2;
    public LadderBlock ladder3;

    [Header("Settings")]
    public float timeBetweenFires = 60f;
    public int maxActiveFires = 3;

    private float timer = 0f;

    void Start()
    {
        foreach (FirePoint fp in firePoints)
        {
            if (fp.fireObject != null)
                fp.fireObject.SetActive(false);
            fp.isActive = false;
        }
    }

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        timer += Time.deltaTime;
        if (timer >= timeBetweenFires)
        {
            timer = 0f;
            TrySpawnFire();
        }
    }

    private void TrySpawnFire()
    {
        if (GetActiveFireCount() >= maxActiveFires) return;

        // check which ladders currently have fires
        bool ladder1HasFire = LadderHasFire(ladder1);
        bool ladder2HasFire = LadderHasFire(ladder2);
        bool ladder3HasFire = LadderHasFire(ladder3);

        // count how many ladders have fires
        int laddersWithFire = (ladder1HasFire ? 1 : 0) +
                              (ladder2HasFire ? 1 : 0) +
                              (ladder3HasFire ? 1 : 0);

        List<FirePoint> available = new List<FirePoint>();

        foreach (FirePoint fp in firePoints)
        {
            if (fp.isActive) continue;

            // if 2 ladders already have fires, only allow fires on those same ladders
            // never allow fire on the third clean ladder
            if (laddersWithFire >= 2 && !fp.ladder.isBlocked)
                continue;

            available.Add(fp);
        }

        if (available.Count == 0) return;

        FirePoint selected = available[Random.Range(0, available.Count)];
        int index = System.Array.IndexOf(firePoints, selected);
        photonView.RPC("RPC_ActivateFire", RpcTarget.All, index);
    }

    [PunRPC]
    private void RPC_ActivateFire(int index)
    {
        ActivateFire(firePoints[index]);
    }

    public void RequestExtinguishFire(int index)
    {
        photonView.RPC("RPC_ExtinguishFire", RpcTarget.All, index);
    }

    [PunRPC]
    private void RPC_ExtinguishFire(int index)
    {
        ExtinguishFire(firePoints[index]);
    }

    private bool LadderHasFire(LadderBlock ladder)
    {
        foreach (FirePoint fp in firePoints)
        {
            if (fp.isActive && fp.ladder == ladder)
                return true;
        }
        return false;
    }

    public void ActivateFire(FirePoint fp)
    {
        fp.isActive = true;
        if (fp.fireObject != null)
        {
            fp.fireObject.SetActive(true);
            AlertManager.Instance.Register(fp.fireObject.transform, 3);
        }
        UpdateLadderBlock(fp.ladder);
    }

    public void ExtinguishFire(FirePoint fp)
    {
        fp.isActive = false;
        if (fp.fireObject != null)
        {
            fp.fireObject.SetActive(false);
            AlertManager.Instance.Unregister(fp.fireObject.transform);
        }
        UpdateLadderBlock(fp.ladder);
    }

    private void UpdateLadderBlock(LadderBlock ladder)
    {
        ladder.isBlocked = LadderHasFire(ladder);
    }

    public int GetActiveFireCount()
    {
        int count = 0;
        foreach (FirePoint fp in firePoints)
            if (fp.isActive) count++;
        return count;
    }

    public FirePoint GetFirePointByObject(GameObject obj)
    {
        foreach (FirePoint fp in firePoints)
            if (fp.fireObject == obj) return fp;
        return null;
    }

    public int GetFirePointIndex(FirePoint fp)
    {
        return System.Array.IndexOf(firePoints, fp);
    }
}