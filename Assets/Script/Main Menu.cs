using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuUI;
    public GameObject levelSelectUI;
    public void startGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void levelSelect()
    {
        mainMenuUI.SetActive(false);
        levelSelectUI.SetActive(true);
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
