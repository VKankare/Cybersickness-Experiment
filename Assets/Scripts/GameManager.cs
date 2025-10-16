using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class GameManager : MonoBehaviour
{
    public InputActionManager action;
    public VignetteModifier vm;
    public PassthroughManager pm;
    public GameObject pmObject;
    public GameObject vmObject;
    [SerializeField] int mode;

    private Vector3 pmPosInitial;
    private Vector3 vmPosInitial;

    void Start()
    {
        pmPosInitial = pmObject.transform.localPosition;
        vmPosInitial = vmObject.transform.localPosition;

        mode = 1;

        NextCSMethod();
    }

    public void DisableInput()
    {
        action.DisableInput();
    }

    public void EnableInput()
    {
        action.EnableInput();
    }

    public void NextCSMethod()
    {
        if (mode == 0)
        {
            Vector3 newPos = pmObject.transform.localPosition;
            newPos.z = -2;
            pmObject.transform.localPosition = newPos;

            vmObject.transform.localPosition = vmPosInitial;

            mode = 1;
        }
        else if (mode == 1)
        {
            Vector3 newPos = vmObject.transform.localPosition;
            newPos.z = -2;
            vmObject.transform.localPosition = newPos;

            pmObject.transform.localPosition = pmPosInitial;

            mode = 0;
        }
    }

}
