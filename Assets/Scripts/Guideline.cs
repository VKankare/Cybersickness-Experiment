using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guideline : MonoBehaviour
{
    public LineRenderer lr;
    
    public GameObject[] objects;

    public int lineCount;

    // Start is called before the first frame update
    void Start()
    {
        lr.positionCount = lineCount;

        for (int i = 0; i < objects.Length -1; i++)
        {
            lr.SetPosition(i, objects[i].transform.position);
        }     

    }
}
