using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class FPS_PlayerCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    


    [SerializeField] private float inputSensitivityX;
    [SerializeField] private float inputSensitivityY;

    [SerializeField] private Transform playerTransform;
    [SerializeField] private Camera playerCamera;

 


    float inputX;
    float inputY;

    float multiplier = 0.01f;

    float xRotation;
    float yRotation;

    //[SerializeField] private float maxLookAngle = 89f;
    //[SerializeField] private bool invertY = false;

    private float verticalRotation;
    private Vector2 lookInput;

    private void OnEnable()
    {
        InputManager.Instance.LookEvent += HandleLookInput;
    }

    private void Start()
    {
        // these are temporary, this should be handled within each State.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        
        playerCamera = GetComponentInChildren<Camera>();
        playerTransform = GetComponent<Transform>();

        // add check to see if playerCamera was found
        if (playerCamera == null)
        {
            Debug.LogError("Player Camera not found");
        }


    }

    private void HandleLookInput(Vector2 lookInput)
    {


        inputX = lookInput.x;
        inputY = lookInput.y;




        // The X & Y rotations use different axes for inputs because:  
        // yRotation (hoprizontal) uses inputX for horizontal look (left/right).  
        // xRotation (vertical) uses inputY for vertical look (up/down).  
        yRotation += inputX * inputSensitivityX * multiplier;
        xRotation -= inputY * inputSensitivityY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -75f, 75f);



    }

    private void Update()
    {
        look();

    }

    private void look()
    {
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        playerTransform.rotation = Quaternion.Euler(0, yRotation, 0);
    }





    private void OnDisable()
    {
        InputManager.Instance.LookEvent -= HandleLookInput;
    }


}
