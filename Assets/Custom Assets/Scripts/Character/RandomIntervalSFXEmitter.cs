using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomIntervalSFXEmitter : MonoBehaviour
{
    //Fine-tune min & max TimeBetweenClips in inspector to balance audio clip frequency across all characters in scene
    [SerializeField]
    float minTimeBetweenClips = 5f;
    [SerializeField]
    float maxTimeBetweenClips = 20f;

    float maxDistanceFromPlayer = 30f;

    float offset;
    //TODO: if there's time, can implement character having more than one possible audio clip to play
/*  [SerializeField]
    AudioClip[] clips;

    [SerializeField]
    int preferredClipIndex;

    [SerializeField]
    float preferredClipWeight;
    */


    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        offset = UnityEngine.Random.Range(1, 10);
        StartCoroutine(nameof(PlayClipAtRandomIntervals));
    }
    IEnumerator PlayClipAtRandomIntervals()
    {

        while (gameObject.activeSelf)
        {
            if (Mathf.Abs(Vector3.Distance(transform.position, GameManager.instance.player.transform.position)) < maxDistanceFromPlayer)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(minTimeBetweenClips + offset, maxTimeBetweenClips + offset));
                audioSource.Play();
                offset = UnityEngine.Random.Range(1, 5);
            }

            yield return null;
        }

    }
}
