using UnityEngine;
using UnityEngine.SceneManagement;

public class KillZone : MonoBehaviour
{

    // Note: If you are making a 2D game, use this function instead:
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Checkpoint.PrepareRespawnAtCheckpoint();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    
}