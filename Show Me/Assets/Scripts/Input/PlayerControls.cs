using UnityEngine;

[CreateAssetMenu(menuName = "Input/Player Controls")]
public class PlayerControls : ScriptableObject, IInputHandler
{
    [Header("Directions")]
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode rightKey;
    public KeyCode leftKey;

    [Header("Actions")]
    public KeyCode interactKey = KeyCode.E;
    public KeyCode rowingKey = KeyCode.Q;

    public int GetVertical()
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

    public bool InteractKeyPressed()
    {
        return Input.GetKeyDown(interactKey);
    }

    public bool RowKeyPressed()
    {
        return Input.GetKeyDown(rowingKey);
    }
}

