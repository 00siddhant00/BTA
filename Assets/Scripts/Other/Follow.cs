using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target;
    public float followSpeed;
    public bool destroyIfNullTarget;

    public enum FollowType
    {
        Fixed,
        Eventual
    }

    public FollowType followType;

    // Update is called once per frame
    void Update()
    {
        FollowTarget();
    }

    void FollowTarget()
    {
        if (target != null)
            if (followType == FollowType.Fixed)
                transform.position = target.position;
            else transform.position = Vector3.Lerp(transform.position, target.position, followSpeed);
        else if (destroyIfNullTarget && target == null)
        {
            Destroy(this.gameObject);
        }
    }
}
