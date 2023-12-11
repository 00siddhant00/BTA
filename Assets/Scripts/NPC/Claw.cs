using UnityEngine;
using BeyondAbyss.Enemy;
using System.Collections;
using System;

public class Claw : EnemyBase
{
    #region Range
    public float detectionRadius ;
    public bool playerInsideCircle = false;

    #endregion


    public int actualHealthPoints = 7;
    public bool isSleeping = true;
    private bool isDead = false;
    public bool isAttacking = false;

    public float reincarnationDelay;

    public float attackCompletionDelay;

    private Collider2D col;
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        actualHealthPoints = 7;

        if (healthPoints > 5)
        {
            isSleeping = true;
            isAttacking = false;
        }
        else
        {
            isSleeping = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HitCounter();
        if(isSleeping)
        {
            Sleep();
        }
    }

    void Sleep()
    {
        if (healthPoints == 5 && !isDead && isSleeping)
        {
            isSleeping = false;
            isDead = true;
            Debug.Log("isDead");
            Resurrect();
            col.enabled = false;
        }
    }

    bool DetectIfPlayerInsideAttackRadius()
    {

        Vector2 origin = transform.position;
        //float radius = groundCheckCastLength;

        Collider2D hit = Physics2D.OverlapCircle(origin + groundCheckOffset, detectionRadius, playerLayer);

        if (hit != null)
        {
            // Player is inside the circle
            playerInsideCircle = true;
            Debug.Log("Player is inside the circle");
        }
        else
        {
            // Player is outside the circle
            playerInsideCircle = false;
            Debug.Log("Player is outside the circle");
        }

        return playerInsideCircle;
    }
    
    IEnumerator Wait(Action action, float delay)
    {
        //print("wait");
        yield return new WaitForSeconds(delay);

        action?.Invoke();
    }

    void Resurrection()
    {
        isDead = false;
        col.enabled = true;
        //Debug.Log("isAlive");

        if (!DetectIfPlayerInsideAttackRadius())
        {
            print("Player not Found");
            return;
        }
        else print("Player Found");

        Attack();
    }

    public void Resurrect()
    {
        StartCoroutine(Wait(() => Resurrection(), reincarnationDelay));
    }

    public void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;

            if (!playerInsideCircle) return;

            print("isAttacking");
            StartCoroutine(Wait(() => DamagePlayer(), attackCompletionDelay));
        }
    }

    void DamagePlayer()
    {
        //print("Damagaing Player");
        GameManager.Instance.playerController.playerHealth.DamagePlayer(2);
    }

    void HitCounter()
    {
        //if(isDead) isAttacking = false;

        if (!isDead && !isSleeping && healthPoints == 1)
        {
            isDead = true;
            //Debug.Log("isDead");
            Resurrect();
            col.enabled = false;
            isAttacking = false;
            print("not attacking");
        }

        if (healthPoints == 1) healthPoints = actualHealthPoints - 2;

        if(healthPoints > 5)
        {
            isSleeping = true;
            isAttacking = false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
