using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class cTurret : MonoBehaviour
{
    [SerializeField] private cPellet pelletPrefab;
    [SerializeField] private Collider2D attackBox;
    [Tooltip("c-runtime: empty root at 0,0,0, no rotation. Pellets are parented here, never to c-Stream.")]
    [SerializeField] private Transform runtimeParent;

    [Tooltip("Leave empty to use the sensor on this GameObject.")]
    [SerializeField] private LineOfSightSensor2D sensor;

    [Header("Visual")]
    [Tooltip("Child object that holds the SpriteRenderer. Only this rotates. " +
             "Empty = rotate this object (old behaviour).")]
    [SerializeField] private Transform turretSprite;

    [Tooltip("Empty child of turretSprite at the barrel tip, on the sprite's center line. " +
             "Empty = pellets spawn at the sprite pivot.")]
    [SerializeField] private Transform firePoint;

    [Header("Firing")]
    [Tooltip("Seconds between spotting the target and the first pellet. 0 = fire immediately.")]
    [SerializeField, Min(0f)] private float initialDelay = 0.5f;

    [Tooltip("Total pellets fired per sighting, then it stops. 0 = unlimited.")]
    [SerializeField, Min(0)] private float maxProjectiles = 8;

    [Tooltip("Seconds between pellets inside a burst.")]
    [SerializeField, Min(0.01f)] private float spawnDelay = 0.5f;

    [Tooltip("Pellets fired before the spawner pauses.")]
    [SerializeField, Min(1)] private int burstSize = 5;

    [Tooltip("Seconds of pause after a burst.")]
    [SerializeField, Min(0f)] private float pauseDuration = 1f;

    [Header("Pool")]
    [SerializeField, Min(1)] private int defaultCapacity = 20;
    [SerializeField, Min(1)] private int maxPoolSize = 100;

    private IObjectPool<cPellet> pool;
    private Coroutine fireRoutine;
    private SpriteRenderer spriteRenderer;

    private Transform Muzzle => firePoint != null ? firePoint : turretSprite;

    private void Awake()
    {
        if (sensor == null) sensor = GetComponent<LineOfSightSensor2D>();

        // No child assigned: fall back to rotating this object, as before.
        if (turretSprite == null) turretSprite = transform;
        spriteRenderer = turretSprite.GetComponent<SpriteRenderer>();

        pool = new ObjectPool<cPellet>(
            createFunc: CreatePellet,
            actionOnGet: null,
            actionOnRelease: p => p.gameObject.SetActive(false),
            actionOnDestroy: p => { if (p != null) Destroy(p.gameObject); },
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxPoolSize);
    }

    private void OnEnable()
    {
        sensor.LineOfSightGained += StartFiring;
        sensor.LineOfSightLost += StopFiring;
    }

    private void OnDisable()
    {
        sensor.LineOfSightGained -= StartFiring;
        sensor.LineOfSightLost -= StopFiring;
        StopFiring();
    }

    private void FixedUpdate()
    {
        if (!sensor.HasLineOfSight) return;

        Aim(sensor.DirectionToTarget);
    }

    private void StartFiring()
    {
        if (fireRoutine != null) return;   // never stack coroutines

        Aim(sensor.DirectionToTarget);
        fireRoutine = StartCoroutine(FireRoutine());
    }

    private void StopFiring()
    {
        if (fireRoutine == null) return;

        StopCoroutine(fireRoutine);
        fireRoutine = null;
    }

    // cPellet travels along local -X (Vector3.left), so point local -X at the target.
    // Only turretSprite rotates; the root (collider, sensor) stays still.
    private void Aim(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180f;
        turretSprite.rotation = Quaternion.Euler(0f, 0f, angle);

        // Aiming right means a ~180 degree rotation, which turns the art upside down.
        // Flipping Y cancels that. Assumes the art's front faces local -X (left).
        if (spriteRenderer != null)
            spriteRenderer.flipY = direction.x > 0f;
    }

    private cPellet CreatePellet()
    {
        cPellet pellet = Instantiate(pelletPrefab, runtimeParent);
        pellet.Pool = pool;
        pellet.SetAttackBox(attackBox);
        pellet.gameObject.SetActive(false);
        return pellet;
    }

    private IEnumerator FireRoutine()
    {
        // Initial delay. Re-read every frame so it can be tuned live in the Inspector.
        // With 0 the loop never runs, so the first pellet fires immediately.
        float waited = 0f;
        while (waited < initialDelay)
        {
            waited += Time.deltaTime;
            yield return null;
        }

        int total = 0;
        int inBurst = 0;

        while (maxProjectiles == 0 || total < maxProjectiles)
        {
            Fire();
            total++;
            inBurst++;

            // Nothing left to wait for after the last pellet.
            if (maxProjectiles != 0 && total >= maxProjectiles)
                break;

            bool burstDone = inBurst >= burstSize;
            if (burstDone) inBurst = 0;

            // do/while always yields at least one frame, so a zero wait can never freeze the editor.
            float elapsed = 0f;
            do
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
            while (elapsed < (burstDone ? pauseDuration : spawnDelay));
        }

        // Finished on its own: clear the handle so the next sighting can start again.
        fireRoutine = null;
    }

    private void Fire()
    {
        cPellet pellet = pool.Get();

        // Position from the muzzle, rotation from the aimed sprite.
        pellet.transform.SetPositionAndRotation(Muzzle.position, turretSprite.rotation);
        pellet.gameObject.SetActive(true);
    }

    // Scene-view arrow showing the firing direction (local left, matching cPellet).
    private void OnDrawGizmos()
    {
        Transform aimed = turretSprite != null ? turretSprite : transform;
        Transform origin = firePoint != null ? firePoint : aimed;

        Gizmos.color = Color.yellow;
        Vector3 tip = origin.position - aimed.right;
        Gizmos.DrawLine(origin.position, tip);
        Gizmos.DrawSphere(tip, 0.07f);
    }
}
