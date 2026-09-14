using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VignetteModifier : MonoBehaviour
{
    public Material vignetteMat;
    public ActionBasedContinuousMoveProvider moveProvider;
    public CharacterController controller;
    private Vector3 currentPos;
    private Vector3 lastPos;
    [SerializeField] private MeshRenderer mesh;
    public GameManager gm;
    private enum intensityLevel {Low, Medium, High, Max}
    [SerializeField] private float timeLow;
    [SerializeField] private float timeMedium;
    [SerializeField] private float timeHigh;
    [SerializeField] private float timeMax;
    [SerializeField] private float combinedInputSum;
    public float combinedInputDuration;
    [SerializeField] private float averageStrengthPercent;


    public float vignetteSmoothSpeed;
    public float maxVignetteSize;
    public float minVignetteSize;
    public float maxVelocity;
    public float combinedInput;
    public float oneStickMax;

    private float currentVignetteSize;

    void Start()
    {
        currentPos = controller.transform.position;
        lastPos = controller.transform.position;
    }

    void Update()
    {
        if(gm.coasterMode && gm.mode == 1)
        {
            Vector2 left = moveProvider.leftHandMoveAction.action.ReadValue<Vector2>();
            Vector2 right = moveProvider.rightHandMoveAction.action.ReadValue<Vector2>();

            float leftStrength  = Mathf.Min(left.magnitude, oneStickMax);
            float rightStrength = Mathf.Min(right.magnitude, oneStickMax);

            combinedInput = Mathf.Clamp01(leftStrength + rightStrength);
            float visualInput = 1f - Mathf.Pow(1f - combinedInput, 2f);

            moveProvider.moveSpeed = maxVelocity * combinedInput;

            float targetVignetteSize = Mathf.Lerp(maxVignetteSize, minVignetteSize, visualInput);

            currentVignetteSize = Mathf.Lerp(currentVignetteSize, targetVignetteSize, Time.deltaTime * vignetteSmoothSpeed);

            vignetteMat.SetFloat("_ApertureSize", currentVignetteSize);   
            
            switch(GetLevel(combinedInput))
            {
                case intensityLevel.Low:
                    timeLow += Time.deltaTime;
                    break;
                case intensityLevel.Medium:
                    timeMedium += Time.deltaTime;
                    break;
                case intensityLevel.High:
                    timeHigh += Time.deltaTime;
                    break;
                case intensityLevel.Max:
                    timeMax += Time.deltaTime;
                    break;
            }

            combinedInputSum += combinedInput * Time.deltaTime;

            averageStrengthPercent = GetAverageStrengthPercent();      
        }
        else if(!gm.coasterMode)
        {
            currentPos = controller.transform.position;
            var velocity = (currentPos - lastPos) / Time.deltaTime;
            lastPos = currentPos;

            float velocityMag = velocity.magnitude;
            float t = Mathf.Clamp01(velocityMag / maxVelocity);
            float visualInput = 1f - Mathf.Pow(1f - t, 2f);

            float targetVignetteSize = Mathf.Lerp(maxVignetteSize, minVignetteSize, visualInput);

            currentVignetteSize = Mathf.Lerp(currentVignetteSize, targetVignetteSize, Time.deltaTime * vignetteSmoothSpeed);

            vignetteMat.SetFloat("_ApertureSize", currentVignetteSize);            
        }
    }

    intensityLevel GetLevel(float combinedInput)
    {
        if(combinedInput <= 0.25f)
        {
            return intensityLevel.Low;
        }
        else if(combinedInput <= 0.5f)
        {
            return intensityLevel.Medium;
        }
        else if(combinedInput <= 0.75f)
        {
            return intensityLevel.High;
        }
        else
        {
            return intensityLevel.Max;
        }   
    }

    public float GetTimeLow()
    {
        return timeLow;
    }

    public float GetTimeHigh()
    {
        return timeHigh;
    }
    public float GetTimeMedium()
    {
        return timeMedium;
    }
    public float GetTimeMax()
    {
        return timeMax;
    }

    public float GetDuration()
    {
        combinedInputDuration = timeLow + timeMedium + timeHigh + timeMax;
        return combinedInputDuration;
    }

    public float GetAverageStrengthPercent()
    {
        float averageInput = combinedInputSum / GetDuration();

        float percent = averageInput * 100f;

        return percent;
    }
}
