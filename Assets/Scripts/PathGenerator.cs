using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class PathGenerator : MonoBehaviour
{
    private enum Directions { FORWARD, RIGHT, LEFT }

    public GameObject cubePrefab;
    public Vector3 startPos;
    public float posOffset;
    public int maxNumOfCubes;

    private void Start()
    {
        StartCoroutine(Generate(maxNumOfCubes));
    }

    public IEnumerator Generate(int maxCubes)
    {
        Queue nextCubes = new Queue();
        List<GameObject> allCubesPlaced = new List<GameObject>();

        int numOfCubesPlaced = 0;

        var firstCube = Instantiate(cubePrefab, transform.position, Quaternion.identity);
        firstCube.transform.parent = transform;

        nextCubes.Enqueue(firstCube);
        allCubesPlaced.Add(firstCube);

        numOfCubesPlaced++;

        while (numOfCubesPlaced < maxCubes)
        {
            List<Directions> possibleDirs = new List<Directions>();
            possibleDirs.AddRange((Directions[])Enum.GetValues(typeof(Directions)));

            var currentCube = (GameObject)nextCubes.Dequeue();
            var currentWalkable = currentCube.GetComponent<Walkable>();

            var nextCube = Instantiate(cubePrefab, transform.position, Quaternion.identity);
            var nextWalkable = nextCube.GetComponent<Walkable>();
            nextCube.transform.parent = transform;

            currentWalkable.possiblePaths.Add(new Path(nextWalkable));
            nextWalkable.possiblePaths.Add(new Path(currentWalkable));

            NewDir:

            if (possibleDirs.Count <= 0)
            {
                Destroy(nextCube);
                continue;
            }

            var randDir = possibleDirs[Random.Range(0, possibleDirs.Count - 1)];
            possibleDirs.Remove(randDir);

            switch (randDir)
            {
                case Directions.FORWARD:
                    nextCube.transform.position = (currentCube.transform.position + Vector3.forward * posOffset);
                    break;
                case Directions.RIGHT:
                    nextCube.transform.position = (currentCube.transform.position + Vector3.right * posOffset);
                    break;
                case Directions.LEFT:
                    nextCube.transform.position = (currentCube.transform.position + Vector3.left * posOffset);
                    break;
              
            }

            for (int j = 0; j < allCubesPlaced.Count; j++)
            {
                if (nextCube.transform.position == allCubesPlaced[j].transform.position)
                    goto NewDir;
            }

            nextCubes.Enqueue(nextCube);
            allCubesPlaced.Add(nextCube);
            numOfCubesPlaced++;

            yield return null;
        }
    }
}