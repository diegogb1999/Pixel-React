using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    private float startTime;

    [SerializeField] private Animator animator;
    [SerializeField] private BoxCollider2D collision;

    void Start()
    {
        animator = GetComponent<Animator>();
        collision = GetComponent<BoxCollider2D>();
        startTime = Time.time;
    }

    void Update()
    {
        enemyKilled();
    }

    void enemyKilled()
    {
        if (Time.time >= startTime + 5f)
        {
            animator.SetTrigger("Open");
            StartCoroutine(DisableColliderAfterDelay(0.667f));
        }
    }

    IEnumerator DisableColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        collision.enabled = false;
    }
}
