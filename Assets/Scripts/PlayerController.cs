using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{

    [Header("Values")]

    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private Vector3 targetCubePunchSize;
    [SerializeField]
    private float targetCubePunchDuration;

    public Color targetColor;
    public Color cubeColor;
    public float dur;

    private Sequence _playerMove_Seq;
    private PathFinder _pathFinder;
    private Walkable _lastClickedCube;

    private void Awake()
    {
        _pathFinder = GetComponent<PathFinder>();
    }

    private void OnEnable()
    {
        _pathFinder.OnPathFound += MoveAlongPath;
    }

    private void OnDisable()
    {
        _pathFinder.OnPathFound -= MoveAlongPath;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var targetCube = GetClickedCube();
            var startingCube = GetCubeStandingOn();

            if (_lastClickedCube == targetCube)
                return;

            if (targetCube != null && startingCube != null)
            {
                if (_lastClickedCube != null)
                    _lastClickedCube.GetComponent<MeshRenderer>().material.DOColor(Color.white, dur);

                targetCube.transform.DOPunchScale(targetCubePunchSize, targetCubePunchDuration, elasticity: 0, vibrato: 0);
                targetCube.GetComponent<MeshRenderer>().material.DOColor(targetColor, dur);

                _playerMove_Seq.Kill();
                StartCoroutine(_pathFinder.Search(startingCube, targetCube));
                _lastClickedCube = targetCube;
            }
        }
    }

    public float fade;

    private void MoveAlongPath(List<Transform> path)
    {
        _playerMove_Seq = DOTween.Sequence();
        var punchCube_Seq = DOTween.Sequence();
        var fadeCubeColor_Seq = DOTween.Sequence();

        for (int i = 0; i < path.Count; i++)
        {
           // punchCube_Seq.Append(path[i].DOPunchScale(targetCubePunchSize, targetCubePunchDuration, elasticity: 0, vibrato: 0));
            
            _playerMove_Seq.Append(transform.DOMove(path[i].GetComponent<Walkable>().GetWalkPoint(), .2f).SetEase(Ease.Linear));
            _playerMove_Seq.Join(transform.DOLookAt(path[i].position, .1f, AxisConstraint.Y, Vector3.up));
        }

        _playerMove_Seq.Append(_lastClickedCube.GetComponent<MeshRenderer>().material.DOColor(Color.white, dur));
    }

    private Walkable GetClickedCube()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;

        if (Physics.Raycast(ray, out rayHit))
        {
            return rayHit.transform.GetComponent<Walkable>();
        }

        return null;
    }

    private Walkable GetCubeStandingOn()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit rayHit;

        if (Physics.Raycast(ray, out rayHit))
        {
            return rayHit.transform.GetComponent<Walkable>();
        }

        return null;
    }
}