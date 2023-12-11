using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyData
{
    public string name;
    public int id;
    public int healthPoints;
    public GameObject enemyPrefab;
}

[System.Serializable]
public class Enemy
{
    public int id;
    public int CurrentHealthPoints;
    public bool isImmortal;

    public float enemyParentPosX;
    public float enemyParentPosY;

    public float enemyPosX;
    public float enemyPosY;

    public int wayPointAproching;
}
