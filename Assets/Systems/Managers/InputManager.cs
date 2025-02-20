using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// The InputManager handles all player input and distributes it via events.
// It follows the Singleton pattern to ensure only one instance exists at all times.

public class InputManager : MonoBehaviour, Inputs.IPlayerActions
{
    #region Singleton
    private static InputManager instance;

  
    // Provides global access to the InputManager instance.
    // If no instance exists, it creates one automatically.
  
    public static InputManager Instance
    {
        get
        {
            if (instance == null)
            {
                // Attempt to find an existing InputManager in the scene
                instance = FindObjectOfType<InputManager>();

                // If no instance exists, create a new GameObject with an InputManager component
                if (instance == null)
                {
                    GameObject obj = new GameObject("InputManager");
                    instance = obj.AddComponent<InputManager>();
                    DontDestroyOnLoad(obj);
                }
            }
            return instance;
        }
    }
    #endregion

    // Reference to the generated Input System class
    private Inputs inputs;



    void Awake()
    {
        #region Enforce Singleton
        // Enforce Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Prevents destruction between scene loads
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Prevents multiple instances
            return;
        }
        #endregion

        // Initialize the Input System
        inputs = new Inputs();
        inputs.Player.Enable(); // Enables the "Player" action map

        // Set this script to handle input callbacks
        inputs.Player.SetCallbacks(this);
    }

    #region Input Events
    // Events triggered when player inputs are detected
    public event Action<Vector2> MoveEvent;
    public event Action<Vector2> LookEvent;
    public event Action JumpEvent;
    public event Action<bool> SprintEvent;
    public event Action<bool> CrouchEvent;
    public event Action PauseEvent;
    #endregion





    #region Input Callbacks
    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        LookEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started) JumpEvent?.Invoke();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        SprintEvent?.Invoke(context.started || context.performed);
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        CrouchEvent?.Invoke(context.started || context.performed);
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started) PauseEvent?.Invoke();
    }
    #endregion




    void OnDisable()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    void OnDestroy()
    {
        // If this instance is being destroyed (like exiting play mode), clear the reference
        if (instance == this)
        {
            instance = null;
        }
    }
}




