using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour
{
    [SerializeField] private GameObject spawnFx;
    private SpriteRenderer sr;
    private float moveImp;
    private GameObject light2D;

    void LightVisiblity(bool visiable) => light2D.SetActive(visiable);

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        light2D = transform.GetChild(0).gameObject;
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
        sr.flipX = !GetComponent<Follow>().target.parent.GetComponent<PlayerMovement>().lookingRight;
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
        LightVisiblity(true);
    }

    public void ShowPet()
    {
        sr.enabled = true;
        LightVisiblity(true);
    }

    public void HidePet()
    {
        sr.enabled = false;
        LightVisiblity(false);
    }
}
