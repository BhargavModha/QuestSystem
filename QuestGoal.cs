using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[System.Serializable]
public class QuestGoal
{
    public enum RewardType{
        DoNothing,
        StartDialogue,
        // OpenCompendium
    }


    [Header("Info")] 
    public string goalName;
    [TextArea]
    public string goalDescription;

    public bool isGoalActive;
    public bool isGoalCompleted;

    [Header("Parameter")] 
    public QuestParameters questParameters;

    [Header("Reward (Optional)")]
    public List<QuestReward> questRewards;

    [Header("Optional Features")] 
    public bool debugSkip;
    public bool hideInBook;

    
    public void Reset() {
        isGoalActive = false;
        isGoalCompleted = false;

        if(questParameters == null){
            Debug.Log(goalName + " Has NO parameter!");
        }
        questParameters.Reset();
    }

    public void GoalComplete(){
        isGoalCompleted = true;
        isGoalActive = false;

        GiveReward();
    }

    public void GiveReward(){
        foreach(QuestReward reward in questRewards){
            reward.ActivateReward();
        }
    }

    public void GoalSkip(){

        isGoalCompleted = true;
        isGoalActive = false;

        foreach(QuestReward reward in questRewards){
            if(reward.GetRewardType() == QuestReward.RewardType.CustomEvent){
                Debug.Log(goalName + " Skip Event Reward Activated");
                reward.ActivateReward();
            }
        }
    }

    // Function overloading to faciliate differnt type (add in QuestParamters.cs)

    public void CheckGoalCondition(Item newItem){
        if (questParameters.CheckCondition(newItem) == true){
            GoalComplete();
        }
    }

    public void CheckGoalCondition(NPC npc){
        if (questParameters.CheckCondition(npc) == true){
            GoalComplete();
        }
    }

    public void CheckGoalCondition(string trigger){
        if (questParameters.CheckCondition(trigger) == true){
            GoalComplete();
        }
    }


    // Check goal conditions
    public void CheckGoalConditionOnStart(){
        if (questParameters.CheckConditionOnStart() == true){
            GoalComplete();
        }
    }

}
