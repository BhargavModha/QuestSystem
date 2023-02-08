using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTalkParam", menuName = "Quests/New Talk Parameter")]
public class QuestTalkParameters : QuestParameters
{
    [HideInInspector] public ParameterType parameterType = QuestParameters.ParameterType.Talk;
    public string npcName;
    public string yarnNode = null;

    public override ParameterType GetParameterType(){
        return parameterType;
    }

    public override void Reset(){
        // Do nothing
    }

    public override bool CheckCondition(NPC npc){

        if(npcName == npc.name && yarnNode!=null && yarnNode!=""){
            StoryProgress.currentStory.StartDialogue(yarnNode);
            return true;
        }

        return false;

    }

    public override bool CheckConditionOnStart(){
        return false;
    }


}
