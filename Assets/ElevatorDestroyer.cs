using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorDestroyer : MonoBehaviour
{
    public GameObject elevator;
    private BoxCollider trigger;
    [SerializeField] PlatformMoving platformMoving;

    // Start is called before the first frame update
    void Start()
    {
        platformMoving = GetComponent<PlatformMoving>();
        trigger = GetComponent<BoxCollider>();
        trigger.isTrigger = true;   
    }

    private void OnTriggerEnter(Collider coll)
    {
        DisableElevator();

    }

    IEnumerator DisableElevator()
    {
        elevator.SetActive(false);
        yield return new WaitForSeconds(2);
        elevator.SetActive(true);
        platformMoving.transform.position = platformMoving.points[platformMoving.startPoint].position;
    }
}
