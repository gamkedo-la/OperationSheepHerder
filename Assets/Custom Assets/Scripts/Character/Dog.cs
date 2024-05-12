using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

/*
 * -dog follows player with sheep, unless a sheep has strayed then dog will leave player to retrieve sheep
 * -dog is in idle mode when close enough to player and no sheep to herd or defend
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
    FSM.State _herd, _follow, _knockedOut;

    float idleRadius = 4f;
    AudioSource audioSource;

    bool saveSheep;

    List<Enemy> activeEnemies;

    private void OnEnable()
    {
        onHitCallback += TakeDamage;
    }

    private void Awake()
    {
        fsm = new FSM();
        _herd = FSM_Herd;
        _knockedOut = FSM_KnockedOut;
        _follow = FSM_Follow;
    }

    private void Start()
    {
        fsm.OnSpawn(_follow);
        audioSource = GetComponent<AudioSource>();
        GameManager.instance.onUpdateEnemiesCallback += UpdateEnemies;
        activeEnemies = GameManager.instance.activeEnemies;
    }

    void UpdateEnemies()
    {
        activeEnemies = GameManager.instance.activeEnemies;
    }

    private void FixedUpdate()
    {
        fsm.OnUpdate();

        // Check if the agent is moving
        _animator.SetBool("isMoving", IsMoving());
    
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
                if (!SceneManager.GetActiveScene().name.Equals("WoodsDay"))
                {
                    fsm.TransitionTo(_herd);
                }
            }
        }

        if (step == FSM.Step.Exit)
        {

        }
    }

    void FSM_Herd(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (_agent == null)
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
            //_animator.SetBool("isHerding", true);
            saveSheep = false;
        }

        if (step == FSM.Step.Update)
        {

            Vector3 goToPoint;

            int sheepToTarget = GameManager.instance.farthestSheep;
            if (sheepToTarget != -1 && sheepToTarget <= GameManager.instance.activeSheep.Count - 1)
            {
                if (!GameManager.instance.activeSheep[sheepToTarget])
                {
                    return;
                }
                else
                {
                    if (!saveSheep)
                    {
                        goToPoint = GameManager.instance.activeSheep[sheepToTarget].transform.position;
                        saveSheep = true;
                    }
                    else
                    {
                        goToPoint = player.transform.position;
                        saveSheep = false;
                    }

                }

            }
            else
            {
                goToPoint = player.transform.position;
                saveSheep = false;
            }

            if (Vector3.Distance(goToPoint, player.transform.position) < 0.5f)
            {
                if (sheepToTarget != -1&& sheepToTarget <= GameManager.instance.activeSheep.Count - 1)
                {
                    goToPoint = GameManager.instance.activeSheep[sheepToTarget].transform.position;
                }
            }
            if (Vector3.Distance(transform.position, goToPoint) <= idleRadius)
            {
                if (sheepToTarget != -1 && GameManager.instance.activeSheep[sheepToTarget])
                {
                    Debug.Log("dog returns sheep");
                    GameManager.instance.activeSheep[sheepToTarget].ReturnToPlayer();
                    saveSheep = false;
                }
            }
            _agent.SetDestination(goToPoint);

        }

        if (step == FSM.Step.Exit)
        {
            //_animator.SetBool("isHerding", false);
        }
    }

    //Didn't get to
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
