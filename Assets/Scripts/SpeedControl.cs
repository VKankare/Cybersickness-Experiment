using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpeedControl : MonoBehaviour
{
    public float targetSpeed;
    private float currentSpeed;
    public MoveAlongSpline splAnim;

    // Update is called once per frame
    private void OnTriggerEnter(Collider coll)
    {
        //splAnim.speed = targetSpeed;
    }
}
