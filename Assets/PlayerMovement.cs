using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Photon.Realtime;

public class PlayerMovement : MonoBehaviourPun
{
    public Rigidbody2D player1;
    public float moveSpeed = 5f;
    public float climbSpeed = 4f;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public LayerMask ladderLayer;

    private float horizontalMovement;
    private bool climbInput;
    private bool descendInput;
    private bool isOnLadder;
    private float defaultGravity;
    private Collider2D currentLadderCollider;

    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        defaultGravity = player1.gravityScale;
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        player1.linearVelocity = new Vector2(horizontalMovement * moveSpeed, player1.linearVelocity.y);

        if (isOnLadder)
        {
            player1.gravityScale = 0f;

            float verticalInput = 0f;

            // check if ladder is on fire
            bool ladderOnFire = currentLadderCollider != null &&
                                currentLadderCollider.GetComponent<LadderBlock>() != null &&
                                currentLadderCollider.GetComponent<LadderBlock>().isBlocked;

            if (!ladderOnFire)
            {
                if (climbInput) verticalInput = 1f;
                else if (descendInput) verticalInput = -1f;
            }

            player1.linearVelocity = new Vector2(player1.linearVelocity.x, verticalInput * climbSpeed);
        }
        else
        {
            player1.gravityScale = defaultGravity;
        }

        //animator.SetFloat("yVelocity", player1.linearVelocity.y);
        animator.SetFloat("magnitude", Mathf.Abs(horizontalMovement));

        if (horizontalMovement > 0.01f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (horizontalMovement < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine) return;
        horizontalMovement = context.ReadValue<Vector2>().x;
        Debug.Log("Move called, value: " + horizontalMovement);
    }

    public void Climb(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine) return;
        climbInput = context.ReadValueAsButton();
    }

    public void Crouch(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine) return;
        descendInput = context.ReadValueAsButton();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & ladderLayer) != 0)
        {
            isOnLadder = true;
            currentLadderCollider = other;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & ladderLayer) != 0)
        {
            isOnLadder = false;
            currentLadderCollider = null;
            player1.gravityScale = defaultGravity;
        }
    }
}