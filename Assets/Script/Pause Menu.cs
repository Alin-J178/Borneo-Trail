using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject container;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenPauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        if (container == null)
        {
            Debug.LogError("PauseMenu requires a container.", this);
            return;
        }

        if (container.activeSelf)
        {
            ResumeButton();
        }
        else
        {
            OpenPauseMenu();
        }
    }

    private void OpenPauseMenu()
    {
        if (container == null)
        {
            Debug.LogError("PauseMenu requires a container.", this);
            return;
        }

        container.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeButton()
    {
        container.SetActive(false);
        Time.timeScale = 1f; // Resume the game
    }

    public void RestartButton()
    {
        Time.timeScale = 1f;
        Checkpoint.ClearCheckpoint();
        Player.ResetCoinProgress();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenuButton()
    {
        Time.timeScale = 1f; // Resume the game before loading the main menu
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
    }
}
