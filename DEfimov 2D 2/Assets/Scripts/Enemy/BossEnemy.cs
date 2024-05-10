using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootPoint; 
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float shootInterval = 1f; 
    [SerializeField] private float stateChangeInterval = 3f;
    [SerializeField] private Transform groundDetection;

    private Rigidbody2D rb;
    private BossState currentState = BossState.Idle;
    private float timer = 0f;
    private float shootTimer = 0f;
    private enum BossState
    {
        Idle,
        Jump,
        Shoot
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= stateChangeInterval)
        {
            SwitchState();
            timer = 0f;
        }
        shootTimer += Time.deltaTime;
        ExecuteState();
    }

    void SwitchState()
    {
        currentState = (BossState)Random.Range(0, 3);
    }

    void ExecuteState()
    {
        switch (currentState)
        {
            case BossState.Idle:
                break;
            case BossState.Jump:
                Jump();
                break;
            case BossState.Shoot:
                if (shootTimer >= shootInterval)
                {
                    Shoot();
                    shootTimer = 0f;
                }
                break;
        }
    }

    void Jump()
{
    Vector2 groundDetectionPoint = groundDetection.position;
    RaycastHit2D groundCheck = Physics2D.Raycast(groundDetectionPoint, Vector2.down, 0.1f);
    if (groundCheck.collider != null && groundCheck.collider.CompareTag("Ground"))
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }
}

    void Shoot()
    {
        Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
    }
}
