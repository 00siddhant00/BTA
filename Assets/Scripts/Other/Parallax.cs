using Unity.VisualScripting;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private Vector2 length, startPos;
    private Camera cam;

    public enum MoveDir
    {
        X,
        Y,
        Both
    }

    public MoveDir dir = MoveDir.X;

    [SerializeField] private float pVal;
    //[SerializeField] private float moveSpeed = 0f;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        startPos = dir == MoveDir.Both ? new Vector2(transform.position.x, transform.position.y) : dir == MoveDir.X ? new Vector2(transform.position.x, 0) : new Vector2(0, transform.position.y);
        //length = dir == MoveDir.X ? new Vector2(GetComponent<SpriteRenderer>().bounds.size.x, 0) : new Vector2(0, GetComponent<SpriteRenderer>().bounds.size.y);
    }

    // Update is called once per frame
    void Update()
    {
        //float temp = (dir == MoveDir.X ? cam.transform.position.x : cam.transform.position.y) * (1 - pVal) - accumulatedMove;
        Vector2 distance = (dir == MoveDir.Both ? new Vector2(cam.transform.position.x, cam.transform.position.y) : dir == MoveDir.X ? new Vector2(cam.transform.position.x, 0) : new Vector2(0, cam.transform.position.y)) * pVal;

        // Calculate the adjusted position
        Vector2 adjustedPosition = startPos + distance;

        // Update the object's position
        transform.position = new Vector3(dir == MoveDir.Both ? adjustedPosition.x : dir == MoveDir.X ? adjustedPosition.x : transform.position.x, dir == MoveDir.Both ? adjustedPosition.y : dir == MoveDir.X ? transform.position.y : adjustedPosition.y, transform.position.z);

        //if (temp > startPos + length) startPos += length;
        //else if (temp < startPos - length) startPos -= length;
    }
}
