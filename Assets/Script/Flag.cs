using TMPro;
using UnityEngine;

public class Flag : MonoBehaviour
{
    public GameObject winUi;
    public TMP_Text winText;

    private bool hasWon;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasWon && collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player == null)
            {
                Debug.LogError("Flag trigger requires a Player component.", this);
                return;
            }

            int totalSeconds = Mathf.FloorToInt(Time.timeSinceLevelLoad);
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            if (winText != null)
            {
                winText.enableAutoSizing = true;
                winText.fontSizeMin = 120f;
                winText.fontSizeMax = 120f;
                winText.fontSize = 120f;
                winText.text = string.Format(
                    "You Win\nTime: {0:00}:{1:00}  |  Coins: {2}",
                    minutes,
                    seconds,
                    player.coin
                );
            }
            else
            {
                Debug.LogError("Assign the win text on the Flag component.", this);
            }

            hasWon = true;
            Time.timeScale = 0f;
            winUi.SetActive(true);
        }
    }
}
