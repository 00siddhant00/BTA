using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    //Mele combat
    //Heal from Zin
    public bool meleAllowed;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackRate = 0.44f;
    [SerializeField] private float attackDistance = 1.5f;
    private float nextTimeToAttack;

    private PlayerMovement playerMovement;
    Rigidbody2D rb;
    Vector2 hitDirection;
    public LayerMask damagableLayer;
    public GameObject Slash;
    public bool slashing;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        SwitchingAttackPoint();

        if (meleAllowed)
            if (Time.time >= nextTimeToAttack)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Attack();
                    nextTimeToAttack = Time.time + attackRate;
                }
            }
    }

    void Attack()
    {
        //Detect enemies in range of attack
        Collider2D[] hitDamagable = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, damagableLayer);

        foreach (Collider2D hit in hitDamagable)
        {
            if (hit.TryGetComponent<EnemyHealth>(out var enemyHealth))
            {
                enemyHealth.Damage();
            }

            if (hit.TryGetComponent<BreakableWall>(out var breakableWall))
            {
                breakableWall.Damage();
            }

            if (hit.TryGetComponent<EnemyData>(out var enemyData))
            {
                // Check if the hit object has an EnemyData component
                // You can access the child class (e.g., Neph) through the parent class reference
                Neph neph = enemyData as Neph;
                if (neph != null)
                {
                    neph.ApplyKnockBack(hitDirection);
                    if (hitDirection == new Vector2(0, -1) || hitDirection == new Vector2(0, 1))
                        ApplyKnockBack(hitDirection * -1, 0.5f, 20);
                    // You have successfully accessed the Neph component
                    // You can now use neph for specific Neph behavior
                }
            }
        }

        slashing = true;
        Slash.SetActive(true);

        StartCoroutine(OffSlash());
    }

    IEnumerator OffSlash()
    {
        GameManager.Instance.pet.HidePet();
        yield return new WaitForSeconds(0.1f);

        GameManager.Instance.pet.ShowPet();
        slashing = false;
        Slash.SetActive(false);
    }

    public void ApplyKnockBack(Vector2 direction, float duration, float force)
    {
        rb.velocity = Vector2.zero; // Clear any existing velocity.
        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);

        StartCoroutine(EndKnockBack(duration));
    }

    private IEnumerator EndKnockBack(float duration)
    {
        yield return new WaitForSeconds(duration);
        rb.velocity = Vector2.zero;
    }

    /// <summary>
    /// Changes the attack point position based on the direction input
    /// </summary>
    void SwitchingAttackPoint()
    {
        Vector3 newPosition = new Vector3(attackDistance, 0, 0); // Default position
        Slash.transform.localRotation = Quaternion.Euler(0, 0, 0);

        if (Input.GetKey(KeyCode.A))
        {
            hitDirection = new Vector2(-1, 0);
            newPosition = new Vector3(attackDistance, 0, 0);
            Slash.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        if (Input.GetKey(KeyCode.D))
        {
            hitDirection = new Vector2(1, 0);
            newPosition = new Vector3(attackDistance, 0, 0);
            Slash.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        if (Input.GetKey(KeyCode.W))
        {
            hitDirection = new Vector2(0, 1);
            newPosition = new Vector3(0, attackDistance, 0);
            Slash.transform.localRotation = Quaternion.Euler(0, 0, 90);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            hitDirection = new Vector2(0, -1);
            newPosition = new Vector3(0, -attackDistance, 0);
            Slash.transform.localRotation = Quaternion.Euler(0, 0, -90);
        }

        attackPoint.transform.localPosition = newPosition;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
