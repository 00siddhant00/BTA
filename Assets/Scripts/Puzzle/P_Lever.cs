using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P_Lever : PuzzleBase
{
    private Transform GFX;
    public P_Door pDoor;

    bool leverPulled;

    // Start is called before the first frame update
    void Start()
    {
        if (transform.childCount > 0)
            GFX = transform.GetChild(0);
        else
            Debug.LogWarning("GFX doesn't exist");

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
            EnableLeaver();
        }
    }

    void EnableLeaver()
    {
        if (leverPulled) return;
        // Rotate GFX from its original rotation to current rotation + 60 degrees
        StartCoroutine(RotateGFX(transform.localScale.x < 0 ? -60f : 60f, 0.1f));
    }

    IEnumerator RotateGFX(float angle, float duration)
    {
        float elapsed = 0f;
        Quaternion startRotation = GFX.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, 0, angle);

        while (elapsed < duration)
        {
            GFX.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        GFX.rotation = targetRotation; // Ensure it reaches the exact target rotation
        leverPulled = true;
    }
}
