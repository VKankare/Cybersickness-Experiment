using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class MovementSpeedModifier : MonoBehaviour
{
    public ActionBasedContinuousMoveProvider moveProvider;
    public float maxSpeed;  

    void Update()
    {
        Vector2 left = moveProvider.leftHandMoveAction.action.ReadValue<Vector2>();
        Vector2 right = moveProvider.rightHandMoveAction.action.ReadValue<Vector2>();

        float combinedInput = left.magnitude + right.magnitude;

        float newSpeed = maxSpeed * combinedInput;

        moveProvider.moveSpeed = newSpeed;
    }
}
