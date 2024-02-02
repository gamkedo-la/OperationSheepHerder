using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class DitherMeshBlockingCamera : MonoBehaviour
{
    [SerializeField]
    PlayerController playerController;

    [SerializeField]
    Camera main;

    [SerializeField]
    LayerMask obstructionLayerMask;

    [SerializeField]
    float dither = 0.5f;

    float noDither = 1f;
    InputActionAsset actions;
    InputAction leftRight;
    InputAction upDown;

    Transform player;

    float timer;
    float checkInterval = 0.2f;
    bool check;

    List<GameObject> ditheredTrees;

    MaterialPropertyBlock propBlock;

    private void Awake()
    {
        propBlock = new MaterialPropertyBlock();
        ditheredTrees = new List<GameObject>();
    }
    private void OnEnable()
    {
        player = playerController.transform;

        actions = playerController.GetComponent<PlayerInput>().actions;
        leftRight = GetComponent<CameraFollow>().leftRight;
        upDown = GetComponent<CameraFollow>().upDown;

        if (!actions.enabled)
        {
            actions.Enable();
        }

        actions.FindAction("Move", true).performed += _ => CheckForObstacles();
        leftRight.performed += _ => CheckForObstacles();
        upDown.performed += _ => CheckForObstacles();
    }

    private void Update()
    {
        if (check == false)
        {
            timer += Time.deltaTime;
        }
        if (timer >= checkInterval)
        {
            check = true;
            CheckForObstacles();
        }
    }

    public void CheckForObstacles()
    {
        Debug.Log("checking for obstacles");
        Vector3 direction = player.position - main.transform.position; 
        //if player is within the cameras field of view, raycast to check for something blocking view of player
        if (Vector3.Angle(main.transform.forward, direction) <= main.fieldOfView)
        {
            RaycastHit[] hits = Physics.RaycastAll(main.transform.position, direction, direction.magnitude, obstructionLayerMask);
            AdjustDither(hits);
        }

        timer = 0;
        check = false;
    }

    void AdjustDither(RaycastHit[] hits)
    {
        List<GameObject> currentHits = new();

        List<GameObject> treesToRemove = new();
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Tree"))
            {
                if (GameManager.instance.debugAll)
                {
                    Debug.Log("Tree obstructing view");
                }
                GameObject tree = hit.transform.gameObject;
                currentHits.Add(tree);

                if (!ditheredTrees.Contains(tree))
                {
                    SetDither(tree, dither);
                    ditheredTrees.Add(tree);
                }
            }
        }
        if (ditheredTrees.Count > 0)
        {
            foreach (GameObject tree in ditheredTrees)
            {
                if (!currentHits.Contains(tree))
                {
                    SetDither(tree, noDither);
                    treesToRemove.Add(tree);
                }
            }
        }
        foreach (GameObject tree in treesToRemove)
        {
            ditheredTrees.Remove(tree);
        }
    }

    void SetDither(GameObject tree, float dither)
    {
        MeshRenderer renderer = tree.GetComponent<MeshRenderer>();
        renderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_Dither", dither);
        renderer.SetPropertyBlock(propBlock);
    }

    private void OnDisable()
    {
        if (actions.enabled)
        {
            actions.FindAction("Move", true).performed -= _ => CheckForObstacles();
            actions.Disable();
        }

        if (leftRight.enabled)
        {
            leftRight.performed -= _ => CheckForObstacles();
            leftRight.Disable();
        }

        if (upDown.enabled)
        {
            upDown.performed -= _ => CheckForObstacles();
            upDown.Disable();
        }
    }
}
