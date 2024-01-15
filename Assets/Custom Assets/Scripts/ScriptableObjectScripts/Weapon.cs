using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AttackPowerSO")]
public class AttackPowerSO : ScriptableObject
{
    [SerializeField] float _value;
    public float Value { get { return _value; } set { _value = value; } }
}
