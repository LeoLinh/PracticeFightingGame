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

    private int facingDir = 1; // flip
    private bool facingRight = true; // flip

    private bool isAttacking;

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
    }

    private void CheckInput()
    {
        xInput = Input.GetAxisRaw("Horizontal"); // di chuyển thường

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
}
