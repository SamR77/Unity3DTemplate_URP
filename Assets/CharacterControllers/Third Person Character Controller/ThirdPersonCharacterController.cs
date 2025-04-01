using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sam Robichaud 
// NSCC Truro 2025
// This work is licensed under CC BY-SA 4.0 (https://creativecommons.org/licenses/by-sa/4.0/)

[RequireComponent(typeof(CharacterController))]
[DefaultExecutionOrder(-1)]
public class ThirdPersonCharacterController : MonoBehaviour
{
    #region Class Variables

    [SerializeField] private bool holdToSprint = true;  // true = HOLD to sprint, false = TOGGLE to sprint

    public bool sprintingOn;


    [Header("Move Settings")]
    [SerializeField] private float walkAcceleration = 0.25f;
    [SerializeField] private float walkSpeed = 4.0f;
    [SerializeField] private float drag = 0.1f;
    [SerializeField] private float movingThreshold = 0.01f;

    [SerializeField] private float sprintAcceleration = 0.25f;
    [SerializeField] private float sprintSpeed = 0.25f;

    [Header("Look Settings")]
    [SerializeField] private float lookSensitivityHorizontal = 0.1f;
    [SerializeField] private float lookSensitivityVertical = 0.1f;
    [SerializeField] private float lookLimitVertical = 65;






    private CharacterState characterState;

    private Vector2 cameraRotation = Vector2.zero;
    private Vector2 playerTargetRotation = Vector2.zero;

    // Component References
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera playerCamera;

    // Input References
    public Vector2 moveInput;
    private Vector2 lookInput;

    #endregion

    #region Startup

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        characterState = GetComponent<CharacterState>();
    }

    #endregion

    #region Update Logic


    // Update is called once per frame
    void Update()
    {
        UpdateMomementState();
        HandleLateralMovement();
    }

    private void UpdateMomementState()
    {
        bool isMovementInput = moveInput != Vector2.zero;                       // order of 
        bool isMovingLaterally = IsMovingLaterally();                           // these 3 lines
        bool isSprinting = sprintingOn == true && isMovingLaterally == true;    // matter


      

        CharacterMovementState lateralState = isSprinting ? CharacterMovementState.Sprinting : 
                                              isMovingLaterally || isMovementInput ? CharacterMovementState.Walking :
                                              CharacterMovementState.Idling;

        characterState.SetCharacterMovementState(lateralState);

    }


    private void HandleLateralMovement()
    {
        // create quick reference for current state
        bool isSprinting = characterState.currentCharacterMovementState == CharacterMovementState.Sprinting;
        

        // state dependant acceleration and speed
        float lateralAcceleration = isSprinting ? sprintAcceleration : walkAcceleration;
        float clampLateralMagnitude = isSprinting ? sprintSpeed : walkSpeed;

        // set cameraForward and cameraRight based on the objects XZ plane
        Vector3 cameraForwardXZ = new Vector3(playerCamera.transform.forward.x, 0f, playerCamera.transform.forward.z).normalized;
        Vector3 cameraRightXZ = new Vector3(playerCamera.transform.right.x, 0f, playerCamera.transform.right.z).normalized;
        // Move direction is the direction the player will move based on the camera's forward and right vectors
        Vector3 moveDirection = cameraRightXZ * moveInput.x + cameraForwardXZ * moveInput.y;


        // Move delta is how much the player will move this frame
        Vector3 moveDelta = moveDirection * lateralAcceleration * Time.deltaTime;
        // New velocity is the current characterControler velocity plus the moveDelta
        Vector3 newVelocity = characterController.velocity + moveDelta;

        // add drag to player
        Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
        // Ternary operator (if newVelocity.magnitude is greater than drag, subtract drag from newVelocity, else set newVelocity to zero)
        newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
        newVelocity = Vector3.ClampMagnitude(newVelocity, clampLateralMagnitude);




        // apply movment to characterController, Time.deltaTime is used to normalize the movement to be frame rate independent
        // Note: characterController.Move (Unity suggegts only callinmg this once per frame to avoid issues)
        characterController.Move(newVelocity * Time.deltaTime);
    }

    #endregion

    #region LateUpdate Logic

    private void LateUpdate()
    {
        cameraRotation.x += lookSensitivityHorizontal * lookInput.x;
        cameraRotation.y = Mathf.Clamp(cameraRotation.y - lookSensitivityVertical * lookInput.y, -lookLimitVertical, lookLimitVertical);

        playerTargetRotation.x += transform.eulerAngles.x + lookSensitivityHorizontal * lookInput.x;
        transform.rotation = Quaternion.Euler(0f, playerTargetRotation.x, 0);

        playerCamera.transform.rotation = Quaternion.Euler(cameraRotation.y, cameraRotation.x, 0f);
    }

    #endregion

    #region State Checks

    private bool IsMovingLaterally()
    {
        Vector3 laeralVelocity = new Vector3(characterController.velocity.x, 0f, characterController.velocity.z);

        return laeralVelocity.magnitude > movingThreshold;
    }

    #endregion

    #region Input methods

    private void SetMoveInput(Vector2 inputVector)
    {
        moveInput = new Vector2(inputVector.x, inputVector.y);
    }

    private void SetLookInput(Vector2 inputVector)
    {
        lookInput = new Vector2(inputVector.x, inputVector.y);
    }

    

    private void SprintInputStarted()
    {
        if(holdToSprint == true)
        {
            sprintingOn = true;
        }
        else if (holdToSprint == false)
        {
            sprintingOn = !sprintingOn;
        }
    }

    private void SprintInputCanceled()
    {
        if(holdToSprint == true)
        {
            sprintingOn = false;
        }
        else if (holdToSprint == false)
        {
            return;
        }
        
    }



    private void OnEnable()
    {
        InputManager.Instance.LookEvent += SetLookInput;
        InputManager.Instance.MoveEvent += SetMoveInput;
        InputManager.Instance.SprintStartedEvent += SprintInputStarted;
        InputManager.Instance.SprintCanceledEvent += SprintInputCanceled;


        //InputManager.Instance.JumpEvent += HandleJump;
        //InputManager.Instance.CrouchEvent += HandleCrouchInput;
    }



    private void OnDisable()
    {
        InputManager.Instance.LookEvent -= SetLookInput;
        InputManager.Instance.MoveEvent -= SetMoveInput;
        InputManager.Instance.SprintStartedEvent -= SprintInputStarted;
        InputManager.Instance.SprintCanceledEvent -= SprintInputCanceled;

        //InputManager.Instance.JumpEvent -= HandleJump;
        //InputManager.Instance.CrouchEvent -= HandleCrouchInput;
    }



    #endregion
}
