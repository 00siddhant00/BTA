using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public EnemyData[] enemyDatas;

    public GameObject GetEnemyFromId(int enemyId)
    {
        foreach (EnemyData enemy in enemyDatas)
        {
            if (enemy.id == enemyId)
                return enemy.enemyPrefab;
        }

        Debug.LogError($"No Enemy with {enemyId} id found");
        return null;
    }
}
