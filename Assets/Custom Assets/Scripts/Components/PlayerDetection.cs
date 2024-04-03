using UnityEngine;
using UnityEngine.Events;

public class PlayerDetection : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.instance.NextLevel();
        }
    }
}
