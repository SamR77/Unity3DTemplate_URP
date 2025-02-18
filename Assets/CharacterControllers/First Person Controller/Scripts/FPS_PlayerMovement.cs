using System;
using UnityEngine;



[RequireComponent(typeof(Rigidbody))]
public class FPS_PlayerMovement : MonoBehaviour
{
    // https://www.youtube.com/watch?v=LqnPeqoJRFY&list=PLRiqz5jhNfSo-Fjsx3vv2kvYbxUDMBZ0u


    [Header("Movemewnt")]
    public float moveSpeed = 10f;
    float movementMultiplier = 10f;

    float rbDrag = 6f;    

    float verticalMovement;
    float horizontalMovement;

    Vector3 moveDirection;

    Rigidbody rb;


    private void OnEnable()
    {
        InputManager.Instance.MoveEvent += HandleMoveInput;      
    }


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        HandleDrag();        
    }

    private void HandleDrag()
    {
        rb.drag = rbDrag;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);

        //rb.AddForce(new Vector3(moveDirection.x, 0, moveDirection.y) * moveSpeed, ForceMode.Force);
    }

    private void HandleMoveInput(Vector2 moveInput)
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
        InputManager.Instance.MoveEvent -= HandleMoveInput;

    }





}