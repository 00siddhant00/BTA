using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Pet : MonoBehaviour
{
    [SerializeField] private GameObject spawnFx;
    [SerializeField] private float maxTilt;
    [SerializeField][Range(0, 1)] private float tiltSpeed;
    private SpriteRenderer sr;
    private float moveImp;
    private GameObject light2D;
    private Vector2 initialScale;
    private Transform PetRing;

    void ObjVisiblity(GameObject obj, bool visiable) => obj.SetActive(visiable);

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        initialScale = transform.localScale;
        PetRing = transform.GetChild(0);
        light2D = transform.GetChild(1).gameObject;
    }


    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.playerController.petAbality.aquiredPet)
        {
            HidePet();
            return;
        }

        //sr.flipX = !GameManager.Instance.playerController.playerMovement.lookingRight;
        //sr.flipX = !GetComponent<Follow>().target.parent.GetComponent<PlayerMovement>().lookingRight;
        transform.localScale = !GetComponent<Follow>().target.parent.GetComponent<PlayerMovement>().lookingRight ? new(initialScale.x * -1, initialScale.y) : new(initialScale.x, initialScale.y);
    }

    private void LateUpdate()
    {
        PetAnimation();
    }

    public void SpawnFX()
    {
        GameObject spwnFx = Instantiate(spawnFx, transform.position - new Vector3(0, 0.7f, 0), Quaternion.Euler(-90, 0, 0));
        StartCoroutine(MakePetVisiable());
        Destroy(spwnFx, 1);
    }

    IEnumerator MakePetVisiable()
    {
        yield return new WaitForSeconds(0.06f);

        sr.enabled = true;
        ObjVisiblity(light2D, true);
        ObjVisiblity(PetRing.gameObject, true);
    }

    public void ShowPet()
    {
        sr.enabled = true;
        ObjVisiblity(light2D, true);
        ObjVisiblity(PetRing.gameObject, true);
    }

    public void HidePet()
    {
        sr.enabled = false;
        ObjVisiblity(light2D, false);
        ObjVisiblity(PetRing.gameObject, false);
    }

    void PetAnimation()
    {
        //Tilts the ring in moving direction according to given input
        RingTilt();
    }

    private void RingTilt()
    {
        var mov = GameManager.Instance.playerController.playerMovement;
        var tiltProgress = Mathf.InverseLerp(-mov.Data.runMaxSpeed, mov.Data.runMaxSpeed, mov.RB.velocity.x);
        var mult = (mov.IsFacingRight) ? 1 : -1;

        float newRot = ((tiltProgress * maxTilt * 2) - maxTilt);
        float rot = Mathf.LerpAngle(PetRing.localRotation.eulerAngles.z * mult, newRot, tiltSpeed);
        PetRing.localRotation = Quaternion.Euler(0, 0, rot * mult);
    }
}
