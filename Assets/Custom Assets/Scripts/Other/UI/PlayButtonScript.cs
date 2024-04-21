using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButtonScript : MonoBehaviour
{
    public void HandlePlayButtonClick()
    {
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
