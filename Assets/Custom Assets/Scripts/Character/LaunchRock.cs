using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class LaunchRock : MonoBehaviour
{
    [SerializeField]
    PlayerController player;

    [SerializeField]
    GameObject rockPrefab;

    [SerializeField] 
    int rockCount = 0;

    [SerializeField] 
    int maxRockCount = 5;

    public float launchVelocity;

    GameObject[] rocks;

    
    
    int counter = 0;
    void Awake()
    {
        rocks = new GameObject[5];
        for (int i = 0; i < rocks.Length; i++)
        {
            rocks[i] = null;
        }
    }

    private void OnEnable()
    {
        player.onAttackCallback += OnPlayerAttack;
    }

    private void OnDisable()
    {
        player.onAttackCallback -= OnPlayerAttack;

    }
    public void OnPlayerAttack()
    {
        var fireVector = transform.forward;
        float distanceFromTargetMultiplier = 1f;
        if(player.LockedOn && player.Target){
            var directionVector = player.Target.position - transform.position;
            if (directionVector.y <= 0)
            {
                directionVector.y = 0.1f;
            }
            distanceFromTargetMultiplier = Vector3.Distance(player.Target.position, transform.position);
            directionVector.y *= distanceFromTargetMultiplier / 2;
            directionVector.y += player.Target.GetChild(0).GetChild(0).localPosition.y * player.Target.localScale.y * 2;

            fireVector = directionVector.normalized;
            
        }

        // to begin level, instantiate new rock objects until reaching maxRockCount
        if (rockCount < maxRockCount)
        {
            GameObject rock = Instantiate(rockPrefab, transform.position, transform.rotation);
            Rigidbody rockRB = rock.GetComponent<Rigidbody>();
            rockRB.AddForce(fireVector * launchVelocity, ForceMode.VelocityChange);
            rocks[System.Array.FindIndex(rocks, x => x == null)] = rock;
        }

        // once maxRockCount has been reached, instead of instantiating new game objects, move existing rock back to start position and fire that one again
        // this cycles through the rocks starting with the one that was launched least recently, so they disappear from where they landed in the order they were fired
        else
        {
            rockCount = rocks.Count();

            rocks[counter].GetComponent<OnCollisionTarget>().firstHit = true;
            rocks[counter].GetComponent<Rigidbody>().position = transform.position;
            rocks[counter].GetComponent<Rigidbody>().velocity = Vector3.zero;
            rocks[counter].GetComponent<Rigidbody>().AddForce(fireVector * launchVelocity, ForceMode.VelocityChange);
            counter++;
        }
        if (counter >= maxRockCount)
        {
            counter = 0;
        }
        rockCount++;
    }
}
