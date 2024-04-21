using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerEnemyCombat : MonoBehaviour
{

    [Header("Gas values")]

    [SerializeField] private Transform FlowerAttackPoint;
    [SerializeField] private float attackRangeFlowerX;
    [SerializeField] private float attackRangeFlowerY;
    [SerializeField] private float nextAttackTimeFlower;

    [Header("Audio")]
    [SerializeField] private AudioClip gasSound;
    [SerializeField] private AudioClip hittedSound;

    private AudioSource audioSource;
    private AudioSource loopSource;

    [Header("Physics and animations")]

    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask playerLayer;

    void Start()
    {
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

        if (Time.time >= nextAttackTimeFlower)
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
