using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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
        playerController.playerMovement.Data.jumpCutGravityMult = 3.5f;
    }

    private void Update()
    {
        SwitchingAttackPoint();

        POGO();

        if (meleAllowed)
            if (Time.time >= nextTimeToAttack)
            {
                //if (Input.GetMouseButtonDown(0))
                if (playerController.playerMovement.isReplayingFuture ? attack : PlayerInputHandler.Instance.IsMele)
                {
                    Attack();
                    nextTimeToAttack = Time.time + attackRate;
                }
            }
    }

    private void POGO()
    {
        if (transform.position.y > maxPogoHight && Pogoed)
        {
            print("StoppedKnockBack");
            rb.velocity = new(rb.velocity.x, 0);
            Pogoed = false;
        }
    }

    public GameObject altPet;

    void Attack()
    {
        if (altPet != null && altPet.activeSelf)
            altPet.SetActive(false);

        FindObjectOfType<AudioManager>().Play("Slash");
        slashing = true;
        //Detect enemies in range of attack
        Collider2D[] hitDamagable = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, damagableLayer);

        foreach (Collider2D hit in hitDamagable)
        {
            float topPoint = hit.bounds.max.y;
            //Get the IDamageable interface if exist on the hit object and apply the damage and Fx to those who inherite from IDamageable interface 
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                // Check if the hit object has an EnemyData component
                if (damageable != null)
                {
                    if (damageable.onHitFreezTime) GameManager.Instance.TimeSlow(100, 0.15f); //when enemy is hit it time stops for 0.1 sec for emphasis effect

                    damageable.Damage();
                    damageable.ApplyKnockBack(hitDirection);
                }
            }


            //Handle knockback for both axis inclusing in air and on ground is dynamic
            if ((hitDirection == new Vector2(0, -1) || hitDirection == new Vector2(0, 1)) && hit.gameObject.layer != LayerMask.NameToLayer("Ground"))
            {
                ApplyKnockBack(hitDirection * -1, knockBackForce, topPoint + playerController.playerMovement.Data.jumpHeight);
            }
            else if (hitDirection == new Vector2(-1, 0) || hitDirection == new Vector2(1, 0))
            {
                ApplyKnockBack(hitDirection * -1, GameManager.Instance.playerController.playerAnimator.isGrounded ? (knockBackForce * 2f) : (knockBackForce / 1.6f), 1000000000000);
            }

            #region Effects

            if (hit.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                GameManager.Instance.CameraShake.ShakeCamera(3.5f, 0.2f, 0.2f);
                FindObjectOfType<AudioManager>().Play("WallHit");
                playerController.playerAnimator.WallHit(attackPoint);
                break;
            }
            else if (hit.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                GameManager.Instance.CameraShake.ShakeCamera(4.5f, 0.25f, 0.22f);
                //FindObjectOfType<AudioManager>().Play("WallHit");
                playerController.playerAnimator.WallHit(attackPoint);
                break;
            }

            #endregion
        }

        Slash.SetActive(true);

        StartCoroutine(OffSlash());
    }

    IEnumerator OffSlash()
    {
        GameManager.Instance.pet.HidePet();
        yield return new WaitForSeconds((attackRate / 2) - 0.05f);
        //yield return new WaitForSeconds(0.12f);

        GameManager.Instance.pet.ShowPet();
        slashing = false;
        Slash.SetActive(false);
        //playerController.playerMovement.allowPlayerMovement = true;
    }

    float maxPogoHight;
    bool Pogoed = false;

    public void ApplyKnockBack(Vector2 direction, float force, float maxPogoHight)
    {
        this.maxPogoHight = maxPogoHight;
        rb.velocity = Vector2.zero; // Clear any existing velocity.
                                    //playerController.playerMovement.Data.jumpCutGravityMult = 1f;

        //print(playerController.playerMovement.Data.jumpCutGravityMult);
        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
        Pogoed = true;

        //print($"{transform.position.y} : {maxPogoHight}");

        //rb.velocity = new Vector2(0, direction.y * force);

    }

    internal bool attack;

    //public void OnMele(InputAction.CallbackContext ctx)
    //{
    //    if (ctx.performed)
    //    {
    //        attack = true;
    //    }
    //}

    //void SwitchingAttackPoint()
    //{
    //    if (slashing) return;

    //    Vector2 inputDirection = playerController.playerMovement.move.action.ReadValue<Vector2>();

    //    hitDirection = inputDirection.normalized;

    //    // Calculation for newPosition goes here... 
    //    Vector3 newPosition = new Vector3(attackDistance * hitDirection.x, attackDistance * hitDirection.y, 0);

    //    // Rotation adjustment
    //    if (inputDirection.x < 0)
    //    { // Left
    //        Slash.transform.localRotation = Quaternion.Euler(0, 0, 0);
    //    }
    //    else if (inputDirection.x > 0)
    //    { // Right
    //        Slash.transform.localRotation = Quaternion.Euler(0, 0, 0);
    //    }
    //    else if (inputDirection.y > 0)
    //    { // Up
    //        Slash.transform.localRotation = Quaternion.Euler(0, 0, 90);
    //    }
    //    else if (inputDirection.y < 0 && !playerController.playerAnimator.isGrounded)
    //    { // Down
    //        Slash.transform.localRotation = Quaternion.Euler(0, 0, -90);
    //    }

    //    attackPoint.transform.localPosition = newPosition;
    //}

    /// <summary>
    /// Changes the attack point position based on the direction input
    /// </summary>
    void SwitchingAttackPoint()
    {
        if (slashing) return;

        Vector2 inputDirection = playerController.playerMovement.move.action.ReadValue<Vector2>();

        //print(inputDirection);

        Vector3 newPosition = new Vector3(attackDistance, 0, 0); // Default position
        Slash.transform.localRotation = Quaternion.Euler(0, 0, 0);

        #region Main 4 Directions

        //if (Input.GetKey(KeyCode.A))
        if (inputDirection.x < 0)
        {
            hitDirection = new Vector2(-1, 0);
            newPosition = new Vector3(attackDistance, 0, 0);
            Slash.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        //if (Input.GetKey(KeyCode.D))
        if (inputDirection.x > 0)
        {
            hitDirection = new Vector2(1, 0);
            newPosition = new Vector3(attackDistance, 0, 0);
            Slash.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        //if (Input.GetKey(KeyCode.W))
        if (inputDirection.y > 0)
        {
            hitDirection = new Vector2(0, 1);
            newPosition = new Vector3(0, attackDistance, 0);
            Slash.transform.localRotation = Quaternion.Euler(0, 0, 90);
        }

        //if (Input.GetKey(KeyCode.S) && !playerController.playerAnimator.isGrounded)
        if (inputDirection.y < 0 && !playerController.playerAnimator.isGrounded)
        {
            hitDirection = new Vector2(0, -1);
            newPosition = new Vector3(0, -attackDistance, 0);
            Slash.transform.localRotation = Quaternion.Euler(0, 0, -90);
        }

        #endregion

        #region Digonal 4 Directions

        if (inputDirection.x > 0 && inputDirection.y > 0)
        {
            hitDirection = new Vector2(1, 1);
            newPosition = new Vector3(attackDistance, attackDistance, 0);
            Slash.transform.localRotation = Quaternion.Euler(0, 0, 45);
        }

        if (inputDirection.x < 0 && inputDirection.y > 0)
        {
            hitDirection = new Vector2(-1, 1);
            newPosition = new Vector3(attackDistance, attackDistance, 0);
            Slash.transform.localRotation = Quaternion.Euler(0, 0, 45);
        }

        if (inputDirection.x < 0 && inputDirection.y < 0)
        {
            hitDirection = new Vector2(-1, -1);
            newPosition = new Vector3(attackDistance, -attackDistance, 0);
            Slash.transform.localRotation = Quaternion.Euler(0, 0, -45);
        }

        if (inputDirection.x > 0 && inputDirection.y < 0)
        {
            hitDirection = new Vector2(1, -1);
            newPosition = new Vector3(attackDistance, -attackDistance, 0);
            Slash.transform.localRotation = Quaternion.Euler(0, 0, -45);
        }

        #endregion

        attackPoint.transform.localPosition = newPosition;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
