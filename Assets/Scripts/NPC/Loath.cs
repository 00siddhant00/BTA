using System.Collections;
using UnityEngine;

public class Loath : EnemyBase
{
    [Header("Movement")]
    public Transform[] waypoints;
    public float speed = 5f;
    public float wakeUpDelay = 1;

    public float goToSleepDelay;

    private Vector3 targetPosition;

    private int currentWaypointIndex;

    [Header("Visuals")]
    public Sprite[] spriteStates;

    public CapsuleCollider2D colliderStateSleep;
    public CapsuleCollider2D colliderStateAwake;

    public SpriteRenderer GFX;

    //States
    public enum State
    {
        sleeping,
        awake
    }

    public State state = State.sleeping;

    bool isAwake;

    // Start is called before the first frame update
    void Start()
    {
        GFX = transform.GetChild(0).GetComponent<SpriteRenderer>();
        currentWaypointIndex = 0;
    }

    private void Update()
    {
        if (waypoints.Length > 0)
        {
            // Start the movement coroutine
            //MoveToWaypoints();
        }
        else
        {
            Debug.LogError("No waypoints assigned!");
        }
    }

    void MoveToWaypoints()
    {
        // Set the initial target waypoint
        currentWaypointIndex %= waypoints.Length;

        // Calculate the sine wave offset for waypoint movement
        targetPosition = waypoints[currentWaypointIndex].position;

        // Check if we have reached the current waypoint
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentWaypointIndex++;
        }

        // Move towards the modified target position
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    IEnumerator AwakeState()
    {
        yield return new WaitForSeconds(wakeUpDelay);

        GFX.sprite = spriteStates[1];
        colliderStateSleep.enabled = false;
        colliderStateAwake.enabled = true;

        GFX.transform.localPosition = new Vector3(0, 0.12f, 0);

        yield return new WaitForSeconds(goToSleepDelay);
        SleepState();
    }

    void SleepState()
    {
        GFX.sprite = spriteStates[0];
        colliderStateSleep.enabled = true;
        colliderStateAwake.enabled = false;

        GFX.transform.localPosition = new Vector3(0, 0, 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //isAwake = true;
        if (collision.gameObject.CompareTag("Player"))
            StartCoroutine(AwakeState());
    }
}
