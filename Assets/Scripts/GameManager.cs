using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class GameManager : MonoBehaviour
{
    public InputActionManager action;
    public VignetteModifier vm;
    public PassthroughManager pm;
    public GameObject pmObject;
    public GameObject vmObject;
    [SerializeField] int mode;
    [SerializeField] private Canvas questionnaire;
    [SerializeField] private Canvas playerUI;
    [SerializeField] private Button button;
    [SerializeField] private Slider slider;
    [SerializeField] private float[] sections;
    [SerializeField] private ActionBasedContinuousMoveProvider moveProvider;
    public int currentSection;
    public int compassSection;


    private Vector3 pmPosInitial;
    private Vector3 vmPosInitial;

    void Start()
    {
        DisableMovement();

        pmPosInitial = pmObject.transform.localPosition;
        vmPosInitial = vmObject.transform.localPosition;

        currentSection = 0;
        compassSection = 0;

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

    public void DisableMovement()
    {
        moveProvider.enabled = false;
    }

    public void EnableMovement()
    {
        moveProvider.enabled = true;
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

    public void ButtonPress()
    {
        sections[currentSection] = slider.value;
        questionnaire.transform.position = new Vector3(0, 0, 0);
        EnableMovement();
        currentSection++;
    }

    public void EnableSlider()
    {
        DisableMovement();
        questionnaire.transform.position = playerUI.transform.position;
        questionnaire.transform.rotation = playerUI.transform.rotation;
    }
}
