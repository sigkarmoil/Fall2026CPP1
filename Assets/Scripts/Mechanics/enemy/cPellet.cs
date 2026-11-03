using UnityEngine;
using UnityEngine.Pool;

public class cPellet : MonoBehaviour
{
    public PlayerController player;
    public float pelletForce = 0.3f;
    #region adjustable Fields
    [SerializeField, Min(0f)] private float speed = 6f;
    [SerializeField, Min(0.01f)] private float lifetime = 5f;

    // Set by the spawner right after Get(). If null, the pellet falls back to Destroy.
    public IObjectPool<cPellet> Pool { private get; set; }
    #endregion
    private float age;
    private bool released;

    //[SerializeField] LayerMask releaseLayer;
    [SerializeField] private LayerMask launchLayers; 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Translate defaults to Space.Self, so the pellet keeps the angle it spawned with.
        transform.Translate(Vector3.left * (speed * Time.deltaTime));

        age += Time.deltaTime;
        if (age >= lifetime)
            Release();

    }

    //Put item back into pool when pellets touch collider.
    //OnTriggerEnter2D causes trigger when it touches layer



    private void OnTriggerEnter2D(Collider2D other)
    {
        int layerBit = 1 << other.gameObject.layer;
    if ((launchLayers.value & layerBit) != 0)
        {
            pelletLaunchUpward();
            Release();
        }
        else
        {
            Release();
        }
    }


    private void OnEnable() // send back items to the pool
    {
        age = 0f;
        released = false;
    }

    private void Release()
    {
        // Two triggers can fire in one frame. Releasing twice throws when pool collection checks are on.
        if (released == true) return;
        released = true;

        if (Pool != null)
            Pool.Release(this);
        else
            Destroy(gameObject);
    }

    public void pelletLaunchUpward()
    {
        //player.Rigidbody2D.AddForce(Vector2.up * player.jumpForce* pelletForce, ForceMode2D.Impulse);
        Debug.Log("pellet Launch Upward");

    }
}
