using System.Collections;
using UnityEngine;
using UnityEngine.AI;

//TODO: Re-work enemy structure
public class Enemy : MonoBehaviour
{
    public EnemyType type;

    public HitCount hitCount;

    public GameObject player;

    public GameObject target = null;

    public NavMeshAgent _agent;

    [SerializeField]
    Animator _animator;


    bool isFacingTarget = false;

    bool isTargetedByPlayer = false;

    public Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;
        NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);
        return navHit.position;
    }
    //if player or sheep is targeted, face it-- needs fixing
    public IEnumerator FaceTarget()
    {

        if (target == null)
        {
            target = player;
        }
        while (!isFacingTarget)
        {
            
            Vector3 direction = (target.transform.position - transform.position).normalized;
            if (direction.sqrMagnitude <= 0.1f)
            {
                direction = Vector3.zero;
            }
            if (direction == Vector3.zero)
            {
                isFacingTarget = true;
                yield break;
            }
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            float angle = Vector3.Angle(transform.position, target.transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, (angle / 360) * 2f);
            yield return null;  
        }
    }
}
