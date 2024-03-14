using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name References")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Action Name References")]
    [SerializeField] private string save = "Save";
    [SerializeField] private string move = "Move";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string mele = "Mele";
    [SerializeField] private string petThrow = "PetThrow";
    [SerializeField] private string petCallBack = "PetCallBack";
    [SerializeField] private string heal = "Heal";

    [Header("Action Ability Name References")]
    [SerializeField] private string ability = "Ability";

    internal InputAction saveAction;
    internal InputAction moveAction;
    internal InputAction jumpAction;
    internal InputAction meleAction;
    internal InputAction petThrowAction;
    internal InputAction petCallBackAction;
    internal InputAction healAction;
    internal InputAction abilityAction;

    public bool SaveInput { get; private set; }
    public Vector2 MoveInput { get; private set; }
    public int JumpTriggered { get; set; } = 0;
    public bool IsMele { get; private set; }
    public int IsPetThrow { get; set; } = 0;
    public bool IsPetCallBack { get; set; }
    public bool IsHealing { get; private set; }
    public int AbilityActivate { get; set; } = 0;

    public static PlayerInputHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }

        saveAction = playerControls.FindActionMap(actionMapName).FindAction(save);
        moveAction = playerControls.FindActionMap(actionMapName).FindAction(move);
        jumpAction = playerControls.FindActionMap(actionMapName).FindAction(jump);
        meleAction = playerControls.FindActionMap(actionMapName).FindAction(mele);
        petThrowAction = playerControls.FindActionMap(actionMapName).FindAction(petThrow);
        petCallBackAction = playerControls.FindActionMap(actionMapName).FindAction(petCallBack);
        healAction = playerControls.FindActionMap(actionMapName).FindAction(heal);
        abilityAction = playerControls.FindActionMap(actionMapName).FindAction(ability);

        RegisterInputActions();
    }

    private void RegisterInputActions()
    {
        saveAction.performed += context => SaveInput = (context.ReadValue<float>() > 0.1f);
        saveAction.canceled += context => SaveInput = false;

        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;

        jumpAction.performed += context => JumpTriggered = 1;
        jumpAction.canceled += context => JumpTriggered = 2;

        meleAction.performed += context => IsMele = true;
        meleAction.canceled += context => IsMele = false;

        petThrowAction.performed += context => IsPetThrow = 1;
        petThrowAction.canceled += context => IsPetThrow = 2;

        petCallBackAction.performed += context => IsPetCallBack = true;
        petCallBackAction.canceled += context => IsPetCallBack = false;

        healAction.performed += context => IsHealing = (context.ReadValue<float>() > 0.1f);
        healAction.canceled += context => IsHealing = false;

        abilityAction.performed += context => AbilityActivate = 1;
        abilityAction.canceled += context => AbilityActivate = 2;
    }

    private void OnEnable()
    {
        saveAction.Enable();
        moveAction.Enable();
        jumpAction.Enable();
        meleAction.Enable();
        petThrowAction.Enable();
        petCallBackAction.Enable();
        healAction.Enable();

        abilityAction.Enable();
    }

    private void OnDisable()
    {
        saveAction?.Disable();
        moveAction?.Disable();
        jumpAction?.Disable();
        meleAction?.Disable();
        petThrowAction?.Disable();
        petCallBackAction?.Disable();
        healAction?.Disable();

        abilityAction?.Disable();
    }
}
