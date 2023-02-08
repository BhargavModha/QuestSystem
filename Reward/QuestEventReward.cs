using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(fileName = "NewEventReward", menuName = "Quests/New Event Reward")]
public class QuestEventReward : QuestReward
{
    [HideInInspector] public RewardType rewardType = QuestReward.RewardType.CustomEvent;

    public System.Action rewardTrigger;

    public override RewardType GetRewardType(){
        return rewardType;
    }

    public override void ActivateReward(){
        rewardTrigger?.Invoke();
    }

}
