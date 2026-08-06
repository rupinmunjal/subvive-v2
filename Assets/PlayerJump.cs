using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviourPun
{
    public Rigidbody2D rb;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    private bool isGrounded;

    void Update()
    {
        if (!photonView.IsMine) return;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        
        Debug.Log("isGrounded: " + isGrounded); // is it true or false?
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine) return;
        Debug.Log("Jump called! Phase: " + context.phase); // what phase is firing?
        if (context.performed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }
}
