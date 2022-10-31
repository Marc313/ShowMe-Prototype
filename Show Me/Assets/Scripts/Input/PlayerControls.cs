using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public enum PlayerType { Player1, Player2 }

[CreateAssetMenu(menuName = "Input/Player Controls")]
public class PlayerControls : ScriptableObject, IInputHandler
{
    public PlayerType type;
    private Controls controls;

    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }

    public int HorizontalRaw
    {
        get
        {
            if (Horizontal > .01f) return 1;
            else if (Horizontal < -0.01f) return -1;
            return 0;
        }
    }
    public event Action RowingPressed;
    public event Action InteractPressed;

    public void Awake()
    {
        controls = new Controls();

        if (type == PlayerType.Player1)
        {
            controls.Player1.Movement.performed += context => OnMove(context);
            controls.Player1.Movement.canceled += context => OnMoveStop();
            controls.Player1.InteractButton.performed += context => InteractPressed?.Invoke();
            controls.Player1.RowingButton.performed += context => RowingPressed?.Invoke();
        }
        else if (type == PlayerType.Player2)
        {
            controls.Player2.Movement.performed += context => OnMove(context);
            controls.Player2.Movement.canceled += context => OnMoveStop();
            controls.Player2.InteractButton.performed += context => InteractPressed?.Invoke();
            controls.Player2.RowingButton.performed += context => RowingPressed?.Invoke();
        }
    }

  

    private void OnEnable()
    {
        if (controls == null) Awake();
        controls.Enable();

        RowingPressed = null;
        InteractPressed = null;
    }

    private void OnDisable()
    {
        if (controls == null) Awake();
        controls.Disable();
    }

    public void OnMove(CallbackContext context)
    {
        Vector2 moveDirection = context.ReadValue<Vector2>();
        Horizontal = moveDirection.x;
        Vertical = moveDirection.y;
        /*if (Horizontal > 0 && context.action.WasPerformedThisFrame())
        {
            HorizontalPressed?.Invoke();
        }*/
    }

    private void OnMoveStop()
    {
        Horizontal = 0;
        Vertical = 0;
    }

    /*public int GetVertical()
    {
        int vertical = 0;
        if (Input.GetKey(upKey)) vertical++;
        if (Input.GetKey(downKey)) vertical--;
        return vertical;
    }

    public int GetHorizontal()
    {
        int horizontal = 0;
        if (Input.GetKey(rightKey)) horizontal++;
        if (Input.GetKey(leftKey)) horizontal--;
        return horizontal;
    }

    public int GetVerticalPressed()
    {
        int vertical = 0;
        if (Input.GetKeyDown(upKey)) vertical++;
        if (Input.GetKeyDown(downKey)) vertical--;
        return vertical;
    }

    public int GetHorizontalPressed()
    {
        int horizontal = 0;
        if (Input.GetKeyDown(rightKey)) horizontal++;
        if (Input.GetKeyDown(leftKey)) horizontal--;
        return horizontal;
    }*/

    /*public bool InteractKeyPressed()
    {
        return Interact.IsPressed();
    }*/

    /*public bool RowKeyPressed()
    {
        return Input.GetKeyDown(rowingKey);
    }*/
}

