using UnityEngine;

// Attach to the spike. Drag the player (the object with respawnPlayer) into the "player" field.
// Works whether the spike collider is a trigger or a solid collider.
public class spikeReset : MonoBehaviour
{
    [SerializeField] private respawnPlayer player;

    private void OnTriggerEnter2D(Collider2D other) => tryRespawn(other.gameObject);

    private void OnCollisionEnter2D(Collision2D collision) => tryRespawn(collision.gameObject);

    private void tryRespawn(GameObject other)
    {
        if (!other.CompareTag("Player")) return;

        if (player == null)
        {
            Debug.LogWarning("spikeReset: player is not assigned.", this);
            return;
        }

        player.respawn();
    }
}
