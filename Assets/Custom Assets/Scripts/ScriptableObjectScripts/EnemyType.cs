using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu()]
public class EnemyType : ScriptableObject
{
    //TODO: transition hitsToDefeat to health bar
    public HitsToDefeat hitsToDefeat;
    public SpeedSO baseSpeed;
    public GameObject prefab;
}
