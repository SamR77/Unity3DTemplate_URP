using System.Collections;
using UnityEngine;

// Sam Robichaud 
// NSCC Truro 2025
// This work is licensed under CC BY-SA 4.0 (https://creativecommons.org/licenses/by-sa/4.0/)



[RequireComponent(typeof(CharacterController))]
public class FirstPersonCharacterController : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private float crouchMoveSpeed = 2;
    [SerializeField] private float walkMoveSpeed = 3;
    [SerializeField] private float sprintMoveSpeed = 5;

    [Header("Look Settings")]
    [SerializeField] private float horizontalLookSensitivity = 15;
    [SerializeField] private float verticalLookSensitivity = 15;

    [SerializeField] private float minLookAngle = -60;
    [SerializeField] private float maxLookAngle = 60;

    [SerializeField] private bool invertLookY = false;

    private Vector3 newCameraRotation;
    private Vector3 newCharacterRotation;


    [Header("Jump & Gravity")]
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private bool jumpInputPerformed;
    private Vector3 velocity;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchingHeight = 0.5f;
    [SerializeField] private float standingHeight = 2.0f;
    [SerializeField] private float crouchTransitionTime = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0,0.5f,0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);

    private bool isCrouching;
    private bool crouchTransitioning;




    // Input variables
    private Vector2 lookInput;
    private Vector2 moveInput;

    [Header("References")]
    private CharacterController characterController;
    [SerializeField] private Transform cameraHolder;


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        newCameraRotation = cameraHolder.localRotation.eulerAngles;
        newCharacterRotation = transform.localRotation.eulerAngles;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleMove();
        IsGrounded();
        ApplyGravity();
    }

    private void LateUpdate()
    {
        HandleLook();
    }


    private void HandleMove()
    {
        // Step 1: Create a movement vector based on input
        Vector3 moveInputDirection = new Vector3(moveInput.x, 0, moveInput.y);

        // Step 2: Convert local movement into world space based on player’s orientation
        Vector3 worldMoveDirection = transform.TransformDirection(moveInputDirection);

        // Step 3: Apply movement to the CharacterController
        characterController.Move(worldMoveDirection * walkMoveSpeed * Time.deltaTime);
    }

    private void HandleLook()
    {
        // Set camera rotation for Vertical (Up/Down) Look
        newCameraRotation.x += verticalLookSensitivity * (invertLookY ? lookInput.y : -lookInput.y) * Time.deltaTime;
        newCameraRotation.x = Mathf.Clamp(newCameraRotation.x, minLookAngle, maxLookAngle);

        // Set character rotation for Horizontal (Left/Right) Look
        newCharacterRotation.y += horizontalLookSensitivity * lookInput.x * Time.deltaTime;

        // Apply rotations to camera and character
        cameraHolder.localRotation = Quaternion.Euler(newCameraRotation);
        transform.localRotation = Quaternion.Euler(newCharacterRotation);
    }

    private void HandleJump()
    {
        if (IsGrounded() == true)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // Jump physics
        }
    }

    private IEnumerator HandleCrouchStand()
    {
        // TODO: Add a check to see if the player is able to crouch (not under a low ceiling, etc)
        // TODO: Expand to allow player to select between Hold and Toggle crouch


        crouchTransitioning = true;

        float timeElapsed = 0;

        float targetHeight = 0;
        float currentHeight = characterController.height;
        Vector3 targetCenter = Vector3.zero;
        Vector3 currentCenter = Vector3.zero;

        if (isCrouching == true)
        {
            targetHeight = standingHeight;
            targetCenter = standingCenter;
            currentHeight = characterController.height;
            currentCenter = characterController.center;
        }
        else if (isCrouching == false)
        {
            targetHeight = crouchingHeight;
            targetCenter = crouchingCenter;
            currentHeight = characterController.height;
            currentCenter = characterController.center;
        }

        while (timeElapsed < crouchTransitionTime)
        {            
            float percent = timeElapsed / crouchTransitionTime;

            characterController.height = Mathf.Lerp(currentHeight, targetHeight, percent);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, percent);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        characterController.height = targetHeight;
        characterController.center = targetCenter;

        isCrouching = !isCrouching;

        crouchTransitioning = false;
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private bool IsGrounded()
    {
        // eventually will replace with a more robust groundCheck
        return characterController.isGrounded;
    }







    #region Input methods

    private void SetMoveInput(Vector2 inputVector)
    {
        moveInput = new Vector2(inputVector.x, inputVector.y);
    }

    private void SetLookInput(Vector2 vector)
    {
        lookInput = new Vector2(vector.x, vector.y);
    }


    private void HandleCrouchInput()
    {
        StartCoroutine(HandleCrouchStand());
    }




    private void OnEnable()
    {

        InputManager.Instance.LookEvent += SetLookInput;
        InputManager.Instance.MoveEvent += SetMoveInput;
        InputManager.Instance.JumpEvent += HandleJump;
        InputManager.Instance.CrouchEvent += HandleCrouchInput;

    }



    private void OnDisable()
    {
        InputManager.Instance.LookEvent -= SetLookInput;
        InputManager.Instance.MoveEvent -= SetMoveInput;
        InputManager.Instance.JumpEvent -= HandleJump;
        InputManager.Instance.CrouchEvent -= HandleCrouchInput;

    }


#endregion

}
