using UnityEngine;
using UnityEngine.Pool;

public class cPellet : MonoBehaviour
{
    public float pelletForce = 0.3f;

    [SerializeField, Min(0f)] private float speed = 6f;
    [SerializeField, Min(0.01f)] private float lifetime = 5f;
    private Collider2D attackBox;

    // Set by the spawner right after Get(). If null, the pellet falls back to Destroy.
    public IObjectPool<cPellet> Pool { private get; set; }

    private PlayerController player;
    private float age;
    private bool released;



    // Lets the spawner assign the attack box, since a prefab cannot hold scene references.
    public void SetAttackBox(Collider2D box)
    {
        attackBox = box;
        player = box != null ? box.GetComponentInParent<PlayerController>() : null;
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
        if (other == attackBox && player != null)
            player.hitPellet();

        Release();
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