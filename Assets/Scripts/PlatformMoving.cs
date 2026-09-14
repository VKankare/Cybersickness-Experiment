using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMoving : MonoBehaviour
{
    public bool isMoving;

    [SerializeField] float speed;
    public int startPoint;
    public Transform[] points;
    [SerializeField] private GameObject gate;
    public MeshRenderer mesh;
    public BoxCollider buttonCollider;
    public BoxCollider secondCollider;
    public GameObject start;

    int i;
    bool reverse;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = start.transform.position;
        i = startPoint;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, points[i].position) < 0.01f)
        {
            isMoving = false;
            gate.SetActive(false);
            
            if (i == points.Length - 1)
            {
                reverse = true;
                i--;
                return;
            }
            else if (i == 0)
            {
                reverse = false;
                i++;
                return;
            }

            if (reverse)
            {
                i--;
            }
            else
            {
                i++;
            }
        }

        if(isMoving)
        {
            gate.SetActive(true);
            //mesh.enabled = false;
            buttonCollider.enabled = false;
            secondCollider.enabled = false;
            transform.position = Vector3.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
        }
    }
}
