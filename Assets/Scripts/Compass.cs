using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    public Transform headset;
    public Transform[] target;
    public GameManager gm;
    [SerializeField] private Canvas canvas; 

    void Update()
    {
        if(gm.compassSection == 3)
        {
            canvas.transform.localPosition = new Vector3(-2, -2, -2); 
        }

        if (!headset || !target[gm.compassSection]) 
        {
            return;
        }

        Vector3 direction = target[gm.compassSection].position - headset.position;

        Vector3 flatDir = Vector3.ProjectOnPlane(direction, Vector3.up);

        if (flatDir.sqrMagnitude < 0.001f) return;

        Quaternion lookRotation = Quaternion.LookRotation(flatDir);

        Vector3 euler = lookRotation.eulerAngles;
        transform.rotation = Quaternion.Euler(90, euler.y + 90, 0);
    }
}
