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
    private bool is_movingRight = true;
    

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
    }

    // Update is called once per frame
    void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && IsGrounded()) 
        {
            jumpSFX.Play();
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, 0);
        }

        UpdateAnimationState();
        
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

    private void Flip()
    {
        is_movingRight = !is_movingRight;
        tr.Rotate(0, 180f, 0);
    }

    private bool IsGrounded() 
    // Creates a smol box which is used to check if there is something beneath the player
    {
        return Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }
}
