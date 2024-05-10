using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHP = 3;
    public float speed = 3f;
    protected int currentHP;

    protected virtual void Start() => currentHP = maxHP;

    protected virtual void Update() => MoveLeft();

    protected virtual void MoveLeft() => transform.Translate(Vector2.left * speed * Time.deltaTime);

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall")) Destroy(gameObject);
        else if (other.CompareTag("Player") || other.CompareTag("Bullet")) TakeDamage(1);
    }

    protected virtual void TakeDamage(int amount)
    {
        currentHP -= amount;
        if (currentHP <= 0) Destroy(gameObject);
    }
}
