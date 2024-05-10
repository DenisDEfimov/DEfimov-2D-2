using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollingEnemy : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f; 
    [SerializeField] private Transform groundDetectionLeft; 
    [SerializeField] private bool startFacingLeft = true; 
    private Transform player; 

    private Rigidbody2D rb;
    private bool isFacingLeft; 
    private bool isChasing = false; 
    [SerializeField] private float detectionRadius = 5f; 
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        isFacingLeft = startFacingLeft;
        if (!startFacingLeft)
        {
            Vector3 newScale = transform.localScale;
            newScale.x *= -1;
            transform.localScale = newScale;
        }
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) <= detectionRadius)
        {
            isChasing = true;
        }
        else
        {
            isChasing = false;
        }
        if (isChasing)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
        else
        {
            rb.velocity = new Vector2((isFacingLeft ? -1 : 1) * moveSpeed, rb.velocity.y);
            RaycastHit2D groundInfoLeft = Physics2D.Raycast(groundDetectionLeft.position, Vector2.left, 0.1f);
            if (groundInfoLeft.collider != null && (groundInfoLeft.collider.CompareTag("Ground") || groundInfoLeft.collider.CompareTag("Enemy")))
            {
                Flip();
            }
        }
    }

    void Flip()
    {
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
        isFacingLeft = !isFacingLeft;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
