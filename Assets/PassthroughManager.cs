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
    private enum intensityLevel {Off, Low, High}
    [SerializeField] private float timeOff;
    [SerializeField] private float timeLow;
    [SerializeField] private float timeHigh;

    private float currentScaleX;
    private float yRatio;

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

            float combinedInput = Mathf.Clamp01((left.magnitude + right.magnitude) * 0.5f);
            float targetScaleX = Mathf.Lerp(maxScaleX, minScaleX, combinedInput);

            currentScaleX = Mathf.Lerp(currentScaleX, targetScaleX, Time.deltaTime * scaleChangeSpeed);

            float currentScaleY = currentScaleX * yRatio;

            Vector3 newScale = transform.localScale;
            newScale.x = currentScaleX;
            newScale.y = currentScaleY;
            transform.localScale = newScale;

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

            Debug.Log("passthrough intensity: " + GetLevel(combinedInput));
        }
        else if(!gm.coasterMode)
        {
            currentPos = controller.transform.position;
            var velocity = (currentPos - lastPos) / Time.deltaTime;
            lastPos = currentPos;

            float velocityMag = velocity.magnitude;
            float t = Mathf.Clamp01(velocityMag / maxVelocity);

            float targetScaleX = Mathf.Lerp(maxScaleX, minScaleX, t);

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
        if(combinedInput < 0.01f)
        {
            return intensityLevel.Off;            
        }
        else if(combinedInput <= 0.5f)
        {
            return intensityLevel.Low;
        }
        else
        {
            return intensityLevel.High;
        }   
    }
}
