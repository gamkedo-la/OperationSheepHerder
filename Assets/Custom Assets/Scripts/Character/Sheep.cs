using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sheep : Character
{
    public GameObject attacker = null;
    public bool safe;

    [SerializeField] float wanderTime;
    [SerializeField] float wanderRadius;
    [SerializeField] float followTime;

    [SerializeField] float fleeTimerEnd;
    [SerializeField] float bellRadius;
    [SerializeField] float fleeSpeed;
    [SerializeField] GameObject dog;
    [SerializeField] GameObject barn;
    [SerializeField] Timer timer;

    FSM fsm;
    FSM.State _follow;
    FSM.State _flee;
    FSM.State _wander;
    FSM.State _die;

    float wanderTimer;
    Vector3 attackerDirection;

    NavMeshAgent agent;
    float safeRadiusWithPlayer = 5f;

    Vector2 followOffset;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        wanderTimer = wanderTime;
        _flee = FSM_Flee;
        _follow = FSM_Follow;
        _wander = FSM_Wander;
        _die = FSM_Die;
        fsm = new FSM();
        fsm.OnSpawn(_follow);
    }

    void Update()
    {
        fsm.OnUpdate();

        if (safe)
        {
            _agent.SetDestination(barn.transform.position);
        }

        if (_agent.velocity.sqrMagnitude > 0.1f)
        {
            _animator.SetBool("IsWalking", true);
        }

        else
        {
            _animator.SetBool("IsWalking", false);
        }

        if (uiHealthObject.activeSelf)
        {
            uiHealthObject.transform.LookAt(Camera.main.transform.position);
        }
    }

    void FSM_Flee(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {
            agent.speed = fleeSpeed;

            if (attacker != null)
            {
                //establish point in opposite direction of attack direction
                attackerDirection = transform.position - attacker.transform.position;
                attackerDirection.y = 0;
            }

            timer.StartCoroutine(timer.FleeTimer(fleeTimerEnd));

            if (GameManager.instance.debugAll || GameManager.instance.debugFSM)
            {
                Debug.Log("sheep entered flee state");
            }


        }
        if (step == FSM.Step.Update)
        {
            //begin movement towards established flee point
            if (attacker != null)
            {
                agent.SetDestination(attacker.transform.position + (attackerDirection * 100));

            }

            if (!timer.isFleeing)
            {
                fsm.TransitionTo(_wander);
            }
        }
        if (step == FSM.Step.Exit)
        {
            attacker = null;
            _agent.speed = speed;
        }
    }
    void FSM_Follow(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {
            if (GameManager.instance.debugAll || GameManager.instance.debugFSM)
            {
                Debug.Log("sheep is following player");
            }

            followOffset = Random.insideUnitCircle;
            followOffset.Normalize();
            followOffset *= Random.Range(4f, 8f);
            followOffset.y -= Random.Range(4f, 8f);

        }
        if (step == FSM.Step.Update)
        {
            //begin movement to player position
            agent.speed = speed;
            agent.SetDestination(player.transform.position + followOffset.x * player.transform.right + followOffset.y * player.transform.forward);
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance <= safeRadiusWithPlayer)
            {
                fsm.TransitionTo(_wander);
            }
            if (attacker != null)
            {
                fsm.TransitionTo(_flee);
            }
            
        }
        if (step == FSM.Step.Exit)
        {

        }

    }
    void FSM_Wander(FSM fsm, FSM.Step step, FSM.State state)
    {

        if (step == FSM.Step.Enter)
        {
            if (GameManager.instance.debugAll || GameManager.instance.debugFSM)
            {
                Debug.Log("sheep is wandering");
            }
            wanderTimer = wanderTime;
        }
        if (step == FSM.Step.Update)
        {
            if (agent != null)
            {
                wanderTimer += Time.deltaTime;
                if (wanderTimer >= wanderTime)
                {
                    Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);

                    agent.speed = speed;
                    agent.SetDestination(newPos);
                    wanderTimer = 0;
                }

                if (Vector3.Distance(transform.position, player.transform.position) < safeRadiusWithPlayer && Vector3.Distance(transform.position, dog.transform.position) < safeRadiusWithPlayer) 
                {
                    fsm.TransitionTo(_follow);
                }

                if (!GameManager.instance.activeEnemies.Find(x => x.GetComponent<Wolf>()))
                {
                    int distantEnemies = 0;
                    for (int i = 0; i < GameManager.instance.activeEnemies.Count; i++)
                    {
                        if (Vector3.Distance(player.transform.position, GameManager.instance.activeEnemies[i].transform.position) > 30f)
                        {
                            distantEnemies++;
                        }
                    }
                    if (distantEnemies == GameManager.instance.activeEnemies.Count)
                    {
                        fsm.TransitionTo(_follow);
                    }
                }
                
            }
        }
        if (step == FSM.Step.Exit)
        {

        }
    }
    public void FSM_Die(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {
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

    public void ReturnToPlayer()
    {
        fsm.TransitionTo(_follow);
    }
    public Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;
        NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);
        return navHit.position;
    }
    void OnDestroy()
    {
        if (Application.isPlaying)
        {
            GameManager.instance.UpdateActiveSheep();
        }
    }

    public override void TakeDamage(float damage, Weapon weapon = null, GameObject enemy = null)
    {
        
        attacker = enemy;
        if (GameManager.instance.debugAll)
        {
            Debug.Log(name + "'s attacker is " + attacker);
        }
        fsm.TransitionTo(_flee);

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
}
