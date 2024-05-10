using UnityEngine;
using System.Collections;

public class PlayerControllerGrid : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1f; // Adjust this value to control the player's movement speed
    private bool isMoving = false;
    private Vector3 targetPosition;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!isMoving)
        {
            // Check for input
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            if (horizontalInput != 0 || verticalInput != 0)
            {
                // Calculate target position
                Vector3Int direction = new Vector3Int(Mathf.RoundToInt(horizontalInput), Mathf.RoundToInt(verticalInput), 0);
                targetPosition = transform.position + direction;
                
                // Check if the target position is valid
                if (IsValidMove(targetPosition))
                {
                    // Move the player to the target position
                    StartCoroutine(MovePlayer(targetPosition));

                    // Flip the player's sprite if moving left
                    if (horizontalInput < 0)
                    {
                        spriteRenderer.flipX = true;
                    }
                    // Flip the player's sprite back if moving right
                    else if (horizontalInput > 0)
                    {
                        spriteRenderer.flipX = false;
                    }
                }
            }
        }
    }

    IEnumerator MovePlayer(Vector3 target)
    {
        isMoving = true;
        while (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        isMoving = false;
    }

    bool IsValidMove(Vector3 position)
    {
        // Cast a ray from the current position to the target position
        RaycastHit2D hit = Physics2D.Raycast(transform.position, position - transform.position, Vector3.Distance(position, transform.position));
        
        // Check if the ray hit anything
        if (hit.collider != null)
        {
            // If the ray hit a wall or obstacle, the move is invalid
            return false;
        }
        
        // If the ray didn't hit anything, the move is valid
        return true;
    }
}
