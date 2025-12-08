using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRemover : MonoBehaviour
{
    public GameObject walls;

    public void DisableWalls()
    {
        walls.SetActive(false);
    }
}
