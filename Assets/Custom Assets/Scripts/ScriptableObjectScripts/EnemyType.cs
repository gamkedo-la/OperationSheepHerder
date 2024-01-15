using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu()]
public class EnemyType : ScriptableObject
{
    //TODO: transition hitsToDefeat to health bar
    public float baseSpeed;
    public GameObject prefab;
    public float baseAttack;
}
