using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestParameters : ScriptableObject
{
    public enum ParameterType{
        Talk,
        GetItem,
        Trigger
    }

    /**
        Each parameter child class should have these functions
        GetParameterType()
        Reset()
        CheckCondition()
        CheckConditionOnStart()
    **/

    public abstract ParameterType GetParameterType();

    public virtual void Reset(){
        Debug.Log("Reset() not overriden");
    }

    // Function overloading to faciliate differnt type (add in QuestGoal.cs)

    public virtual bool CheckCondition(Item item){
        Debug.Log("CheckCondition(Item) not overriden");
        return false;
    }

    public virtual bool CheckCondition(NPC npc){
        Debug.Log("CheckCondition(NPC) not overriden");
        return false;
    }

     public virtual bool CheckCondition(string trigger){
        Debug.Log("CheckCondition(string) not overriden");
        return false;
    }



    public virtual bool CheckConditionOnStart(){
        Debug.Log("CheckConditionOnStart() not overriden");
        return false;
    }

}
