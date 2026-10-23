using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [SerializeField] private float startingHealth;
    [SerializeField] private float damagePerHit = 10f;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float bounceUpSpeed = 8f;
    public float currentHealth { get; private set; }

    private Player player;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine blinkCoroutine;

    private void Awake()
    {
        currentHealth = startingHealth;
        player = GetComponent<Player>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Damage"))
        {
            HandleDamageTile(collision.collider);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Damage"))
        {
            HandleDamageTile(other);
        }
    }

    private void HandleDamageTile(Collider2D damageTile)
    {
        TakeDamage(damagePerHit);

        if (currentHealth > 0 && player != null)
        {
            player.ApplyKnockback(knockbackForce, bounceUpSpeed);
        }
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0)
        {
            return;
        }

        if (player != null)
        {
            player.PlayHurtSFX();
        }

        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);

        if (currentHealth > 0)
        {
            if (spriteRenderer != null)
            {
                if (blinkCoroutine != null)
                {
                    StopCoroutine(blinkCoroutine);
                }

                blinkCoroutine = StartCoroutine(BlinkRed());
            }
        }
        else
        {
            Checkpoint.PrepareRespawnAtCheckpoint();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private IEnumerator BlinkRed()
    {
        for (int i = 0; i < 3; i++)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }

        blinkCoroutine = null;
    }

}
