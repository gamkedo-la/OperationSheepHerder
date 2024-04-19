using UnityEngine;
using UnityEngine.Events;

public class DetectionZone : MonoBehaviour
{
    [SerializeField] UnityEvent onEnter = default, onExit = default;
    int count;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Sheep"))
        {
            other.GetComponent<Sheep>().safe = true;
            count++;
            if (GameManager.instance.debugAll)
            {
                Debug.Log(count);
            }
            if (count >= GameManager.instance.activeSheep.Count)
            {
                onEnter?.Invoke();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (onExit != null)
        {
            count--;
            onExit.Invoke();
        }
    }
}
