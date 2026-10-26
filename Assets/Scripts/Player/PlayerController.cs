using Unity.VisualScripting;
using UnityEngine;

//Add require component to enforce having certain component
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent (typeof(Animator))]

public class PlayerController : MonoBehaviour
{
    #region Tunable Parameters
    public float movesSpeed = 5f; // public allows it to be edited on UI

    public float jumpForce = 5f;
    public Rigidbody2D rb;

    private SpriteRenderer sr;
    private BoxCollider2D bc;
    private Animator anim;
    public float attackForceMultiplier = 0.5f;

    private Vector2 CalculateGroundCheckPos()
    {
        Bounds bounds = bc.bounds; // what is bounds in Unity?
        return new Vector2(bounds.center.x, bounds.min.y);
    }


    #endregion

    #region Ground Check Parameters

    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;
    public bool isGrounded;


    #endregion

    #region
    public bool isAttack = false;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        #region Groundcheck
        //if (rb.linearVelocityY == 0f )
        //{
        //    isGrounded = true;
        //}
        //else
        //{
        //    isGrounded = false;
        //}

        if (rb.linearVelocityY <= 0f)
        {
            isGrounded = Physics2D.OverlapCircle(CalculateGroundCheckPos(), groundCheckRadius, groundLayer);
        }
        #endregion

        //float yInput = Input.GetAxisRaw("Vertical");
        bool upPressed = Input.GetAxisRaw("Vertical") > 0f;
        float moveX = Input.GetAxisRaw("Horizontal");
        bool jumpPressed = Input.GetButtonDown("Jump");
        bool attackPressed = Input.GetButtonDown("Fire1");


        #region movement
        rb.linearVelocityX = moveX * movesSpeed; // move left and right
        #endregion

        #region attack
        if (attackPressed && !upPressed)
        {
            anim.SetTrigger("regularAttack");
            rb.AddForce(Vector2.up * jumpForce * attackForceMultiplier, ForceMode2D.Impulse);
        }
        #endregion

        #region jump
        if (jumpPressed && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce , ForceMode2D.Impulse);
        }

        #endregion

        #region twirlingAttack
        if (upPressed && attackPressed)
        {
            anim.SetTrigger("twirlingAttack");
        }   
        #endregion

        #region flip
        if (moveX != 0f)
        {
            SpriteFlip(moveX);
        }
        #endregion

        #region Animator Variable
        //Connect Animator with 
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("moveX", Mathf.Abs(moveX));
        anim.SetBool("isAttack", isAttack);
        #endregion
    }
    void SpriteFlip(float moveX) => sr.flipX = (moveX < 0f);
    #region Attack Animation

    public void isAttackOn()
    {
        isAttack = true;
    }

    public void isAttackOff(){
        isAttack = false;
    }

    #endregion

    void gravityOff()
    {
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

    }

    void gravityOn()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    public void twirlForce()
    {
        rb.AddForce(Vector2.up * jumpForce * attackForceMultiplier, ForceMode2D.Impulse);
    }

}

    //{
    //    if (sr.flipX && moveX > 0f || !sr.flipX && moveX < 0f)
    //    {
    //        sr.flipX = !sr.flipX;
    //    }

    //}

