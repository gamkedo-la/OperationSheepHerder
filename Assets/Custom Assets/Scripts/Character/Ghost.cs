using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Enemy
{
    [SerializeField]
    float attackRadius;
    [SerializeField]
    float attackCooldown;

    FSM fsm;
    FSM.State _attack, _die, _follow;

    private void OnEnable()
    {
        onHitCallback += TakeDamage;
    }
    private void Awake()
    {
        fsm = new FSM();
        _follow = FSM_Follow;
        _attack = FSM_Attack;
        _die = FSM_Die;
    }

    private void Start()
    {
        fsm.OnSpawn(_follow);
    }

    void FSM_Follow(FSM fsm, FSM.Step step, FSM.State state)
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


    public override void TakeDamage(float damage, WeaponSO weapon = null, GameObject enemy = null)
    {

    }

    private void OnDisable()
    {
        onHitCallback -= TakeDamage;
    }
}
