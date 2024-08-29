using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    private Rigidbody2D rb;
    private float xInput;
    public float jumpForce;
    private Animator anim;

    [Header("Fire Ball")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 5f;
    Vector2 shootDirection;
    public float delayBeforeShooting = 1f;

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

    [Header("Dash")]
    public float dashSpeed;
    public float dashDuration;
    public float dashTimer;

    [Header("Ground Check")]
    public float groundCheckDistance;
    public LayerMask WhatIsGround;
    private bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        CheckInput(); // trong này có cả check dash input
        Animation();
        FlipController();
        GroundCheck();
    }

    public void WinOver()
    {
        isWin = false;
    }

    public void HurtOver()
    {
        isHurt = false;
    }

    public void DizzyOver()
    {
        isDizzy = false;
    }

    public void DieOver()
    {
        isDie = false;
    }

    public void CastOver()
    {
        isCasting = false;
    }

    public void BlockOver()
    {
        isBlocking = false;
    }

    public void CrouchOver()
    {
        isCrouching = false;
    }

    public void AttackOver()
    {
        isAttacking = false;
    }

    private void GroundCheck()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, WhatIsGround);
    }

    private void Animation()
    {
        bool isMoving = rb.velocity.x != 0; //run
        anim.SetBool("isMoving", isMoving); //run
        anim.SetBool("isGrounded", isGrounded); // check ground for once jump
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
    }

    private void CheckInput()
    {
        xInput = Input.GetAxisRaw("Horizontal"); // di chuyển thường

        if(Input.GetKeyDown(KeyCode.J))
        {
            isWin = true;
        }

        if(Input.GetKeyDown(KeyCode.H))
        {
            isHurt = true;
        }

        if(Input.GetKeyDown(KeyCode.M))
        {
            isDizzy = true;
        }

        if(Input.GetKeyDown(KeyCode.N))
        {
            isDie = true;
        }

        if(Input.GetKeyDown(KeyCode.B))
        {
            isCasting = true;
            StartCoroutine(DelayedShooting());
        }

        if(Input.GetKeyDown(KeyCode.V))
        {
            isBlocking = true;
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = true;
        }

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            isAttacking = true;
        }

        if (Input.GetButtonDown("Jump")) // Input nhảy
        {
            Jump();
        }

        // Dash
        dashTimer -= Time.deltaTime; // dash
        if (Input.GetKeyDown(KeyCode.LeftShift)) // check input dash
        {
            dashTimer = dashDuration;
        }
    }

    private void Movement()
    {
        if (dashTimer > 0) // nếu dash timer sẽ liên tục đếm ngược và sau mỗi lần leftshift thì dashtimer sẽ = dashDuration nghĩa là sẽ lớn hơn 0.
                           // Tức là lúc này có thể dash và dash dựa trên tốc dashSpeed
        {
            rb.velocity = new Vector2(xInput * dashSpeed, 0); // Dash
        }
        else
        {
            rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y); // Di chuyển thường
        }

    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Nhảy kèm điều kiện Ground Check

    }

    private void Flip()
    {
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    private void FlipController()
    {
        if (rb.velocity.x > 0 & !facingRight)
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

    private IEnumerator DelayedShooting()
    {
        yield return new WaitForSeconds(delayBeforeShooting);
        Shooting();
    }

    public void Shooting()
    {
        UpdateShootDirection();
        var bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = shootDirection.normalized * bulletForce;
        bullet.transform.localScale = new Vector3(shootDirection.x, 1, 1);

    }

    private void UpdateShootDirection()
    {
        if (transform.localScale.x > 0)
        {
            shootDirection = Vector2.right;
        }
        else
        {
            shootDirection = Vector2.left;
        }
    }

}
