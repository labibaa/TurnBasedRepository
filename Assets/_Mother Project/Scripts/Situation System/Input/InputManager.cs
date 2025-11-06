using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using Starter;
using System;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    StarterInput starterInput;

    

    public static event Action OnInteractionPressed;

    private void OnEnable()
    {
        starterInput = new StarterInput();

        starterInput.Player.Enable();

        starterInput.Player.Interact.performed += Interect;
        starterInput.Player.Interact.Enable();

    }

    private void OnDisable()
    {
        starterInput.Player.Interact.Disable();
    }

    private void Interect(InputAction.CallbackContext obj)  //invoke when grid is enabled and disabled
    {
        OnInteractionPressed?.Invoke();
        //Debug.Log("pressed E");
    }
}
