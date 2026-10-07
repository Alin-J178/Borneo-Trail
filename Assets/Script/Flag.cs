using Unity.VisualScripting;
using UnityEngine;

public class Flag : MonoBehaviour
{
    public GameObject winUi;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            Time.timeScale = 0f; // Pause the game
            winUi.SetActive(true);
        }
    }
}
