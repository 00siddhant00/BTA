using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;

public class CutSceneTrigger : MonoBehaviour
{
    [SerializeField]
    private int sceneID;

    [Range(0f, 3f)]
    [SerializeField]
    private float waitDelay;

    public enum Mode
    {
        Duration,
        Event
    }

    public Mode mode = Mode.Duration;

    private GameObject Player;

    private void Start()
    {
        Player = GameObject.Find("Player");
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Play());
        }
    }

    private IEnumerator Play()
    {
        yield return new WaitForSeconds(waitDelay);

        foreach (CutSceneData scene in transform.parent.parent.parent.GetComponent<CutSceneManager>().cutScenes)
        {
            if (scene.id == sceneID)
            {
                CheckSpecificEvents(scene, 2, 0);   //Pet Interaction
                CheckSpecificEvents(scene, 3, 0);   //Pet Aquire
                GetComponent<Collider2D>().enabled = false;
                if (scene.played)
                {
                    CheckSpecificEvents(scene, 2, 1);
                    yield break;
                }

                if (mode == Mode.Event) yield break;

                try
                {
                    scene.Animation.Play();
                }
                catch (NullReferenceException)
                {
                    Debug.LogWarning($"{transform.parent.name} Timeline sequence is not assigined in CutsceneManager");
                }
                yield return new WaitForSeconds(scene.Animation != null ? (float)scene.Animation.duration : 0);

                CheckSpecificEvents(scene, 2, 1);   //Pet Interaction
                CheckSpecificEvents(scene, 3, 1);   //Pet Aquire
                scenePlaying = false;
                scene.played = true;

                yield break;
            }
        }
    }

    /// <summary>
    /// Trigger specific events by event index of that scene
    /// </summary>
    /// <param name="scene">Current scene</param>
    /// <param name="id">id to check</param>
    /// <param name="eventIndex">Execution event Index</param>
    private void CheckSpecificEvents(CutSceneData scene, int id, int eventIndex)
    {
        if (scene.id != id) return;

        switch (id)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                Player.GetComponent<PlayerAnimator>().isGrounded = false;
                scene.ModifyEvent[eventIndex].Invoke();
                break;
            case 3:
                scene.ModifyEvent[eventIndex].Invoke();
                break;
        }

        scenePlaying = true;
    }

    private void Update()
    {
        PlayerMoveToPetScene();
    }

    #region Dynamic Animations

    public bool scenePlaying;

    public void PlayerMoveToPetScene()
    {
        if (!Player.GetComponent<PlayerAnimator>().isGrounded) return;
        if (!scenePlaying) return;
        if (sceneID != 2) return;

        Vector3 targetPosition = new Vector3(40.1f, -4.81f, 0f);
        float moveSpeed = 3f; // Adjust the speed as needed

        float distanceToTarget = Vector3.Distance(Player.transform.position, targetPosition);

        // Check if the player is close enough to the target position
        if (distanceToTarget > 0.1f)
        {
            Player.transform.localScale = new(0.8f, 0.8f, 0.8f);
            Player.GetComponent<PlayerAnimator>().MoveAnim(true);
            // Adjust the Lerp factor based on the remaining distance
            float lerpFactor = Mathf.Clamp01(moveSpeed * Time.deltaTime / distanceToTarget);

            // Move the player towards the target position using adjusted Lerp
            Player.transform.position = Vector3.Lerp(Player.transform.position, targetPosition, lerpFactor);
        }
        else
        {
            // If the player is close enough, set the position directly to the target
            Player.GetComponent<PlayerAnimator>().MoveAnim(false);
            Player.transform.position = targetPosition;
            transform.parent.GetChild(1).GetChild(0).gameObject.SetActive(true);
            Player.SetActive(false);
            TurnOffFog();
            mode = Mode.Duration;
            StartCoroutine(Play());
        }

        void TurnOffFog()
        {
            foreach (Transform t in transform.parent.GetChild(2))
            {
                if (t.TryGetComponent<ParticleSystem>(out var particleSystem))
                {
                    var mainModule = particleSystem.main;
                    mainModule.maxParticles = 0; // or any other property you want to modify
                }
            }
        }
    }
    #endregion
}
