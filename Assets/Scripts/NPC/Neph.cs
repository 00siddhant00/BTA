using UnityEngine;
using BeyondAbyss.Enemy;

public class Neph : EnemyBase
{
    [Header("Movement")]
    [SerializeField] private Transform[] wayPoints;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float frequency = 2.0f;  // Adjust the frequency of the sine wave
    [SerializeField] private float amplitude = 1.0f;  // Adjust the amplitude of the sine wave
    float time = 0f;

    [Header("Attack")]
    public float radius = 2f; // Set the radius of the circle cast.
    [SerializeField] private float attackRate;
    [SerializeField] private float attackDecrementMultiplier;
    [SerializeField] private float minimumAttackDelay;

    [HideInInspector] public Animator eyeAnim; // Reference to the Animator component for triggering animations.
    [HideInInspector] public Animator topAnim; // Reference to the Animator component for triggering animations.
    [HideInInspector] public Animator bottomAnim; // Reference to the Animator component for triggering animations.

    private float initialAttackRate;
    private bool playerInsideCircle = false; // Track whether the player is inside the circle.
    private float nextTimeToAttack;
    private Collider2D playerCollider;
    private Vector3 targetPosition;


    //Miscellaneous
    private SpriteRenderer wave;
    //private Rigidbody2D rb; // Reference to the Rigidbody2D component.

    // Start is called before the first frame update
    void Start()
    {
        wave = transform.GetChild(transform.childCount - 1).GetComponent<SpriteRenderer>(); ;
        initialAttackRate = attackRate;
        currentIndex = 0;
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component.
        topAnim = GetComponent<Animator>();
        bottomAnim = transform.GetChild(0).GetComponent<Animator>();
        eyeAnim = transform.GetChild(1).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Attack();
    }

    /// <summary>
    /// Enemy Movement Pattern
    /// </summary>
    void Move()
    {
        StopCollide();
        if (!isKnockedBack)
        {
            currentIndex %= wayPoints.Length;

            if (playerInsideCircle)
            {
                // Define the circle cast parameters
                Vector2 origin = transform.position;
                float radius = groundCheckCastLength;

                // Perform a circle cast to check for obstacles
                Collider2D hit = Physics2D.OverlapCircle(origin + groundCheckOffset, radius, groundLayerMask);

                if (hit != null)
                {
                    Vector2 direction = transform.position - (Vector3)hit.ClosestPoint(transform.position); // Use hit.point as the obstacle's point of contact
                    // Calculate the repel direction away from the obstacle
                    Vector3 repelDirection = direction.normalized;

                    // Set the target position to move away from the obstacle
                    targetPosition = transform.position + (repelDirection * repelDistance);
                }
                else
                {
                    // Chase the player when detected
                    targetPosition = playerCollider != null ? playerCollider.transform.position : transform.position;
                }
            }
            else
            {
                // Define the circle cast parameters
                Vector2 origin = transform.position;
                float radius = groundCheckCastLength;

                // Perform a circle cast to check for obstacles
                Collider2D hit = Physics2D.OverlapCircle(origin + groundCheckOffset, radius, groundLayerMask);

                if (hit != null)
                {
                    Vector2 direction = transform.position - (Vector3)hit.ClosestPoint(transform.position); // Use hit.point as the obstacle's point of contact
                    // Calculate the repel direction away from the obstacle
                    Vector3 repelDirection = direction.normalized;

                    // Set the target position to move away from the obstacle
                    targetPosition = transform.position + (repelDirection * repelDistance);
                }
                else
                {
                    // Calculate the sine wave offset for waypoint movement
                    targetPosition = wayPoints[currentIndex].position;
                    float yOffset = Mathf.Sin(time * frequency) * amplitude;
                    targetPosition.y += yOffset;

                    // Check if we have reached the current waypoint
                    if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
                    {
                        currentIndex++;
                    }
                }
            }

            // Move towards the modified target position
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, (!playerInsideCircle ? walkSpeed : walkSpeed * 2) * Time.deltaTime);

            time += Time.deltaTime;
        }
    }

    /// <summary>
    /// Enemy Attack Pattern
    /// </summary>
    void Attack()
    {
        // Cast a circle to check for player presence.
        playerCollider = Physics2D.OverlapCircle(transform.position, radius, playerLayer);

        if (playerCollider != null && !playerInsideCircle)
        {
            // Player has entered the circle cast area for the first time.
            playerInsideCircle = true;
            nextTimeToAttack = Time.time + attackRate;
            // Trigger the animation.
            eyeAnim.SetBool("OpenEye", true); // Replace with your animation trigger name.
        }
        else if (playerCollider == null && playerInsideCircle)
        {
            // Player has left the circle cast area.
            playerInsideCircle = false;
            attackRate = initialAttackRate;
            // Trigger the same animation parameter to signal player departure.
            eyeAnim.SetBool("OpenEye", false); // Replace with your animation trigger name.
        }

        if (playerInsideCircle)
        {
            float distance = (transform.position - playerCollider.transform.position).magnitude;

            GameManager.Instance.playerController.playerMovement.SlowPlayerByPercent(Helper.Instance.Remap(5f, 2f, 0f, 70f, distance));

            if (Time.time >= nextTimeToAttack)
            {
                playerCollider.GetComponent<PlayerHealth>().DamagePlayer();
                nextTimeToAttack = Time.time + attackRate;
                attackRate = attackRate <= minimumAttackDelay ? minimumAttackDelay : attackRate - attackDecrementMultiplier;
            }

            wave.material.SetFloat("_ShockWaveStrength", Helper.Instance.Remap(5f, 1.5f, 0f, -0.3f, distance));

            topAnim.speed = Helper.Instance.Remap(5f, 3.5f, 1, 3f, distance);
            bottomAnim.speed = Helper.Instance.Remap(5f, 3.5f, 1, 3f, distance);


            // Draw a line from your transform to the player
            Debug.DrawLine(transform.position, playerCollider.transform.position, Color.green);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + (Vector3)groundCheckOffset, groundCheckCastLength);
    }
}
