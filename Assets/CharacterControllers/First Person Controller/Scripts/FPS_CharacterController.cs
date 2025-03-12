using UnityEngine;


// Sam Robichaud 
// NSCC Truro 2025
// This work is licensed under CC BY-SA 4.0 (https://creativecommons.org/licenses/by-sa/4.0/)

// Potential fix to the character controller not sliding down slopes. https://discussions.unity.com/t/character-controller-slide-down-slope/188130
// Alernate Ground Check in scipt "REF_FPS_PlayerMovement1"

[RequireComponent(typeof(CharacterController))]
public class FPS_PlayerController : MonoBehaviour
{
    private enum PlayerState { Idle, Walking, Sprinting, Jumping, Falling, Crouching }

    private CharacterController characterController;
    public string DebugCurrentState;  // used to display the current state in the inspector.

    [Header("Move Settings")]
    [SerializeField] private float walkingSpeed = 4;
    [SerializeField] private float walkingStrafeSpeed = 3;
    [SerializeField] private float walkingBackwardsSpeed = 2;




    [Header("Look Settings")]
    [SerializeField] private float horizontalLookSensitivity = 15;
    [SerializeField] private float verticalLookSensitivity = 15;

    [SerializeField] private float minLookAngle = -60;
    [SerializeField] private float maxLookAngle = 60;

    [SerializeField] private bool invertLookY = false;

    private Vector3 newCameraRotation;
    private Vector3 newCharacterRotation;



    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 10.0f;
    [SerializeField] private float jumpHeight = 2.0f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float gravityScale = 5.0f;
    public float verticalVelocity;
    private float originalStepOffset;





    // Input variables
    private Vector2 lookInput;
    private Vector2 moveInput;



    [Header("References")]
    [SerializeField] private Transform cameraHolder;


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        newCameraRotation = cameraHolder.localRotation.eulerAngles;
        newCharacterRotation = transform.localRotation.eulerAngles;

        originalStepOffset = characterController.stepOffset;
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
        ApplyGravity();
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

    private void HandleMovement()
    {
        // Calculate movement speed based on input
        var forwardSpeed = walkingSpeed * moveInput.y ;
        var strafeSpeed = walkingStrafeSpeed * moveInput.x ;

        // Set the newMovementSpeed
        var newMovementSpeed = new Vector3(strafeSpeed, verticalVelocity, forwardSpeed);

        // Orients the character to move relative to the characters forward facing direction
        newMovementSpeed = this.transform.TransformDirection(newMovementSpeed);





        // apply movement to character controller
        characterController.Move(newMovementSpeed * Time.deltaTime);



        // Log the actual velocity magnitude (movement speed)
        // Debug.Log(" Character Velocity = " + characterController.velocity.magnitude);

    }


    private void ApplyGravity()
    {
        if (characterController.isGrounded == true)
        {
            characterController.stepOffset = originalStepOffset;
            if (verticalVelocity < 0)
                verticalVelocity = -0.5f; // Small downward force to ensure stable grounding
        }
        else if (characterController.isGrounded == false)
        {            
            characterController.stepOffset = 0;
            verticalVelocity += gravity * Time.deltaTime; // Apply gravity
        }
    }











    #region Input methods


    private void SetLookInput(Vector2 vector)
    {
        lookInput = new Vector2(vector.x, vector.y);
    }

    private void SetMoveInput(Vector2 inputVector)
    {
        moveInput = new Vector2(inputVector.x, inputVector.y);
    }

    private void OnSprintInput(bool sprintInputState)
    {
        //isSprinting = sprintInputState;
    }

    private void OnCrouchInput(bool crouchInputState)
    {
        //isCrouching = crouchInputState;
    }

    private void OnJumpInput()
    {
        if (characterController.isGrounded)
        {
            verticalVelocity = jumpForce;
        }
    }



    private void OnEnable()
    {

        InputManager.Instance.LookEvent += SetLookInput;        
        InputManager.Instance.MoveEvent += SetMoveInput;
        InputManager.Instance.JumpEvent += OnJumpInput;
        InputManager.Instance.SprintEvent += OnSprintInput;
        InputManager.Instance.CrouchEvent += OnCrouchInput;
    }



    private void OnDisable()
    {
        InputManager.Instance.LookEvent -= SetLookInput;
        InputManager.Instance.MoveEvent -= SetMoveInput;
        InputManager.Instance.JumpEvent += OnJumpInput;
        InputManager.Instance.SprintEvent -= OnSprintInput;
        InputManager.Instance.CrouchEvent -= OnCrouchInput;
    }


    #endregion









}