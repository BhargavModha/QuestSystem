using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTalkReward", menuName = "Quests/New Talk Reward")]
public class QuestTalkReward : QuestReward
{
    [HideInInspector] public RewardType rewardType = QuestReward.RewardType.StartDialogue;
    public string yarnNode;

    public override RewardType GetRewardType(){
        return rewardType;
    }

    public override void ActivateReward(){

        if(CustomGameManager.gameManager.isPlayerBusy == false){
            StoryProgress.currentStory.StartDialogue(yarnNode);
        }
        else{
            Debug.Log("Dialogue already running, cannot start node: " + yarnNode);
        }

    }
}
