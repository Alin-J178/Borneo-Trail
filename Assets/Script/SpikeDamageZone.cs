
using System.Collections.Generic;
using UnityEngine;

public class SpikeDamageZone : MonoBehaviour
{
    private readonly HashSet<Collider2D> playersInside =
        new HashSet<Collider2D>();

    public void CheckForDamage()
    {
        foreach (Collider2D player in playersInside)
        {
            if (player == null) continue;

            Component health =
                player.GetComponentInParent(typeof(MonoBehaviour));

            if (health != null)
            {
                health.SendMessage("TakeDamage", 1, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playersInside.Add(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        playersInside.Remove(other);
    }
}
