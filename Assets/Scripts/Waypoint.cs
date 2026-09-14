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
    public GameObject walls;
    public bool hasWalls;

    void Start()
    {
        currentPoint = 0;
        transform.position = points[currentPoint].position;
    }

    private void OnTriggerEnter(Collider coll)
    {
        gm.previousPoint = points[currentPoint];
        currentPoint++;

        if (currentPoint >= points.Length)
        {
            gm.previousPoint = null;
            currentPoint = 0;
            gm.StopTimer();
            gm.DisableInput();
            gm.ToggleLines();

            if (sectionCounter < sections)
            {
                sectionCounter++;
                gm.NextCSMethod();
                StartCoroutine(TeleportToStart(coll));
            }
            else if(sectionCounter == sections && gm.compassSection < 3)
            {
                gm.NextCSMethod();
                StartCoroutine(CSMethodChange());
                if(hasWalls)
                {
                    DisableWalls();
                }
                gm.compassSection++;
            }
            else if(sectionCounter == sections && gm.compassSection >= 3)
            {
                gm.EmptyCSMethod();
                StartCoroutine(TeleportToStart(coll));
                if(hasWalls)
                {
                    DisableWalls();
                }
            }
        }
        transform.position = points[currentPoint].position;
        gm.SaveToCSV();
    }

    public void DisableWalls()
    {
        walls.SetActive(false);
    }

    IEnumerator TeleportToStart(Collider coll)
    {
        coll.transform.SetParent(null);
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);
        coll.transform.position = sectionStart.position;
        moveProv.useGravity = true;
        gm.EnableSlider();
        if(gm.coasterMode)
        {
            gm.ToggleCoasterMode();
        }
        fadeScreen.FadeIn();
        gm.EnableInput();
    }

    IEnumerator CSMethodChange()
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);
        moveProv.useGravity = true;
        gm.EnableSlider();
        fadeScreen.FadeIn();
        if(gm.coasterMode)
        {
            gm.ToggleCoasterMode();
        }
        gm.EnableInput();
    }
}
