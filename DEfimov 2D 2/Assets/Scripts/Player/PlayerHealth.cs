using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 2;
    [SerializeField] private float damageCooldown = 2f; 
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float blinkDuration = 0.1f; 
    [SerializeField] private Renderer playerRenderer; 
    private int currentHealth;
    private bool isInvincible = false;
    private Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;
        originalColor = playerRenderer.material.color;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && !isInvincible)
        {
            TakeDamage(1);
            StartCoroutine(DamageCooldown());
        }
    }

    void TakeDamage(int amount)
    {
        currentHealth -= amount;
        playerRenderer.material.color = damageColor;

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    IEnumerator DamageCooldown()
    {
        isInvincible = true;
        for (float i = 0; i < damageCooldown; i += blinkDuration * 2)
        {
            playerRenderer.material.color = damageColor;
            yield return new WaitForSeconds(blinkDuration);
            playerRenderer.material.color = originalColor;
            yield return new WaitForSeconds(blinkDuration);
        }

        isInvincible = false;
    }

    void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
