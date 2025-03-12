using System;
using Unity.VisualScripting.InputSystem;
using UnityEditor.ShaderGraph.Drawing;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.XR;

[RequireComponent(typeof(CharacterController))]
public class Archive_FPS_PlayerController : MonoBehaviour
{
    private enum PlayerState { Idle, Walking, Sprinting, Jumping, Falling, Crouching }
    private PlayerState currentState = PlayerState.Idle;

    public string DebugCurrentState;

    private float gravity = -9.81f;

    [Header("Movement Settings")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 7f;
    public float crouchSpeed = 2f;
    public float airControlSpeed = 0.5f;

    private Vector2 moveInput;

    private float moveSpeed;

    public bool isGrounded;
    public bool isSprinting;
    public bool isCrouching;


    [Header("Look Settings")]
    public float lookSensitivity = 2f;
    public float maxLookAngle = 85f;

    private CharacterController characterController;
    //private Vector3 moveVelocity;



    private float verticalLookRotation = 0f;
    private Transform cameraTransform;



    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
        moveSpeed = walkSpeed;
        ChangeState(PlayerState.Idle);
    }

    private void Update()
    {
        HandleMovement();
        CheckGrounded();
        ApplyGravity();
    }

    private void ApplyGravity()
    {
        //apply gravity to the player
        if (characterController.isGrounded == false)
        {
            moveInput.y += gravity * Time.deltaTime;
        }
        
    }

    private void CheckGrounded()
    {
        //TODO:  implement a better ground check method to replace the Character controller isGrounded

        isGrounded = characterController.isGrounded; 
    }


    void HandleMovement()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                // no movement to apply... although we might want to set the the movement to zero?
                // set idle animation

                if (isGrounded == true && moveInput.magnitude > 0)
                {
                    ChangeState(PlayerState.Walking);
                }
                else if (isGrounded == false)
                {
                    Debug.Log("Falling");
                }
                break;


            case PlayerState.Walking:

                // set things related to Walking, speed, animations etc
                ApplyMovement(walkSpeed);

                if (isGrounded == true && moveInput.magnitude == 0)
                {
                    ChangeState(PlayerState.Idle);
                }

                if (isGrounded == true && isSprinting == true)
                {
                    ChangeState(PlayerState.Sprinting);
                }

                else if (isGrounded == false)
                {
                    Debug.Log("Falling");
                }
                break;

            case PlayerState.Sprinting:

                // set things related to Sprinting, speed, animations etc
                ApplyMovement(sprintSpeed);

                if (isGrounded == true && isSprinting == false)
                {
                    ChangeState(PlayerState.Walking);
                }

                else if (isGrounded == false)
                {
                    Debug.Log("Falling");
                }
                break;


            default:
                break;

        }



    }

    private void ApplyMovement(float speed)
    {
        Vector3 moveVector = new Vector3(moveInput.x, 0, moveInput.y) * speed * Time.deltaTime;
        characterController.Move(moveVector);
    }


    private void ChangeState(PlayerState newState)
    {
        currentState = newState;
        DebugCurrentState = currentState.ToString();
    }





    #region Input methods

    private void SetMoveInput(Vector2 inputVector)
    {
        moveInput = new Vector3(inputVector.x, 0, inputVector.y);
    }

    private void CheckSprintInput(bool sprintInputState)
    {
        isSprinting = sprintInputState;
    }

    private void CheckCrouchInput(bool crouchInputState)
    {
        isCrouching = crouchInputState;
    }

    private void OnEnable()
    {
        InputManager.Instance.MoveEvent += SetMoveInput;

        InputManager.Instance.SprintEvent += CheckSprintInput;

        InputManager.Instance.CrouchEvent += CheckCrouchInput;
    }



    private void OnDisable()
    {
        InputManager.Instance.MoveEvent -= SetMoveInput;

        InputManager.Instance.SprintEvent -= CheckSprintInput;

        InputManager.Instance.CrouchEvent -= CheckCrouchInput;
    }


    #endregion









}