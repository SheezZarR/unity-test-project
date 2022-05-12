using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLevel : MonoBehaviour
{
    private AudioSource finishSFX;

    private bool levelCompleted = false;
    // Start is called before the first frame update
    void Start()
    {
        finishSFX = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() { }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.name == "Player" && !levelCompleted){
            finishSFX.Play();
            Invoke("CompleteLevel", 2f);
        }
    }

    private void CompleteLevel() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
