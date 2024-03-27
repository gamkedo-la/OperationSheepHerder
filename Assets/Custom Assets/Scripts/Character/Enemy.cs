using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

//TODO: Re-work enemy structure
public abstract class Enemy : Character
{
    public EnemyType type;

    public GameObject target = null;

}
