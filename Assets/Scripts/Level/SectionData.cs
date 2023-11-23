using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SectionData : MonoBehaviour
{
    public int sectionID;
    public bool playerInSection;
    public bool hasEnemies;
    public int noOfEnemies;
    public List<GameObject> Enemies = new List<GameObject>();
    public int noOfObstacles;
    public int noOfDetachedPlatforms;
    public int noOfStaticBlocks;
    public int noOfBreakableWalls;
    public int noOfExits;

    [HideInInspector] public Collider2D confinerBoundry;
    public List<Transform> Gates = new List<Transform>();

    private void Awake()
    {
        Initialize();
        confinerBoundry = transform.GetChild(playerInSection ? 3 : 2).GetChild(0).GetComponent<Collider2D>();
    }

    private void Initialize()
    {
        CheckPlayerAvaliblity();
        CheckEnemyData();
        CheckExits();
        CheckPlatforms();
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerAvaliblity();
        CheckEnemyData();
        CheckObsteclas();
    }

    void CheckPlayerAvaliblity()
    {
        playerInSection = transform.GetChild(0).name == "Player";
    }

    void CheckEnemyData()
    {
        var enemies = transform.GetChild(playerInSection ? 1 : 0);
        if (enemies.name == "Enemies" && enemies.childCount > 0)
        {
            hasEnemies = true;
        }
        else
        {
            hasEnemies = false;
            noOfEnemies = 0;
        }

        if (hasEnemies)
        {
            Enemies.Clear();
            noOfEnemies = enemies.childCount;

            for (int i = 0; i < enemies.childCount; i++)
            {
                Enemies.Add(enemies.GetChild(i).gameObject);
            }
        }
        else Enemies.Clear();
        //if (!Once && playerInSection)
        //{
        //    print($"{gameObject.name} : {hasEnemies}");
        //    Once = true;
        //}
    }

    //bool Once;

    void CheckObsteclas()
    {
        var obsteclas = transform.GetChild(playerInSection ? 2 : 1);
        noOfObstacles = obsteclas.childCount;
    }

    void CheckPlatforms()
    {
        var elements = transform.GetChild(playerInSection ? 3 : 2);
        noOfDetachedPlatforms = elements.GetChild(1).childCount;
        noOfStaticBlocks = elements.GetChild(2).childCount;
        noOfBreakableWalls = elements.GetChild(3).childCount;
    }

    void CheckExits()
    {
        var exits = transform.GetChild(playerInSection ? 4 : 3);

        Gates.Clear();
        noOfExits = exits.childCount;

        for (int i = 0; i < noOfExits; i++)
        {
            Gates.Add(exits.GetChild(i));
        }
    }
}
