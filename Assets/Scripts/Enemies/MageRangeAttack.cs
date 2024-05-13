using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageRangeAttack : MonoBehaviour
{
    private BoxCollider2D col;
    [SerializeField] private AudioClip rangeAttackSound;

    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateDmg()
    {
        col.enabled = true;
    }
    public void DeactivateDmg()
    {
        col.enabled = false;
    }
    public void soundEffect()
    {
        audioSource.PlayOneShot(rangeAttackSound);
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
