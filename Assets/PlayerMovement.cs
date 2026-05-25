using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Photon.Realtime;
public class PlayerMovement : MonoBehaviourPun
{
    public Rigidbody2D player1;

    public float moveSpeed = 5f;

    private float horizontalMovement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;
        player1.linearVelocity = new Vector2(horizontalMovement*moveSpeed, player1.linearVelocity.y);
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine) return;
        horizontalMovement = context.ReadValue<Vector2>().x;
    }
}
