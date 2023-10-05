using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int healthPoints = 2;

    public void Damage(int damagePointMultiplier = 1)
    {
        GameManager.Instance.CameraShake.ShakeCamera(3, 0.15f, 0.35f);
        healthPoints -= damagePointMultiplier;
        if (healthPoints <= 0)
        {
            if (transform.GetChild(transform.childCount - 1).GetComponent<PetHealMode>() != null && transform.GetChild(transform.childCount - 1).GetComponent<PetHealMode>().foundEnemy)
            {
                GameManager.Instance.playerController.petAbality.CallBackPet();
                GameManager.Instance.playerController.petAbality.StopHealing();
                GameManager.Instance.playerController.petAbality.isButtonUp = true;
            }
            Destroy(gameObject);
        }
    }
}
