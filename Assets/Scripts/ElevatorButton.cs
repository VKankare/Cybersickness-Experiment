using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorButton : MonoBehaviour
{
    [SerializeField] private PlatformMoving platformMoving;
    [SerializeField] private GameObject walls;

    void Start()
    {        
        walls.SetActive(false);
        platformMoving.isMoving = false;   
    }

    private void OnTriggerEnter(Collider coll)
    {
        walls.SetActive(true);
        platformMoving.isMoving = true;        
    }
}
