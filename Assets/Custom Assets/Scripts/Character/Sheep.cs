using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sheep : Character
{
    public GameObject attacker = null;

    [SerializeField] float wanderTime;
    [SerializeField] float wanderRadius;
    [SerializeField] float followTime;

    [SerializeField] float fleeTimerEnd;
    [SerializeField] bool isFleeing = false;
    [SerializeField] float bellRadius;
    [SerializeField] float baseSpeed;
    [SerializeField] float fleeSpeed;

    FSM fsm;
    FSM.State _follow;
    FSM.State _flee;
    FSM.State _wander;
    FSM.State _die;
    FSM.State _currentState;

    Timer timer;
    float wanderTimer;
    Vector3 attackerDirection;

    NavMeshAgent agent;
    

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = FindObjectOfType<Timer>();
        wanderTimer = wanderTime;
        speed = baseSpeed;
        _flee = FSM_Flee;
        _follow = FSM_Follow;
        _wander = FSM_Wander;
        _die = FSM_Die;
        fsm = new FSM();
        fsm.OnSpawn(_wander);

        
    }

    //TODO: make sheep follow player automatically, need to ring bell to recall wandering sheep...? not sure
    public void OnPlayerBell()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < bellRadius)
        {
            if (_currentState == _wander)
            {
                fsm.TransitionTo(_follow);
            }
        }
    }
    public void OnPlayerBellEnded()
    {
        if (_currentState == _follow)
        {
            fsm.TransitionTo(_wander);
        }
    }
    void FixedUpdate()
    {
        fsm.OnUpdate();
    }

    void FSM_Flee(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {
            _currentState = _flee;
            //keep track of previous hit count in order to restart flee timer if attacked again
            //fleeTimerEnd = Random.Range(3.5f, 6);

            //set speed to flee speed
            //animator.SetBool("isRunning", true);
            speed = fleeSpeed;
            agent.speed = speed;

            //establish point in opposite direction of attack direction
            attackerDirection = transform.position - attacker.transform.position;
            timer.StartCoroutine(timer.FleeTimer(fleeTimerEnd));

            if (GameManager.instance.debugAll || GameManager.instance.debugFSM)
            {
                Debug.Log("sheep entered flee state");
            }


        }
        if (step == FSM.Step.Update)
        {
            //begin movement towards established flee point
            
            agent.SetDestination(attacker.transform.position + (attackerDirection * 10));

            if (!timer.isFleeing)
            {
                fsm.TransitionTo(_wander);
            }
        }
        if (step == FSM.Step.Exit)
        {
            //animator.SetBool("isRunning", false);
            attacker = null;
        }
    }
    void FSM_Follow(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {
            _currentState = _follow;
            if (GameManager.instance.debugAll || GameManager.instance.debugFSM)
            {
                Debug.Log("sheep is following player");
            }
            //animator.SetBool("isWalking", true);
        }
        if (step == FSM.Step.Update)
        {
            //begin movement to player position
            speed = baseSpeed;
            agent.speed = speed;
            agent.SetDestination(player.transform.position);
            if (attacker != null)
            {
                fsm.TransitionTo(_flee);
            }
            
        }
        if (step == FSM.Step.Exit)
        {
            //animator.SetBool("isWalking", false);
            //return to wander after 3-4 seconds, or transition to flee if attacked
        }

    }
    void FSM_Wander(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {

            _currentState = _wander;
                
            if (GameManager.instance.debugAll || GameManager.instance.debugFSM)
            {
                Debug.Log("sheep is wandering");
            }
            wanderTimer = wanderTime;
            //animator.SetBool("isWalking", true);
        }
        if (step == FSM.Step.Update)
        {
            if (agent != null)
            {
                wanderTimer += Time.deltaTime;
                if (wanderTimer >= wanderTime)
                {
                    Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);

                    speed = baseSpeed;
                    agent.speed = speed;
                    agent.SetDestination(newPos);
                    wanderTimer = 0;
                }
            }
        }
        if (step == FSM.Step.Exit)
        {
            //animator.SetBool("isWalking", false);
        }
    }
    public void FSM_Die(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {
            _currentState = _die;

            if (GameManager.instance.debugAll || GameManager.instance.debugFSM)
            {
                Debug.Log("sheep died");
            }
            Destroy(gameObject);

        }
        if (step == FSM.Step.Update)
        {
            //
        }
        if (step == FSM.Step.Exit)
        {

            //
        }
    }
    public Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;
        NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);
        return navHit.position;
    }
    public IEnumerator FaceTarget()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
        yield return null;
    }
    void OnDestroy()
    {
        if (Application.isPlaying)
        {
            GameManager.instance.UpdateActiveSheep();
        }
    }

    public override void TakeDamage(GameObject weapon, float damage)
    {
        
        attacker = weapon;
        Debug.Log(attacker);
        fsm.TransitionTo(_flee);

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
            fsm.TransitionTo(_die);
        }
    }
}
