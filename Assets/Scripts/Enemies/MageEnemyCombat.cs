using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyCombat : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private Animator animatorRangeAttack;
    private CapsuleCollider2D capsuleCollider;
    private Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(a());
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void triggerChildAnim()
    {
           animatorRangeAttack.SetTrigger("Attack");
    }

    IEnumerator a ()
    {
        while  (true)
        {
            yield return new WaitForSeconds(2f);
            animator.SetTrigger("Attack");
        }
        
        
    }
}
