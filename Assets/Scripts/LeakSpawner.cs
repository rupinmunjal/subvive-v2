using UnityEngine;
using Photon.Pun;

public class LeakSpawner : MonoBehaviourPun
{
    public LeakPoint[] leakPoints;
    public float timeBetweenLeaks = 45f;

    private float timer = 0f;

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        timer += Time.deltaTime;

        if (timer >= timeBetweenLeaks)
        {
            timer = 0f;
            TriggerRandomLeak();
        }
    }

    private void TriggerRandomLeak()
    {
        // build a list of indices for leak points that are NOT currently leaking
        System.Collections.Generic.List<int> available = new();

        for (int i = 0; i < leakPoints.Length; i++)
        {
            if (!leakPoints[i].isLeaking)
                available.Add(i);
        }

        if (available.Count == 0)
        {
            Debug.Log("All leak points are already leaking!");
            return;
        }

        int randomIndex = available[Random.Range(0, available.Count)];
        photonView.RPC("RPC_TriggerLeak", RpcTarget.All, randomIndex);
    }

    [PunRPC]
    private void RPC_TriggerLeak(int index)
    {
        leakPoints[index].TriggerLeak();
    }

    public void RequestCompleteRepair(int index)
    {
        photonView.RPC("RPC_CompleteRepair", RpcTarget.All, index);
    }

    [PunRPC]
    private void RPC_CompleteRepair(int index)
    {
        leakPoints[index].CompleteRepairNetworked();
    }
}