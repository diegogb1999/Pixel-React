using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishPointScript : MonoBehaviour
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
        StartCoroutine(EsperarMientrasCondicion());
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    IEnumerator EsperarMientrasCondicion()
    {
        yield return new WaitForSeconds(3f);
        animator.SetTrigger("endGame");
        Debug.Log("La condición ya cumple.");
        // Ejecuta aquí cualquier otro código que necesites después de que la condición deje de cumplirse.
    }
}
