// Sam Robichaud 
// NSCC Truro 2025
// This work is licensed under CC BY-NC-SA 4.0 (https://creativecommons.org/licenses/by-nc-sa/4.0/)

using UnityEditor;
using UnityEngine;

public class StateManager : MonoBehaviour
{


    [Header("Debug (read only)")]
    [SerializeField] private string lastActiveState;
    [SerializeField] private string currentActiveState;


    // Private variables to store state information
    private IState currentState;  // Current active state
    private IState lastState;     // Last active state (kept private for encapsulation)

    // Public getter for accessing the lastState externally (read-only access)
    public IState LastState
    {
        get { return lastState; }
    }

    // Instantiate game state objects
    public State_Init state_Init = new State_Init();
    public State_MainMenu state_MainMenu = new State_MainMenu();
    public State_Gameplay state_Gameplay = new State_Gameplay();
    public State_Paused state_Paused = new State_Paused();


    private static StateManager instance;

    public static StateManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<StateManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("StateManager");
                }
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }



    void Start()
    {
        // Sets currentState to State_Init when StateManager is initialized / first loaded
        // State_Init is responsible for initializing/resetting the game
        currentState = state_Init;

        // Enter the initial State
        currentState.EnterState(this);
    }

    #region State Machine Update Calls


    // Fixed update is called before update, and is used for physics calculations
    private void FixedUpdate()
    {
        // Handle physics updates in the current active state (if applicable)
        currentState.Update(this);
    }

    private void Update()
    {
        // Handle regular frame updates in the current active state
        currentState.Update(this);

        // Keeping track of active and last states for debugging purposes
        // TODO: I can probably move these out of Update and just set them when switching states ... look into moving down into SwitchToState method
        currentActiveState = currentState.ToString();   // Show current state in Inspector
        lastActiveState = lastState?.ToString();        // Show last state in Inspector
    }

    // LateUpdate for any updates that need to happen after regular Update
    private void LateUpdate()
    {
        currentState.LateUpdate(this);
    }

    #endregion

    // Method to switch between states
    public void SwitchToState(IState newState)
    {
        // Exit the current state (handling cleanup and transitions)
        currentState.ExitState(this);

        // Store the current state as the last state before switching
        lastState = currentState;

        // Switch to the new state
        currentState = newState;

        // Enter the new state (initialize any necessary logic)
        currentState.EnterState(this);
    }



    public void Pause()
    {
        if (currentState != state_Paused)
        {

            SwitchToState(state_Paused);
        }
    }

    // UI Button calls this to resume the game when paused()
    public void Resume()
    {
        if (currentState == state_Paused)
        {
            SwitchToState(state_Gameplay);
        }
    }







    public void LoadGameInit() { SwitchToState(state_Init); }
    // public void GameOver() { SwitchToState(gameState_GameOver); }
    // public void OpenCredits() { SwitchToState(gameState_Credits); }
    // public void OpenOptions() { SwitchToState(gameState_Options); }
    public void LoadMainMenu() { SwitchToState(state_MainMenu); }
    public void GoBack() { SwitchToState(lastState); }
    public void LoadGameplay() { SwitchToState(state_Gameplay); }

    public void Quit() 
    {
        // Close the application
        Application.Quit();


        /*
        // If running in the Unity Editor, stop playing the game
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
        }
        */
    }


    





}

