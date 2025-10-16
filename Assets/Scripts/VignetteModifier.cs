using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VignetteModifier : MonoBehaviour
{
    public Material vignetteMat;

    public CharacterController controller;
    private Vector3 currentPos;
    private Vector3 lastPos;
    [SerializeField] private MeshRenderer mesh;


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

    // Update is called once per frame
    void Update()
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
