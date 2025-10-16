using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Waypoint : MonoBehaviour
{
    [SerializeField] private int currentPoint;
    [SerializeField] private Transform[] points;
    [SerializeField] private Transform sectionStart;
    public GameManager gm;
    [SerializeField] private FadeScreen fadeScreen;
    [SerializeField] ActionBasedContinuousMoveProvider moveProv;
    [SerializeField] private int sectionCounter;
    private int sections = 1;
    [SerializeField] private VignetteModifier vm;

    void Start()
    {
        currentPoint = 0;
        transform.position = points[currentPoint].position;
    }

    private void OnTriggerEnter(Collider coll)
    {
        currentPoint++;

        if (currentPoint >= points.Length)
        {
            currentPoint = 0;

            if (sectionCounter < sections)
            {
                sectionCounter++;
                gm.DisableInput();
                gm.NextCSMethod();
                StartCoroutine(TeleportToStart(coll));
            }
            else
            {
                gm.NextCSMethod();
            }

        }

        transform.position = points[currentPoint].position;
    }

    IEnumerator TeleportToStart(Collider coll)
    {
        coll.transform.SetParent(null);
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);
        coll.transform.position = sectionStart.position;
        moveProv.useGravity = true;
        fadeScreen.FadeIn();
        gm.EnableInput();
    }
}
