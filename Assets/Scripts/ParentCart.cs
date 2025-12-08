using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ParentCart : MonoBehaviour
{

    [SerializeField] MoveCart moveCart;
    [SerializeField] ActionBasedContinuousMoveProvider moveProv;
    public GameManager gm;

    void Start()
    {
        moveCart = GetComponent<MoveCart>();
    }
    private void OnTriggerEnter(Collider coll)
    {
        coll.transform.SetParent(transform);
        moveProv.useGravity = false;
        gm.DisableMovement();
    }
}
