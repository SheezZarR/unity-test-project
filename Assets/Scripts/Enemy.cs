using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private int health = 100;
    private bool is_facingRight = false;
    private bool is_hit = false;
    private float dirX = 0.0f;

    private Animator animator;
    private Rigidbody2D rb;
    private Transform tr;

    public GameObject deathEffect;
    public GameObject damageText;

    [SerializeField] private float damageTextYOffSet = 0f;

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
        // Some wandering mechanic
        dirX = -rb.velocity.x;
        //
        UpdateAnimationState();

    }

    private void UpdateAnimationState()
    {

        if (is_hit)
        {
            state = EnemyState.hit;
        } 
        else if (dirX > 0f)
        {
            state = EnemyState.running;
            if (!is_facingRight)
            {
                Flip();
            }

        }
        else if (dirX < 0f)
        {
            state = EnemyState.running;
            if (is_facingRight)
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
        is_facingRight = !is_facingRight;
        tr.Rotate(0, 180f, 0);
    }

    public void TakeDamage(int damage)
    {
        is_hit = true;
        UpdateAnimationState();
        
        health -= damage;
        GameObject damageTxt = Instantiate(damageText, new Vector3(transform.position.x, transform.position.y + damageTextYOffSet, 0f), new Quaternion(0f, 0f, 0f, 0f));
        damageTxt.transform.GetChild(0).GetComponent<TextMeshPro>().SetText($"-{damage}");

        if (health <= 0)
        {
            Die();
        }
    }

    public void DisableHitState()
    {
        is_hit = false;
    }

    void Die()
    {
        GameObject dethEffect = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
        Destroy(dethEffect, 0.7f);
    }
}
