using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

//Add require component to enforce having certain component
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent (typeof(Animator), (typeof(GroundCheck)))]

public class PlayerController : MonoBehaviour
{


    #region Tunable Parameters
    public float movesSpeed = 5f; // public allows it to be edited on UI

    public float jumpForce = 5f;
    public Rigidbody2D rb;

    private SpriteRenderer sr;
    private BoxCollider2D bc;
    private Animator anim;
    private GroundCheck gndChk;

    [SerializeField] private Collider2D attackCollider;

    public float attackForceMultiplier = 0.5f;

    //public float freezeTimer = 0.2f;

    //private Vector2 CalculateGroundCheckPos()
    //{
    //    Bounds bounds = bc.bounds; // what is bounds in Unity?
    //    return new Vector2(bounds.center.x, bounds.min.y);
    //}


    #endregion
    #region freezeTime
    private bool isFrozen;
    public float hitStopTime = 0.2f;
    #endregion
    #region Ground Check Parameters

    //public Transform groundCheck;
    //public LayerMask groundLayer;
    //public float groundCheckRadius = 0.2f;
    //public bool isGrounded;


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

        gndChk = GetComponent<GroundCheck>();
        gndChk.Init(bc, rb);


        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
    }

    // Update is called once per frame
    void Update()
    {
        #region Groundcheck

        gndChk.checkGround();

        #region Regular Attack Reset
        if (gndChk.checkGround() == true)
        {
            isAttack = true;
        }
        #endregion
        #endregion

        bool upPressed = Input.GetAxisRaw("Vertical") > 0f;
        float moveX = Input.GetAxisRaw("Horizontal");
        bool jumpPressed = Input.GetButtonDown("Jump");
        bool attackPressed = Input.GetButtonDown("Fire1");
        bool shootPressed = Input.GetButtonDown("Fire2");


        #region movement
        rb.linearVelocityX = moveX * movesSpeed; // move left and right
        #endregion

        #region attack
        if (attackPressed && !upPressed && isAttack)
        {
            isAttack = false;
            anim.SetTrigger("regularAttack");
            //rb.AddForce(Vector2.up * jumpForce * attackForceMultiplier, ForceMode2D.Impulse);
        }
        #endregion

        #region jump


        if (jumpPressed && gndChk.isGrounded)
        {
            gndChk.setGroundAfterJump();
            rb.linearVelocityY = 0f;
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
        anim.SetBool("isGrounded", gndChk.isGrounded);
        anim.SetFloat("moveX", Mathf.Abs(moveX));
        anim.SetBool("isAttack", isAttack);
        #endregion


        if (shootPressed)
        {
            anim.SetTrigger("shoot");
        }
    }
    void SpriteFlip(float moveX) => sr.flipX = (moveX < 0f);
    #region Attack Animation

    public void isAttackOn()
    {
        //isAttack = true;
        attackCollider.enabled = true;
    }

    public void isAttackOff(){
        //isAttack = false;
        attackCollider.enabled = false;
    }

    #endregion

    void gravityOff()
    {

        //rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0.0f;

    }

    void gravityOn()
    {
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0.3f;
        //rb.bodyType = RigidbodyType2D.Dynamic;
    }

    public async Task twirlForce()
    {
        rb.AddForce(Vector2.up * jumpForce * attackForceMultiplier, ForceMode2D.Impulse);
    }
    public void hitPellet()
    {
        isAttack = true;
        rb.AddForce(Vector2.up * jumpForce * attackForceMultiplier, ForceMode2D.Impulse);
        FreezeTime(hitStopTime);
        Debug.Log("hitPellet called");
    }

    //public void hitStop()
    //{
    //    //sr.color = Color.white;
    //    FreezeTime(hitStopTime);
    //    //sr.color = Color;

    //}
    public void FreezeTime(float duration)
    {
        if (!isFrozen)
            StartCoroutine(FreezeRoutine(duration));
    }

    private System.Collections.IEnumerator FreezeRoutine(float duration)
    {
        isFrozen = true;
        float previousScale = Time.timeScale;

        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = previousScale;
        isFrozen = false;
    }

}
