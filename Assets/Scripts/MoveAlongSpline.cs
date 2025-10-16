using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class MoveAlongSpline : MonoBehaviour
{
    public SplineContainer spline;
    public float speed = 0;
    public float targetSpeed;
    float distancePercentage = 0f;
    public float easeDuration;
    public bool trigger = false;

    float splineLength;

    void Start()
    {
        splineLength = spline.CalculateLength();
        trigger = false;
    }

    void Update()
    {
        distancePercentage += speed * Time.deltaTime / splineLength;

        Vector3 currentPosition = spline.EvaluatePosition(distancePercentage);
        transform.position = currentPosition;

        if (distancePercentage > 1f)
        {
            distancePercentage = 0f;
        }

        Vector3 nextPosition = spline.EvaluatePosition(distancePercentage + 0.05f);
        Vector3 direction = nextPosition - currentPosition;   
        Unity.Mathematics.float3 upFloat3 = spline.EvaluateUpVector(distancePercentage);
        Vector3 upVector3 = new(upFloat3.x, upFloat3.y, upFloat3.z);
        transform.rotation = Quaternion.LookRotation(direction, upVector3);
    }

    public void StartMovement()
    {
        StartCoroutine(EaseInSpeed(speed, targetSpeed, easeDuration));
    }

    public IEnumerator EaseInSpeed(float _speed, float _targetSpeed, float _easeDuration)
    {
        float timer = 0;
        while (timer <= _easeDuration)
        {
            timer += Time.deltaTime;
            _speed = Mathf.Lerp(_speed, _targetSpeed, timer / _easeDuration);
            yield return null;
        }

        yield return null;
    }
}
