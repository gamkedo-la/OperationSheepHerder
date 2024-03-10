using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostHover : MonoBehaviour
{
    public Transform childModel;
    Vector3 startPos;
    private void Start()
    {
        startPos = childModel.transform.localPosition;
    }
    private void Update()
    {
        childModel.transform.localPosition = startPos + Vector3.up * 0.25f * Mathf.Cos(Time.timeSinceLevelLoad * 2.0f);
    }
}
