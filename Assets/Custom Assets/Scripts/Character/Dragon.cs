using UnityEngine;

public class Dragon : Enemy
{
    /*
     *  Shoots poison gas? OR fire? from nose to attack
     *      -affects all sheep within certain range
     *  After being attacked, flies up to avoid close range attacks while still attacking sheep
     *      -can't stay up long so must land after a little while
     *  Die when health drops to 0
    */
    [SerializeField]
    float attackRadius;
    [SerializeField]
    float attackCooldown;

    FSM fsm;
    FSM.State _wait, _attack, _die;

    private void OnEnable()
    {
        onHitCallback += TakeDamage;
    }

    private void Awake()
    {
        fsm = new();
        _wait = FSM_Wait;
        _attack = FSM_Attack;
        _die = FSM_Die;
    }

    private void Start()
    {
        fsm.OnSpawn(_wait);
        transform.LookAt(player.transform.position);

    }

    private void FixedUpdate()
    {
        //fsm.OnUpdate();
        _agent.Move(Vector3.forward * 0.005f);
    }

    void FSM_Wait(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {

        }

        if (step == FSM.Step.Update)
        {
            for (int i = 0; i < GameManager.instance.activeSheep.Count; i++)
            {
                if (Vector3.Distance(transform.position, GameManager.instance.activeSheep[i].transform.position) < attackRadius)
                {
                    fsm.TransitionTo(_attack);
                }
            }
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

    public override void TakeDamage(float damage, Weapon weapon = null, GameObject enemy = null)
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