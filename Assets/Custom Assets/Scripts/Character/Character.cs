using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class Character : MonoBehaviour
{
    public NavMeshAgent _agent;

    [SerializeField]
    Animator _animator;

    public float currentHealth, maxHealth;

    public Slider uiHealthValue;

    public GameObject uiHealthObject;

    public GameObject player;

    public abstract void TakeDamage(GameObject weapon);

}
