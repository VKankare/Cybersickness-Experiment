using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorWait : MonoBehaviour
{
    //stupid fix for a stupid thing

    [SerializeField] private PlatformMoving platformMoving;

    private void OnTriggerEnter(Collider coll)
    {
        platformMoving.buttonCollider.enabled = true;
        //platformMoving.mesh.enabled = true;
    }
}
