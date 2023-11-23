using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GameData
{
    //Player Data
    public float PlayerHealth;
    public bool PlayerHasPet;
    public float PlayerPositionX;
    public float PlayerPositionY;

    public int LastActiveSection;

    //Enemy Data
    public List<Enemy> EnemiesInActiveSection = new List<Enemy>();
}
