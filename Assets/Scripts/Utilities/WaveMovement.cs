using UnityEngine;

public class WaveMovement : MonoBehaviour
{
    public float amplitude = 1.0f; // Adjust the amplitude of the sine wave.
    public float speed = 1.0f;     // Adjust the speed of the sine wave movement.

    private Vector3 initialPosition;

    void Start()
    {
        // Store the initial position of the object.
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        // Calculate the new Y position based on the sine wave.
        float newY = initialPosition.y + Mathf.Sin(Time.time * speed) * amplitude;

        // Update the object's position.
        transform.localPosition = new Vector3(initialPosition.x, newY, initialPosition.z);
    }
}
