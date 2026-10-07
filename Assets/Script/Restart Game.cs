using UnityEngine;

public class RestartGame : MonoBehaviour
{
    public void reloadcurrentScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");

        Time.timeScale = 1f; // Resume the game if it was paused

    }
}
