using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Runtime.CompilerServices;
using UnityEngine.AI;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float radius;

    [SerializeField]
    float attackCooldown;

    [SerializeField]
    float bellTimer;

    [SerializeField]
    bool followBell = false;

    [SerializeField]
    Camera mainCamera;



    [SerializeField]
    GameEvent OnRingBellEvent;

    [SerializeField] 
    HitsToDefeat hitsToDefeat;

    [SerializeField] 
    SpeedSO speed;

    [SerializeField] 
    float rotationSpeed;

    [SerializeField] 
    Transform playerInputSpace = default;

    [SerializeField] 
    Rect allowedArea;

    [SerializeField, Min(0f)] 
    float probeDistance = 1f;

    public delegate void Attack();
    public Attack onAttackCallback;
    public readonly Vector3 gravity = new Vector3(0, -3f, 0);

    CharacterController playerController;
    //movement direction 
    Vector3 move;
    Timer timer;
    int stepsSinceLastGrounded;
    bool onGround;
    bool holdingAim = false;
    InputActionAsset playerInput;
    RockTrajectory trajectoryRenderer;

    private void Awake()
    {
        RenderSettings.fog = true;
        Debug.Log("turning on fog, it's off in scene mode");
    }
    void Start()
    {
        playerController = GetComponent<CharacterController>();
        timer = FindObjectOfType<Timer>();
        trajectoryRenderer = GetComponentInChildren<RockTrajectory>();
        playerInput = GetComponent<PlayerInput>().actions;
        playerInput.Enable();
        playerInput.FindAction("LaunchAttack", true).started += _ => StartAiming();
        playerInput.FindAction("LaunchAttack", true).canceled += _ => EndAiming();
    }
    public void OnMove(InputValue input)
    {
        Vector2 inputVec = input.Get<Vector2>();
        if (playerInputSpace)
        {
            Vector3 forward = playerInputSpace.forward;
            forward.y = 0f;
            forward.Normalize();
            Vector3 right = playerInputSpace.right;
            right.y = 0f;
            right.Normalize();
            move = ((forward * inputVec.y + right * inputVec.x));
            move.y = gravity.y * Time.deltaTime * speed.Value;
            move.Normalize();
        }
        else
        {
            Debug.Log("Must assign Player Input Space (which should be the camera following the player).");
        }
    }

    public void StartAiming()
    {
        holdingAim = true;
    }
    public void EndAiming()
    {
        holdingAim = false;
        PlayerAttack();
    }
    private void Update()
    {
        if(holdingAim)
        {
            OnAimRock();
        }   
    }


    //called when attack button is held down, should be repeatedly called like an update function
    public void OnAimRock()
    {
        Debug.Log("aim rock");
        //if arrow keys pressed, move rock trajectory, otherwise launch rock in forward direction
        //TODO: Not working for some reason
        Vector2 input = playerInput.FindAction("AimAttack", true).ReadValue<Vector2>();
        //Vector2 input = playerInput.Player.AimAttack.ReadValue<Vector2>();
        trajectoryRenderer.transform.Rotate(new Vector2(input.x, -input.y) * 1.5f, Space.World);
        trajectoryRenderer.DrawTrajectory();
    }

    //called when attack button is released
    public void PlayerAttack()
    {
        trajectoryRenderer.ClearTrajectory();

        if (timer.playerCooldownTimerActive == false)
        {
            timer.StartCoroutine(timer.CooldownTimer(attackCooldown, "Player"));

            onAttackCallback?.Invoke();
        }
        else
        {
            return;
        }
        
        trajectoryRenderer.transform.localEulerAngles = Vector3.zero;
    }
    public void OnRingBell()
    {
        if (timer.followBellTimerActive == false)
        {
            timer.StartCoroutine(timer.CooldownTimer(bellTimer, "Player"));
            OnRingBellEvent.Raise();
        }
    }
    void FixedUpdate()
    {
        CheckOnGround();
        if (move.x != 0 || move.z != 0)
        {
            transform.Rotate(new Vector3(move.x, 0, move.y) * Time.deltaTime, Space.Self);
            transform.forward = new Vector3(move.x, 0, move.z);

            Vector3 newPosition = transform.localPosition + move;
            if (newPosition.x < allowedArea.xMin)
            {
                newPosition.x = allowedArea.xMin;
                move.x = 0f;
            }
            else if (newPosition.x > allowedArea.xMax)
            {
                newPosition.x = allowedArea.xMax;
                move.x = 0f;
            }
            if (newPosition.z < allowedArea.yMin)
            {
                newPosition.z = allowedArea.yMin;
                move.z = 0f;
            }
            else if (newPosition.z > allowedArea.yMax)
            {
                newPosition.z = allowedArea.yMax;
                move.z = 0f;
            }
        }
        playerController.Move(speed.Value * Time.deltaTime * move);
    }
    void CheckOnGround()
    {
        stepsSinceLastGrounded += 1;
        if (onGround || SnapToGround())
        {
            stepsSinceLastGrounded = 0;
        }
    }
    bool SnapToGround()
    {
        if (stepsSinceLastGrounded <= 1)
        {
            return false;
        }
        if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, probeDistance))
        {
            Debug.Log("no hit collider within probe distance");
            return false;
        }
        if (!hit.collider.gameObject.CompareTag("Ground"))
        {
            Debug.Log("hit object not ground");
            return false;
        }
        float dot = Vector3.Dot(move, hit.normal);
        if (dot > 0f)
        {
            move = (move - hit.normal * dot).normalized * speed.Value;
        }
        return true;
    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            onGround = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            onGround = false;
        }
    }
}
