using UnityEngine;

public class PlayerBullet : Bullet
{
    protected override void Start()
    {
        base.Start();
        Fly();
    }

    protected override void Fly() => rb.velocity = new Vector2(bulletSpeed, 0f);

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("Enemy") || collision.CompareTag("Wall")) Destroy(gameObject);
    }
}
