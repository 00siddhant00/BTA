using UnityEngine;

[ExecuteInEditMode]
public class EventDataUpdate : MonoBehaviour
{
    int eventID;

    private EventManager eventM;

    private void Awake()
    {
        eventM = FindObjectOfType<EventManager>();
    }

    // Update is called once per frame
    void Update()
    {
        eventID = 0;
        foreach (var eventData in eventM.eventData)
        {
            // Update the tutorial ID
            eventData.EventID = eventID;

            eventID++;
        }
    }
}
