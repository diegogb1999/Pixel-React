using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageRangeAttack : MonoBehaviour
{
    private BoxCollider2D collider;
    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateDmg()
    {
        collider.enabled = true;
    }
    public void DeactivateDmg()
    {
        collider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("EEEEEEE");
            other.gameObject.GetComponent<PlayerCombat>().TakeDmg(5, Vector2.zero);
        }
    }
}
