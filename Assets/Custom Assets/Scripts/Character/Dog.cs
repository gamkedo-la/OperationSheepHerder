using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
 * -dog follows player with sheep, unless a sheep has strayed then dog will leave player to retrieve sheep
 * -dog is in idle mode when close enough to player and no sheep to herd or defend
 * Attack an enemy that is attacking a sheep
 * Get knocked out for a short time
 *      -when takes too much damage
 */
public class Dog : Character
{
    [SerializeField]
    float attackRadius;
    [SerializeField]
    float attackCooldown;
    [SerializeField]
    AudioClip knockout;

    FSM fsm;
    FSM.State _herd, _follow, _attack, _knockedOut;

    float idleRadius = 4f;
    AudioSource audioSource;

    List<Wolf> activeEnemies;
    List<Sheep> activeSheep;

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
        _follow = FSM_Follow;
    }

    private void Start()
    {
        fsm.OnSpawn(_follow);
        audioSource = GetComponent<AudioSource>();
        GameManager.instance.onUpdateSheepCallback += UpdateSheep;
        GameManager.instance.onUpdateEnemiesCallback += UpdateWolves;
        activeSheep = GameManager.instance.activeSheep;
        activeEnemies = GameManager.instance.activeEnemies;
    }

    void UpdateWolves()
    {
        activeEnemies = GameManager.instance.activeEnemies;
    }
    //called when GameManager.instance.activeSheep changes
    void UpdateSheep()
    {
        activeSheep = GameManager.instance.activeSheep;
    }
    private void FixedUpdate()
    {
        fsm.OnUpdate();
    }

    void FSM_Follow(FSM fsm, FSM.Step step, FSM.State state)
    {
        NavMeshPath path = new();

        if (step == FSM.Step.Enter)
        {
        }

        if (step == FSM.Step.Update)
        {
            _agent.CalculatePath(player.transform.position, path);
            _agent.SetPath(path);
            
            if (activeEnemies != null && activeEnemies.Count > 0) 
            {
                fsm.TransitionTo(_herd);
            }
        }

        if (step == FSM.Step.Exit)
        {

        }
    }

    void FSM_Herd(FSM fsm, FSM.Step step, FSM.State state)
    {

        if(_agent == null)
        {
            Debug.LogWarning("No Dog Agent Assigned");
            return;
        }

        if (!_agent.isOnNavMesh)
        {
            Debug.LogWarning("Agent is not on the nav mesh");
            return;
        }

        if (step == FSM.Step.Enter)
        {

        }

        if (step == FSM.Step.Update)
        {

            Vector3 goToPoint;
            if (GameManager.instance.farthestSheep != -1)
            {
                goToPoint = GameManager.instance.activeSheep[GameManager.instance.farthestSheep].transform.position;
            }
            else
            {
                goToPoint = player.transform.position;
            }
            if (Vector3.Distance(transform.position, goToPoint) <= idleRadius)
            {
                if (GameManager.instance.farthestSheep != -1)
                {
                    GameManager.instance.activeSheep[GameManager.instance.farthestSheep].ReturnToPlayer();
                }
                _agent.isStopped = true;
            }
            else
            {
                _agent.isStopped = false;
                _agent.SetDestination(goToPoint);
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

    void FSM_KnockedOut(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {
            audioSource.clip = knockout;
            audioSource.Play();
        }

        if (step == FSM.Step.Update)
        {

        }

        if (step == FSM.Step.Exit)
        {
            audioSource.clip = null;
        }
    }
    public override void TakeDamage(float damage, Weapon weapon = null, GameObject enemy = null)
    {
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
            fsm.TransitionTo(_knockedOut);
        }
    }

    private void OnDisable()
    {
        onHitCallback -= TakeDamage;
    }
}
