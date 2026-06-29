using UnityEngine;

public class LadderZone : MonoBehaviour
{
    public Collider2D floorAbove;
    public Collider2D floorBelow;
    public float passThroughDelay = 1.5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerMovement pm = other.GetComponent<PlayerMovement>();
        if (pm != null && pm.enabled)
            pm.SetCurrentLadder(this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerMovement pm = other.GetComponent<PlayerMovement>();
        if (pm != null && pm.enabled)
            pm.SetCurrentLadder(null);
    }

    public void DisableFloorFor(Collider2D playerCollider)
    {
        if (floorAbove != null)
            StartCoroutine(TemporaryPassthrough(playerCollider, floorAbove));
    }

    public void DisableFloorBelowFor(Collider2D playerCollider)
    {
        if (floorBelow != null)
            StartCoroutine(TemporaryPassthrough(playerCollider, floorBelow));
    }

    private System.Collections.IEnumerator TemporaryPassthrough(Collider2D playerCollider, Collider2D floor)
    {
        Physics2D.IgnoreCollision(playerCollider, floor, true);
        yield return new WaitForSeconds(passThroughDelay);
        Physics2D.IgnoreCollision(playerCollider, floor, false);
    }
}