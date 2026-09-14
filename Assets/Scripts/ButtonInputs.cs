using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonInputs : MonoBehaviour
{
    public InputActionReference teleportButton;
    public GameManager gm;
    public GameObject xrOrigin;

    void Awake()
    {
        teleportButton.action.Enable();
        teleportButton.action.started += Teleport; 
    }

    private void OnDestroy()
    {
        teleportButton.action.Disable();
        teleportButton.action.started -= Teleport;
    }

    private void Teleport(InputAction.CallbackContext context)
    {
        Debug.Log("Teleport button pressed");
        if (gm.previousPoint != null && gm.allowTeleport)
        {
            xrOrigin.transform.position = gm.previousPoint.position;
            Debug.Log("Teleporting");    
        }
    }
}
