using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private bool isFacingRight = true;
    private bool isHit = false;
    private bool wallDetected;
    private bool groundDetected;

    private float groundCheckDistance;
    private float wallCheckDistance;
    private float dirX = 0.0f;
    
    // TODO: revisit isFacingRight if statements.
    private int facingDirection = 1;

    private Animator animator;
    private Rigidbody2D rb;
    private Transform tr;
    private Transform groundCheck;
    private Transform wallCheck;

    public GameObject deathEffect;
    public GameObject damageText;

    [SerializeField] private int health = 100;
    [SerializeField] private float damageTextYOffSet = 0f;
    [SerializeField] private float knockbackForceX = 0f;
    [SerializeField] private float knockbackForceY = 0f;

    private enum EnemyState {idle, running, hit};
    private EnemyState state = EnemyState.idle;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
        dirX = -rb.velocity.x;
        
        UpdateAnimationState();

    }

    private void UpdateAnimationState()
    {

        if (isHit)
        {
            state = EnemyState.hit;
        } 
        else if (dirX > 0.0f)
        {
            state = EnemyState.running;
            if (!isFacingRight)
            {
                Flip();
            }

        }
        else if (dirX < 0.0f)
        {
            state = EnemyState.running;
            if (isFacingRight)
            {
                Flip();
            }
        }
        else
        {
            state = EnemyState.idle;
        }

        animator.SetInteger("state", (int)state);
    }
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        facingDirection *= -1;
        tr.Rotate(0, 180f, 0);
    }

    public void TakeDamage(DamageDetails details)
    {
        isHit = true; 
        UpdateAnimationState();
        
        health -= details.damage;
        GameObject damageTxt = Instantiate(damageText, new Vector3(transform.position.x, transform.position.y + damageTextYOffSet, 0f), new Quaternion(0f, 0f, 0f, 0f));
        damageTxt.transform.GetChild(0).GetComponent<TextMeshPro>().SetText($"-{details.damage}");

        if (health <= 0)
        {
            Die();
        } 
        else
        {
            ApplyKnockback(details.incomingDirection);
        }
    }

    private void ApplyKnockback(int incomingDirection)
    {
        rb.velocity = new Vector2(knockbackForceX * incomingDirection, knockbackForceY);
    }

    public void DisableHitState()
    {
        isHit = false;
    }

    void Die()
    {
        GameObject dethEffect = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
        Destroy(dethEffect, 0.7f);
    }
}
