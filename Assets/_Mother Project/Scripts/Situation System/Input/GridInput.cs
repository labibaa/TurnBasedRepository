using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using Starter;
//using UnityEngine.Rendering.Universal.Internal;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.Windows;

public class GridInput : MonoBehaviour
{
    StarterInput starterInput;
    public delegate void GridMoveEventHandler(Direction moveDirection);

    public static event GridMoveEventHandler GridMove;

    public static event Action GridSelect;

    public static event Action GridReset;

    public static event Action GridEscape;
    public static event Action GridMoveUndo;

    private void OnEnable() //Initializes input system and subscribes to input events.
    {
        starterInput = new StarterInput();

        starterInput.Grid.Enable();

        starterInput.Grid.Forward.performed += Forward;
        starterInput.Grid.Backward.performed += Backward;
        starterInput.Grid.Right.performed += Right;
        starterInput.Grid.Left.performed += Left;
        starterInput.Grid.Enter.performed += Select;
        starterInput.Grid.GridReset.performed += GridRes;
        starterInput.Grid.ExitGrid.performed += GridExit;
        starterInput.Grid.UndoMove.performed += GridUndo;
    }

    private void GridUndo(InputAction.CallbackContext obj) // Triggers the GridMoveUndo event when the undo move input is performed.
    {
        GridMoveUndo?.Invoke();
    }

    private void OnDisable()
    {
        starterInput.Grid.Forward.performed -= Forward;
        starterInput.Grid.Backward.performed -= Backward;
        starterInput.Grid.Right.performed -= Right;
        starterInput.Grid.Left.performed -= Left;
        starterInput.Grid.Enter.performed -= Select;
        starterInput.Grid.GridReset.performed -= GridRes;
        starterInput.Grid.ExitGrid.performed -= GridExit;
        starterInput.Grid.UndoMove.performed -= GridUndo;
    }



    private void GridExit(InputAction.CallbackContext obj)
    {
      //  GridEscape?.Invoke();
    }


    private void GridRes(InputAction.CallbackContext obj)  //Triggers the GridReset event when the reset input is performed.
    {
        GridReset?.Invoke();
    }
    private void Select(InputAction.CallbackContext obj)
    {
        GridSelect?.Invoke();
    }

    private void Left(InputAction.CallbackContext obj)
    {
        GridMove?.Invoke(Direction.left);
    }

    private void Right(InputAction.CallbackContext obj)
    {
        GridMove?.Invoke(Direction.right);
    }

    private void Backward(InputAction.CallbackContext obj)
    {
        GridMove?.Invoke(Direction.down);
    }

    private void Forward(InputAction.CallbackContext obj)
    {
        GridMove?.Invoke(Direction.up);
    }
}
