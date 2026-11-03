using UnityEngine;

public class Shoot : MonoBehaviour
{
    #region variable declaration
    private SpriteRenderer sr; // set this so we can change the sprite when destroyed
    [SerializeField] private Vector2 initShotVelocity = new Vector2(0, 5);
    


    #region assignment1


    //Actual - 1
    //[SerializeField] private Transform spawnUpward;

    [SerializeField] private Transform spawnPointRight;

    [SerializeField] private Transform spawnPointLeft;

    #endregion

    //[SerializeField] private Transform spawnPointRight;
    [SerializeField] private Projectile projectilePrefab;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        #region assignment2

        //sr = GetComponent<SpriteRenderer>();

        //if (initShotVelocity == Vector2.zero)
        //{
        //    initShotVelocity = new Vector2(0, 5);
        //    Debug.LogWarning("Shoot: InitShotVelocity not defined - setting to a default 0, 5");
        //}

        //if (spawnUpward == null || projectilePrefab == null)
        //{
        //    Debug.LogError("Shoot: one or more spawn points or projectile prefab is not assigned - in order to use the shoot component - it has to be assigned");
        //}

        sr = GetComponent<SpriteRenderer>();

        //error warning 1
        if (initShotVelocity == Vector2.zero)
        {
            initShotVelocity = new Vector2(1, 1);
            Debug.LogWarning("Shoot: InitShotVelocity not defined - setting to a default 0, 5");
        }

        //error warning 2
        if (spawnPointLeft == null ||  spawnPointRight == null || projectilePrefab == null)
        {
            Debug.LogError("Shoot: one or more spawn points or projectile prefab is not assigned - in order to use the shoot component - it has to be assigned");
        }

        #endregion

    }

    // Update is called once per frame
    public void Fire()
    {

        #region assignment3
        //if (spawnUpward == null || projectilePrefab == null)
        //{
        //    Debug.LogError("Fire will not work because the shoot script is missing a spawn point or projectile prefab reference");
        //    return;
        //}

        //Projectile curProjectile;
        //curProjectile = Instantiate(projectilePrefab, spawnUpward.position, Quaternion.identity);
        //curProjectile.SetVelocity(initShotVelocity);
        if (spawnPointLeft == null || spawnPointRight == null || projectilePrefab == null)
        {
            Debug.LogError("Fire will not work because the shoot script is missing a spawn point or projectile prefab reference");
            return;
        }

        Projectile curProjectile;

        if (!sr.flipX)
        {
            curProjectile = Instantiate(projectilePrefab, spawnPointRight.position, Quaternion.identity);
            curProjectile.SetVelocity(initShotVelocity);
            Debug.Log("shoot right");
        }
        else if (sr.flipX)
        {
            #region debug
            Vector2 shotVelocity = sr.flipX
                ? new Vector2(-initShotVelocity.x, initShotVelocity.y)
                : initShotVelocity;

            Debug.Log($"Shoot: flipX={sr.flipX}, initShotVelocity={initShotVelocity}, sending={shotVelocity}");

            #endregion

            curProjectile = Instantiate(projectilePrefab, spawnPointLeft.position, Quaternion.identity);
            curProjectile.SetVelocity(new Vector2(-1 *(initShotVelocity.x), initShotVelocity.y));


        }
    }


        #endregion
}



