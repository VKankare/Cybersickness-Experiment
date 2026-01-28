using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoasterTeleporter : MonoBehaviour
{

    [SerializeField] private GameObject cart;
    [SerializeField] MoveCart moveCart;
    [SerializeField] GameObject playerCamera;
    public GameManager gm;

    private void OnTriggerEnter(Collider coll)
    {
        coll.transform.position = cart.transform.position;
        coll.transform.rotation = cart.transform.rotation;
        playerCamera.transform.rotation = cart.transform.rotation;
        moveCart.trigger.isTrigger = true;
        gm.toggleCoasterMode();
        gm.coasterSection = true;
    }
}
