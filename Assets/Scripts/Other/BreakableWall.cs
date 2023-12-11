using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour, IDamageable
{
    public bool onHitFreezTime { get; set; }

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
        onHitFreezTime = false;
        effectSpawn = transform.GetChild(2);
        mask = transform.GetChild(0).GetComponent<SpriteMask>();
    }

    public void Damage(int damagePointMultiplier = 1)
    {
        hitPoints -= damagePointMultiplier;

        if (hitPoints > 0)
        {
            GameManager.Instance.CameraShake.ShakeCamera(3f, 0.15f, 0.2f);
            takenHits++;
            mask.sprite = stages[takenHits - 1];
            SpawnFx(Fx);
        }
        else
        {
            GameManager.Instance.CameraShake.ShakeCamera(5f, 0.2f, 0.3f);
            SpawnFx(Fx);
            mask.gameObject.SetActive(false);
            GameObject endFx = Instantiate(finalFx, effectSpawn.position + new Vector3(-7, 1.5f, 0), Quaternion.Euler(90, 0, 0));
            var p = endFx.GetComponent<ParticleSystem>();
            // Access the main module and set the start color
            var mainModule = p.main;
            mainModule.startColor = transform.GetChild(1).GetComponent<SpriteRenderer>().color;

            Destroy(endFx, 1);
            var sr = transform.GetChild(1).GetComponent<SpriteRenderer>();
            Color color = sr.color;
            color.a = afterBrokenOpacity;
            sr.color = color;

            GetComponent<BoxCollider2D>().enabled = false;
            transform.GetChild(1).GetComponent<BoxCollider2D>().enabled = false;

            if (transform.GetChild(1).childCount > 0)
            {
                foreach (Transform child in transform.GetChild(1))
                {
                    SpriteRenderer spr = child.GetComponent<SpriteRenderer>();
                    Color c = spr.color;
                    c.a = afterBrokenOpacity;
                    spr.color = c;
                    child.GetComponent<BoxCollider2D>().enabled = false;
                }
            }
        }
    }

    public void ApplyKnockBack(Vector2 direction)
    {
        //Wall wont get knocked backed, so the implimentation is NULL
    }

    void SpawnFx(GameObject FX)
    {
        GameObject fx = Instantiate(FX, effectSpawn.position, effectSpawn.rotation);
        var p = fx.GetComponent<ParticleSystem>();

        // Access the main module and set the start color
        var mainModule = p.main;
        mainModule.startColor = transform.GetChild(1).GetComponent<SpriteRenderer>().color;

        Destroy(fx, 1);
    }

}
