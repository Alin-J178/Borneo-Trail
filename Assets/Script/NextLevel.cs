using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    public string NextLevelName; // Name of the next level to load

    public void LoadNextLevel()
    {
        Time.timeScale = 1f; // Resume the game if it was paused
        SceneManager.LoadScene(NextLevelName);
    }
}
