using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class coin : MonoBehaviour
{
  public AudioClip collectSound;

  private string collectionId;

  private void Awake()
  {
    collectionId = string.Format(
      CultureInfo.InvariantCulture,
      "{0}:{1:R}:{2:R}:{3:R}",
      SceneManager.GetActiveScene().buildIndex,
      transform.position.x,
      transform.position.y,
      transform.position.z
    );

    if (Player.HasCollectedCoin(collectionId))
    {
      gameObject.SetActive(false);
      Destroy(gameObject);
    }
  }

    private void OnTriggerEnter2D(Collider2D other)
    {
    if (!other.CompareTag("Player"))
        {
      return;
    }

    Player player = other.GetComponent<Player>();
    if (player == null || !player.CollectCoin(collectionId))
    {
      return;
    }

    if (collectSound != null)
    {
      AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

    Destroy(gameObject);
    }
}
