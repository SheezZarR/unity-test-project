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

    private int lastWallJumpDirection = 0;

    private float jumpTimer;
    private float turnTimer;
    private float wallJumpTimer;

    private float dirX = 0f;
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private int amountOfJumpsLeft;
    [SerializeField] private float wallCheckDisTance = 0.5f;
    [SerializeField] private float wallSlideSpeed = 1f;
    [SerializeField] private float airDragMultiplier = 0.95f;
    [SerializeField] private float variableJumpHeightMultiplier = 0.5f;
    [SerializeField] private float wallJumpForce;
    [SerializeField] private int facingDirection = 1;
    [SerializeField] private float jumpTimerSet = 0.15f;
    [SerializeField] private float wallJumpTimerSet = 0.5f;
    [SerializeField] private float turnTimerSet = 0.1f;

    private bool isMovingRight = true;
    private bool isGrounded = true;
    private bool canNormalJump = true;
    private bool canWallJump = false;
    private bool canMove;
    private bool canFlip;
    private bool hasWallJumped;
    private bool isAttemptingToJump;
    private bool isTouchingWall = false;
    private bool isWallSliding = false;
    private bool checkJumpMultiplier;
  

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
        CheckJump();
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
            if (!isMovingRight)
            {
                Flip();
            }

        }
        else if (dirX < 0f)
        {
            state = MovementState.running;
            if (isMovingRight)
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
            if (isGrounded || (amountOfJumpsLeft > 0 && isTouchingWall))
            {
                NormalJump();
            } 
            else
            {
                jumpTimer = jumpTimerSet;
                isAttemptingToJump = true;
            }
        } 

        if (Input.GetButtonDown("Horizontal") && isTouchingWall)
        {
            if (!isGrounded && dirX != facingDirection)
            {
                canMove = false;
                canFlip = false;

                turnTimer = turnTimerSet;
            }
        }

        if (!canMove )
        {
            turnTimer -= Time.deltaTime;

            if (turnTimer <= 0)
            {
                canMove = true;
                canFlip = true;
            }
        }

        if (checkJumpMultiplier && !Input.GetButton("Jump"))
        {
            checkJumpMultiplier = false;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
        }
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDisTance, jumpableGround);
    }

    private void CheckIfCanJump()
    {
        if (isGrounded && rb.velocity.y <= 0.01f)
        {
            amountOfJumpsLeft = amountOfJumps;
        }

        if (isTouchingWall)
        {
            canWallJump = true;
        }

        if (amountOfJumpsLeft <= 0)
        {
            canNormalJump = false;
        }
        else
        {
            canNormalJump = true;
        }
    }

    private void CheckIfWallSliding()
    {
        if (isTouchingWall && dirX == facingDirection && rb.velocity.y < 0f)
        {
            isWallSliding = true;
        } 
        else
        {
            isWallSliding = false;
        }
    }

    private void ApplyForce()
    {
        if (!isGrounded && !isWallSliding && dirX == 0) // Jump in place
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }
        else if (canMove) // Walking
        {
            rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

        } 
        //else if (!isGrounded && !isWallSliding && dirX != 0) // Jump with horizontal input
        //{
        //    Vector2 forceToAdd = new Vector2(movementForceInAir * dirX, 0);
        //    rb.AddForce(forceToAdd);

        //    if (Mathf.Abs(rb.velocity.x) > moveSpeed)
        //    {
        //        rb.velocity = new Vector2(moveSpeed * dirX, rb.velocity.y);
        //    }
        //} 
        
        if (isWallSliding) // Wall Sliding
        {
            if (rb.velocity.y < -wallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }

    }

    private void CheckJump()
    {  
        if (jumpTimer > 0)
        {
            // Wall jump

            if (!isGrounded && isTouchingWall && dirX != 0 && dirX != facingDirection)
            {
                WallJump();
            }
            else if (isGrounded)
            {
                NormalJump();
            }
        } 
        if (isAttemptingToJump)
        {
            jumpTimer -= Time.deltaTime;
        }

        if (wallJumpTimer > 0)
        {
            if (hasWallJumped && dirX == -lastWallJumpDirection)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0.0f);
                hasWallJumped = false;
            } 
            else if (wallJumpTimer <= 0)
            {
                hasWallJumped = false;
            } 
            else
            {
                wallJumpTimer -= Time.deltaTime;
            }
        }
        //else if (isWallSliding && dirX == 0 && canJump) // Wall Hop
        //{
        //    Debug.Log("Wall Hop");
        //    isWallSliding = false;
        //    amountOfJumpsLeft--;
        //    Vector2 forceAdd = new Vector2(wallHopForce * wallHopDirection.x * -facingDirection, wallHopForce * wallHopDirection.y);

        //    rb.AddForce(forceAdd, ForceMode2D.Impulse);
        //} 
         
    }

    private void NormalJump()
    {
        if (canNormalJump)
        {
            //Debug.Log("Regular Jump");
            amountOfJumpsLeft--;
            jumpSFX.Play();
           
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, 0);

            jumpTimer = 0;
            isAttemptingToJump = false;
            checkJumpMultiplier = true;
        }
    }

    private void WallJump()
    {
        if (canWallJump) // Wall Jump 
        {
            Debug.Log("Wall Jump");
            rb.velocity = new Vector2(rb.velocity.x, 0.0f);
            isWallSliding = false;
            isTouchingWall = false;
            amountOfJumpsLeft = amountOfJumps;
            amountOfJumpsLeft--;

            Vector2 forceAdd = new Vector2(wallJumpForce * wallJumpDirection.x * -facingDirection, wallJumpForce * wallJumpDirection.y);

            rb.AddForce(forceAdd, ForceMode2D.Impulse);

            jumpTimer = 0;
            isAttemptingToJump = false;
            checkJumpMultiplier = true;
            turnTimer = 0;
            canMove = true;
            canFlip = true;
            hasWallJumped = true;
            wallJumpTimer = wallJumpTimerSet;
            lastWallJumpDirection = -facingDirection;
        }
    }

    private void Flip()
    {
        if (!isWallSliding && canFlip)
        {
            facingDirection *= -1;
            isMovingRight = !isMovingRight;
            tr.Rotate(0, 180f, 0);
        }
    }

    public virtual void OnDrawGizmos()
    {
        float local_wallCheckDisTance = isMovingRight ? wallCheckDisTance : -wallCheckDisTance;
        float local_wallJumpXDirection = isMovingRight ? 1f : -1f;

        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + local_wallCheckDisTance, wallCheck.position.y, wallCheck.position.z));

        Gizmos.DrawLine(transform.position, transform.position + new Vector3(local_wallJumpXDirection, 0.5f, 0));
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(local_wallJumpXDirection, 2f, 0));
    }
}
