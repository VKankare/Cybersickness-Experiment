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
    public Transform previousPoint;
    [SerializeField] private float[] sectionTimes;
    [SerializeField] private float[] sectionAvgSpeeds;
    [SerializeField] private float timer;
    [SerializeField] private float averageFPS;
    [SerializeField] private ActionBasedContinuousMoveProvider moveProvider;
    [SerializeField] private ActionBasedContinuousTurnProvider turnProvider;
    public int currentSection;
    public int compassSection;
    public bool coasterMode;
    public bool coasterSection;
    public bool timerRunning;
    public MovementSpeedModifier msm;
    public XRInteractorLineVisual lrLeft;
    public XRInteractorLineVisual lrRight;
    public bool allowTeleport;

    public int participantID;
    public int totalSections;

    private Vector3 pmPosInitial;
    private Vector3 vmPosInitial;
    private string filePath;
    private int startMode;
    private string header;
    private float totalFPS;
    private float frameCount;

    void Start()
    {
        startMode = mode;

        if(mode == 2)
        {
            SetControlGroup();
        }

        string fileName = $"Experiment_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv";
        filePath = Path.Combine(Application.persistentDataPath, fileName);

        totalSections = sections.Length;

        header = "participantID";
        header += ",group";
        for (int i = 1; i <= totalSections; i++)
        {
            header += $",section{i}";
        }
        header += ",time1,avgSpeed1,time2,avgSpeed2,time3,avgSpeed3,time4,avgSpeed4";
        header += ",pt_timeLow,pt_timeMedium,pt_timeHigh,pt_timeMax,pt_avgStrengthPercent";
        header += ",vig_timeLow,vig_timeMedium,vig_timeHigh,vig_timeMax,vig_avgStrengthPercent";
        header += ",total_time";
        header += ",average_FPS\n";

        DisableMovement();

        pmPosInitial = pmObject.transform.localPosition;
        vmPosInitial = vmObject.transform.localPosition;

        currentSection = 0;
        compassSection = 0;

        NextCSMethod();
        coasterMode = false;
        coasterSection = false;
    }

    void Update()
    {
        if(timerRunning)
        {
            timer += Time.deltaTime;
        }

        totalFPS += 1f / Time.deltaTime;
        frameCount++;
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

    public void SetControlGroup()
    {
        Vector3 newPosp = pmObject.transform.localPosition;
        newPosp.z = -2;
        pmObject.transform.localPosition = newPosp;

        Vector3 newPosv = vmObject.transform.localPosition;
        newPosv.z = -2;
        vmObject.transform.localPosition = newPosv;
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
        else if (mode == 2)
        {
            Debug.Log("Control group");
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
        if(currentSection == 1 || currentSection == 2 || currentSection == 5 || currentSection == 6)
        {
            msm.InputSectionData();
        }
        questionnaire.transform.position = new Vector3(0, 0, 0);
        EnableMovement();
        EnableTurning();
        currentSection++;
        StartTimer();
        ToggleLines();
        SaveToCSV();
        if (currentSection >= sections.Length)
        {
            DisableMovement();
            DisableTurning();
            StopTimer();
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
            allowTeleport = true;
        }
        else if(!coasterMode)
        {
            coasterMode = true;
            allowTeleport = false;
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

    public void StartTimer()
    {
        timerRunning = true;
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    public void ToggleLines()
    {
        if(lrLeft.enabled || lrRight.enabled)
        {
            lrLeft.enabled = false;
            lrRight.enabled = false;
        }
        else
        {
            lrLeft.enabled = true;
            lrRight.enabled = true;
        }
    }

    public float GetAverageFPS()
    {
        return  totalFPS / frameCount;
    }

    void OnApplicationQuit()
    {
        SaveToCSV();
    }

    public void SaveToCSV()
    {
        sectionTimes = msm.GetSectionTimes();
        sectionAvgSpeeds = msm.GetSectionAvgSpeeds();
        averageFPS = GetAverageFPS();

        string line = participantID.ToString();

        line += $",{startMode}";

        for (int i = 0; i < sections.Length; i++)
        {
            line += $",{sections[i]}";
        }

        for (int i = 0; i < sectionTimes.Length; i++)
        {
            line += $",{sectionTimes[i]:F3}";
            line += $",{sectionAvgSpeeds[i]:F3}";
        }

        line += $",{pm.GetTimeLow():F3}";
        line += $",{pm.GetTimeMedium():F3}";
        line += $",{pm.GetTimeHigh():F3}";
        line += $",{pm.GetTimeMax():F3}";
        line += $",{pm.GetAverageStrengthPercent():F3}";

        line += $",{vm.GetTimeLow():F3}";
        line += $",{vm.GetTimeMedium():F3}";
        line += $",{vm.GetTimeHigh():F3}";
        line += $",{vm.GetTimeMax():F3}";
        line += $",{vm.GetAverageStrengthPercent():F3}";

        line += $",{timer:F3}";
        line += $",{averageFPS:F3}\n";

        string csv = header + line;

        File.WriteAllText(filePath, csv);
        Debug.Log($"Data saved to {filePath}");
    }
}
