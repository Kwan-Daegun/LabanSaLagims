using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float destroyObject = 5f;      // bullet auto-destroys after some time
    public GameObject Splash;             // splash particle prefab
    public AudioClip splashSound;         // assign in inspector

    private void Start()
    {
        Invoke(nameof(DestroySelf), destroyObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            SpawnSplashAndSound();
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }

    private void DestroySelf()
    {
        SpawnSplashAndSound();
        Destroy(gameObject);
    }

    private void SpawnSplashAndSound()
    {
        GameObject splashObj = null;

        // spawn splash particles
        if (Splash != null)
        {
            splashObj = Instantiate(Splash, transform.position, Quaternion.identity);
        }

        // spawn temporary audio source for sound
        if (splashSound != null)
        {
            GameObject audioObj = new GameObject("SplashSound");
            AudioSource audioSource = audioObj.AddComponent<AudioSource>();
            audioSource.clip = splashSound;
            audioSource.Play();

            // destroy audio object after clip ends
            Destroy(audioObj, splashSound.length);

            // also sync particle system with sound
            if (splashObj != null)
            {
                Destroy(splashObj, splashSound.length);
            }
        }
    }
}
