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

    public void DrawTrajectory()
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
        if (hit.collider)
        {
            if (hit.collider.gameObject.CompareTag("Enemy"))
            {
                LockOn(hit.collider.gameObject);
            }
        }
        line.positionCount = curvePoints.Count;
        line.SetPositions(curvePoints.ToArray());
    }

    void LockOn(GameObject target)
    {
        //TODO: visually show player they are targeting an enemy
        //TODO: Not sure how to implement this yet
        //TODO: let that enemy script know that it is targeted, for evasion?

        //player.onTargetFoundCallback.Invoke(target);
        //line.SetPositions(new Vector3[] { transform.position, target.transform.position });
    }

    public void ClearTrajectory()
    {
        line.positionCount = 0;
    }
}
