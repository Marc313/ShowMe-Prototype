using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
{
    Controls controls;

    private void Awake()
    {
        /*up.performed += context => Debug.Log("Up");
        left.performed += context => Debug.Log("L");
        right.performed += context => Debug.Log("R");*/
        controls = new Controls();
        //controls.Player1.Test.performed += context => Debug.Log("Yayy");
    }



    private void OnEnable()
    {
        /*up.Enable();
        left.Enable();
        right.Enable();
        down.Enable();*/
        controls.Enable();
    }

    private void OnDisable()
    {
        /*up.Disable();
        left.Disable();
        right.Disable();
        down.Disable();*/
        controls.Disable();
    }


}
