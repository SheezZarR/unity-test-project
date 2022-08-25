using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private SpriteRenderer sprite;
    private Animator animator;
    private Transform tr;

    [SerializeField] private LayerMask jumpableGround;

    private float dirX = 0f;
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private int amountOfJumpsLeft;
    [SerializeField] private float wallCheckDistance = 0.5f;
    [SerializeField] private float wallSlideSpeed = 1f;
    [SerializeField] private float movementForceInAir = 2f;
    [SerializeField] private float airDragMultiplier = 0.95f;
    [SerializeField] private float variableJumpHeightMultiplier = 0.5f;
    [SerializeField] private float wallHopForce;
    [SerializeField] private float wallJumpForce;
    [SerializeField] private int facingDirection = 1;

    private bool is_movingRight = true;
    private bool is_grounded = true;
    private bool canJump = true;
    private bool is_touchingWall = false;
    private bool is_wallSliding = false;

    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;
    public int amountOfJumps = 1;
    public Transform wallCheck;

    private enum MovementState { idle, running, jumping, falling };
    private MovementState state = MovementState.idle;

    [SerializeField] private AudioSource jumpSFX;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        tr = GetComponent<Transform>();

        amountOfJumpsLeft = amountOfJumps;
        
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
    }

    // Update is called once per frame
    void Update()
    {

        CheckInput();
        CheckIfCanJump();
        CheckIfWallSliding();
        UpdateAnimationState();

    }

    private void FixedUpdate()
    {
        ApplyForce();
        CheckSurroundings();
    }

    private void UpdateAnimationState()
    {
        if (dirX > 0f)
        {
            state = MovementState.running;
            if (!is_movingRight)
            {
                Flip();
            }

        }
        else if (dirX < 0f)
        {
            state = MovementState.running;
            if (is_movingRight)
            {
                Flip();
            }
        }
        else
        {
            state = MovementState.idle;
        }

        if (rb.velocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }

        animator.SetInteger("state", (int)state);
    }

    private void CheckInput()
    {
        dirX = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        } 

        if (Input.GetButtonUp("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
        }
    }

    private void CheckSurroundings()
    {
        is_grounded = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
        is_touchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, jumpableGround);
    }

    private void CheckIfCanJump()
    {
        if ((is_grounded && rb.velocity.y <= 0) || is_wallSliding)
        {
            amountOfJumpsLeft = amountOfJumps;
        }

        if (amountOfJumpsLeft <= 0)
        {
            canJump = false;
        }
        else
        {
            canJump = true;
        }
    }

    private void CheckIfWallSliding()
    {
        if (is_touchingWall && !is_grounded && rb.velocity.y < 0)
        {
            is_wallSliding = true;
        } 
        else
        {
            is_wallSliding = false;
        }
    }

    private void ApplyForce()
    {
        if (is_grounded) // Walking
        {
            rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

        } 
        else if (!is_grounded && !is_wallSliding && dirX != 0) // Jump with horizontal input
        {
            Vector2 forceToAdd = new Vector2(movementForceInAir * dirX, 0);
            rb.AddForce(forceToAdd);

            if (Mathf.Abs(rb.velocity.x) > moveSpeed)
            {
                rb.velocity = new Vector2(moveSpeed * dirX, rb.velocity.y);
            }
        } 
        else if (!is_grounded && !is_wallSliding && dirX == 0) // Jump in place
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }
        
        if (is_wallSliding) // Wall Sliding
        {
            if (rb.velocity.y < -wallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }

    }

    private void Jump()
    {  
        if (canJump && !is_wallSliding)
        {
            Debug.Log("Regular Jump");
            amountOfJumpsLeft--;
            jumpSFX.Play();
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, 0);
        }

        else if (is_wallSliding && dirX == 0 && canJump) // Wall Hop
        {
            Debug.Log("Wall Hop");
            is_wallSliding = false;
            amountOfJumpsLeft--;
            Vector2 forceAdd = new Vector2(wallHopForce * wallHopDirection.x * -facingDirection, wallHopForce * wallHopDirection.y);

            rb.AddForce(forceAdd, ForceMode2D.Impulse);
        } 

        else if ((is_wallSliding || is_touchingWall) && dirX != 0 && canJump) // Wall Jump 
        {
            Debug.Log("Wall Jump");
            is_wallSliding = false;
            is_touchingWall = false;
            amountOfJumpsLeft--;

            Vector2 forceAdd = new Vector2(wallJumpForce * wallJumpDirection.x * -facingDirection, wallJumpForce * wallJumpDirection.y);

            rb.AddForce(forceAdd, ForceMode2D.Impulse);
        } 
    }

    private void Flip()
    {
        if (!is_wallSliding)
        {
            facingDirection *= -1;
            is_movingRight = !is_movingRight;
            tr.Rotate(0, 180f, 0);
        }
    }

    public virtual void OnDrawGizmos()
    {
        float local_wallCheckDistance = is_movingRight ? wallCheckDistance : -wallCheckDistance;
        float local_wallJumpXDirection = is_movingRight ? 1f : -1f;

        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + local_wallCheckDistance, wallCheck.position.y, wallCheck.position.z));

        Gizmos.DrawLine(transform.position, transform.position + new Vector3(local_wallJumpXDirection, 0.5f, 0));
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(local_wallJumpXDirection, 2f, 0));
    }
}
