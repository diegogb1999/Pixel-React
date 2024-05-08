using KS.Reactor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlowerEnemyCombat : MonoBehaviour, EnemyInterface
{
    public ParticleSystem deathParticles;

    [Header("Stats")]

    [SerializeField] private float hp;


    [Header("Gas values")]

    [SerializeField] private Transform FlowerAttackPoint;
    [SerializeField] private float attackRangeFlowerX;
    [SerializeField] private float attackRangeFlowerY;
    [SerializeField] private float nextAttackTimeFlower;

    [Header("Audio")]

    [SerializeField] private AudioClip gasSound;
    [SerializeField] private AudioClip hittedSound;
    [SerializeField] private AudioClip deadSound;

    private AudioSource audioSource;
    private AudioSource loopSource;

    [Header("Physics and animations")]

    [SerializeField] private LayerMask playerLayer;
    private Animator animator;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        AudioSource[] sources = GetComponents<AudioSource>();
        audioSource = sources[0];
        loopSource = sources[1];
    }

    void Update()
    {
        GasAttack();
    }

    void GasAttack()
    {

        if (Time.time >= nextAttackTimeFlower && !animator.GetBool("Dead"))
        {
            animator.SetTrigger("Attack");
            audioSource.PlayOneShot(gasSound);
            StartCoroutine(ApplyGasDamage());
            nextAttackTimeFlower = Time.time + 5f;
        }
    }

    IEnumerator ApplyGasDamage()
    {
        float damageInterval = 0.2f;

        float endTime = Time.time + 1f;

        while (Time.time < endTime)
        {
            Vector2 hitBoxSize = new Vector2(attackRangeFlowerX, attackRangeFlowerY);
            Vector2 hitBoxPosition = new Vector2(FlowerAttackPoint.position.x, FlowerAttackPoint.position.y);
            Collider2D[] hitPlayers = Physics2D.OverlapBoxAll(hitBoxPosition, hitBoxSize, 0, playerLayer);

            foreach (Collider2D player in hitPlayers)
            {
                Debug.Log("HIT SMOKE " + player.name);
                player.gameObject.GetComponent<PlayerCombat>().TakeDmg(4, Vector2.zero);
            }

            yield return new WaitForSeconds(damageInterval);
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

    void OnDrawGizmos()
    {

        if (FlowerAttackPoint != null)
        {
            Vector2 hitBoxSize = new Vector2(attackRangeFlowerX, attackRangeFlowerY);


            Vector2 hitBoxPosition = new Vector2(FlowerAttackPoint.position.x, FlowerAttackPoint.position.y);

            Gizmos.DrawWireCube(hitBoxPosition, hitBoxSize);
        }
    }
}
