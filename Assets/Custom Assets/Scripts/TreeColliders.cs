using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class TreeColliders : MonoBehaviour
{
    [SerializeField] 
    Mesh mesh;
    Terrain terrain;
    

    void Awake()
    {
        terrain = FindObjectOfType<Terrain>();
    }
    void Start()
    {
        PlaceTreeColliders();
    }
    void PlaceTreeColliders()
    {
        TreeInstance[] trees = terrain.terrainData.treeInstances;
        TerrainData tData = terrain.terrainData;
        GameObject colliderParent = new GameObject("ColliderParent");
            for (int i = 0; i < trees.Length - 1; i++)
            {
                GameObject colliderObject = new GameObject("colliderObject")
                {
                    layer = 11
                };
                Vector3 colliderPosition = Vector3.Scale(trees[i].position, tData.size);
                colliderObject.transform.parent = colliderParent.transform;
                colliderObject.transform.position = colliderPosition;
                MeshCollider meshCollider = colliderObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = mesh;
                meshCollider.convex = true;
                meshCollider.transform.position = new Vector3(colliderPosition.x, tData.GetTreeInstance(i).position.y, colliderPosition.z);
                colliderObject.AddComponent<NavMeshObstacle>();
                colliderObject.GetComponent<NavMeshObstacle>().carving = true;
                colliderObject.transform.hasChanged = false;
            }
        colliderParent.transform.hasChanged = false;
    }
}
