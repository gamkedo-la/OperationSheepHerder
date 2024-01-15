using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="WeaponSO")]
public class WeaponSO : ScriptableObject
{
    [SerializeField] 
    string _name;
    public string Name { get { return _name; } set { _name = value; } }

    [SerializeField]
    float _damageValue;

    public float DamageValue { get { return _damageValue; } set { _damageValue = value; } }
}
