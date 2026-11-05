using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

//Add require component to enforce having certain component
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent (typeof(Animator), (typeof(GroundCheck)))]

public class PlayerController : MonoBehaviour
{


    #region Tunable Parameters
    public float movesSpeed = 5f; // public allows it to be edited on UI

    public float jumpForce = 5f;
    public Rigidbody2D rb;

    private SpriteRenderer sr;
    private CapsuleCollider2D bc;
    private Animator anim;
    private GroundCheck gndChk;

    [SerializeField] private Collider2D attackCollider;

    [SerializeField, Range(0.01f, 1f)] private float boostEndSpeedFactor = 0.1f; // speed at the very end, as a fraction of peak speed

    public float attackForceMultiplier = 0.5f;

    //public float freezeTimer = 0.2f;

    //private Vector2 CalculateGroundCheckPos()
    //{
    //    Bounds bounds = bc.bounds; // what is bounds in Unity?
    //    return new Vector2(bounds.center.x, bounds.min.y);
    //}


    #endregion

    #region Boost
    [SerializeField] private float boostHeight = 4f;
    [SerializeField] private float boostTime = 0.3f;

    [SerializeField] private LayerMask groundLayer;

    #region power management
    public int currentPower = 0;
    [SerializeField] private int maxPower = 5;
    #endregion

    private float boostVelocity;
    private float boostTargetY;
    private bool isBoosting;
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
        bc = GetComponent<CapsuleCollider2D>();
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
        if (upPressed && attackPressed && currentPower >=1)
        {
            anim.SetTrigger("twirlingAttack");
            currentPower -= 1;
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
        //anim.SetBool("isAttack", isAttack);
        #endregion


        if (shootPressed)
        {
            anim.SetTrigger("shoot");
        }
    }
    void SpriteFlip(float moveX) => sr.flipX = (moveX < 0f);

    #region Fixed Updates
    void FixedUpdate()
    {
        if (!isBoosting) return;

        boostUp();
    }
    #endregion

    #region Boost Ceiling Stop
    void OnCollisionEnter2D(Collision2D col) => CheckBoostCeiling(col);
    void OnCollisionStay2D(Collision2D col) => CheckBoostCeiling(col);

    void CheckBoostCeiling(Collision2D col)
    {
        if (!isBoosting) return;
        if ((groundLayer.value & (1 << col.gameObject.layer)) == 0) return;

        for (int i = 0; i < col.contactCount; i++)
        {
            if (col.GetContact(i).normal.y < -0.5f)
            {
                boostTargetY = rb.position.y; // target = where we are now
                return;
            }
        }
    }
    #endregion

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
    public void freezeTimePellet()
    {
        FreezeTime(hitStopTime);
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
    #region Boosting Up Attacks
    
    //Use StartBoost to call the boost function. This also replace the gravity off.
    //The intention is to separate between signal and calculation, to prevent stacking calculation flying to the roof
    public void startBoost()
    {
        // Restart: kill any existing motion
        rb.linearVelocity = Vector2.zero;

        // Lock the target only on a fresh boost, so repeat calls can't stack 4 into 8
        if (!isBoosting)
        {
            boostTargetY = rb.position.y + boostHeight;
            isBoosting = true;
            anim.SetTrigger("sommersault");
        }
    }

    //Triggered when Boost is true
    void boostUp()
    {
        // Peak speed. At 2x the average, a linear taper to zero still covers
        // boostHeight in about boostTime.
        boostVelocity = 2f * boostHeight / boostTime;

        // 1 at the start of the boost, 0 at the target
        float remaining = Mathf.Clamp01((boostTargetY - rb.position.y) / boostHeight);

        // Linear taper from full speed down to the end factor
        float speedScale = Mathf.Lerp(boostEndSpeedFactor, 1f, remaining);

        float dt = Time.fixedDeltaTime;
        float newY = Mathf.MoveTowards(rb.position.y, boostTargetY, boostVelocity * speedScale * dt);

        rb.linearVelocityY = (newY - rb.position.y) / dt;

        if (Mathf.Approximately(newY, boostTargetY))
        {
            rb.linearVelocityY = 0f;
            isBoosting = false;
        }

    }

    #endregion

}
