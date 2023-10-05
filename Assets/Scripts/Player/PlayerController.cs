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

    public PlayerData playerDataNoPet;
    public PlayerData playerDataWithPet;
    public PlayerData playerDataHeal;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerHealth = GetComponent<PlayerHealth>();
        playerAnimator = GetComponent<PlayerAnimator>();
        playerCombat = GetComponent<PlayerCombat>();
        petAbality = GetComponent<PetAbality>();
    }
}
