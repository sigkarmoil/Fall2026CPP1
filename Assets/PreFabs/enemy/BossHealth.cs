using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private LayerMask playerProjectileLayer;
    [SerializeField] private respawnPlayer respawner; // drag the player here
    [SerializeField] private GameObject objectToDestroy; // drag plaque4 here
    [SerializeField] private CameraFollow camf;

    private int currentHealth;
    private bool isDefeated;
    private bool wasRespawning;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (isDefeated || respawner == null) return;

        // Restore health once, on the frame the respawn begins
        if (respawner.isRespawning && !wasRespawning)
            currentHealth = maxHealth;

        wasRespawning = respawner.isRespawning;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDefeated) return;
        if (respawner != null && respawner.isRespawning) return; // ignore hits mid-respawn

        if ((playerProjectileLayer.value & (1 << other.gameObject.layer)) == 0) return;

        Destroy(other.gameObject); // consume the projectile
        currentHealth--;

        if (currentHealth <= 0)
            Defeat();
    }

    private void Defeat()
    {
        isDefeated = true;
        Debug.Log("you win!");
        Destroy(objectToDestroy != null ? objectToDestroy : gameObject);
        camf.SetFollow();
    }
}