using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTriggerParam", menuName = "Quests/New Trigger Parameter")]
public class QuestTriggerParameters : QuestParameters
{

    [HideInInspector] public ParameterType parameterType = QuestParameters.ParameterType.Trigger;

    public string triggerToCheck;

    public override ParameterType GetParameterType(){
        return parameterType;
    }

    public override void Reset(){
        // Do nothing
    }

    public override bool CheckCondition(string triggerName){

        if(triggerToCheck != "" && triggerToCheck != null){
            if(triggerName == triggerToCheck){
                return true;
            }
        }
        return false;
    }

    public override bool CheckConditionOnStart(){
        return false;
    }
}
