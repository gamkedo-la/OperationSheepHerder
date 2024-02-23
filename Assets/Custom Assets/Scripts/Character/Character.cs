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

    public delegate void OnHit(WeaponSO weapon, float damage);
    public OnHit onHitCallback;

    public abstract void TakeDamage(WeaponSO weapon, float damage);

}
