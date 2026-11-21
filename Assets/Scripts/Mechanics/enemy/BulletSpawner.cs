using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private getBullet bulletPrefab;
    [SerializeField] private PlayerController player;
    [SerializeField] private float spawnTime = 2f;   // seconds between spawns

    private float timer;
    public bool canSpawnBullet = true;

    private void Start()
    {
        if (bulletPrefab == null || player == null)
        {
            Debug.LogError("BulletSpawner: bullet prefab or player not assigned");
        }
    }

    private void Update()
    {
        if (bulletPrefab == null || player == null) return;

        // Player can shoot -> stop spawning and reset the timer
        if (player.canShoot)
        {
            timer = 0f;
            return;
        }

        // Player can't shoot -> spawn an item every spawnTime seconds
        timer += Time.deltaTime;

        if (canSpawnBullet && timer >= spawnTime)
        {
            timer -= spawnTime;
            Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        }
    }
}