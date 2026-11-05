using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    public virtual void OnPlayerPickup(PlayerController interactor) { }
    //public virtual void OnEnemyPickup(GameObject interactor) { }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnPlayerPickup(collision.GetComponent<PlayerController>());
            Destroy(gameObject);
        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.collider.CompareTag("Player"))
    //    {
    //        OnPlayerPickup(collision.collider.GetComponent<PlayerController>());
    //        Destroy(gameObject);
    //    }
    //}
}
