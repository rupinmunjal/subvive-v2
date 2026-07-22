using UnityEngine;
using System.Collections.Generic;

public class AlertManager
{
    private static AlertManager instance;
    public static AlertManager Instance => instance ??= new AlertManager();

    private readonly Dictionary<Transform, int> activeAlerts = new Dictionary<Transform, int>();

    public void Register(Transform alertTransform, int severity = 1)
    {
        if (alertTransform != null)
            activeAlerts[alertTransform] = severity;
    }

    public void Unregister(Transform alertTransform)
    {
        if (alertTransform != null)
            activeAlerts.Remove(alertTransform);
    }

    public IEnumerable<KeyValuePair<Transform, int>> GetAll()
    {
        List<Transform> stale = null;

        foreach (var kvp in activeAlerts)
        {
            if (kvp.Key == null)
            {
                stale ??= new List<Transform>();
                stale.Add(kvp.Key);
            }
        }

        if (stale != null)
            foreach (Transform t in stale)
                activeAlerts.Remove(t);

        return activeAlerts;
    }
}
