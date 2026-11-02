using UnityEngine;

public class Shoot : MonoBehaviour
{
    #region variable declaration
    private SpriteRenderer sr; // set this so we can change the sprite when destroyed
    [SerializeField] private Vector2 initShotVelocity = new Vector2(0, 5);
    [SerializeField] private Transform spawnUpward;
    //[SerializeField] private Transform spawnPointRight;
    [SerializeField] private Projectile projectilePrefab;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (initShotVelocity == Vector2.zero)
        {
            initShotVelocity = new Vector2(0, 5);
            Debug.LogWarning("Shoot: InitShotVelocity not defined - setting to a default 0, 5");
        }

        if (spawnUpward == null || projectilePrefab == null)
        {
            Debug.LogError("Shoot: one or more spawn points or projectile prefab is not assigned - in order to use the shoot component - it has to be assigned");
        }
    }

    // Update is called once per frame
    public void Fire()
    {

        if (spawnUpward == null || projectilePrefab == null)
        {
            Debug.LogError("Fire will not work because the shoot script is missing a spawn point or projectile prefab reference");
            return;
        }

        Projectile curProjectile;
        curProjectile = Instantiate(projectilePrefab, spawnUpward.position, Quaternion.identity);
        curProjectile.SetVelocity(initShotVelocity);

    }
}