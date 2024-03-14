using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sloo : EnemyBase
{

    [Header("Movement")]
    public Transform[] waypoints;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float nextMoveTime = 0.2f; // Set the initial countdown time in seconds

    [Header("Attack")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private Vector2 playerCheckOffset;
    [SerializeField] private bool playerInCircle;

    [Header("Animation")]
    [SerializeField] private Animator bodyAnim;
    [SerializeField] private Animator tongueAnim;

    private float currentTime;
    private float initialScale;

    bool stop;


    private void Start()
    {
        currentTime = nextMoveTime;
        bodyAnim = transform.GetChild(0).GetComponent<Animator>();
        tongueAnim = transform.GetChild(1).GetComponent<Animator>();
        initialScale = transform.localScale.x;
    }

    void Update()
    {
        if (timeFreezed) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0.0f)
        {
            if (!stop)
            {
                Invoke(nameof(ResetTimer), 0.25f);
                stop = true;
            }
            MoveBetweenWaypoints();
        }

        DetectPlayer();
    }

    void ResetTimer()
    {
        currentTime = nextMoveTime; // Reset the timer
        stop = false;
    }

    void MoveBetweenWaypoints()
    {
        if (playerInCircle) return;

        transform.position = Vector2.MoveTowards(transform.position, waypoints[currentIndex].position, moveSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, waypoints[currentIndex].position) < 0.1f)
        {
            currentIndex = (currentIndex + 1) % waypoints.Length;
        }
    }

    private RaycastHit2D storedHit; // Declare a class-level variable to store hit

    void DetectPlayer()
    {

        RaycastHit2D hit = Physics2D.CircleCast(transform.position + (Vector3)playerCheckOffset, detectionRange, Vector2.zero, 0f, LayerMask.GetMask("Player"));
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            playerInCircle = true;
            tongueAnim.SetTrigger("attack");

            playerCheckOffset = new Vector2(currentIndex == 0 ? -1.5f : 1.5f, 100);

            storedHit = hit; // Store hit in the class-level variable

            Invoke(nameof(Run), 0f);
        }
        //else playerInCircle = false;

        if (playerInCircle) return;
        playerCheckOffset = new Vector2(currentIndex == 0 ? -1.5f : 1.5f, 0);

        //GFX.flipX = playerCheckOffset.x > 0;
        transform.localScale = playerCheckOffset.x > 0 ? new Vector2(-initialScale, transform.localScale.y) : new Vector2(initialScale, transform.localScale.y);
    }

    void Run()
    {
        bodyAnim.SetTrigger("turn");
        tongueAnim.SetTrigger("tongueOff");
        storedHit.collider.GetComponent<PlayerHealth>().DamagePlayer();
        currentIndex = (currentIndex + 1) % waypoints.Length;
        playerCheckOffset = new Vector2(currentIndex == 0 ? -1.5f : 1.5f, 0);
        playerInCircle = false;
        currentTime = 0; // Reset the timer
        stop = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + (Vector3)playerCheckOffset, detectionRange);
    }
}

