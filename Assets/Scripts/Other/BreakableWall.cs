using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    public enum TypeOfWall
    {
        Noraml,
        Hidden
    }

    public TypeOfWall typeOfWall;

    public bool broken;

    [SerializeField]
    private int hitPoints;

    [Range(0f, 1f)]
    public float afterBrokenOpacity;

    [SerializeField]
    private Sprite[] stages;

    private SpriteMask mask;

    private int takenHits;

    [SerializeField] private GameObject Fx;
    [SerializeField] private GameObject finalFx;
    private Transform effectSpawn;

    // Start is called before the first frame update
    void Start()
    {
        effectSpawn = transform.GetChild(2);
        mask = transform.GetChild(0).GetComponent<SpriteMask>();
    }

    public void Damage()
    {
        hitPoints--;

        if (hitPoints > 0)
        {
            GameManager.Instance.CameraShake.ShakeCamera(3f, 0.15f, 0.2f);
            takenHits++;
            mask.sprite = stages[takenHits - 1];
            SpawnFx();
        }
        else
        {
            GameManager.Instance.CameraShake.ShakeCamera(5f, 0.2f, 0.3f);
            SpawnFx();
            GameObject endFx = Instantiate(finalFx, effectSpawn.position + new Vector3(-7, 1.5f, 0), Quaternion.Euler(90, 0, 0));
            Destroy(endFx, 1);
            var sr = transform.GetChild(1).GetComponent<SpriteRenderer>();
            Color color = sr.color;
            color.a = afterBrokenOpacity;
            sr.color = color;

            mask.gameObject.SetActive(false);
            GetComponent<BoxCollider2D>().enabled = false;
            transform.GetChild(1).GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    void SpawnFx()
    {
        GameObject fx = Instantiate(Fx, effectSpawn.position, effectSpawn.rotation);
        Destroy(fx, 1);
    }
}
