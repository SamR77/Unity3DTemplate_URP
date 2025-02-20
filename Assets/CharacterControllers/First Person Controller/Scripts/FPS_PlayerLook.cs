using UnityEngine;

public class FPS_PlayerLook : MonoBehaviour
{
    [Header("Look Settings")]
    [SerializeField] private float horizontalSensitivity = 10f;
    [SerializeField] private float verticalSensitivity = 7.5f;
    [SerializeField] private float maxLookUpAngle = 60f;
    [SerializeField] private float maxLookDownAngle = -60f;
    [SerializeField] private Transform playerCamera;

    private float verticalRotation = 0f;
    private Vector2 lookDirection;

    private float multiplier = 0.01f;

    private void OnEnable()
    {
        InputManager.Instance.LookEvent += ReferenceLookInputs;
        
        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void ReferenceLookInputs(Vector2 lookInput)
    {
        lookDirection = lookInput * multiplier;
    }

    private void Update()
    {
        // Handle horizontal rotation (player body)
        float horizontalRotation = lookDirection.x * horizontalSensitivity;
        transform.Rotate(Vector3.up * horizontalRotation);

        // Handle vertical rotation (camera only)
        verticalRotation -= lookDirection.y * verticalSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, maxLookDownAngle, maxLookUpAngle);
        playerCamera.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }


    private void OnDisable()
    {
        InputManager.Instance.LookEvent -= ReferenceLookInputs;
        
        // Unlock and show cursor when disabled
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}