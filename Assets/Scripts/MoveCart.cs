using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class MoveCart : MonoBehaviour
{
    public SplineAnimate splAnim;
    public BoxCollider trigger;
    [SerializeField] private bool canMove;

    void Start()
    {
        splAnim.Pause();
        trigger.isTrigger = true;
        canMove = false;
    }

    private void OnTriggerEnter(Collider coll)
    {
        splAnim.Restart(true);
        splAnim.Play();
        trigger.isTrigger = false;
        canMove = true;
    }
}
