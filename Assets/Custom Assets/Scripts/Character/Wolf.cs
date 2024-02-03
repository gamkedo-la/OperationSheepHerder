using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.UI;
using Unity.AI.Navigation;
using UnityEngine.Android;

public class Wolf : Enemy
{
    //if distance to target is less than chase radius, transition to chase
    [SerializeField] 
    float chaseRadius;

    // if turned to true, will look for another wolf to follow instead of the player or sheep
    [SerializeField]
    bool packFollower; // if something happens to the leader or they need to split, just flip false
    Vector2 packChaseRandomFormationOffset; // to avoid bunching up

    //TODO: Decide whether to use hunger variable to make game harder; maybe increase speed or attack power
    /*  speed += something if wolves hungry, harder over time
        [SerializeField] 
        float hungerThreshold;
        track current hunger level
        TODO: have increased hunger level make wolves more desperate; less likely to attack in packs, easier to kill; hunger goes down when it kills a sheep 
        TODO: decide if above comment is how things will work
        float hunger;*/


    //if distance is less than attackRadius, wolf will transition to attack
    [SerializeField] 
    float attackRadius;
    [SerializeField]
    float attackTimerCooldown;

    FSM fsm;
    FSM.State _chase, _attack, _evade, _die, _currentState;

    Timer timer;
    Vector3 previousTargetPosition;


    List<Wolf> activeWolves;
    List<Sheep> activeSheep;
    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<PlayerController>().gameObject;
        timer = FindObjectOfType<Timer>();
        speed = type.baseSpeed;
        attackPower = type.baseAttack;

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
        onHitCallback += TakeDamage;
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

    //chase player or sheep to attack
    void FSM_Chase(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {
            _currentState = _chase;
            _agent.speed = speed;
            if (GameManager.instance.debugAll || GameManager.instance.debugFSM)
            {
                Debug.Log("wolf enter chase state");
            }

            // todo: add chance to target player?
            if (packFollower)
            {
                target = null;
                Wolf targetWolfScript = null;
                int safetyLockBreak = 40; // 40 tries to find a non follower wolf, should be ample
                packChaseRandomFormationOffset = Random.insideUnitCircle;
                packChaseRandomFormationOffset.Normalize();
                packChaseRandomFormationOffset *= Random.RandomRange(2f,6f); // how far is pack formation?
                packChaseRandomFormationOffset.y -= Random.RandomRange(2f, 6f); // shift the circle behind the leader
                while (target == null || targetWolfScript == this || targetWolfScript.packFollower)
                {
                    //TODO: Physics.SphereOverlap to check if wolf is close enough to bother following/using pack behaviour
                    target = activeWolves[Random.Range(0, activeWolves.Count)].gameObject;
                    targetWolfScript = target.GetComponent<Wolf>();
                    if(safetyLockBreak-- < 0) // prevent infinite loop from bad dice rolls or few wolves left
                    {
                        target = player;
                        packFollower = false; // couldn't find another wolf to follow, give up
                        break;
                    }
                }
            } else if (activeSheep.Count > 0)
            {
                //TODO: Switch to closest sheep
                // note: there was a  - 1 here, but Random.Range already excludes the higher end, so no -1 :) -chris
                target = activeSheep[Random.Range(0, activeSheep.Count)].gameObject;
            }
            else
            {
                target = player;
            }

            previousTargetPosition = target.transform.position;

        }
        if (step == FSM.Step.Update)
        {
            if (target != null)
            {
                Vector3 targetPos = target.transform.position;
                if (packFollower)
                {
                    // pack formation offset relative to leader orientation
                    targetPos += packChaseRandomFormationOffset.x * target.transform.right +
                                    packChaseRandomFormationOffset.y * target.transform.forward;
                }
                if (!_agent.hasPath || targetPos != previousTargetPosition)
                {
                    
                    _agent.SetDestination(targetPos);
                    NavMeshPath path = new();
                    if (NavMesh.CalculatePath(transform.position, targetPos, 0, path))
                    {
                        _agent.SetPath(path);
                    }
                }
                if (Vector3.Distance(transform.position, target.transform.position) <= attackRadius)
                {
                    fsm.TransitionTo(_attack);
                }
            }

        }
        if (step == FSM.Step.Exit)
        {
            _agent.ResetPath();
        }

    }
    //state wolf must be in to cause damage to player or sheep
    void FSM_Attack(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {
            _currentState = _attack;
            if (GameManager.instance.debugAll || GameManager.instance.debugFSM)
            {
                Debug.Log("wolf enter attack state");
            }
            
            //will be switching to event 
            if (target == null)
            {
                fsm.TransitionTo(_chase);
            }
        }
        if (step == FSM.Step.Update)
        {

            if (target != null)
            {
                if (!timer.wolfCooldownTimerActive)
                {
                    if (!timer.wolfCooldownTimerActive)
                    {
                        target.GetComponent<Character>().TakeDamage(this.gameObject, attackPower);
                        StartCoroutine(timer.CooldownTimer(attackTimerCooldown, name));
                        fsm.TransitionTo(_chase);
                    }
                    //add code to play attack animation
                }

                if (Vector3.Distance(transform.position, target.transform.position) > attackRadius)
                {
                    fsm.TransitionTo(_chase);
                }
            }
            else
            {
                fsm.TransitionTo(_chase);
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
            if (GameManager.instance.debugAll || GameManager.instance.debugFSM)
            {
                Debug.Log("wolf died");
            }
            Destroy(gameObject);
            //playAnimation
            //playSound
        }
        if (step == FSM.Step.Update)
        {
            //if animation is done playing, transition to exit 
        }
        if (step == FSM.Step.Exit)
        {
/*            Debug.Log($"{this.name} died!");
            Destroy(gameObject);*/
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
    public override void TakeDamage(GameObject weapon, float damage)
    {
        //when the player presses 'I' they take 5 damage
        currentHealth -= damage;
        //this updates the Slider value of Current Health / Max Health
        uiHealthValue.value = currentHealth / maxHealth;

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

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            fsm.TransitionTo(_die);
        }
    }

    void OnDestroy()
    {
        if (Application.isPlaying)
        {
            GameManager.instance.UpdateActiveWolves();
        }
    }
}
