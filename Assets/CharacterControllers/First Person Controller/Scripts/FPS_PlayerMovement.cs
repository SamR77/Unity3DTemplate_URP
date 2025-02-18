using System;
using UnityEngine;



[RequireComponent(typeof(Rigidbody))]
public class FPS_PlayerMovement : MonoBehaviour
{
    // https://www.youtube.com/watch?v=LqnPeqoJRFY&list=PLRiqz5jhNfSo-Fjsx3vv2kvYbxUDMBZ0u

    float playerHeght = 2f;

    [Header("Move Settings")]
    public float moveSpeed = 10f;
    float movementMultiplier = 10f;
    [SerializeField] private float airMultiplier = 0.4f;

    [Header("Jump Settings")]
    public float jumpForce = 15f;


    float groundDrag = 6f; 
    float airDrag = 2f;

    float verticalMovement;
    float horizontalMovement;

    bool isGrounded;

    Vector3 moveDirection;

    Rigidbody rb;


    private void OnEnable()
    {
        InputManager.Instance.MoveEvent += ReferenceMoveInputs;   
        InputManager.Instance.JumpEvent += HandleJump;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        HandleMove();
    }

    private void Update()
    {
        isGrounded = Physics.Raycast(this.transform.position, Vector3.down, playerHeght / 2 + 0.1f);

        HandleDrag();        
    }


    private void HandleMove()
    {
        if (isGrounded)
        { 
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }
            
        rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);

        //rb.AddForce(new Vector3(moveDirection.x, 0, moveDirection.y) * moveSpeed, ForceMode.Force);
    }


    private void HandleJump()
    {
        if (isGrounded)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }


    private void HandleDrag()
    {
        if (isGrounded)
        { rb.drag = groundDrag; }
        else
        { rb.drag = airDrag; }
    }


    private void ReferenceMoveInputs(Vector2 moveInput)
    {
        verticalMovement = moveInput.y;
        horizontalMovement = moveInput.x;

        //Debug.Log("verticalMovement = :" + verticalMovement);
        //Debug.Log("horizontalMovement = :" + horizontalMovement);

        moveDirection = transform.forward * verticalMovement + transform.right * horizontalMovement;

        //Debug.Log("moveDirection = :" + moveDirection);
    }




    private void OnDisable()
    {
        InputManager.Instance.MoveEvent -= ReferenceMoveInputs;
        InputManager.Instance.JumpEvent -= HandleJump;
    }





}