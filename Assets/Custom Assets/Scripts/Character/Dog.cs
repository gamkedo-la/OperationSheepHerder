using UnityEngine;

public class Dog : Character
{
    [SerializeField]
    float attackRadius;
    [SerializeField]
    float attackCooldown;

    FSM fsm;
    FSM.State _herd, _attack, _knockedOut;

    private void OnEnable()
    {
        onHitCallback += TakeDamage;
    }

    private void Awake()
    {
        fsm = new FSM();
        _herd = FSM_Herd;
        _attack = FSM_Attack;
        _knockedOut = FSM_KnockedOut;
    }

    private void Start()
    {
        fsm.OnSpawn(_herd);
    }

    void FSM_Herd(FSM fsm, FSM.Step step, FSM.State state)
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

    void FSM_KnockedOut(FSM fsm, FSM.Step step, FSM.State state)
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
    public override void TakeDamage(float damage, WeaponSO weapon = null, GameObject enemy = null)
    {
        throw new System.NotImplementedException();
    }

    private void OnDisable()
    {
        onHitCallback -= TakeDamage;
    }
}
