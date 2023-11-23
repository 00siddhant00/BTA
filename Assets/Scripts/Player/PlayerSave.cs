using UnityEngine;

public class PlayerSave : MonoBehaviour
{
    private bool isEKeyPressed = false;
    private float eKeyPressTime = 0.0f;
    private bool hasSaved = false; // Add a flag to track if save has been performed

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            // Check if the 'E' key is being pressed and update the timer
            if (!isEKeyPressed)
            {
                isEKeyPressed = true;
                eKeyPressTime = Time.time;
            }
        }
        else
        {
            // 'E' key is not being pressed, reset the timer and the save flag
            isEKeyPressed = false;
            eKeyPressTime = 0.0f;
            hasSaved = false; // Reset the flag
        }

        // If 'E' key has been pressed for 2 seconds or more and hasn't been saved, call Save()
        if (isEKeyPressed && Time.time - eKeyPressTime >= 2.0f && !hasSaved)
        {
            Save();
            hasSaved = true; // Set the flag to indicate that the save has been performed
        }
    }

    void Save()
    {
        GameManager.Instance.playerController.playerHealth.DamagePlayer();

        GameManager.Instance.SaveGameData();
    }
}
