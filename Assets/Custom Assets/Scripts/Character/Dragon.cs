using System.Collections.Generic;
using UnityEngine;

public class Dragon : Enemy
{
    /*
     *  Shoots poison gas? OR fire? from nose to attack
     *      -affects all sheep within certain range
     *  After being attacked, flies up to avoid close range attacks while still attacking sheep
     *      -can't stay up long so must land after a little while
     *  Die when health drops to 0
    */
    [SerializeField]
    AudioClip FireSFX;
    [SerializeField] 
    ParticleSystem FireVFX;
    [SerializeField]
    float attackRadius;
    [SerializeField]
    float attackTimerCooldown;
    [SerializeField]
    Timer timer;

    AudioSource audioSource;
    float wakeUpRadius = 40f;
    FSM fsm;
    FSM.State _wait, _chase, _attack, _die;

    List<Sheep> activeSheep;


    void OnEnable()
    {
        GameManager.instance.onUpdateSheepCallback += UpdateSheep;
        onHitCallback += TakeDamage;
    }

    private void Awake()
    {
        fsm = new();
        _wait = FSM_Wait;
        _chase = FSM_Chase;
        _attack = FSM_Attack;
        _die = FSM_Die;
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        activeSheep = GameManager.instance.activeSheep;
        fsm.OnSpawn(_wait);
        transform.LookAt(player.transform.position);

    }

    private void Update()
    {
        fsm.OnUpdate();
        if (uiHealthObject.activeSelf)
        {
            uiHealthObject.transform.LookAt(Camera.main.transform.position);
        }
    }

    void FSM_Wait(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {

        }

        if (step == FSM.Step.Update)
        {

            if (Vector3.Distance(player.transform.position, transform.position) < wakeUpRadius)
            {
                fsm.TransitionTo(_chase);
            }

            for (int i = 0; i < GameManager.instance.activeSheep.Count; i++)
            {
                if (Vector3.Distance(transform.position, GameManager.instance.activeSheep[i].transform.position) < attackRadius)
                {
                    target = GameManager.instance.activeSheep[i].gameObject;
                    fsm.TransitionTo(_attack);
                }
            }
        }

        if (step == FSM.Step.Exit)
        {

        }
    }

    void FSM_Chase(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {
            if (!target)
            {
                //index negative until closest is found
                int indexOfClosestSheep = -1;
                //start max distance at random larger number, all sheep should be closer than this
                float maxDistance = 100;
                for (int i = 0; i < GameManager.instance.activeSheep.Count; i++)
                {
                    float distance = Vector3.Distance(transform.position, GameManager.instance.activeSheep[i].transform.position);
                    if (distance < maxDistance)
                    {
                        //index only updated if that sheep is closer than a previous one
                        indexOfClosestSheep = i;
                        maxDistance = distance;
                    }
                }
                target = GameManager.instance.activeSheep[indexOfClosestSheep].gameObject;
            }
        }

        if (step == FSM.Step.Update)
        {
            if (target)
            {
                _agent.SetDestination(target.transform.position);

                if (Vector3.Distance(transform.position, target.transform.position) < attackRadius)
                {
                    fsm.TransitionTo(_attack);
                }
            }
            else
            {
                fsm.TransitionTo(_wait);
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
            if (GameManager.instance.debugAll)
            {
                Debug.Log("Play FireSound & VFX");
            }
            //emmit fire from FireVFX
            FireVFX.Play();
            audioSource.Play();
        }

        if (step == FSM.Step.Update)
        {
            if (target)
            {
                if (Vector3.Distance(transform.position, target.transform.position) > attackRadius)
                {
                    fsm.TransitionTo(_chase);
                }

                transform.LookAt(target.transform.position);
                if (!cooldownTimerActive && !GameManager.instance.win && !GameManager.instance.gameOver)
                {
                    target.GetComponent<Character>().TakeDamage(attackPower, null, this.gameObject);
                    StartCoroutine(timer.CooldownTimer(attackTimerCooldown, this));
                    fsm.TransitionTo(_chase);
                }
            }
            else
            {
                fsm.TransitionTo(_wait);
            }
        }

        if (step == FSM.Step.Exit)
        {

        }
    }

    void FSM_Die(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {
            if (GameManager.instance.debugAll || GameManager.instance.debugFSM)
            {
                Debug.Log("dragon died");
            }
            Destroy(gameObject);
            //TODO: Create poof particles effect to play when any character dies
        }

        if (step == FSM.Step.Update)
        {

        }

        if (step == FSM.Step.Exit)
        {

        }
    }

    void UpdateSheep()
    {
        activeSheep = GameManager.instance.activeSheep;
    }

    public override void TakeDamage(float damage, Weapon weapon = null, GameObject enemy = null)
    {
        currentHealth -= damage;

        fsm.TransitionTo(_chase);

        uiHealthValue.value = currentHealth / maxHealth;

        if (uiHealthObject)
        {
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
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            fsm.TransitionTo(_die);
        }
    }

    private void OnDisable()
    {
        GameManager.instance.onUpdateSheepCallback -= UpdateSheep;

        onHitCallback -= TakeDamage;
    }
}