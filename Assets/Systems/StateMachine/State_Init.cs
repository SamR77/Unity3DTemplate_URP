using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Sam Robichaud 
// NSCC Truro 2025
// This work is licensed under CC BY-NC-SA 4.0 (https://creativecommons.org/licenses/by-nc-sa/4.0/)

public class State_Init : IState
{
    private UIManager uiManager;



public void EnterState(StateManager stateManager) 
    {
        


        Time.timeScale = 0f;        
        Cursor.visible = false;
        UIManager.Instance.disableAllMenus();


        // call any reset methods in here


        StateManager.Instance.SwitchToState(StateManager.Instance.state_MainMenu);

    }

    public void FixedUpdate(StateManager stateManager) { }

    public void Update(StateManager stateManager) { }

    public void LateUpdate(StateManager stateManager) { }

    public void ExitState(StateManager stateManager) { }

}
