using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;

    [SerializeField] private Transform meleeAttackHitBoxPos;
    [SerializeField] private float meleeAtackRadius;
    [SerializeField] private int meleeAtackDamage = 20;

    [SerializeField] LayerMask whatIsDamagable;

    private bool combatEnabled;
    private bool isAttacking;
    private bool isMeleeAttacking = false;
    private bool gotInputController = false;

    private float lastInputTime = Mathf.NegativeInfinity;
    [SerializeField] private float inputTimer = 0f;

    private float lastMeleeTime = Mathf.NegativeInfinity;
    [SerializeField] private float meleeTimer = 0f;

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckCombatInput();
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }


    private void CheckCombatInput()
    {
        if (Input.GetButtonDown("Fire1") && lastMeleeTime <= 0.0f)
        {
            Shoot();
        }

        if (Input.GetButtonDown("Melee"))
        {
            lastMeleeTime = meleeTimer;
            MeleeAttack();
        }

        if (lastMeleeTime > 0.0f)
        {
            lastMeleeTime -= Time.deltaTime;
        } 
        else
        {
            isMeleeAttacking = false;
        }
    }


    private void MeleeAttack()
    {
        isMeleeAttacking = true;
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(meleeAttackHitBoxPos.position, meleeAtackRadius, whatIsDamagable);
        

        foreach (Collider2D collider in detectedObjects)
        {
            collider.gameObject.SendMessage("TakeDamage", meleeAtackDamage);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(meleeAttackHitBoxPos.position, meleeAtackRadius);
    }
}
