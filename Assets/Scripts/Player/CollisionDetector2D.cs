using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Template: fires a UnityEvent when this Rigidbody2D object touches another object.
/// Attach to the object that has the Rigidbody2D and a Collider2D.
/// </summary>
[RequireComponent(typeof(Collider2D))]

public class CollisionDetector2D : MonoBehaviour
{
    //public PlayerController player;
    public enum ContactMode
    {
        Collision, // good for collision -> OnCollisionEnter2D
        Trigger    // overlap, good for hitbox  -> OnTriggerEnter2D (collider needs Is Trigger on) 
    }

    [Header("Detection")]
    //Change contact mode here
    [SerializeField] private ContactMode contactMode = ContactMode.Trigger;



    [Header("Response")]
    [Tooltip("Called once when contact starts. Passes the other GameObject. Insert the gameobject to respond with.")]
    [SerializeField] private UnityEvent<GameObject> onContactEnter;

    // ---- Unity callbacks ----



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (contactMode != ContactMode.Collision) return;
        HandleContact(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (contactMode != ContactMode.Trigger) return;
        //insert other criterias here
        {
            HandleContact(other.gameObject);
        }

        
    }

    // ---- Logic ----

    private void HandleContact(GameObject other)
    {
        onContactEnter?.Invoke(other);
    }




}
