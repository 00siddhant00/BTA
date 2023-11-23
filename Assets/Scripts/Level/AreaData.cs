using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaData : MonoBehaviour
{
    public bool playerInArea;
    public int noOfTotalSecttions;
    public int activeSectionId;
    public int noOfTotalEnemies;
    public int noOfTotalObstacles;
    public int noOfTotalDetachedPlatforms;
    public int noOfTotalStaticBlocks;
    public int noOfTotalBreakableWalls;

    private void Start()
    {
        noOfTotalSecttions = transform.childCount;
        AssignSectionID();
        if (playerInArea) print("Player in area");
    }

    void AssignSectionID()
    {
        for (int i = 0; i < noOfTotalSecttions; i++)
        {
            transform.GetChild(i).GetComponent<SectionData>().sectionID = i;
        }
    }

    void Update()
    {
        CheckPlayerAvaliblity();
        CheckActiveSection();
        CheckNoData();
    }

    void CheckPlayerAvaliblity()
    {
        foreach (Transform section in transform)
        {
            if (section.GetComponent<SectionData>().playerInSection == true)
            {
                this.playerInArea = true;
                return;
            }
        }

        this.playerInArea = false;
    }

    void CheckActiveSection()
    {
        foreach (Transform section in transform)
        {
            if (section.gameObject.activeSelf) this.activeSectionId = section.GetComponent<SectionData>().sectionID;
        }
    }

    void CheckNoData()
    {
        this.noOfTotalEnemies = 0;
        this.noOfTotalObstacles = 0;
        this.noOfTotalDetachedPlatforms = 0;
        this.noOfTotalStaticBlocks = 0;
        this.noOfTotalBreakableWalls = 0;

        foreach (Transform section in transform)
        {
            this.noOfTotalEnemies += section.GetComponent<SectionData>().noOfEnemies;
            this.noOfTotalObstacles += section.GetComponent<SectionData>().noOfObstacles;
            this.noOfTotalDetachedPlatforms += section.GetComponent<SectionData>().noOfDetachedPlatforms;
            this.noOfTotalStaticBlocks += section.GetComponent<SectionData>().noOfStaticBlocks;
            this.noOfTotalBreakableWalls += section.GetComponent<SectionData>().noOfBreakableWalls;
        }
    }
}
