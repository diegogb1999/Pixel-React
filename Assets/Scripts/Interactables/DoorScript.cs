using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{

    private GameObject enemy;
    private float startTime;

    [SerializeField] private Animator animator;
    [SerializeField] private BoxCollider2D collision;
    // Start is called before the first frame update
    void Start()
    {
        //enemy = GameObject.Find("Flower Enemy Pathing AB");
        animator = GetComponent<Animator>();
        collision = GetComponent<BoxCollider2D>();
        startTime = Time.time;
    }

    // Update is called once per frame
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
