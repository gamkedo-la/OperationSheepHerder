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
    [SerializeField]
    ParticleSystem starParticles;
    float timeStamp;
    [SerializeField]
    WeaponSO weaponType;
    float coolDownSeconds;
    


    void Awake()
    {
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
                //TODO: add dust particles for when ground hit, if time
                //particles.Play();
            }

            else
            {
                if (timeStamp <= Time.time)
                {
                    if (hitObject.CompareTag("Enemy"))
                    {
                        Debug.Log("hit enemy");
                        hitObject.GetComponent<Enemy>().onHitCallback.Invoke(weaponType.DamageValue, weaponType, null);
                        Instantiate(starParticles, collision.contacts[0].point, Quaternion.identity);
                    }
                }
            }
            //cooldown to ensure hit character does not get more than one point added, maybe redundant
            timeStamp = Time.time + coolDownSeconds;
        }

    }
}
