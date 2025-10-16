using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class ResetCoaster : MonoBehaviour
{
    public SplineAnimate splAnim;

    private void OnTriggerEnter(Collider coll)
    {
        splAnim.Restart(true);
    }
}
