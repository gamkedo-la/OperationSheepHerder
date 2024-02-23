using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu()]
public class EnemyType : ScriptableObject
{
    public string type;
    public float baseSpeed;
    public GameObject prefab;
    public float baseAttack;
}
