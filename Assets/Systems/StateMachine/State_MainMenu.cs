// Sam Robichaud 
// NSCC Truro 2025
// This work is licensed under CC BY-NC-SA 4.0 (https://creativecommons.org/licenses/by-nc-sa/4.0/)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_MainMenu : IState
{
    public void EnterState(StateManager stateManager) 
    {
        Time.timeScale = 0f;
        Cursor.visible = true;
        UIManager.Instance.ShowMainMenu();

    }

    public void FixedUpdate(StateManager stateManager) { }

    public void Update(StateManager stateManager) { }

    public void LateUpdate(StateManager stateManager) { }

    public void ExitState(StateManager stateManager) { }

}
