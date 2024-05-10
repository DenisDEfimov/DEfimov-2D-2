using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 10f;
    public Rigidbody2D rb;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Fly();
    }

    protected virtual void Fly() => rb.velocity = new Vector2(-bulletSpeed, 0f);

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("Player") || collision.CompareTag("Wall")) Destroy(gameObject);
    }
}
