using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlowerEnemyController : MonoBehaviour
{
    [Header("Movimiento")]

    [SerializeField] private GameObject pointA;
    [SerializeField] private GameObject pointB;
    [SerializeField] private float speed;
    private Transform currentPoint;

    [Header("Fisicas y animaciones")]
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentPoint = pointB.transform;
    }

    // Update is called once per frame
    void Update()
    {
        EnemyPathing();
    }

    #region Enemy movement / AI

    private void EnemyPathing()
    {
        Vector2 point = currentPoint.position - transform.position;

        if (currentPoint == pointB.transform)
        {
            rb.velocity = new Vector2(speed, 0);
        }
        else
        {
            rb.velocity = new Vector2(-speed, 0);
        }

        if (Vector2.Distance(transform.position, currentPoint.position) < 1f && currentPoint == pointB.transform)
        {
            turnAround();
            currentPoint = pointA.transform;
        }
        if (Vector2.Distance(transform.position, currentPoint.position) < 1f && currentPoint == pointA.transform)
        {
            turnAround();
            currentPoint = pointB.transform;
        }
    }

    private void turnAround()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    #endregion

    #region Visualize invisible stuff
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(pointA.transform.position, 1f);
        Gizmos.DrawWireSphere(pointB.transform.position, 1f);
        Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);
    }

    #endregion

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Knockback");
            other.gameObject.GetComponent<PlayerCombat>().TakeDmg(0, other.GetContact(0).normal);
        }
    }

}
