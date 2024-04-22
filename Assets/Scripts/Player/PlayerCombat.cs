using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    [Header("Movement")]

    [SerializeField] private Vector2 knockbackSpeed;

    [Header("Audio")]

    [SerializeField] private AudioClip basicAttackSound;
    [SerializeField] private AudioClip eSkillSound;
    [SerializeField] private AudioClip receiveDmgSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip finishSound;

    private GameObject soundManager;
    private AudioSource audioSource;
    private AudioSource runningSource;

    [Header("Stats")]

    [SerializeField] private float hp;

    private float maxHp = 10;
    private Image fill;
    private bool isInvulnerable = false;
    private bool isDead = false;

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

    void Start()
    {
        fill = GameObject.Find("Canvas Pause Menu/GameUI/HealthBar/Fill Area/Fill HP").GetComponent<Image>();
        playerController = GetComponent<PlayerController>();
        Physics2D.IgnoreLayerCollision(8, 10, false);
        hp = maxHp;
        UpdateHealthBar();
        rigidBody = GetComponent<Rigidbody2D>();
        soundManager = GameObject.Find("SoundManager");
        AudioSource[] sources = GetComponents<AudioSource>();
        audioSource = sources[0];
        runningSource = sources[1];
    }

    void Update()
    {
        if (!isDead && playerController.canMove)
        {
            BasicAttack();
            SecondaryAttack();
            Eskill();
        }


    }

    public void updateHp(int amount)
    {
        hp -= amount;
        hp = Mathf.Clamp(hp, 0, maxHp);
        UpdateHealthBar();
        if (hp <= 0)
        {
            StartCoroutine(Death());
        }
    }

    public void UpdateHealthBar()
    {
        float targetFillAmount = hp / maxHp;
        fill.fillAmount = targetFillAmount;
    }

    #region Character attacks / skills

    void BasicAttack()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextAttackTimeBasicAttack)
        {
            animator.SetTrigger("basicAttack");
            audioSource.PlayOneShot(basicAttackSound);
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
            audioSource.PlayOneShot(eSkillSound);

            // Especifica el tama�o de la hitbox. Aumenta el primer valor para hacerla m�s ancha.
            // Ajusta el tama�o para hacer la hitbox m�s larga.
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

    public IEnumerator Death()
    {
        if (hp <= 0)
        {
            isDead = true;

            animator.SetTrigger("isDead");

            runningSource.mute = true;

            playerController.canMove = false;

            rigidBody.velocity = Vector2.zero;

            //rigidBody.constraints = RigidbodyConstraints2D.FreezePositionX;

            Physics2D.IgnoreLayerCollision(8, 10, true);
            isInvulnerable = true;

            soundManager.GetComponent<AudioSource>().volume = 0.05f;

            audioSource.PlayOneShot(deathSound);

            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + 1);

            audioSource.PlayOneShot(finishSound);

            yield return new WaitForSeconds(finishSound.length + 2);

            GameManager.instance.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }


    }

    public void TakeDmg(int dmg, Vector2 pos)
    {

        if (isInvulnerable) return;

        updateHp(dmg);

        audioSource.PlayOneShot(receiveDmgSound);


        animator.SetBool("isRunning", false);
        animator.SetBool("isJumping", false);
        animator.SetTrigger("hitted");

        if (isDead) return;

        ApplyKnockback(pos);
        StartCoroutine(loseControl());
        StartCoroutine(desactivateCollision());

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
