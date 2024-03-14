using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetHealMode : MonoBehaviour
{
    public bool foundEnemy;
    public EnemyBase enemyHealth;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag("Player") && !col.gameObject.CompareTag("Boundry"))
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        if (col.gameObject.CompareTag("enemy"))
        {
            foundEnemy = true;
            enemyHealth = col.GetComponent<EnemyBase>();
            transform.parent = col.gameObject.transform;
        }
    }

    public void SetParentNull()
    {
        foundEnemy = false;
        transform.parent = null;
    }
}
