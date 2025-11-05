using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{

    private Vector2 startPos;
    [Header("Animation")]
    [SerializeField] private Animator Enemyanimator;
    [Header("Blood Effect")]
    [SerializeField] private GameObject bloodSplash;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Example: hit enemy
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("Egg hit enemy: " + collision.gameObject.name);

            // Spawn blood only when killed by bullet
            if (bloodSplash != null)
            {
                Instantiate(bloodSplash, transform.position, Quaternion.identity);
            }

            // Destroy enemy and bullet
            Destroy(collision.gameObject); // bullet
            Destroy(gameObject);           // enemy
        }
    }
}
