using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState : MonoBehaviour
{
    [field: SerializeField] public CharacterMovementState currentCharacterMovementState { get; private set; }

    public void SetCharacterMovementState (CharacterMovementState characterMovementState)
    {
        currentCharacterMovementState = characterMovementState;
    } 


}

public enum CharacterMovementState
{
    Idling = 0,
    Walking = 1,
    Running = 2,
    Sprinting = 3,
    Jumping = 4,
    Falling = 5,
    Strafing = 6,
}
