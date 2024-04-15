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

    // Method to check if the character is moving
    public bool IsMoving()
    {
        return _agent.velocity.sqrMagnitude > 0.1f; // Adjust the threshold value as needed
    }
}
