using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Door : MonoBehaviour
{
    public bool OpenJustOnce = false;
    public float openSpeed;
    public enum MoveDirection
    {
        up,
        down,
        left,
        right
    }

    public enum OpenType
    {
        Smooth,
        Snap
    }

    public OpenType openType;

    public MoveDirection moveDirection;
    public float maxMoveDistance = 1f;

    private Vector3 originalPosition;
    private Vector3 targetPosition;
    public bool isOpening = false;

    public bool opened;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        targetPosition = originalPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isOpening)
        {
            if (opened) return;
            // If not opening, smoothly move back to the original position
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, (openType == OpenType.Smooth ? openSpeed : 1000) * Time.deltaTime);
        }

        if (isOpening && OpenJustOnce)
        {
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        isOpening = true;

        // Set the target position based on the selected move direction
        switch (moveDirection)
        {
            case MoveDirection.up:
                targetPosition = originalPosition + Vector3.up * maxMoveDistance;
                break;
            case MoveDirection.down:
                targetPosition = originalPosition + Vector3.down * maxMoveDistance;
                break;
            case MoveDirection.left:
                targetPosition = originalPosition + Vector3.left * maxMoveDistance;
                break;
            case MoveDirection.right:
                targetPosition = originalPosition + Vector3.right * maxMoveDistance;
                break;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, (openType == OpenType.Smooth ? openSpeed : 1000) * Time.deltaTime);

        if (OpenJustOnce)
        {
            opened = true;
        }
    }

    public void CloseDoor()
    {
        isOpening = false;
    }
}
