using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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

    PlayerController player;


    void Awake()
    {
        player = GetComponentInParent<PlayerController>();
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

    Transform GetNearestTarget()
    {
        var colliders = Physics.OverlapSphere(transform.position, maxCurveLength);
        Transform bestTarget = null;

        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (var potentialTarget in colliders)
        {
            if (!potentialTarget.CompareTag("Enemy"))
            {
                continue;
            }

            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget.transform;
            }
        }

        return bestTarget;
    }

    public void DrawTrajectory()
    {
        if (player.LockedOn)
        {
            player.Target = GetNearestTarget();
            if (player.Target == null)
            {
                DrawUnlockedTrajectory();
                return;
            }
            DrawLockedTrajectory();
            return;
        }
        DrawUnlockedTrajectory();
    }

    public void DrawLockedTrajectory()
    {
        var curvePoints = new List<Vector3>();
        startPosition = transform.position;
        startVelocity = (player.Target.position - startPosition).normalized * GetComponent<LaunchRock>().launchVelocity;
        startVelocity.y = 0;
        curvePoints.Add(startPosition);
        var currentPosition = startPosition;
        var currentVelocity = startVelocity;

        int i = 0;
        int numSteps = 10;
        float dt = 0.1f;
        float time = 0.0f;
        while (i < numSteps && Vector3.Distance(currentPosition, player.Target.position) > Vector3.kEpsilon)
        {
            i++;
            time += dt;
            currentPosition = startPosition + time * startVelocity;
            currentPosition.y = startPosition.y + startVelocity.y * time + (Physics.gravity.y / 2f * time * time);

            curvePoints.Add(currentPosition);

            Vector3 lastPosition = line.GetPosition(line.positionCount - 1);

            if (Physics.Raycast(lastPosition, (currentPosition - lastPosition).normalized, out var hit, (currentPosition - lastPosition).magnitude))
            {
                if(hit.transform.CompareTag("Enemy") || hit.transform.CompareTag("Ground")){
                    curvePoints.Add(hit.point);
                    break;
                }
                
            }

        }
        line.positionCount = curvePoints.Count;
        line.SetPositions(curvePoints.ToArray());
    }

    private void DrawUnlockedTrajectory()
    {
        var curvePoints = new List<Vector3>();
        startPosition = transform.position;
        startVelocity = transform.forward * GetComponent<LaunchRock>().launchVelocity;
        curvePoints.Add(startPosition);
        var currentPosition = startPosition;
        var currentVelocity = startVelocity;

        Ray ray = new Ray(currentPosition, currentVelocity.normalized);
        RaycastHit hit;
        int i = 0;
        while (!Physics.Raycast(ray, out hit, trajectoryVertDist) && Vector3.Distance(startPosition, currentPosition) < maxCurveLength)
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
