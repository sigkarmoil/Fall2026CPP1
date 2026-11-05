using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask hardLayer;
    [SerializeField] private LayerMask veinLayer;
    [SerializeField] private float groundCheckRadius = 0.05f;

    public bool isGrounded { get; private set; }

    private Collider2D col;
    private Rigidbody2D rb;
    private LayerMask combinedMask;

    private Vector2 groundCheckPos => CalculateGroundCheckPos();

    private void Awake()
    {
        // OR the three masks into one. LayerMask converts implicitly to int and back.
        combinedMask = groundLayer | hardLayer | veinLayer;
    }

    // Foot position: bottom-center of the collider bounds
    private Vector2 CalculateGroundCheckPos()
    {
        Bounds bounds = col.bounds;
        return new Vector2(bounds.center.x, bounds.min.y);
    }

    public void Init(Collider2D col, Rigidbody2D rb)
    {
        this.col = col;
        this.rb = rb;
    }

    public bool checkGround()
    {
        // Skip re-grounding while rising after a jump
        if (isGrounded || rb.linearVelocityY <= 0)
        {
            // One query, one assignment. Non-null collider means grounded.
            isGrounded = Physics2D.OverlapCircle(groundCheckPos, groundCheckRadius, combinedMask) != null;
        }
        return isGrounded;
    }

    public void setGroundAfterJump()
    {
        isGrounded = false;
    }
}