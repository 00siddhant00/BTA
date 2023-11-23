using System.Collections;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int EnemyId;
    public int currentIndex;

    [Header("Health")]
    public int healthPoints = 2;

    [Header("Collision Check")]
    [SerializeField] protected float groundCheckCastLength;
    [SerializeField] protected Vector2 groundCheckOffset;
    [SerializeField] protected LayerMask groundLayerMask;
    [SerializeField] protected LayerMask playerLayer; // Set the layer for the player.
    protected float repelDistance = 100;

    [Header("Damage")]
    [SerializeField] protected bool isKnockedBack = false;
    [SerializeField] protected float knockbackForce = 10f; // Adjust the force of the knockback.
    [SerializeField] protected float knockbackDuration = 0.5f; // Adjust the duration of the knockback.

    //Miscellaneous
    protected Rigidbody2D rb; // Reference to the Rigidbody2D component.

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

    protected void StopCollide()
    {
        // Define the circle cast parameters
        Vector2 origin = transform.position;
        float radius = groundCheckCastLength;

        // Perform a circle cast to check for obstacles
        Collider2D hit = Physics2D.OverlapCircle(origin + groundCheckOffset, radius, playerLayer);

        if (hit != null)
        {
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

    public void Damage(int damagePointMultiplier = 1)
    {
        GameManager.Instance.CameraShake.ShakeCamera(3, 0.15f, 0.35f);
        healthPoints -= damagePointMultiplier;
        if (healthPoints <= 0)
        {
            if (transform.GetChild(transform.childCount - 1).GetComponent<PetHealMode>() != null && transform.GetChild(transform.childCount - 1).GetComponent<PetHealMode>().foundEnemy)
            {
                GameManager.Instance.playerController.petAbality.CallBackPet();
                GameManager.Instance.playerController.petAbality.StopHealing();
                GameManager.Instance.playerController.petAbality.isButtonUp = true;
            }
            GameManager.Instance.playerController.playerMovement.SlowPlayerByPercent(0f);
            Destroy(transform.parent.gameObject);
        }
    }
}
