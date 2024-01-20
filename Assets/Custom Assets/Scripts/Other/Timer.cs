using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

//cooldown timer for attacks, sheep flee
public class Timer : MonoBehaviour
{
    public bool wolfCooldownTimerActive;
    public bool playerCooldownTimerActive;
    public bool followBellTimerActive;
    public bool isFleeing;

    public GameEvent OnRingBell;
    public GameEvent OnRingBellEnded;

    //TODO: change timer to static class, invoke event to let relevant scripts know when timer is over
    public IEnumerator CooldownTimer(float cooldownTime, string caller, [CallerMemberName] string callingMethod = "")
    {
        float timer = 0;
        if (caller.Contains("Wolf"))
        {
            wolfCooldownTimerActive = true;
        }
        else if (caller == "Player")
        {
            if (callingMethod == "OnRingBell")
            {
                OnRingBell.Raise();
                followBellTimerActive = true;
            }
            else
            {
                playerCooldownTimerActive = true;
            }
        }
        while (timer <= cooldownTime)
        {
            timer += Time.deltaTime;
            yield return null;//new WaitForSeconds(0.25f);
        }
        if (GameManager.instance.debugAll)
        {
            Debug.Log("timer over");
        }
        wolfCooldownTimerActive = false;
        playerCooldownTimerActive = false;
        followBellTimerActive = false;
        if(OnRingBellEnded)
        {
            //OnRingBellEnded.Raise();
        } else
        {
            if (GameManager.instance.debugAll)
            {
                Debug.Log("OnRingBellEnded null, avoided error on .Raise() call");
            }
        }
        yield break;
    }

    //timer for how long sheep flees for after being attacked
    public IEnumerator FleeTimer(float fleeTimerEnd)
    {
        float fleeTimer = 0;
        isFleeing = true;

        while (fleeTimer < fleeTimerEnd)
        {
            fleeTimer += Time.deltaTime;
            yield return null;
        }
        isFleeing = false;
        yield break;
    }
}
