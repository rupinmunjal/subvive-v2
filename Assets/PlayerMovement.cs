using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Photon.Realtime;

public class PlayerMovement : MonoBehaviourPun
{
    public Rigidbody2D player1;
    public float moveSpeed = 5f;
    public float climbSpeed = 4f;
    public float sprintMultiplier = 1.5f;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public LayerMask ladderLayer;

    [Header("Audio")]
    public AudioClip footstepSound;

    private float horizontalMovement;
    private bool climbInput;
    private bool descendInput;
    private bool sprintInput;
    private bool isOnLadder;
    private float defaultGravity;
    private Collider2D currentLadderCollider;
    private AudioSource audioSource;

    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        defaultGravity = player1.gravityScale;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = footstepSound;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        float currentMoveSpeed = sprintInput ? moveSpeed * sprintMultiplier : moveSpeed;
        player1.linearVelocity = new Vector2(horizontalMovement * currentMoveSpeed, player1.linearVelocity.y);

        // check if ladder is on fire
        bool ladderOnFire = currentLadderCollider != null &&
                            currentLadderCollider.GetComponent<LadderBlock>() != null &&
                            currentLadderCollider.GetComponent<LadderBlock>().isBlocked;

        if (isOnLadder && !ladderOnFire)
        {
            player1.gravityScale = 0f;

            float verticalInput = 0f;
            if (climbInput) verticalInput = 1f;
            else if (descendInput) verticalInput = -1f;

            player1.linearVelocity = new Vector2(player1.linearVelocity.x, verticalInput * climbSpeed);
        }
        else
        {
            player1.gravityScale = defaultGravity;
        }

        //animator.SetFloat("yVelocity", player1.linearVelocity.y);
        animator.SetFloat("magnitude", Mathf.Abs(horizontalMovement));

        bool isWalking = Mathf.Abs(horizontalMovement) > 0.01f && !isOnLadder;
        if (isWalking && footstepSound != null)
        {
            audioSource.pitch = sprintInput ? sprintMultiplier : 1f;

            if (!audioSource.isPlaying)
            {
                audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
                audioSource.Play();
            }
        }
        else if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

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

    public void Sprint(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine) return;
        sprintInput = context.ReadValueAsButton();
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