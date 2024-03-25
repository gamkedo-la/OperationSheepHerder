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

    [SerializeField]
    protected float currentHealth, maxHealth, attackPower, speed;

    public delegate void OnHit(float damage, WeaponSO weapon = null, GameObject enemy = null);
    public OnHit onHitCallback;

    public abstract void TakeDamage(float damage, WeaponSO weapon = null, GameObject enemy = null);

}
