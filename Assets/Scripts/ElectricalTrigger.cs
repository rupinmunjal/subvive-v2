using UnityEngine;

public class ElectricalTrigger : MonoBehaviour
{
    public ElectricalSystem electricalSystem;
    public int boxIndex;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            electricalSystem.SetPlayerNearby(boxIndex, true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            electricalSystem.SetPlayerNearby(boxIndex, false);
    }
}