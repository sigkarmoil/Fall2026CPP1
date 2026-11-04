using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Template: fires a UnityEvent when this Rigidbody2D object touches another object.
/// Attach to the object that has the Rigidbody2D and a Collider2D.
/// </summary>

public class CollisionDetector2D : MonoBehaviour
{
    //public PlayerController player;
    [SerializeField] private PlayerController player;

    private void Start()
    {


        player = GetComponent<PlayerController>();

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            Debug.Log("pelletHit Script works");
            player.hitPellet();
        }
    }

    private void Update()
    {
        
    }
}
