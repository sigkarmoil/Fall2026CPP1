using Unity.VisualScripting;
using UnityEngine;

//Add require component to enforce having certain component
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]

public class PlayerController : MonoBehaviour
{
    public float movesSpeed = 5f; // public allows it to be edited on UI

    public float jumpForce = 5f;
    public Rigidbody2D rb;

    private SpriteRenderer sr;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        bool jumpPressed = Input.GetButtonDown("Jump");

        rb.linearVelocityX = moveX * movesSpeed; // move left and right

        if (jumpPressed)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        if (moveX != 0f)
        {
            SpriteFlip(moveX);
        }
        
    }

    void SpriteFlip(float moveX) => sr.flipX = (moveX < 0f);
    //{
    //    if (sr.flipX && moveX > 0f || !sr.flipX && moveX < 0f)
    //    {
    //        sr.flipX = !sr.flipX;
    //    }

    //}
}
