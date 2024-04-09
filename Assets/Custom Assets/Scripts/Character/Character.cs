using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class Character : MonoBehaviour
{
    public NavMeshAgent _agent;

    
    public Animator _animator;


    public Slider uiHealthValue;

    public GameObject uiHealthObject;

    public GameObject player;

    public bool cooldownTimerActive = false;

    [SerializeField]
    protected float currentHealth, maxHealth, attackPower, speed;

    public delegate void OnHit(float damage, Weapon weapon = null, GameObject enemy = null);
    public OnHit onHitCallback;

    public abstract void TakeDamage(float damage, Weapon weapon = null, GameObject enemy = null);


}
