using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFlyingEnemy : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float moveSpeed = 3f;

    private Transform player;
    private Vector3 initialPosition;
    private bool isChasing = false;

    void Start()
    {
        initialPosition = transform.position;
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
            Vector3 direction = (initialPosition - transform.position).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
