using System.Collections;
using UnityEngine;

public class Neph : EnemyData
{
    [Header("Movement")]
    [SerializeField] private Transform[] wayPoints;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float frequency = 2.0f;  // Adjust the frequency of the sine wave
    [SerializeField] private float amplitude = 1.0f;  // Adjust the amplitude of the sine wave
    int currentIndex;
    float time = 0f;

    [Header("Attack")]
    public float radius = 2f; // Set the radius of the circle cast.
    [SerializeField] private float attackRate;
    [SerializeField] private float attackDecrementMultiplier;
    [SerializeField] private float minimumAttackDelay;
    private float initialAttackRate;
    public LayerMask playerLayer; // Set the layer for the player.
    public Animator eyeAnim; // Reference to the Animator component for triggering animations.
    public Animator topAnim; // Reference to the Animator component for triggering animations.
    public Animator bottomAnim; // Reference to the Animator component for triggering animations.
    private bool playerInsideCircle = false; // Track whether the player is inside the circle.
    private float nextTimeToAttack;
    Collider2D playerCollider;
    Vector3 targetPosition;

    [Header("Damage")]
    [SerializeField] private float knockbackForce = 10f; // Adjust the force of the knockback.
    [SerializeField] private float knockbackDuration = 0.5f; // Adjust the duration of the knockback.
    private bool isKnockedBack = false;

    [Header("Collision Check")]
    [SerializeField] float groundCheckCastRadius;
    [SerializeField] Vector2 groundCheckOffset;
    [SerializeField] float repelDistance;
    [SerializeField] LayerMask groundLayerMask;

    [Header("FX")]
    [SerializeField] private SpriteRenderer wave;

    //Miscellaneous
    private Rigidbody2D rb; // Reference to the Rigidbody2D component.

    // Start is called before the first frame update
    void Start()
    {
        initialAttackRate = attackRate;
        currentIndex = 0;
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component.
        topAnim = GetComponent<Animator>();
        bottomAnim = transform.GetChild(0).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isKnockedBack)
        {
            currentIndex %= wayPoints.Length;

            if (playerInsideCircle)
            {
                // Define the circle cast parameters
                Vector2 origin = transform.position;
                float radius = groundCheckCastRadius;

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
                StopCollide();
            }
            else
            {
                // Define the circle cast parameters
                Vector2 origin = transform.position;
                float radius = groundCheckCastRadius;

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

        // Check for player presence
        Attack();
    }

    void StopCollide()
    {
        // Define the circle cast parameters
        Vector2 origin = transform.position;
        float radius = groundCheckCastRadius;

        // Perform a circle cast to check for obstacles
        Collider2D hit = Physics2D.OverlapCircle(origin + groundCheckOffset, radius, playerLayer);

        if (hit != null)
        {
            print("Freezed");
            // Freeze the Rigidbody2D's position and rotation
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else
        {
            // Freeze the Rigidbody2D's position and rotation
            rb.constraints = RigidbodyConstraints2D.None;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }


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
            if (Time.time >= nextTimeToAttack)
            {
                playerCollider.GetComponent<PlayerHealth>().DamagePlayer();
                nextTimeToAttack = Time.time + attackRate;
                attackRate = attackRate <= minimumAttackDelay ? minimumAttackDelay : attackRate - attackDecrementMultiplier;
            }

            float distance = (transform.position - playerCollider.transform.position).magnitude;
            wave.material.SetFloat("_ShockWaveStrength", Remap(5f, 1.5f, 0f, -0.3f, distance));

            topAnim.speed = Remap(5f, 3.5f, 1, 3f, distance);
            bottomAnim.speed = Remap(5f, 3.5f, 1, 3f, distance);

            // Draw a line from your transform to the player
            Debug.DrawLine(transform.position, playerCollider.transform.position, Color.green);
        }
    }

    float Remap(float origFrom, float origTo, float targetFrom, float targetTo, float value)
    {
        float rel = Mathf.InverseLerp(origFrom, origTo, value);
        return Mathf.Lerp(targetFrom, targetTo, rel);
    }

    public void ApplyKnockBack(Vector2 direction)
    {
        isKnockedBack = true;
        rb.velocity = Vector2.zero; // Clear any existing velocity.
        rb.AddForce(direction.normalized * knockbackForce, ForceMode2D.Impulse);
        //check if any obstecal in way if yes then make velocity zero
        StartCoroutine(EndKnockBack(knockbackDuration));
    }


    private IEnumerator EndKnockBack(float duration)
    {
        float startTime = Time.time;
        Vector2 initialVelocity = rb.velocity;

        while (Time.time < startTime + duration)
        {
            // Calculate the reduction factor based on time progress.
            float reductionFactor = 1f - ((Time.time - startTime) / duration);

            // Gradually reduce the velocity based on the reduction factor.
            rb.velocity = initialVelocity * reductionFactor;

            yield return null; // Wait for the next frame.
        }

        // Ensure velocity is completely zeroed out when the coroutine finishes.
        rb.velocity = Vector2.zero;

        isKnockedBack = false;
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + (Vector3)groundCheckOffset, groundCheckCastRadius);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

        }
    }
}
