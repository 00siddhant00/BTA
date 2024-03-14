using UnityEngine;

[ExecuteInEditMode]
public class CutSceneDataUpdate : MonoBehaviour
{
    int sceneID;

    private CutSceneManager cutsceneM;

    private void Awake()
    {
        cutsceneM = FindObjectOfType<CutSceneManager>();
    }

    // Update is called once per frame
    void Update()
    {
        sceneID = 0;
        foreach (var cutSceneData in cutsceneM.cutScenes)
        {
            // Update the tutorial ID
            cutSceneData.id = sceneID;

            sceneID++;
        }
    }
}

