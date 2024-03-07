using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    float runSpeed;
    [SerializeField]
    float jumpSpeed;
    [SerializeField]
    float climbSpeed;

    [SerializeField]
    float deathkickSpeed;

    Vector2 moveInput;
    Rigidbody2D rigid;
    Animator animator;
    CapsuleCollider2D bodyCollider;

    BoxCollider2D feetCollider;
    float gravityScale;

    bool isAlive = true;

    [SerializeField]
    GameObject bullet;
    [SerializeField]
    Transform gunTransform;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        bodyCollider = GetComponent<CapsuleCollider2D>();
        gravityScale = rigid.gravityScale;
        feetCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) return;
        Run();
        FlipSprite();
        ClimbLadder();
        Die();
    }

    void OnJump(InputValue value)
    {
        if (!isAlive) return;
        if (!feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) return;
        if (value.isPressed)
        {
            rigid.velocity += new Vector2(0f, jumpSpeed);
        }
    }

    void OnFire(InputValue value)
    {
        if (!isAlive) return;
        Instantiate(bullet, gunTransform.position, transform.rotation);
    }

    void OnMove(InputValue value)
    {
        if (!isAlive) return;
        moveInput = value.Get<Vector2>();
        animator.SetBool("isRunning", Mathf.Abs(moveInput.x) > Mathf.Epsilon);
    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, rigid.velocity.y);
        rigid.velocity = playerVelocity;
    }

    void FlipSprite()
    {
        if (Mathf.Abs(rigid.velocity.x) > Mathf.Epsilon)
        {
            transform.localScale = new Vector2(Mathf.Sign(rigid.velocity.x), 1f);
        }
    }

    void ClimbLadder()
    {
        if (!feetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            rigid.gravityScale = gravityScale;
            animator.SetBool("isClimbing", false);
            return;
        }

        rigid.velocity = new Vector2(rigid.velocity.x, moveInput.y * climbSpeed);
        rigid.gravityScale = 0f;

        animator.SetBool("isClimbing", (Mathf.Abs(rigid.velocity.y) > Mathf.Epsilon));
    }

    void Die()
    {
        if (bodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Harzards")))
        {
            isAlive = false;
            animator.SetTrigger("Die");
            rigid.velocity = Vector2.up * deathkickSpeed;
        }
    }
}
