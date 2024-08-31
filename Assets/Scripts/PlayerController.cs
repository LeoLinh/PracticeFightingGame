using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject fireBallsPrefabs;
    public float moveSpeed;
    private Rigidbody2D rb;
    private float xInput;
    public float jumpForce;
    private Animator anim;

    private int facingDir = 1; // flip
    private bool facingRight = true; // flip

    private bool isAttacking;
    private bool isCrouching;
    private bool isBlocking;
    private bool isCasting;
    private bool isDie;
    private bool isDizzy;
    private bool isHurt;
    private bool isWin;
    private bool canMove = true;
    private float castCooldownTimer;
    private float castCooldown;
    private bool isDashing;
    private bool isStrike;
    private bool isJumping;

    [Header("Strike")]
    public float strikeSpeed;
    public float strikeDuration;
    private float strikeTimer;

    [Header("Dash")]
    public float dashSpeed;
    public float dashDuration;
    private float dashTimer; // moved here for clarity

    [Header("Ground Check")]
    public float groundCheckDistance;
    public LayerMask WhatIsGround;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        Movement();
        CheckInput(); // includes dash input
        Animation();
        FlipController();
        GroundCheck();
    }

    public void WinOver() { isWin = false; }
    public void HurtOver() { isHurt = false; }
    public void DizzyOver() { isDizzy = false; }
    public void DieOver() { isDie = false; }
    public void CastOver() { isCasting = false; }
    public void BlockOver() { isBlocking = false; }
    public void CrouchOver() { isCrouching = false; }
    public void AttackOver() { isAttacking = false; }
    public void StrikeOver() { isStrike = false; }

    private void GroundCheck()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, WhatIsGround);
    }

    private void Animation()
    {
        bool isMoving = rb.velocity.x != 0; // run
        anim.SetBool("isMoving", isMoving); // run
        anim.SetBool("isGrounded", isGrounded); // check ground for jump
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isDashing", dashTimer > 0);
        anim.SetBool("isAttacking", isAttacking);
        anim.SetBool("isCrouching", isCrouching);
        anim.SetBool("isBlocking", isBlocking);
        anim.SetBool("isCasting", isCasting);
        anim.SetBool("isDie", isDie);
        anim.SetBool("isDizzy", isDizzy);
        anim.SetBool("isHurt", isHurt);
        anim.SetBool("isWin", isWin);
        anim.SetBool("isStrike", isStrike);
        
    }

    private void CheckInput()
    {
        xInput = Input.GetAxisRaw("Horizontal"); // di chuyển thường

        if (Input.GetKeyDown(KeyCode.J)) { isWin = true; }
        if (Input.GetKeyDown(KeyCode.H)) { isHurt = true; }
        if (Input.GetKeyDown(KeyCode.M)) { isDizzy = true; }
        if (Input.GetKeyDown(KeyCode.N)) { isDie = true; }
        if (Input.GetKeyDown(KeyCode.B)) { isCasting = true; Cast(); }
        if (Input.GetKeyDown(KeyCode.V)) { isBlocking = true; }
        if (Input.GetKeyDown(KeyCode.C)) { isCrouching = true; }
        if (Input.GetKeyDown(KeyCode.Mouse0)) 
        {
            isAttacking = true; 
        }
        if (Input.GetButtonDown("Jump")) 
        { 
            Jump();
            isGrounded = false;
            isJumping = true;
            
        }

        if(Input.GetKeyDown(KeyCode.Mouse1) && isGrounded == false && isJumping == true)
        {
            anim.SetTrigger("isJumpAttack");
        }
        else
        {
            anim.ResetTrigger("isJumpAttack");
        }

        // Dash
        dashTimer -= Time.deltaTime; // dash
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isDashing = true;
            dashTimer = dashDuration;
        }
        else
        {
            isDashing = false;
        }
        strikeTimer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isStrike = true;
            strikeTimer = strikeDuration;
        }
        else
        {
            isStrike = false;
        }
    }

    private void Movement()
    {
        if (dashTimer > 0) // nếu dash timer sẽ liên tục đếm ngược và sau mỗi lần leftshift thì dashtimer sẽ = dashDuration nghĩa là sẽ lớn hơn 0.
        {
            rb.velocity = new Vector2(xInput * dashSpeed, rb.velocity.y); // Dash
        }
        else if (strikeTimer > 0)
        {
            rb.velocity = new Vector2(xInput * strikeSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y); // Di chuyển thường
        }
    }

    private void Jump()
    {
        if (isGrounded) // Nhảy kèm điều kiện Ground Check
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    private void Flip()
    {
        facingDir *= -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    private void FlipController()
    {
        if (rb.velocity.x > 0 && !facingRight)
        {
            Flip();
        }
        else if (rb.velocity.x < 0 && facingRight)
        {
            Flip();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundCheckDistance));
    }

    private void Cast()
    {
        if (castCooldownTimer >= castCooldown && isGrounded && !isDizzy && !isDashing && canMove && !anim.GetCurrentAnimatorStateInfo(0).IsName("Cast"))
        {
            castCooldownTimer = 0;
            anim.SetBool("isCasting", true);
            canMove = false;

            if (fireBallsPrefabs != null)
            {
                GameObject fireBalls = Instantiate(fireBallsPrefabs, firePoint.position, Quaternion.identity);
                Projectile projectileScript = fireBalls.GetComponent<Projectile>();
                if (projectileScript != null)
                {
                    // Sử dụng facingDir để xác định hướng của viên đạn
                    projectileScript.SetDirection(facingDir);
                }
            }

            StartCoroutine(ResetAction("isCasting"));
        }
    }

    private IEnumerator ResetAction(string actionBoolName)
    {
        // Xử lý reset trạng thái hành động (Casting, Attacking)
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);
        anim.SetBool(actionBoolName, false);
        canMove = true;
    }
}
