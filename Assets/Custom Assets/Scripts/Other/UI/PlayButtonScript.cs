using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButtonScript : MonoBehaviour
{
    bool alreadyClicked = false;

    public void HandlePlayButtonClick()
    {
        if(alreadyClicked)
        {
            Debug.Log("Play already clicked, prevent staggered loading animation soft lock");
            return;
        }
        alreadyClicked = true; // preventing bad state from repeat clicking

        if (LoadingManager.Instance == null)
        {
            Debug.LogError("LoadingManager instance is null");
            return;
        }

        LoadingManager.Instance.transform.GetChild(0).gameObject.SetActive(true);
        LoadingManager.Instance.LoadScene(GameEnumsNamespace.GameSceneEnums.WoodsDay);
        print("button clicked");
    }
}
