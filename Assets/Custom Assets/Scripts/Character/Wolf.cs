using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using UnityEngine.UI;
using Unity.AI.Navigation;
using UnityEngine.Android;
using Unity.VisualScripting;

public class Wolf : Enemy
{
    //if distance to target is less than chase radius, transition to chase
    [SerializeField] 
    float chaseRadius;

    // if turned to true, will look for another wolf to follow instead of the player or sheep
    [SerializeField]
    bool packFollower; // if something happens to the leader or they need to split, just flip false
    Vector2 packChaseRandomFormationOffset; // to avoid bunching up
    [SerializeField]
    GameObject leader;
    //if distance is less than attackRadius, wolf will transition to attack
    [SerializeField] 
    float attackRadius;
    //time in seconds wolf must wait before able to attack again - to avoid nonstop attacking
    [SerializeField]
    float attackTimerCooldown;

    FSM fsm;
    FSM.State _chase, _attack, _die;

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
        _die = FSM_Die;
    }

    void OnEnable()
    {
        if (GameManager.instance != null && !GameManager.instance.activeEnemies.Contains(this))
        {
            GameManager.instance.UpdateActiveEnemies();
        }
        onHitCallback += TakeDamage;
    }
    void Start()
    {
        GameManager.instance.onUpdateEnemiesCallback += UpdateWolves;
        GameManager.instance.onUpdateSheepCallback += UpdateSheep;
        activeWolves = GameManager.instance.activeEnemies.FindAll(enemy => enemy.name.Contains("Wolf"));
        activeSheep = GameManager.instance.activeSheep;
        fsm.OnSpawn(_chase);
        _agent.speed = speed;
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
            if (GameManager.instance.debugAll || GameManager.instance.debugFSM)
            {
                Debug.Log("wolf enter chase state");
            }

            if (target == null || target.IsDestroyed())
            {
                if (packFollower)
                {
                    Wolf targetWolfScript = null;
                    int safetyLockBreak = 40; // 40 tries to find a non follower wolf, should be ample
                    packChaseRandomFormationOffset = Random.insideUnitCircle;
                    packChaseRandomFormationOffset.Normalize();
                    packChaseRandomFormationOffset *= Random.Range(2f, 6f); // how far is pack formation?
                    packChaseRandomFormationOffset.y -= Random.Range(2f, 6f); // shift the circle behind the leader
                    while (target == null || targetWolfScript == this || targetWolfScript.packFollower)
                    {
                        //TODO: Physics.SphereOverlap to check if wolf is close enough to bother following/using pack behaviour
                        leader = activeWolves[Random.Range(0, activeWolves.Count)].gameObject;
                        if (Vector3.Distance(transform.position, leader.transform.position) > 15)
                        {
                            leader = activeWolves[Random.Range(0, activeWolves.Count)].gameObject;
                        }
                        target = leader;
                        targetWolfScript = leader.GetComponent<Wolf>();
                        if (safetyLockBreak-- < 0) // prevent infinite loop from bad dice rolls or few wolves left
                        {
                            target = player;
                            packFollower = false; // couldn't find another wolf to follow, give up
                            break;
                        }

                    }
                }
                else if (activeSheep.Count > 0)
                {
                    //TODO: Switch to closest sheep
                    target = activeSheep[Random.Range(0, activeSheep.Count)].gameObject;
                }
                else
                {
                    target = player;
                }
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
                    if (Vector3.Distance(leader.transform.position, leader.GetComponent<Wolf>().target.transform.position) < 5)
                    {
                        target = leader.GetComponent<Wolf>().target;
                        targetPos = target.transform.position;
                    }

                    else
                    {
                        // pack formation offset relative to leader orientation
                        targetPos = leader.transform.position;
                        targetPos += packChaseRandomFormationOffset.x * target.transform.right +
                                        packChaseRandomFormationOffset.y * target.transform.forward;
                    }

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
            else
            {
                fsm.TransitionTo(_chase);
            }

        }
        if (step == FSM.Step.Exit)
        {
            _agent.ResetPath();
        }

    }
    //state wolf must be in to cause damage to player, dog or sheep
    void FSM_Attack(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {
            if (GameManager.instance.debugAll || GameManager.instance.debugFSM)
            {
                Debug.Log("wolf enter attack state");
            }
            
            if (target == null)
            {
                fsm.TransitionTo(_chase);
            }

            //TODO: Test fix
            else if (target.CompareTag("Enemy") && target.name.Contains("Wolf"))
            {
                if (target.GetComponent<Wolf>().target.CompareTag("Sheep"))
                {
                    target = target.GetComponent<Wolf>().target;
                }
            }
        }
        if (step == FSM.Step.Update)
        {

            if (target != null)
            {
                if (Vector3.Distance(transform.position, target.transform.position) > attackRadius)
                {
                    fsm.TransitionTo(_chase);
                }

                else
                {
                    if (!cooldownTimerActive)
                    {

                        target.GetComponent<Character>().TakeDamage(attackPower, null, this.gameObject);
                        StartCoroutine(timer.CooldownTimer(attackTimerCooldown, this));
                        fsm.TransitionTo(_chase);
                    }
                    //add code to play attack animation
                }
            }

            else
            {
                fsm.TransitionTo(_chase);
            }

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
        activeWolves = GameManager.instance.activeEnemies;
    }
    //called when GameManager.instance.activeSheep changes
    void UpdateSheep()
    {
        activeSheep = GameManager.instance.activeSheep;
    }
    public override void TakeDamage(float damage, Weapon weapon = null, GameObject enemy = null)
    {

        if (enemy.CompareTag("Dog"))
        {
            target = enemy;
            fsm.TransitionTo(_chase);
        }
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
            GameManager.instance.UpdateActiveEnemies();
        }
    }
}
