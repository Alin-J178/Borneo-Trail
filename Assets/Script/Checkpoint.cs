using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;

    private static bool hasCheckpoint;
    private static bool respawnAtCheckpointOnNextLoad;
    private static int checkpointSceneIndex;
    private static Vector3 checkpointPosition;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        checkpointPosition = respawnPoint != null
            ? respawnPoint.position
            : transform.position;
        checkpointSceneIndex = SceneManager.GetActiveScene().buildIndex;
        hasCheckpoint = true;
    }

    public static bool TryGetPositionForCurrentScene(out Vector3 position)
    {
        position = checkpointPosition;
        bool shouldRespawnAtCheckpoint =
            respawnAtCheckpointOnNextLoad &&
            hasCheckpoint &&
            checkpointSceneIndex == SceneManager.GetActiveScene().buildIndex;
        respawnAtCheckpointOnNextLoad = false;
        return shouldRespawnAtCheckpoint;
    }

    public static void PrepareRespawnAtCheckpoint()
    {
        respawnAtCheckpointOnNextLoad = true;
    }

    public static void ClearCheckpoint()
    {
        hasCheckpoint = false;
        respawnAtCheckpointOnNextLoad = false;
        checkpointSceneIndex = -1;
        checkpointPosition = Vector3.zero;
    }
}