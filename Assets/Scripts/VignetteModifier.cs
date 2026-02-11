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
    private enum intensityLevel {Off, Low, High}
    [SerializeField] private float timeOff;
    [SerializeField] private float timeLow;
    [SerializeField] private float timeHigh;


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

            moveProvider.moveSpeed = maxVelocity * combinedInput;

            float targetVignetteSize = Mathf.Lerp(maxVignetteSize, minVignetteSize, combinedInput);

            currentVignetteSize = Mathf.Lerp(currentVignetteSize, targetVignetteSize, Time.deltaTime * vignetteSmoothSpeed);

            vignetteMat.SetFloat("_ApertureSize", currentVignetteSize);   
            
            switch(GetLevel(combinedInput))
            {
                case intensityLevel.Off:
                    timeOff += Time.deltaTime;
                    break;
                case intensityLevel.Low:
                    timeLow += Time.deltaTime;
                    break;
                case intensityLevel.High:
                    timeHigh += Time.deltaTime;
                    break;
            }

            Debug.Log("vignette intensity: " + GetLevel( combinedInput));         
        }
        else if(!gm.coasterMode)
        {
            currentPos = controller.transform.position;
            var velocity = (currentPos - lastPos) / Time.deltaTime;
            lastPos = currentPos;

            float velocityMag = velocity.magnitude;
            float t = Mathf.Clamp01(velocityMag / maxVelocity);

            float targetVignetteSize = Mathf.Lerp(maxVignetteSize, minVignetteSize, t);

            currentVignetteSize = Mathf.Lerp(currentVignetteSize, targetVignetteSize, Time.deltaTime * vignetteSmoothSpeed);

            vignetteMat.SetFloat("_ApertureSize", currentVignetteSize);            
        }
    }

    intensityLevel GetLevel(float combinedInput)
    {
        if(combinedInput < 0.01f)
        {
            return intensityLevel.Off;            
        }
        else if(combinedInput <= oneStickMax)
        {
            return intensityLevel.Low;
        }
        else
        {
            return intensityLevel.High;
        }   
    }
}
