using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PassthroughManager : MonoBehaviour
{
    public ActionBasedContinuousMoveProvider moveProvider;
    public GameObject passthroughPlane;
    public CharacterController controller;
    private Vector3 currentPos;
    private Vector3 lastPos;
    public float maxScaleX;
    public float minScaleX;
    public float minScaleY;
    public float maxVelocity;
    public float scaleChangeSpeed;
    public GameManager gm;
    private enum intensityLevel {Low, Medium, High, Max}
    [SerializeField] private float timeLow;
    [SerializeField] private float timeMedium;
    [SerializeField] private float timeHigh;
    [SerializeField] private float timeMax;
    [SerializeField] private float combinedInputSum;
    [SerializeField] private float combinedInputDuration;
    [SerializeField] private float averageStrengthPercent;

    private float currentScaleX;
    private float yRatio;
    public float combinedInput;
    public float oneStickMax;

    // Start is called before the first frame update
    void Start()
    {
        currentPos = controller.transform.position;
        lastPos = controller.transform.position;

        yRatio = minScaleY / minScaleX;

        currentScaleX = maxScaleX;
    }

    void Update()
    {
        if(gm.coasterMode && gm.mode == 0)
        {
            Vector2 left = moveProvider.leftHandMoveAction.action.ReadValue<Vector2>();
            Vector2 right = moveProvider.rightHandMoveAction.action.ReadValue<Vector2>();

            float leftStrength  = Mathf.Min(left.magnitude, oneStickMax);
            float rightStrength = Mathf.Min(right.magnitude, oneStickMax);

            float combinedInput = Mathf.Clamp01(leftStrength + rightStrength);
            float visualInput = 1f - Mathf.Pow(1f - combinedInput, 2f);
            float targetScaleX = Mathf.Lerp(maxScaleX, minScaleX, visualInput);

            currentScaleX = Mathf.Lerp(currentScaleX, targetScaleX, Time.deltaTime * scaleChangeSpeed);

            float currentScaleY = currentScaleX * yRatio;

            Vector3 newScale = transform.localScale;
            newScale.x = currentScaleX;
            newScale.y = currentScaleY;
            transform.localScale = newScale;

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
            float targetScaleX = Mathf.Lerp(maxScaleX, minScaleX, visualInput);

            currentScaleX = Mathf.Lerp(currentScaleX, targetScaleX, Time.deltaTime * scaleChangeSpeed);
            float currentScaleY = currentScaleX * yRatio;
            Vector3 newScale = transform.localScale;

            newScale.x = currentScaleX;
            newScale.y = currentScaleY;
            transform.localScale = newScale;
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
