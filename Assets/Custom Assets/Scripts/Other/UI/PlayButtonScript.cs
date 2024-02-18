using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButtonScript : MonoBehaviour
{
    public void HandlePlayButtonClick()
    {
        LoadingManager.Instance.LoadScene(GameEnumsNamespace.GameSceneEnums.WoodsDayLevel1);
    }
}
