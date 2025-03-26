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
    [Header("Move Settings")]
    [SerializeField] private float runAcceleration = 0.25f;
    [SerializeField] private float runSpeed = 4.0f;
    [SerializeField] private float drag = 0.1f;

    [Header("Look Settings")]
    [SerializeField] private float lookSensitivityHorizontal = 0.1f;
    [SerializeField] private float lookSensitivityVertical = 0.1f;
    [SerializeField] private float lookLimitVertical = 65;







    private Vector2 cameraRotation = Vector2.zero;
    private Vector2 playerTargetRotation = Vector2.zero;

    // Component References
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera playerCamera;

    // Input References
    private Vector2 moveInput;
    private Vector2 lookInput;



 
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // set cameraForward and cameraRight based on the objects XZ plane
        Vector3 cameraForwardXZ = new Vector3(playerCamera.transform.forward.x, 0f, playerCamera.transform.forward.z).normalized;
        Vector3 cameraRightXZ = new Vector3(playerCamera.transform.right.x, 0f, playerCamera.transform.right.z).normalized;
        // Move direction is the direction the player will move based on the camera's forward and right vectors
        Vector3 moveDirection = cameraRightXZ * moveInput.x + cameraForwardXZ * moveInput.y;


        // Move delta is how much the player will move this frame
        Vector3 moveDelta = moveDirection * runAcceleration * Time.deltaTime;
        // New velocity is the current characterControler velocity plus the moveDelta
        Vector3 newVelocity = characterController.velocity + moveDelta;

        // add drag to player
        Vector3  currentDrag = newVelocity.normalized * drag * Time.deltaTime;
        // Ternary operator (if newVelocity.magnitude is greater than drag, subtract drag from newVelocity, else set newVelocity to zero)
        newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
        newVelocity = Vector3.ClampMagnitude(newVelocity, runSpeed);




        // apply movment to characterController, Time.deltaTime is used to normalize the movement to be frame rate independent
        // Note: characterController.Move (Unity suggegts only callinmg this once per frame to avoid issues)
        characterController.Move(newVelocity * Time.deltaTime);

    }

    private void LateUpdate()
    {
        cameraRotation.x += lookSensitivityHorizontal * lookInput.x;
        cameraRotation.y = Mathf.Clamp(cameraRotation.y - lookSensitivityVertical * lookInput.y, -lookLimitVertical, lookLimitVertical);

        playerTargetRotation.x += transform.eulerAngles.x + lookSensitivityHorizontal * lookInput.x;
        transform.rotation = Quaternion.Euler(0f, playerTargetRotation.x, 0);

        playerCamera.transform.rotation = Quaternion.Euler(cameraRotation.y, cameraRotation.x, 0f);
    }

    #region Input methods

    private void SetMoveInput(Vector2 inputVector)
    {
        moveInput = new Vector2(inputVector.x, inputVector.y);
    }

    private void SetLookInput(Vector2 inputVector)
    {
        lookInput = new Vector2(inputVector.x, inputVector.y);
    }


    private void OnEnable()
    {
        InputManager.Instance.LookEvent += SetLookInput;
        InputManager.Instance.MoveEvent += SetMoveInput;
        //InputManager.Instance.SprintEvent += SetSprintBool;
        //InputManager.Instance.JumpEvent += HandleJump;
        //InputManager.Instance.CrouchEvent += HandleCrouchInput;
    }



    private void OnDisable()
    {
        InputManager.Instance.LookEvent -= SetLookInput;
        InputManager.Instance.MoveEvent -= SetMoveInput;
        //InputManager.Instance.SprintEvent -= SetSprintBool;
        //InputManager.Instance.JumpEvent -= HandleJump;
        //InputManager.Instance.CrouchEvent -= HandleCrouchInput;
    }

    #endregion
}
