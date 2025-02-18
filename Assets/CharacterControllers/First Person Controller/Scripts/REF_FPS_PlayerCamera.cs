using UnityEngine;

public class REF_FPS_PlayerCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Transform playerBody;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 89f;
    [SerializeField] private bool invertY = false;

    private float verticalRotation;
    private Vector2 lookInput;

    private void OnEnable()
    {
        InputManager.Instance.LookEvent += OnLook;
    }

    private void OnDisable()
    {
        InputManager.Instance.LookEvent -= OnLook;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        playerBody.Rotate(Vector3.up * lookInput.x * mouseSensitivity);

        float lookY = invertY ? lookInput.y : -lookInput.y;
        verticalRotation = Mathf.Clamp(verticalRotation + lookY * mouseSensitivity, -maxLookAngle, maxLookAngle);
        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    private void OnLook(Vector2 input)
    {
        lookInput = input;
    }
}
