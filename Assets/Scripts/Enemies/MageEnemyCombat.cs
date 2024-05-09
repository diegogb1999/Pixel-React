using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyCombat : MonoBehaviour
{
    private Animator animator;

    public GameObject rangeAttackMage;
    [SerializeField] private Animator animatorRangeAttack;


    private BoxCollider2D collider;
    private Rigidbody2D rb;
    public float detectionRange = 5f;
    public float cdTime;
    public LayerMask playerLayer;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        cdTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerDetection();
    }
    public void triggerChildAnim()
    {
           animatorRangeAttack.SetTrigger("Attack");
    }

    void PlayerDetection()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        if (player != null && Time.time > cdTime)
        {
            FlipTowardsPlayer(player.transform.position.x);

            Vector3 attackPosition = player.transform.position;
            attackPosition.y += .3f;
            rangeAttackMage.transform.position = attackPosition;
            cdTime = Time.time + 3f;
            animator.SetTrigger("Attack");
        }
    }

    void FlipTowardsPlayer(float playerPositionX)
    {
        if (playerPositionX > transform.position.x)
        {
            // El jugador está a la derecha, asegurar que el mago mire hacia la derecha
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (playerPositionX < transform.position.x)
        {
            // El jugador está a la izquierda, asegurar que el mago mire hacia la izquierda
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

        private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("aaa");
            other.gameObject.GetComponent<PlayerCombat>().TakeDmg(2, other.GetContact(0).normal);
        }
    }
}
