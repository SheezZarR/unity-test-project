using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D p_rb;
    private BoxCollider2D p_coll;
    private SpriteRenderer p_sprite;
    private Animator p_anim;


    [SerializeField] private LayerMask jumpableGround;

    private float dirX = 0f;
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private float moveSpeed = 7f;
    

    private enum MovementState { idle, running, jumping, falling };
    private MovementState state = MovementState.idle;

    [SerializeField] private AudioSource jumpSFX;

    // Start is called before the first frame update
    void Start()
    {
        p_rb = GetComponent<Rigidbody2D>();
        p_coll = GetComponent<BoxCollider2D>();
        p_sprite = GetComponent<SpriteRenderer>();
        p_anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal");
        p_rb.velocity = new Vector2(dirX * moveSpeed, p_rb.velocity.y);

        if (Input.GetButtonDown("Jump") && IsGrounded()) 
        {
            jumpSFX.Play();
            p_rb.velocity = new Vector3(p_rb.velocity.x, jumpForce, 0);
        }

        UpdateAnimationState();
        
    }

    private void UpdateAnimationState() 
    {
        if (dirX > 0f) 
        {
            state = MovementState.running;
            p_sprite.flipX = false;
        } 
        else if (dirX < 0f) 
        {
            state = MovementState.running;
            p_sprite.flipX = true;
        }
        else
        {
            state = MovementState.idle;
        }
        
        if (p_rb.velocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        else if (p_rb.velocity.y < -.1f) 
        {
            state = MovementState.falling;
        }

        p_anim.SetInteger("state", (int)state);
    }

    private bool IsGrounded() 
    // Creates a smol box which is used to check if there is something beneath the player
    {
        return Physics2D.BoxCast(p_coll.bounds.center, p_coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }
}
