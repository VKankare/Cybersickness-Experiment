using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class MovementSpeedModifier : MonoBehaviour
{
    public ActionBasedContinuousMoveProvider moveProvider;
    public ActionBasedContinuousTurnProvider turnProvider;
    public float maxSpeed;  
    [SerializeField] private float combinedSpeedSum;
    [SerializeField] private float combinedSpeedDuration;
    [SerializeField] private float[] sectionAvgSpeeds;
    [SerializeField] private float[] sectionTimes;
    [SerializeField] private int currentSection;
    public GameManager gm;
    private Vector3 currentPos;
    private Vector3 lastPos;
    public float turnAngle;

    void Start()
    {
        currentSection = 0;
        currentPos = moveProvider.transform.position;
        lastPos = moveProvider.transform.position;
    }

    void Update()
    {
        Vector2 left = moveProvider.leftHandMoveAction.action.ReadValue<Vector2>();
        Vector2 right = moveProvider.rightHandMoveAction.action.ReadValue<Vector2>();

        float leftAngle = Mathf.Atan2(left.y, left.x) * Mathf.Rad2Deg;
        float rightAngle = Mathf.Atan2(right.y, right.x) * Mathf.Rad2Deg;

        if(leftAngle > -turnAngle && leftAngle < turnAngle)
        {
            left = Vector2.zero;
        }
        else if(leftAngle > 180 - turnAngle || leftAngle < -180 + turnAngle)
        {
            left = Vector2.zero;
        }

        if(rightAngle > -turnAngle && rightAngle < turnAngle)
        {
            right = Vector2.zero;
        }
        else if(rightAngle > 180 - turnAngle || rightAngle < -180 + turnAngle)
        {
            right = Vector2.zero;
        }

        float combinedInput = left.magnitude + right.magnitude;

        float newSpeed = maxSpeed * combinedInput;

        moveProvider.moveSpeed = newSpeed;

        currentPos = moveProvider.transform.position;
        var velocity = (currentPos - lastPos) / Time.deltaTime;
        lastPos = currentPos;

        float velocityMag = velocity.magnitude;

        if(gm.timerRunning && (gm.compassSection == 0 || gm.compassSection == 2))
        {
            combinedSpeedSum += velocityMag * Time.deltaTime;
            combinedSpeedDuration += Time.deltaTime;
        }
    }    

    public void InputSectionData()
    {
        sectionTimes[currentSection] = combinedSpeedDuration;
        sectionAvgSpeeds[currentSection] = GetAverageSpeed();
        currentSection++;
        ResetData();
    }

    public void ResetData()
    {
        combinedSpeedSum = 0f;
        combinedSpeedDuration = 0f;
    }

    public float GetAverageSpeed()
    {
        float averageSpeed = combinedSpeedSum / combinedSpeedDuration;

        return averageSpeed;
    }

    public float[] GetSectionTimes()
    {
        return sectionTimes;
    }

    public float[] GetSectionAvgSpeeds()
    {
        return sectionAvgSpeeds;
    }
}
