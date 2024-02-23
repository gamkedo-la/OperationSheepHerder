using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

//TODO: Re-work enemy structure
public abstract class Enemy : Character
{
    public EnemyType type;

    public GameObject target = null;

    //bool isTargetedByPlayer = false;

    public Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;
        NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);
        return navHit.position;
    }

}
