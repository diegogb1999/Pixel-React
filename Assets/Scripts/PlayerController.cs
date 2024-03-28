using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpForce;

    public LayerMask layerFloor;
    private CapsuleCollider2D boxCollider;
    private Rigidbody2D rigidBody;
    private bool lookingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        movePlayer();
        jumpPlayer();
    }

    void movePlayer()
    {
        float inputMovement = Input.GetAxis("Horizontal");
        rigidBody.velocity = new Vector2(inputMovement * speed, rigidBody.velocity.y);

        turnAroundPlayer(inputMovement);
    }

    void jumpPlayer()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isTouchingFloor())
        {
            rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    bool isTouchingFloor()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, new Vector2(boxCollider.bounds.size.x, boxCollider.bounds.size.y), 0f, Vector2.down, 0.2f, layerFloor);
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
}
