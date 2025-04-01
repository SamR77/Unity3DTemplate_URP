using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    // https://youtu.be/PIFQbxMgT0c?t=380

    [SerializeField] private Animator animator;
    [SerializeField] private float animationBlendSpeed = 0.02f;

    //private ThirdPersonCharacterController characterController;
    private CharacterState characterState;

    private static int inputXHash = Animator.StringToHash("InputX");
    private static int inputYHash = Animator.StringToHash("InputY");
    private static int InputMagnitudeHash = Animator.StringToHash("InputMagnitude");

    private Vector3 currentBlendedInput = Vector3.zero;

    private Vector2 moveInput;

    private void Awake()
    {
       // characterController = GetComponent<ThirdPersonCharacterController>();
        characterState = GetComponent<CharacterState>();
    
    }


    private void Update()
    {
        UpdateAnimationState();               
    }

    private void UpdateAnimationState()
    {
        bool isSprinting = characterState.currentCharacterMovementState == CharacterMovementState.Sprinting;

        Vector2 InputTarget = isSprinting ? moveInput * 1.5f : moveInput;
        currentBlendedInput = Vector3.Lerp(currentBlendedInput, InputTarget, animationBlendSpeed);

        animator.SetFloat(inputXHash, moveInput.x);
        animator.SetFloat(inputYHash, moveInput.y);
        animator.SetFloat(InputMagnitudeHash, currentBlendedInput.magnitude);
    }


    private void SetMoveInput(Vector2 input)
    {
        moveInput = new Vector2(input.x, input.y);
    }




    private void OnEnable()
    {
        //InputManager.Instance.LookEvent += SetLookInput;
        InputManager.Instance.MoveEvent += SetMoveInput;
        //InputManager.Instance.SprintEvent += SetSprintBool;
        //InputManager.Instance.JumpEvent += HandleJump;
        //InputManager.Instance.CrouchEvent += HandleCrouchInput;
    }



    private void OnDisable()
    {
        //InputManager.Instance.LookEvent -= SetLookInput;
        InputManager.Instance.MoveEvent -= SetMoveInput;
        //InputManager.Instance.SprintEvent -= SetSprintBool;
        //InputManager.Instance.JumpEvent -= HandleJump;
        //InputManager.Instance.CrouchEvent -= HandleCrouchInput;
    }


}
