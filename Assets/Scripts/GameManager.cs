using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    public int mode;
    [SerializeField] private Canvas questionnaire;
    [SerializeField] private Canvas playerUI;
    [SerializeField] private Button button;
    [SerializeField] private Slider slider;
    [SerializeField] private float[] sections;
    [SerializeField] private ActionBasedContinuousMoveProvider moveProvider;
    [SerializeField] private ActionBasedContinuousTurnProvider turnProvider;
    public int currentSection;
    public int compassSection;
    public bool coasterMode;
    public bool coasterSection;

    public int participantID;
    public int totalSections;

    private Vector3 pmPosInitial;
    private Vector3 vmPosInitial;
    private string filePath;

    void Start()
    {
        string fileName = $"P{participantID}_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv";
        filePath = Path.Combine(Application.persistentDataPath, fileName);

        totalSections = sections.Length;

        string header = "participantID";
        for (int i = 1; i <= totalSections; i++)
        {
            header += $",section{i}";
        }
        header += ",pt_timeOff,pt_timeLow,pt_timeHigh";
        header += ",vig_timeOff,vig_timeLow,vig_timeHigh\n";

        File.WriteAllText(filePath, header);

        DisableMovement();

        pmPosInitial = pmObject.transform.localPosition;
        vmPosInitial = vmObject.transform.localPosition;

        currentSection = 0;
        compassSection = 0;

        NextCSMethod();
        coasterMode = false;
        coasterSection = false;
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

    public void DisableTurning()
    {
        turnProvider.enabled = false;
    }

    public void EnableTurning()
    {
        turnProvider.enabled = true;
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

    public void EmptyCSMethod()
    {
        Vector3 newPosp = pmObject.transform.localPosition;
        newPosp.z = -2;

        pmObject.transform.localPosition = newPosp;

        Vector3 newPosv = vmObject.transform.localPosition;
        newPosv.z = -2;
        vmObject.transform.localPosition = newPosv;

        mode = 2;
    }

    public void ButtonPress()
    {
        sections[currentSection] = slider.value;
        questionnaire.transform.position = new Vector3(0, 0, 0);
        EnableMovement();
        EnableTurning();
        currentSection++;

        if (currentSection >= sections.Length)
        {
            DisableMovement();
            DisableTurning();
            SaveToCSV();
            Debug.Log("Experiment complete.");
        }
    }

    public void EnableSlider()
    {
        DisableMovement();
        questionnaire.transform.position = playerUI.transform.position;
        questionnaire.transform.rotation = playerUI.transform.rotation;
    }

    public void ToggleCoasterMode()
    {
        if(coasterMode)
        {
            coasterMode = false;
        }
        else if(!coasterMode)
        {
            coasterMode = true;
        }
    }

    public void ToggleCoasterSection()
    {
        if(coasterSection)
        {
            coasterSection = false;
        }
        else if(!coasterSection)
        {
            coasterSection = true;
        }
    }

    public void SaveToCSV()
    {
        string line = participantID.ToString();

        for (int i = 0; i < sections.Length; i++)
        {
            line += $",{sections[i]}";
        }

        line += $",{pm.GetTimeOff():F3}";
        line += $",{pm.GetTimeLow():F3}";
        line += $",{pm.GetTimeHigh():F3}";

        line += $",{vm.GetTimeOff():F3}";
        line += $",{vm.GetTimeLow():F3}";
        line += $",{vm.GetTimeHigh():F3}";

        line += "\n";

        File.AppendAllText(filePath, line);
        Debug.Log($"Data saved to {filePath}");
    }
}
