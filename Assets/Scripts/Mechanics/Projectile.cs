using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float lifetime = 5.0f;

    [SerializeField]
    private Vector2 initVelocity = Vector2.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifetime);

        if(initVelocity != Vector2.zero)
        {
            SetVelocity(initVelocity);
        }
    }

    public void SetVelocity(Vector2 velocity)
    {
        GetComponent<Rigidbody2D>().linearVelocity = velocity;
    }

    //collision detection code
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            Debug.Log("Projectile hit the ground");
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {

    }

    private void OnCollisionExit2D(Collision2D collision)
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {

    }

    private void OnTriggerExit2D(Collider2D collision)
    {

    }
    // Update is called once per frame

}
