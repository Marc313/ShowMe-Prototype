using UnityEngine;

[CreateAssetMenu(menuName = "Input/Direction Controls")]
public class DirectionControls : ScriptableObject, IInputHandler
{
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode rightKey;
    public KeyCode leftKey;

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
}
