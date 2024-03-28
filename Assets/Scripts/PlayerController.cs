using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    private float extraHeight = 0.05f;

    public LayerMask layerFloor;
    private CapsuleCollider2D boxCollider;
    private Rigidbody2D rigidBody;
    private bool lookingRight = true;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        movePlayer();
        jumpPlayer();
        fallingPlayer();
        isLanding();


    }

    void movePlayer()
    {
        float inputMovement = Input.GetAxis("Horizontal");

        if (inputMovement != 0f && isTouchingFloor()) 
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        rigidBody.velocity = new Vector2(inputMovement * speed, rigidBody.velocity.y);

        turnAroundPlayer(inputMovement);
    }

    void jumpPlayer()
    {
        if (Input.GetKeyDown(KeyCode.W) && isTouchingFloor())
        {
            animator.SetBool("isJumping", true);
            rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            
        }
    }

    void fallingPlayer()
    {
        if (!isTouchingFloor() && rigidBody.velocity.y < 0)
        {
            animator.SetBool("isFalling", true);
            animator.SetBool("isJumping", false);
        }
    }

    void isLanding()
    {
        if (isTouchingFloor() && animator.GetBool("isFalling"))
        {
            animator.SetBool("isFalling", false);           
            
        }
    }

    bool isTouchingFloor()
    {
        Vector2 boxCastCenter = new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.min.y);

        Vector2 boxCastSize = new Vector2(boxCollider.bounds.size.x * 0.8f, extraHeight);

        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCastCenter, boxCastSize, 0f, Vector2.down, extraHeight, layerFloor);

        Color rayColor = raycastHit.collider != null ? Color.green : Color.red;
        Debug.DrawRay(boxCastCenter, Vector2.down * (boxCastSize.y / 2 + extraHeight), rayColor);
        Debug.DrawRay(new Vector2(boxCastCenter.x - boxCastSize.x / 2, boxCastCenter.y), Vector2.right * boxCastSize.x, rayColor);

        return raycastHit.collider != null;
    }

    void turnAroundPlayer(float inputMovement)
    {
        if (lookingRight && inputMovement < 0 || !lookingRight && inputMovement > 0)
        {
            lookingRight = !lookingRight;
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }
    }

    void attackPlayer()
    {

    }
}
