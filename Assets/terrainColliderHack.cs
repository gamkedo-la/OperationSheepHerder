using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class terrainColliderHack : MonoBehaviour
{
    void Awake() 
    {
        // some dumb forum post mentioned this might fix the problem
        // with tree colliders not working lolololololololololololol
        GetComponent<TerrainCollider>().enabled = false;
        GetComponent<TerrainCollider>().enabled = true;
    }

}
