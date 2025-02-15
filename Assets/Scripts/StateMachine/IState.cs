// Sam Robichaud 
// NSCC Truro 2025
// This work is licensed under CC BY-NC-SA 4.0 (https://creativecommons.org/licenses/by-nc-sa/4.0/)

public interface IState
{
    void EnterState(StateManager stateManager);

    // Fixed update is called before update, and is used for physics calculations
    void FixedUpdate(StateManager stateManager);


    void Update(StateManager stateManager);

    // LateUpdate for any updates that need to happen after regular Update
    void LateUpdate(StateManager stateManager);
    void ExitState(StateManager stateManager);
}
