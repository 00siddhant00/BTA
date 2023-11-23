using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachineVirtualCamera;

    private float timer;

    private CinemachineBasicMultiChannelPerlin _cbmcp;

    public CinemachineConfiner Confiner;

    // Start is called before the first frame update
    void Awake()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        StopShake();
    }

    void SeeForward()
    {
        float targetOffsetX = GameManager.Instance.playerController.playerMovement.lookingRight ? 2f : -2f;
        float transitionSpeed = 1.2f; // Adjust the transition speed as needed

        // Use Lerp to smoothly transition between the current value and the target value
        float currentOffsetX = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset.x;
        float newOffsetX = Mathf.Lerp(currentOffsetX, targetOffsetX, Time.deltaTime * transitionSpeed);

        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset.x = newOffsetX;

    }

    public void ShakeCamera(float shakeIntensity = 1.0f, float shakeFrequency = 1.0f, float shakeTime = 0.2f)
    {
        _cbmcp = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmcp.m_AmplitudeGain = shakeIntensity;
        _cbmcp.m_FrequencyGain = shakeFrequency;

        timer = shakeTime;
    }

    void StopShake()
    {
        _cbmcp = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbmcp.m_AmplitudeGain = 0f;
        _cbmcp.m_FrequencyGain = 0f;

        timer = 0f;
    }

    /// <summary>
    /// Always looks ahead based on input direction
    /// </summary>

    void Update()
    {
        SeeForward();

        if (timer > 0)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                StopShake();
            }
        }
    }
}
