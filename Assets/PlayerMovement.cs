using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun
{
    public Rigidbody2D player1;
    public float moveSpeed = 5f;
    public float climbSpeed = 3f;

    private float horizontalMovement;
    private bool isOnLadder = false;
    private LadderZone currentLadder;
    private Collider2D myCollider;

    void Start()
    {
        myCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        player1.linearVelocity = new Vector2(horizontalMovement * moveSpeed, player1.linearVelocity.y);

        if (isOnLadder)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                player1.gravityScale = 0f;
                player1.linearVelocity = new Vector2(player1.linearVelocity.x, climbSpeed);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                player1.gravityScale = 0f;
                player1.linearVelocity = new Vector2(player1.linearVelocity.x, -climbSpeed);
                if (currentLadder != null)
                    currentLadder.DisableFloorBelowFor(myCollider);
            }
            else
            {
                player1.gravityScale = 1f;
            }
        }
        else
        {
            player1.gravityScale = 1f;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine) return;
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void SetCurrentLadder(LadderZone ladder)
    {
        currentLadder = ladder;
        isOnLadder = ladder != null;

        if (ladder == null)
            player1.gravityScale = 1f;
    }
}