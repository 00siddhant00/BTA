using Unity.Collections;
using UnityEngine;

public class ReadOnlyAttribute : PropertyAttribute { }
[System.Serializable]
public class EventData
{
    public string EventName;

    [ReadOnly]
    public int EventID;

    [Multiline(5)]
    public string EventDescription;

    public bool EventCompleted;
}
