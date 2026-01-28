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


    public float vignetteSmoothSpeed;
    public float maxVignetteSize;
    public float minVignetteSize;
    public float maxVelocity;

    private float currentVignetteSize;

    void Start()
    {
        currentPos = controller.transform.position;
        lastPos = controller.transform.position;
    }

    void Update()
    {
        if(gm.coasterMode)
        {
            Vector2 left = moveProvider.leftHandMoveAction.action.ReadValue<Vector2>();
            Vector2 right = moveProvider.rightHandMoveAction.action.ReadValue<Vector2>();

            float combinedInput = Mathf.Clamp01((left.magnitude + right.magnitude) * 0.5f);

            moveProvider.moveSpeed = maxVelocity * combinedInput;

            float targetVignetteSize = Mathf.Lerp(maxVignetteSize, minVignetteSize, combinedInput);

            currentVignetteSize = Mathf.Lerp(currentVignetteSize, targetVignetteSize, Time.deltaTime * vignetteSmoothSpeed);

            vignetteMat.SetFloat("_ApertureSize", currentVignetteSize);            
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
}
