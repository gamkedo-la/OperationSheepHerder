using System.Collections;
using UnityEngine;
using UnityEngine.AI;

//TODO: Re-work enemy structure
public abstract class Enemy : MonoBehaviour
{
    public EnemyType type;

    public GameObject player;

    public GameObject target = null;

    public NavMeshAgent _agent;

    [SerializeField]
    Animator _animator;

    public delegate void OnHit(GameObject weapon);
    public OnHit onHitCallback;

    //bool isTargetedByPlayer = false;

    public Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;
        NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);
        return navHit.position;
    }

    public abstract void TakeDamage(GameObject weapon);
}
