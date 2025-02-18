using UnityEngine;

public enum PlayerState
{
    Walking,
    Sprinting,
    Crouching,
    Sliding,
    Jumping,
    Falling
}

[RequireComponent(typeof(Rigidbody))]
public class REF_FPS_PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform playerModel;
    [SerializeField] private Transform groundCheck;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float crouchSpeed = 3f;
    [SerializeField] private float jumpForce = 5f;

    [Header("Movement Control")]
    [SerializeField] private float groundAcceleration = 15f;
    [SerializeField] private float airAcceleration = 5f;
    [SerializeField] private float groundDeceleration = 25f;
    [SerializeField] private float airDeceleration = 2f;
    [SerializeField] private float airControl = 0.3f;
    [SerializeField] private float maxSlopeAngle = 45f;

    [Header("Slide Settings")]
    [SerializeField] private float slideSpeedMultiplier = 2f;
    [SerializeField] private float slideFriction = 2f;
    [SerializeField] private float minSlideSpeed = 2f;

    [Header("Jump Settings")]
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    private PlayerState currentState;
    private bool isGrounded;
    private bool wasGrounded;
    private bool isSprinting;
    private bool isCrouching;
    private bool isJumping;
    private float lastGroundedTime;
    private float lastJumpTime;
    private Vector3 lastGroundNormal;
    private float currentSpeed;
    private float targetSpeed;

    private void OnEnable()
    {
        InputManager.Instance.MoveEvent += OnMove;
        InputManager.Instance.JumpEvent += OnJump;
        InputManager.Instance.SprintEvent += OnSprint;
        InputManager.Instance.CrouchEvent += OnCrouch;
    }

    private void OnDisable()
    {
        InputManager.Instance.MoveEvent -= OnMove;
        InputManager.Instance.JumpEvent -= OnJump;
        InputManager.Instance.SprintEvent -= OnSprint;
        InputManager.Instance.CrouchEvent -= OnCrouch;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void FixedUpdate()
    {
        CheckGround();
        UpdateState();
        HandleMovement();
        ApplyGravity();
    }

    private void CheckGround()
    {
        wasGrounded = isGrounded;

        isGrounded = Physics.SphereCast(
            groundCheck.position,
            0.4f,
            Vector3.down,
            out RaycastHit hitInfo,
            0.2f,
            groundLayer
        );

        if (isGrounded)
        {
            lastGroundNormal = hitInfo.normal;
            lastGroundedTime = Time.time;

            if (!wasGrounded)
            {
                float impactVelocity = -rb.velocity.y;
                if (impactVelocity > 5f)
                {
                    // Handle landing impact (camera shake, sound, etc.)
                }
            }
        }
    }

    private void UpdateState()
    {
        PlayerState newState = DetermineState();

        if (newState != currentState)
        {
            currentState = newState;
        }
    }

    private PlayerState DetermineState()
    {
        if (!isGrounded && rb.velocity.y < -0.1f)
            return PlayerState.Falling;
        if (!isGrounded && rb.velocity.y > 0.1f)
            return PlayerState.Jumping;
        if (currentState == PlayerState.Sliding && rb.velocity.magnitude > minSlideSpeed)
            return PlayerState.Sliding;

        if (isCrouching)
            return PlayerState.Crouching;

        return moveInput.magnitude > 0.1f ?
            (isSprinting ? PlayerState.Sprinting : PlayerState.Walking) :
            PlayerState.Walking;
    }

    private void HandleMovement()
    {
        // Get move direction relative to camera
        Camera mainCamera = Camera.main;
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;

        // Calculate target speed based on state
        targetSpeed = currentState switch
        {
            PlayerState.Sprinting => sprintSpeed,
            PlayerState.Walking => walkSpeed,
            PlayerState.Crouching => crouchSpeed,
            PlayerState.Sliding => rb.velocity.magnitude,
            _ => walkSpeed
        };

        float acceleration = isGrounded ? groundAcceleration : airAcceleration;
        float deceleration = isGrounded ? groundDeceleration : airDeceleration;

        if (moveInput.magnitude > 0.1f)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, deceleration * Time.fixedDeltaTime);
        }

        Vector3 targetVelocity = moveDirection * currentSpeed;
        targetVelocity.y = rb.velocity.y;

        if (isGrounded)
        {
            targetVelocity = Vector3.ProjectOnPlane(targetVelocity, lastGroundNormal);
        }

        if (!isGrounded)
        {
            targetVelocity = Vector3.Lerp(rb.velocity, targetVelocity, airControl);
        }

        rb.velocity = targetVelocity;
    }

    private void ApplyGravity()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && !isJumping)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    #region Input Handlers
    private void OnMove(Vector2 input)
    {
        moveInput = input;
    }

    private void OnJump()
    {
        if (!isGrounded) return;

        isJumping = true;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        lastJumpTime = Time.time;

        Invoke(nameof(ResetJump), 0.1f);
    }

    private void ResetJump()
    {
        isJumping = false;
    }

    private void OnSprint(bool sprintState)
    {
        isSprinting = sprintState;

        if (sprintState && moveInput.magnitude > 0.1f && currentState == PlayerState.Crouching)
        {
            StartSlide();
        }
    }

    private void OnCrouch(bool crouchState)
    {
        isCrouching = crouchState;
    }
    #endregion

    private void StartSlide()
    {
        if (rb.velocity.magnitude < minSlideSpeed) return;

        currentState = PlayerState.Sliding;
        currentSpeed *= slideSpeedMultiplier;
    }
}