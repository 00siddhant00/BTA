using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] bool invinsible;
    [SerializeField] int healthSlotes = 5;
    [SerializeField] Image img;
    float perChunkValue;

    public float playerCurrentHealth;

    PlayerHealth()
    {
        perChunkValue = 1f / healthSlotes;
    }

    // Start is called before the first frame update
    void Start()
    {
        img.material.SetFloat("_Health", 1);
        playerCurrentHealth = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F))
        //{

        //}
    }


    /// <summary>
    /// Damage the player based on number of chunks to damage
    /// </summary>
    /// <param name="damageMultiplier"> number of chunke to remove/take damage</param>
    public void DamagePlayer(int damageMultiplier = 1)
    {
        playerCurrentHealth = img.material.GetFloat("_Health") - (perChunkValue * damageMultiplier);

        float tolerance = 1e-6f; // A small tolerance value
        if (Mathf.Abs(playerCurrentHealth) < tolerance)
        {
            playerCurrentHealth = 0;
        }

        img.material.SetFloat("_Health", playerCurrentHealth);

        if (playerCurrentHealth <= 0)
        {
            if (!invinsible)
                GetComponent<PlayerMovement>().Reset();
        }
    }

    public void HealPlayer(int healMultiplier = 1)
    {
        playerCurrentHealth = img.material.GetFloat("_Health") + (perChunkValue * healMultiplier);

        img.material.SetFloat("_Health", playerCurrentHealth);

        if (playerCurrentHealth >= 1)
        {
            img.material.SetFloat("_Health", 1);
        }
    }
}
