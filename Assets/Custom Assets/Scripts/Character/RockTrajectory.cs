using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RockTrajectory : MonoBehaviour
{

    //TODO: targeting system to target wolves, not auto target but significant helper to target when close enough 
    [SerializeField] 
    Vector3 startPosition;

    [SerializeField]
    Vector3 startVelocity;

    [SerializeField]
    Vector2 input;

    [SerializeField] 
    float trajectoryVertDist = 0.25f;

    [SerializeField] 
    float maxCurveLength = 5f;

    [Header("Debug")]
    [SerializeField] 
    bool debugAlwaysDrawTrajectory = false;

    LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        startPosition = transform.position;
        startVelocity = transform.forward * GetComponent<LaunchRock>().launchVelocity;

    }

    void Update()
    {
        if (debugAlwaysDrawTrajectory)
        {
            DrawTrajectory();
        }
    }

    public void DrawTrajectory()
    {
        var curvePoints = new List<Vector3>();
        startPosition = transform.position;
        startVelocity = transform.forward * GetComponent<LaunchRock>().launchVelocity;
        curvePoints.Add(startPosition);
        var currentPosition = startPosition;
        var currentVelocity = startVelocity;

        Ray ray = new Ray(currentPosition, currentVelocity.normalized);
        int i = 0;
        while (!Physics.Raycast(ray, out RaycastHit hit, trajectoryVertDist) && Vector3.Distance(startPosition, currentPosition) < maxCurveLength)
        {
            i++;
            var t = trajectoryVertDist / currentVelocity.magnitude;

            if (i > 0 && i < 10)
            {
                currentVelocity.x += input.x;
                currentVelocity.y += input.y;
            }

            currentVelocity += t * Physics.gravity;


            currentPosition += t * currentVelocity;

            curvePoints.Add(currentPosition);

            ray = new Ray(currentPosition, currentVelocity.normalized);

            if (hit.transform)
            {
                //add code to visually show player their attack will hit target

                if (hit.collider.gameObject.CompareTag("Enemy"))
                {
                    //let that enemy script know that it is targeted
                }
                curvePoints.Add(hit.point);
            }


        }
        line.positionCount = curvePoints.Count;
        line.SetPositions(curvePoints.ToArray());
    }

    public void ClearTrajectory()
    {
        line.positionCount = 0;
    }
}
