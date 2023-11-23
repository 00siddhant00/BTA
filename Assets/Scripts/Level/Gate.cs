using UnityEngine;

public class Gate : MonoBehaviour
{
    [Tooltip("This Gate ID")]
    public Vector2Int gateId;

    [Tooltip("Go To Gate ID")]
    public Vector2Int goToGateId;

    public enum SpawnDirection
    {
        left,
        right
    }

    [Tooltip("Direction to spawn after going from current gate to the next gate")]
    public SpawnDirection nextSpawnDirection;
}
