using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathFinder : MonoBehaviour
{

    public event Action<List<Transform>> OnPathFound;

    public IEnumerator Search(Walkable startingCube, Walkable targetCube)
    {
        if (targetCube == startingCube)
            yield break;

        List<Walkable> nextCubes = new List<Walkable>();
        List<Walkable> visitedCubes = new List<Walkable>();

        List<Walkable> searchedPath = new List<Walkable>();
        List<Transform> finalPath = new List<Transform>();

        //get starting set of paths

        foreach (var path in startingCube.possiblePaths)
        {
            nextCubes.Add(path.destination);
            searchedPath.Add(path.destination);
            path.destination.GetComponent<Walkable>().lastCube = startingCube;
        }

        //add starting cube as visited

        visitedCubes.Add(startingCube);

        //find target, else search through next cubes paths

        while (nextCubes.Count != 0)
        {
            var currentCube = nextCubes[0];
            nextCubes.Remove(currentCube);

            if (currentCube == targetCube)
            {
                finalPath.AddRange(BuildFinalPath(startingCube, targetCube));
                OnPathFound?.Invoke(finalPath);
                ResetSearchedPath(searchedPath);
                yield break;
            }

            foreach (var path in currentCube.possiblePaths)
            {
                if (!visitedCubes.Contains(path.destination))
                {
                    nextCubes.Add(path.destination);
                    searchedPath.Add(path.destination);
                    path.destination.GetComponent<Walkable>().lastCube = currentCube;
                }
            }

            visitedCubes.Add(currentCube);

            yield return null;
        }
    }

    private List<Transform> BuildFinalPath(Walkable startingCube, Walkable targetCube)
    {
        List<Transform> path = new List<Transform>();
        var cube = targetCube;

        while (cube != startingCube)
        {
            path.Add(cube.transform);
            cube = cube.lastCube;
        }

        path.Reverse();

        return path;
    }

    private void ResetSearchedPath(List<Walkable> pathSearched)
    {
        foreach (var cube in pathSearched)
        {
            cube.lastCube = null;
        }
    }
}