using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentPlatform : MonoBehaviour
{

    [SerializeField] private PlatformMoving platformMoving;
    [SerializeField] private ElevatorButton button;
    [SerializeField] private BoxCollider trigger;
    [SerializeField] private GameObject walls;
    [SerializeField] private GameObject elevator;
    [SerializeField] private GameManager gm;

    private void OnTriggerEnter(Collider coll)
    {
        coll.transform.SetParent(elevator.transform);  
        gm.allowTeleport = false;
    }

    //TODO: fix attaching to platform if it doesn't work in vr
    private void OnTriggerExit(Collider coll)
    {
        coll.transform.SetParent(null);
        platformMoving.isMoving = false;
        platformMoving.mesh.enabled = true;
        platformMoving.buttonCollider.enabled = true;
        platformMoving.secondCollider.enabled = true;
        walls.SetActive(false);
        platformMoving.transform.position = platformMoving.start.transform.position;
        gm.allowTeleport = true;
    }
}
