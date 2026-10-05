using UnityEngine;

public class coin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
          if (other.gameObject.tag == "Player")
        {
           Player player = other.gameObject.GetComponent<Player>();
           player.coin += 1;
             Destroy(gameObject);
        }
    }
}
