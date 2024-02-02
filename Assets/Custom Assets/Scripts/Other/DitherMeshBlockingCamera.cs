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

    private void Awake()
    {
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
        }
    }

    public void CheckForObstacles()
    {
        if (check)
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

        else
        {
            return;
        }
    }

    void AdjustDither(RaycastHit[] hits)
    {

        for (int i = 0; i < hits.Length; i++)
        {

            if (hits[i].collider.CompareTag("Tree"))
            {
                if (GameManager.instance.debugAll)
                {
                    Debug.Log("Tree obstructing view");
                }
                GameObject tree = hits[i].transform.gameObject;
                if (!ditheredTrees.Contains(tree))
                {
                    Material barkMat = hits[i].collider.gameObject.GetComponent<MeshRenderer>().material;
                    Material leafMat = hits[i].collider.gameObject.GetComponent<MeshRenderer>().materials[1];


                    barkMat.SetFloat("_Dither", dither);
                    leafMat.SetFloat("_Dither", dither);
                    ditheredTrees.Add(tree);
                }


            }
        }
        if (ditheredTrees.Count > 0)
        {
            
            List<bool> shouldBeDithered = new();

            //each tree in ditheredTrees should be dithered, so we start with true values
            for (int i = 0; i < ditheredTrees.Count; i++)
            {
                shouldBeDithered.Add(true);
            }

            for (int j = 0; j < ditheredTrees.Count; j++)
            {
                for (int k = 0; k < hits.Length - 1; k++)
                {
                    //if the given dithered tree doesn't equal any of the hit colliders, it shouldn't be dithered anymore
                    if (!ditheredTrees[j].Equals(hits[k].collider.gameObject))
                    {
                        shouldBeDithered[j] = false;
                    }
                }
                Material barkMat = ditheredTrees[j].GetComponent<MeshRenderer>().material;
                Material leafMat = ditheredTrees[j].GetComponent<MeshRenderer>().materials[1];

                if (!shouldBeDithered[j])
                {
                    barkMat.SetFloat("_Dither", noDither);
                    leafMat.SetFloat("_Dither", noDither);
                }
            }
        }
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
