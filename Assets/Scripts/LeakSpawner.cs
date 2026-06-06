using UnityEngine;

public class LeakSpawner : MonoBehaviour
{
    public LeakPoint[] leakPoints;
    public float timeBetweenLeaks = 20f;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= timeBetweenLeaks)
        {
            timer = 0f;
            TriggerRandomLeak();
        }
    }

    private void TriggerRandomLeak()
    {
        // build a list of leak points that are NOT currently leaking
        System.Collections.Generic.List<LeakPoint> available = new();

        foreach (LeakPoint lp in leakPoints)
        {
            if (!lp.isLeaking)
                available.Add(lp);
        }

        if (available.Count == 0)
        {
            Debug.Log("All leak points are already leaking!");
            return;
        }

        int randomIndex = Random.Range(0, available.Count);
        available[randomIndex].TriggerLeak();
    }
}