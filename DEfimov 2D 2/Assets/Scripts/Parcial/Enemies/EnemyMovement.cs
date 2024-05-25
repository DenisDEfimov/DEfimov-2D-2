using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    EnemyStats enemy;
    Transform player;
    
    private SpriteRenderer spriteRenderer;


    void Start()
    {
        enemy = GetComponent<EnemyStats>();
        player = FindObjectOfType<PlayerMovement>().transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = player.transform.position;
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, enemy.currentMoveSpeed * Time.deltaTime);    //Constantly move the enemy towards the player
        Vector2 direction = targetPosition - currentPosition;
        if (direction.x > 0) // Moving right
        {
            spriteRenderer.flipX = false;
        }
        else if (direction.x < 0) // Moving left
        {
            spriteRenderer.flipX = true;
        }
    }
}