using UnityEngine;

/*
 * Circles around nearby sheep to drive them towards the player
 *      -player puts dog in herding mode
 *      -dog follows player with sheep, unless a sheep has strayed then dog will leave player to retrieve sheep
 *      -dog is in idle mode when close enough to player
 * Attack an enemy that is attacking a sheep
 *      -player puts dog in attack mode
 * Get knocked out for a short time
 *      -when takes too much damage
 */
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
            Debug.Log("herd state update");
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
