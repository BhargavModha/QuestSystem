using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestEventRewardListener : MonoBehaviour
{
    public QuestEventReward questReward;
    public UnityEvent onGoalComplete;

    void Awake(){
        questReward.rewardTrigger += TriggerEvent;
    }

    void TriggerEvent(){
        onGoalComplete?.Invoke();
    }
}
