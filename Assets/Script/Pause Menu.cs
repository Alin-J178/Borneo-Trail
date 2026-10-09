using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject container;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            container.SetActive(true);
            Time.timeScale = 0f; // Pause or resume the game
        }
    }

    public void ResumeButton()
    {
        container.SetActive(false);
        Time.timeScale = 1f; // Resume the game
    }

    public void MainMenuButton()
    {
        Time.timeScale = 1f; // Resume the game before loading the main menu
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
    }
}
