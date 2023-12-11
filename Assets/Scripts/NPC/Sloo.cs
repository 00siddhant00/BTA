using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sloo : EnemyBase
{

    [Header("Movement")]
    public Transform[] waypoints;
    [SerializeField] private float moveSpeed = 3f;

    [Header("Attack")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private Vector2 playerCheckOffset;
    [SerializeField] private bool playerInCircle;


    SpriteRenderer GFX;

    private void Start()
    {
        GFX = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        MoveBetweenWaypoints();
        DetectPlayer();
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

    void DetectPlayer()
    {
        playerCheckOffset = new Vector2(currentIndex == 0 ? -0.5f : 0.5f, 0);

        GFX.flipX = playerCheckOffset.x > 0;

        RaycastHit2D hit = Physics2D.CircleCast(transform.position + (Vector3)playerCheckOffset, detectionRange, Vector2.zero, 0f, LayerMask.GetMask("Player"));
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            playerInCircle = true;
            Debug.Log("Player is dead");

            hit.collider.GetComponent<PlayerHealth>().DamagePlayer();
            playerInCircle = false;
            currentIndex = (currentIndex + 1) % waypoints.Length;
        }
        else playerInCircle = false;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + (Vector3)playerCheckOffset, detectionRange);
    }
}

