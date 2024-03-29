using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{

    [SerializeField] private Animator animator;
    [SerializeField] private Transform attackPointBasicAttack;
    [SerializeField] private Transform attackPointEskill;
    [SerializeField] private LayerMask enemyLayers;

    [SerializeField] private float attackRangeBasicAttack; //0.5f
    [SerializeField] private float attackRateBasicAttack; //1.2f

    float nextAttackTimeBasicAttack = 0f;

    [SerializeField] private float attackRangeEskillX; //2.06f
    [SerializeField] private float attackRangeEskillY; //0.67f
    [SerializeField] private float attackRateEskill; //0.2f

    float nextAttackTimeEskill = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        BasicAttack();
        SecondaryAttack();
        Eskill();

    }
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
}
