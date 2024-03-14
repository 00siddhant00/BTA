using UnityEngine;
using UnityEngine.Rendering;

public class Follow : MonoBehaviour
{
    public Transform target;
    public float followSpeed;
    public bool destroyIfNullTarget;

    public bool follow = true;

    public enum FollowType
    {
        Fixed,
        Eventual
    }

    public FollowType followType;

    public void FollowTarget(bool follow)
    {
        this.follow = follow;
    }

    public void ChangeTarget(Transform target)
    {
        this.target = target;
    }

    // Update is called once per frame
    void Update()
    {
        FollowTarget();
    }

    void FollowTarget()
    {
        if (!follow) return;

        if (target != null)
            if (followType == FollowType.Fixed)
                transform.position = target.position;
            else transform.position = Vector3.Lerp(transform.position, target.position, followSpeed * Time.deltaTime);
        else if (destroyIfNullTarget && target == null)
        {
            Destroy(this.gameObject);
        }
    }
}
