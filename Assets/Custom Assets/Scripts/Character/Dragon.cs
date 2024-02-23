using UnityEngine;

public class Dragon : Enemy
{
    [SerializeField]
    float attackRadius;
    [SerializeField]
    float attackCooldown;

    FSM fsm;
    FSM.State _chase, _attack, _die;

    private void OnEnable()
    {
        onHitCallback += TakeDamage;
    }

    private void Awake()
    {
        fsm = new();
        _chase = FSM_Chase;
        _attack = FSM_Attack;
        _die = FSM_Die;
    }

    private void Start()
    {
        
    }

    void FSM_Chase(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {

        }

        if (step == FSM.Step.Update)
        {

        }

        if (step == FSM.Step.Exit)
        {

        }
    }

    void FSM_Attack(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {

        }

        if (step == FSM.Step.Update)
        {

        }

        if (step == FSM.Step.Exit)
        {

        }
    }

    void FSM_Die(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {

        }

        if (step == FSM.Step.Update)
        {

        }

        if (step == FSM.Step.Exit)
        {

        }
    }

    public override void TakeDamage(WeaponSO weapon, float damage)
    {
        currentHealth -= damage;
        
        uiHealthValue.value = currentHealth / maxHealth;

        if (uiHealthObject)
        {
            //if the enemy health is less than the max, turn the UI on
            if (currentHealth <= maxHealth)
            {
                uiHealthObject.SetActive(true);
                uiHealthObject.transform.LookAt(Camera.main.transform);
            }
            //if the enemy health is at (or greater than) max, turn the UI off
            else
            {
                uiHealthObject.SetActive(false);
            }
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
        }
    }

    private void OnDisable()
    {
        onHitCallback -= TakeDamage;
    }
}