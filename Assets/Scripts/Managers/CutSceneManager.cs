using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

[System.Serializable]
public class CutSceneData
{
    public string sceneName;

    [ReadOnly]
    public int id;

    public Type type;
    public PlayableDirector Animation;

    public bool played;

    public UnityEvent[] ModifyEvent;

    public enum Type
    {
        Interactive,
        Static
    }

}

public class CutSceneManager : MonoBehaviour
{
    public CutSceneData[] cutScenes;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
