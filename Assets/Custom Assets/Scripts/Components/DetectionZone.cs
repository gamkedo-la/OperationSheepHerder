using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System.Collections.Generic;

public class DetectionZone : MonoBehaviour
{
    [SerializeField] UnityEvent onEnter = default, onExit = default;

    private void OnTriggerEnter(Collider other)
    {
        //to test if it's working, accept any collider
        onEnter?.Invoke();
    }
    private void OnTriggerExit(Collider other)
    {
        if (onExit != null)
        {
            onExit?.Invoke();
        }
    }
}
