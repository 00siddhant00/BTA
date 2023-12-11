using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    //Mele combat
    //Heal from Zin
    [Header("Mele")]
    public bool meleAllowed;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackRate = 0.44f;
    [SerializeField] private float attackDistance = 1.5f;
    private float nextTimeToAttack;

    Rigidbody2D rb;
    Vector2 hitDirection;
    PlayerController playerController;
    public LayerMask damagableLayer;
    public GameObject Slash;
    public bool slashing;

    [Header("KnockBack")]
    public float knockBackForce = 20;
    public float knockBackDuration = 0.5f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
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
        slashing = true;
        //Detect enemies in range of attack
        Collider2D[] hitDamagable = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, damagableLayer);

        foreach (Collider2D hit in hitDamagable)
        {
            //Get the IDamageable interface if exist on the hit object and apply the damage and Fx to those who inherite from IDamageable interface 
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                // Check if the hit object has an EnemyData component
                if (damageable != null)
                {
                    if (damageable.onHitFreezTime) GameManager.Instance.TimeSlow(100, 0.1f); //when enemy is hit it time stops for 0.1 sec for emphasis effect

                    damageable.Damage();
                    damageable.ApplyKnockBack(hitDirection);
                }
            }

            if (hitDirection == new Vector2(0, -1) || hitDirection == new Vector2(0, 1))
                ApplyKnockBack(hitDirection * -1, knockBackDuration, knockBackForce);
        }

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
        //playerController.playerMovement.allowPlayerMovement = true;
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
        if (slashing) return;

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

        if (Input.GetKey(KeyCode.S) && !playerController.playerAnimator.isGrounded)
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
