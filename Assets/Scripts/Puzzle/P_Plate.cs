using UnityEngine;

public class P_Plate : PuzzleBase
{
    private Transform GFX;
    public P_Door pDoor;

    // Start is called before the first frame update
    void Start()
    {
        if (transform.childCount > 0)
            GFX = transform.GetChild(0);
        else Debug.LogWarning("GFX dosnt exist");

        AddDoorToPlatesCheck();
    }

    public void AddDoorToPlatesCheck()
    {
        OnKeyEnabled += pDoor.OpenDoor;
    }

    private void OnDestroy()
    {
        OnKeyEnabled -= pDoor.OpenDoor;
    }
    // Update is called once per frame
    void Update()
    {
        CheckIfKeyEnabled();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            keyEnabled = true;
            PressPlate(keyEnabled);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            keyEnabled = false;
            PressPlate(keyEnabled);
            pDoor.CloseDoor();
        }
    }

    void PressPlate(bool pressed)
    {
        if (pressed)
        {
            GFX.localPosition = new Vector3(0f, -0.33f, 0f);
        }
        else GFX.localPosition = new Vector3(0f, 0f, 0f);
    }
}
