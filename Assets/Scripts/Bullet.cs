using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject impactEffect;

    [SerializeField] private float speed = 20f;
    [SerializeField] private int damage = 20;
    [SerializeField] private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage); 

        }
        GameObject impctEffect = Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(gameObject);
        Destroy(impctEffect, 0.6f);
    }

}
