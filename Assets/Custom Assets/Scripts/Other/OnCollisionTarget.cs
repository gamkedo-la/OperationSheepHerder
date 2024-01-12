using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//on rock collision with object, add to objects hit count or play particle effect
public class OnCollisionTarget : MonoBehaviour
{
    //keep track of first hit object after launch, once an object is hit it is set to false and will not perform logic for other collisions until set to true when rock is launched again
    public bool firstHit = true;
    [SerializeField]
    ParticleSystem particles;
    float timeStamp;
    [SerializeField] 
    float coolDownSeconds;
    public bool hitEnemy = false;
    Rigidbody rB;


    void Awake()
    {
        rB = GetComponent<Rigidbody>();
        firstHit = true;
    }
    public void OnCollisionEnter(Collision collision)
    {
        GameObject hitObject = collision.gameObject;
        
        if (firstHit)
        {
            firstHit = false;
            if (hitObject.CompareTag("Ground"))
            {
                particles.Play();
            }

            else
            {
                if (timeStamp <= Time.time)
                {
                    if (hitObject.CompareTag("Enemy"))
                    {
                        Debug.Log("hit enemy");
                        hitObject.GetComponent<Enemy>().onHitCallback.Invoke(this.gameObject);
                    }
                }
            }
            //cooldown to ensure hit character does not get more than one point added, maybe redundant
            timeStamp = Time.time + coolDownSeconds;
        }

    }
}
