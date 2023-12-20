using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Wolf : Enemy
{
    [SerializeField] 
    Transform ground;
    //if distance to target is less than chase radius, transition to chase
    [SerializeField] 
    float chaseRadius;
    //speed += something if wolves hungry, harder over time
    [SerializeField] 
    float hungerThreshold;
    //if distance is less than attackRadius, wolf will transition to attack
    [SerializeField] 
    float attackRadius;

    FSM fsm;
    //use current state for anything?
    FSM.State _chase, _attack, _evade, _die, _currentState;

    Timer timer;
    Vector3 previousTargetPosition;

    Vector3 chaseStartLocation;

    float _speed;
    int _hitsToDefeat;

    //track current hunger level
    //TODO: have increased hunger level make wolves more desperate; less likely to attack in packs, easier to kill; hunger goes down when it kills a sheep 
    //TODO: decide if above comment is how things will work
    float hunger;
    //list of active wolves in scene, will be used to influence wolves behavior to prefer staying in a pack and ganging up on nearby sheep 
    List<Wolf> activeWolves;
    List<Sheep> activeSheep;
    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<PlayerController>().gameObject;
        timer = FindObjectOfType<Timer>();
        _speed = type.baseSpeed.Value;
        _hitsToDefeat = type.hitsToDefeat.Value;
        hunger = 0f;

        //set state functions
        fsm = new FSM();
        _chase = FSM_Chase;
        _attack = FSM_Attack;
        //keep evade??
        _evade = FSM_Evade;
        _die = FSM_Die;
    }

    void OnEnable()
    {
        if (GameManager.instance != null && !GameManager.instance.activeWolves.Contains(this))
        {
            GameManager.instance.UpdateActiveWolves();
        }
    }
    void Start()
    {
        GameManager.instance.onUpdateWolvesCallback += UpdateWolves;
        GameManager.instance.onUpdateSheepCallback += UpdateSheep;
        activeWolves = GameManager.instance.activeWolves;
        activeSheep = GameManager.instance.activeSheep;
        fsm.OnSpawn(_chase);

    }
    void FixedUpdate()
    {
        fsm.OnUpdate();
    }

    public void OnHit()
    {
        Debug.Log("on hit called");
        hitCount.Value++;
        if (hitCount.Value >= _hitsToDefeat)
        {
            fsm.TransitionTo(_die);
        }
        else
        {
            fsm.TransitionTo(_chase);
        }
    }

    //chase player or sheep to attack
    void FSM_Chase(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {
            if (_agent.hasPath)
            {
                _agent.ResetPath();
            }
            _currentState = _chase;
            Debug.Log("enter chase state");
            chaseStartLocation = transform.position;
            if (target == null)
            {
                target = activeSheep[Random.Range(1, activeSheep.Count - 1)].gameObject;
            }
            previousTargetPosition = target.transform.position;

        }
        if (step == FSM.Step.Update)
        {
            Vector3 targetPos = target.transform.position;
            if (!_agent.hasPath || targetPos != previousTargetPosition)
            {
                /*_agent.SetDestination(targetPos);*/
            }
            if (Vector3.Distance(transform.position, player.transform.position) < 10)
            {
               if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
                {
                    if (hit.collider.gameObject == player)
                    {
                        float randomChanceToDodge = Random.value * 3;
                        if (randomChanceToDodge < 3 && randomChanceToDodge > 2)
                        {
                            CircleTarget();
                        }
                        _agent.SetDestination(targetPos);
                    }
                }
            }
        }
        if (step == FSM.Step.Exit)
        {
            _agent.ResetPath();
        }

    }
    //state wolf must be in to cause damage (add to hit count) to player or sheep
    void FSM_Attack(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {
            _currentState = _attack;
            Debug.Log("enter attack state");
            //will be switching to event 
        }
        if (step == FSM.Step.Update)
        {
            if (Vector3.Distance(transform.position, target.transform.position) > attackRadius)
            {
                fsm.TransitionTo(_chase);
            }
            if (Vector3.Distance(transform.position, target.transform.position) <= attackRadius)
            {
                if (!timer.wolfCooldownTimerActive)
                {
                    if (target != null)
                    {
                        target.GetComponent<HitCount>().Value++;
                        //add code to play attack animation

                        if (target != player)
                        {
                            target.GetComponent<Sheep>().attacker = this.gameObject;
                            //target.GetComponent<Sheep>().attackerDirection = transform.position - chaseStartLocation;
                        }
                        //switch to trigger event to give sheep direction for flee?
                    }
                }
            }
        }
        if (step == FSM.Step.Exit)
        {
            target = null;
        }
    }

    //evade player after being attacked or if targeted by player
    void FSM_Evade(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {
            CircleTarget();
        }

        if (step == FSM.Step.Update)
        {
            //get player location

        }

        if (step == FSM.Step.Exit)
        {

        }
    }
    //final code to be executed upon wolf being killed by player
    void FSM_Die(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {
            _currentState = _die;
            Debug.Log("enter die state");
            //playAnimation
            //playSound
        }
        if (step == FSM.Step.Update)
        {
            //if animation is done playing, transition to exit 
        }
        if (step == FSM.Step.Exit)
        {
            Debug.Log($"{this.name} died!");
            Destroy(gameObject);
        }
    }
    //called when GameManager.instance.activeWolves changes
    void UpdateWolves()
    {
        activeWolves = GameManager.instance.activeWolves;
    }
    //called when GameManager.instance.activeSheep changes
    void UpdateSheep()
    {
        activeSheep = GameManager.instance.activeSheep;
    }
    void CircleTarget()
    {
        _agent.SetPath(null);
        //transform.RotateAround(player.transform.position, Vector3.up, Random.Range(50, 270));

    }
    void OnDestroy()
    {
        if (Application.isPlaying)
        {
            GameManager.instance.UpdateActiveWolves();
        }
    }
}
