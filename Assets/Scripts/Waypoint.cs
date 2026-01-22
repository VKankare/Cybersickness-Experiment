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
    public GameObject walls;
    public bool hasWalls;

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
                gm.DisableInput();
                gm.NextCSMethod();
                StartCoroutine(CSMethodChange());
                if(hasWalls)
                {
                    DisableWalls();
                }
                gm.compassSection++;
            }
        }

        transform.position = points[currentPoint].position;
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
        gm.EnableInput();
    }
}
