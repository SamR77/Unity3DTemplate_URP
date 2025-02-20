using System;
using UnityEngine;


// Handles player movement using Rigidbody physics.
// Uses input events from the InputManager.

[RequireComponent(typeof(Rigidbody))]
public class FPS_PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f; // Speed of player movement

    private Rigidbody rb; // Reference to the player's Rigidbody component
    private Vector3 moveDirection; // Stores the movement direction


    // Called when the script is enabled.
    // Subscribes to the movement event in InputManager.
    private void OnEnable()
    {        
            InputManager.Instance.MoveEvent += ReferenceMoveInputs;   
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevents rotation due to physics interactions
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    // Stores the player's movement input.
    private void ReferenceMoveInputs(Vector2 inputVector)
    {
        moveDirection = transform.right * inputVector.x + transform.forward * inputVector.y;
    }


    // Applies movement to the player's Rigidbody.
    private void MovePlayer()
    {
        Vector3 velocity = moveDirection * moveSpeed;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z); // Preserve vertical velocity (gravity)
    }


    // Called when the script is disabled.
    // Unsubscribes from the movement event to prevent memory leaks.
    private void OnDisable()
    {
        InputManager.Instance.MoveEvent -= ReferenceMoveInputs;
    }

}