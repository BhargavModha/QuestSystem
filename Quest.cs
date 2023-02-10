using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewQuest", menuName = "Quests/New Quest")]
public class Quest : ScriptableObject {
    
    [Space(5)]
    [Header("Info")] 
    public string questName;
    [TextArea]
    public string questDescription;
    public Sprite icon;
    
    public bool isQuestActive = false;
    public bool isQuestCompleted = false;

    public bool resetOnce;
    

    [Space(5)]
    [Header("Quest Goals")] 
    public List<QuestGoal> allGoals;


    public void Reset(){
        isQuestActive = false;
        isQuestCompleted = false;
        resetOnce = true;
    }

    public void QuestComplete(){
        isQuestActive = false;
        isQuestCompleted = true;
    }

    public void OnGameEnd(){
        resetOnce = false;
    }


}

