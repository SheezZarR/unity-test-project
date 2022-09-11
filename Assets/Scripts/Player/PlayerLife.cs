using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLife : MonoBehaviour
{
    
    [SerializeField] private AudioSource deathSFX;

    private Rigidbody2D p_rb;
    private Animator p_anim;

    // Start is called before the first frame update
    private void Start()
    {
        p_rb = GetComponent<Rigidbody2D>();
        p_anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.CompareTag("Trap"))
        {   
            deathSFX.Play();
            Die();
        }
    }

    private void Die() 
    {   
        p_rb.bodyType = RigidbodyType2D.Static;
        p_anim.SetTrigger("death");
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
