using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public string stateName = "Run";
    public bool isRunningSound = false;

    [Header("Movement")]

    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private Vector2 knockbackSpeed;
    private float extraHeight = 0.05f;
    private bool lookingRight = true;
    public bool canMove = true;

    [Header("Audio")]

    [SerializeField] private AudioClip runningSound;
    [SerializeField] private AudioClip jumpSound;

    private AudioSource audioSource;
    private AudioSource runningSource;

    [Header("Physics and animations")]

    [SerializeField] private LayerMask layerFloor;
    private CapsuleCollider2D boxCollider;
    private Rigidbody2D rigidBody;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();

        AudioSource[] sources = GetComponents<AudioSource>();
        audioSource = sources[0];
        runningSource = sources[1];
    }

    // Update is called once per frame
    void Update()
    {
        movePlayer();
        jumpPlayer();
        fallingPlayer();
        isLanding();
    }

    #region Player Movement
    void movePlayer()
    {
        if (!canMove)
        {
            return;
        }

        float inputMovement = Input.GetAxis("Horizontal");

        if (inputMovement != 0f && isTouchingFloor() & canMove)
        {
            animator.SetBool("isRunning", true);
            
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) && !isRunningSound)
            {
                runningSource.clip = runningSound;
                runningSource.Play();
                isRunningSound = true;
            }
        }
        else
        {
            animator.SetBool("isRunning", false);
            runningSource.Stop();
            isRunningSound = false;
        }

        rigidBody.velocity = new Vector2(inputMovement * speed, rigidBody.velocity.y);

        turnAroundPlayer(inputMovement);
    }

    void jumpPlayer()
    {
        if (Input.GetKeyDown(KeyCode.W) && isTouchingFloor() && canMove)
        {
            
            animator.SetBool("isJumping", true);
            audioSource.PlayOneShot(jumpSound);
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

    #endregion

    #region Visualize invisible stuff

    #endregion

    /*public void ApplyKnockback(Vector2 enemyHit)
        {
            rigidBody.velocity = new Vector2(-knockbackSpeed.x * enemyHit.x, knockbackSpeed.y);
        }*/
}
