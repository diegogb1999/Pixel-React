using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    [Header("Movement")]

    [SerializeField] private Vector2 knockbackSpeed;

    [Header("Stats")]

    [SerializeField] private int hp;
    private bool isInvulnerable = false;

    [Header("Basic Attack")]

    [SerializeField] private Transform attackPointBasicAttack;
    [SerializeField] private float attackRangeBasicAttack; //0.5f
    [SerializeField] private float attackRateBasicAttack; //1.2f
    private float nextAttackTimeBasicAttack = 0f;

    [Header("E Skill")]

    [SerializeField] private Transform attackPointEskill;
    [SerializeField] private float attackRangeEskillX; //2.06f
    [SerializeField] private float attackRangeEskillY; //0.67f
    [SerializeField] private float attackRateEskill; //0.2f
    private float nextAttackTimeEskill = 0f;

    [Header("Physics and animations")]

    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask enemyLayers;
    private Rigidbody2D rigidBody;
    private PlayerController playerController;



    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        BasicAttack();
        SecondaryAttack();
        Eskill();

    }

    #region Character attacks / skills

    void BasicAttack()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextAttackTimeBasicAttack)
        {
            animator.SetTrigger("basicAttack");
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPointBasicAttack.position, attackRangeBasicAttack, enemyLayers);

            foreach (Collider2D enemy in hitEnemies)
            {
                Debug.Log("hit " + enemy.name);
            }

            nextAttackTimeBasicAttack = Time.time + 1f / attackRateBasicAttack;
        }
    }

    void SecondaryAttack()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            animator.SetTrigger("secondaryAttack");
        }
    }

    void Eskill()
    {
        /*if (Input.GetKeyDown(KeyCode.E))
        {
            animator.SetTrigger("eSkill");
        }*/

        if (Input.GetKeyDown(KeyCode.E) && Time.time >= nextAttackTimeEskill)
        {
            animator.SetTrigger("eSkill");

            // Especifica el tamaño de la hitbox. Aumenta el primer valor para hacerla más ancha.
            // Ajusta el tamaño para hacer la hitbox más larga.
            Vector2 hitBoxSize = new Vector2(attackRangeEskillX, attackRangeEskillY);


            Vector2 hitBoxPosition = new Vector2(attackPointEskill.position.x, attackPointEskill.position.y);

            Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(hitBoxPosition, hitBoxSize, 0, enemyLayers);

            foreach (Collider2D enemy in hitEnemies)
            {
                Debug.Log("hit E " + enemy.name);
            }

            nextAttackTimeEskill = Time.time + 1f / attackRateEskill;
        }
    }

    #endregion

    #region Character negative effects

    public void TakeDmg(int dmg, Vector2 pos)
    {

        if (isInvulnerable) return;
        hp -= dmg;
        StartCoroutine(loseControl());
        StartCoroutine(desactivateCollision());
        animator.SetBool("isRunning", false);
        animator.SetBool("isJumping", false);
        animator.SetTrigger("hitted");
        ApplyKnockback(pos);
    }

    public void ApplyKnockback(Vector2 enemyHit)
    {
        rigidBody.velocity = new Vector2(-knockbackSpeed.x * enemyHit.x, knockbackSpeed.y);
    }

    private IEnumerator loseControl()
    {
        playerController.canMove = false;
        yield return new WaitForSeconds(0.5f);
        playerController.canMove = true;
    }

    private IEnumerator desactivateCollision()
    {
        Physics2D.IgnoreLayerCollision(8, 10, true);
        isInvulnerable = true;
        yield return new WaitForSeconds(2f);
        isInvulnerable = false;
        Physics2D.IgnoreLayerCollision(8, 10, false);
    }

    #endregion

    #region Visualize invisible stuff

    void OnDrawGizmos()
    {
        
        if (attackPointBasicAttack != null)
        {

            Gizmos.DrawWireSphere(attackPointBasicAttack.position, attackRangeBasicAttack);
        }

        if (attackPointEskill != null)
        {
            Vector2 hitBoxSize = new Vector2(attackRangeEskillX, attackRangeEskillY);


            Vector2 hitBoxPosition = new Vector2(attackPointEskill.position.x, attackPointEskill.position.y);

            Gizmos.DrawWireCube(hitBoxPosition, hitBoxSize);
        }

    }

    #endregion


}
