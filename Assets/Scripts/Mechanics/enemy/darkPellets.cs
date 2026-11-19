using UnityEngine;
using UnityEngine.Pool;

public class darkPellet : MonoBehaviour
{
    public float pelletForce = 0.3f;

    [SerializeField, Min(0f)] private float speed = 6f;
    [SerializeField, Min(0.01f)] private float lifetime = 5f;
    private Collider2D attackBox;

    // Set by the spawner right after Get(). If null, the pellet falls back to Destroy.
    public IObjectPool<darkPellet> Pool { private get; set; }

    private PlayerController player;
    private respawnPlayer respawner;
    private float age;
    private bool released;



    // Lets the spawner assign the attack box, since a prefab cannot hold scene references.
    public void SetAttackBox(Collider2D box)
    {
        attackBox = box;
        player = box != null ? box.GetComponentInParent<PlayerController>() : null;
        respawner = box != null ? box.GetComponentInParent<respawnPlayer>() : null;
    }

    private void OnEnable()
    {
        age = 0f;
        released = false;
    }

    private void Update()
    {
        transform.Translate(Vector3.left * (speed * Time.deltaTime));

        age += Time.deltaTime;
        if (age >= lifetime)
            Release();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"pellet hit {other.name}", other);
        if (other.CompareTag("Player"))
        {
            tryRespawn(other.gameObject);
        }

        Release();
    }

    // Pellet hit something that is not the attack box: respawn the player if it is the player.
    private void tryRespawn(GameObject other)
    {
        if (!other.CompareTag("Player")) return;

        if (respawner == null)
        {
            Debug.LogWarning("darkPellet: respawner is not set.", this);
            return;
        }

        respawner.respawn();
    }

    private void Release()
    {
        if (released) return;
        released = true;

        if (Pool != null)
            Pool.Release(this);
        else
            Destroy(gameObject);
    }
}
