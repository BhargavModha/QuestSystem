using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestReward : ScriptableObject
{
    public enum RewardType{
        StartDialogue,
        CustomEvent,
    }

    public abstract RewardType GetRewardType(); 

    public virtual void ActivateReward(){
        Debug.Log("ActivateReward() not overriden");
    }

}
