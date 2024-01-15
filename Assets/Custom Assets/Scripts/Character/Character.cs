using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class Character : MonoBehaviour
{
    public NavMeshAgent _agent;

    [SerializeField]
    Animator _animator;


    public Slider uiHealthValue;

    public GameObject uiHealthObject;

    public GameObject player;

    [SerializeField]
    protected float currentHealth, maxHealth, attackPower, speed;

    public abstract void TakeDamage(GameObject weapon, float damage);

}
