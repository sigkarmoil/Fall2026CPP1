using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float groundCheckRadius = 0.05f;
    public bool isGrounded { get; private set; }

    private Collider2D col;
    private Rigidbody2D rb;
    private Vector2 groundcheckPos => CalculateGroundCheckPos();

    // Foot position helper function to calculate the ground check position based on the collider's bounds
    private Vector2 CalculateGroundCheckPos()
    {
        Bounds bounds = col.bounds;
        return new Vector2(bounds.center.x, bounds.min.y);
        //Debug.Log("CalculateGroundCheck is running");
    }

    public void Init(Collider2D col, Rigidbody2D rb)
    {
        this.col = col;
        this.rb = rb;
    }

    public bool checkGround()
    {
        if (!isGrounded && rb.linearVelocityY <= 0 || isGrounded)
        {
            isGrounded = Physics2D.OverlapCircle(groundcheckPos, groundCheckRadius, groundLayer);
            //Debug.Log("Player is grounded");
        }
        return isGrounded;
    }

    public void setGroundAfterJump()
    {
        isGrounded = false;
    }
}