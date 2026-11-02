using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class cSpawner : MonoBehaviour
{
    [SerializeField] private cPellet pelletPrefab;
    [Tooltip("c-runtime: empty root at 0,0,0, no rotation. Pellets are parented here, never to c-Stream.")]
    [SerializeField] private Transform runtimeParent;

    [Header("Firing")]
    [Tooltip("Total pellets this spawner fires, then it stops. 0 = unlimited.")]
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

    private void Awake()
    {
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
        StartCoroutine(FireRoutine());
    }

    private cPellet CreatePellet()
    {
        cPellet pellet = Instantiate(pelletPrefab, runtimeParent);
        pellet.Pool = pool;
        pellet.gameObject.SetActive(false);
        return pellet;
    }

    private IEnumerator FireRoutine()
    {
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

            // Re-read the field every frame so values can be tuned live in the Inspector.
            // do/while always yields at least one frame, so a zero wait can never freeze the editor.
            float elapsed = 0f;
            do
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
            while (elapsed < (burstDone ? pauseDuration : spawnDelay));
        }
    }

    private void Fire()
    {
        cPellet pellet = pool.Get();
        pellet.transform.SetPositionAndRotation(transform.position, transform.rotation);
        pellet.gameObject.SetActive(true);
    }

    // Scene-view arrow showing the firing direction (local right).
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 tip = transform.position + transform.right;
        Gizmos.DrawLine(transform.position, tip);
        Gizmos.DrawSphere(tip, 0.07f);
    }
}

// Start is called once before the first execution of Update after the MonoBehaviour is created
