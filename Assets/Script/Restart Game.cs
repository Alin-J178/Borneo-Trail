using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    public void reloadcurrentScene()
    {
        Time.timeScale = 1f;
        Checkpoint.ClearCheckpoint();
        Player.ResetCoinProgress();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
