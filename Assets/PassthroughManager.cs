using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassthroughManager : MonoBehaviour
{
    public GameObject passthroughPlane;
    public CharacterController controller;
    private Vector3 currentPos;
    private Vector3 lastPos;

    public float maxScaleX;
    public float minScaleX;
    public float minScaleY;
    public float maxVelocity;
    public float scaleChangeSpeed;

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

    // Update is called once per frame
    void Update()
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
