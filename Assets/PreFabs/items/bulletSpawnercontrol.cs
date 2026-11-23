using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class bulletSpawnercontrol : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private LayerMask bouncingLayer;

    [Tooltip("How horizontal a contact normal must be to count as a wall (0-1).")]
    [SerializeField, Range(0f, 1f)] private float wallNormalThreshold = 0.5f;

    private Rigidbody2D rb;
    private float direction = -1f; // -1 = left, 1 = right

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // Keep Y so gravity still works if the body is Dynamic.
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Only react to objects on the ground layer.
        if ((bouncingLayer.value & (1 << collision.gameObject.layer)) == 0)
            return;

        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 normal = collision.GetContact(i).normal;

            // Flip only when hitting a side surface that faces against our movement.
            if (Mathf.Abs(normal.x) >= wallNormalThreshold && normal.x * direction < 0f)
            {
                direction *= -1f;
                break;
            }
        }
    }
}