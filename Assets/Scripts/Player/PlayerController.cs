using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public PlayerHealth playerHealth;
    [HideInInspector] public PlayerAnimator playerAnimator;
    [HideInInspector] public PlayerCombat playerCombat;
    [HideInInspector] public PetAbality petAbality;

    public GameObject TimeFreezPanal;
    public PlayerData playerDataNoPet;
    public PlayerData playerDataWithPet;
    public PlayerData playerDataHeal;

    private void Awake()
    {
        TimeFreezPanal = transform.GetChild(transform.childCount - 1).gameObject;
        TimeFreezPanal.SetActive(false);
        playerMovement = GetComponent<PlayerMovement>();
        playerHealth = GetComponent<PlayerHealth>();
        playerAnimator = GetComponent<PlayerAnimator>();
        playerCombat = GetComponent<PlayerCombat>();
        petAbality = GetComponent<PetAbality>();

        playerMovement.allowPlayerMovement = true;

        playerDataWithPet.runMaxSpeed = playerMovement.runMaxSpeed;
        playerDataNoPet.runMaxSpeed = playerMovement.runMaxSpeedNoPet;
        playerDataHeal.runMaxSpeed = 0.01f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Gate gate))
        {
            StartCoroutine(ChangeSection(gate));
        }
    }

    private IEnumerator ChangeSection(Gate gate)
    {
        ScenesManager scenesManager = GameManager.Instance.SceneManager;
        var _currentSectionId = GameManager.Instance.AreaData.activeSectionId;

        //Sets goto Section as Enabled (to assign and retrive data as well)
        var _nextSetion = GameManager.Instance.AreaData.transform.GetChild(gate.goToGateId.x);
        _nextSetion.gameObject.SetActive(true);

        GameManager.Instance.pet.transform.parent = _nextSetion;

        this.transform.parent = _nextSetion;
        this.transform.SetAsFirstSibling();

        scenesManager.Fade();
        StartCoroutine(TogglePlayerMovement(false, 0.27f));
        yield return new WaitForSeconds((scenesManager.fadeDuration));
        StartCoroutine(TogglePlayerMovement(true, (scenesManager.fadeDuration + (scenesManager.waitForFadeOut / 2)) - 0.12f));

        //Spwans Player to next gate position
        this.transform.position = scenesManager.GetGatePoseFromID(gate.goToGateId, gate.nextSpawnDirection);
        GameManager.Instance.pet.GetComponent<Follow>().followType = Follow.FollowType.Fixed;

        yield return new WaitForSeconds(0.1f);

        //Sets last section exited as Disabled
        GameManager.Instance.AreaData.transform.GetChild(_currentSectionId).gameObject.SetActive(false);
        scenesManager.ChangeSectionConfinier(gate.goToGateId.x);
        GameManager.Instance.pet.GetComponent<Follow>().followType = Follow.FollowType.Eventual;
    }

    IEnumerator TogglePlayerMovement(bool allow, float duration)
    {
        yield return new WaitForSeconds(duration);
        playerMovement.allowPlayerMovement = allow;
    }
}
