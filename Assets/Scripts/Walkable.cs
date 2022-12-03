using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walkable : MonoBehaviour
{

    public List<Path> possiblePaths = new List<Path>();

    [ReadOnly]
    public Walkable lastCube;

    public bool isStairs;
    public float cubeOffset, stairOffset;

    public Vector3 GetWalkPoint()
    {
        return isStairs ? transform.position + transform.up * stairOffset : transform.position + transform.up * cubeOffset;
    }
}

[System.Serializable]
public class Path
{
    public Walkable destination;

    public Path(Walkable value)
    {
        destination = value;
    }
}