using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class MageEnemyCombat : MonoBehaviour, EnemyInterface
{
    private Animator animator;

    public GameObject rangeAttackMage;
    [SerializeField] private Animator animatorRangeAttack;


    private BoxCollider2D collider;
    private Rigidbody2D rb;
    public float detectionRange = 5f;
    public float cdTime;
    public LayerMask playerLayer;
    [SerializeField] private float hp;

    [SerializeField] private AudioClip rangeAttackSound;
    [SerializeField] private AudioClip hittedSound;
    [SerializeField] private AudioClip deadSound;

    private AudioSource audioSource;
    private AudioSource loopSource;

    [SerializeField] private LayerMask layerFloor;

    public float maxRayDistance;




    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        
        AudioSource[] sources = GetComponents<AudioSource>();
        audioSource = sources[0];
        loopSource = sources[1];

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

        if (player != null && Time.time > cdTime && !animator.GetBool("Dead"))
        {
            FlipTowardsPlayer(player.transform.position.x);

            RaycastHit2D hit = Physics2D.Raycast(player.transform.position, Vector2.down, maxRayDistance, layerFloor);
            castAttack(hit, player);

            
        }
    }

    void castAttack(RaycastHit2D hit, Collider2D player)
    {
        if (hit.collider != null)
        {
            Vector3 attackPosition = new Vector3(player.transform.position.x, hit.point.y + 1f, player.transform.position.z);
            rangeAttackMage.transform.position = attackPosition;
            cdTime = Time.time + Random.Range(2, 5);
            animator.SetTrigger("Attack");
        }
    }

    public void soundEffect()
    {
        audioSource.PlayOneShot(rangeAttackSound);
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

        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        if (player != null)
        {
            
            Vector3 playerPosition = player.transform.position;
            Vector3 raycastEnd = playerPosition + Vector3.down * maxRayDistance;

            // Usa Gizmos para dibujar el raycast
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(playerPosition, raycastEnd);
        }
    }

    public void TakeDmg(float dmg)
    {
        if (animator.GetBool("Dead")) return;

        hp -= dmg;

        Debug.Log(hp);

        if (hp > 0)
        {
            audioSource.PlayOneShot(hittedSound);

            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            {
                animator.SetTrigger("Hit");
            }

        }

        if (hp <= 0)
        {
            animator.SetBool("Dead", true);
            animator.SetTrigger("Hit");
            StartCoroutine(Death());
        }

    }

    public IEnumerator Death()
    {
        //runningSource.mute = true;

        rb.constraints = RigidbodyConstraints2D.FreezePositionX;

        Physics2D.IgnoreLayerCollision(8, 10, true);

        audioSource.PlayOneShot(deadSound);

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + 2);

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        float fadeDuration = 2f; // Duración en segundos del desvanecimiento
        float fadeRate = 1f / fadeDuration;
        float alpha = 1f;

        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeRate;
            spriteRenderer.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        Destroy(gameObject);
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
