using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.Events;

public class StoryProgress : MonoBehaviour
{
    public static StoryProgress currentStory;

    public Inventory inventory;
    public InventoryWorld customInventory; 

    public QuestMasterlist questMasterlist;

    [HideInInspector] public DialogueRunner dialogueRunner;
    public TabGroup bookTabGroup;


    [HideInInspector] public bool isPlayerNearNPC = false;
    [HideInInspector] public bool isPlayerNearObject = false;
    [HideInInspector] public ItemWorld currentItem;
    [HideInInspector] public CustomItem lastPickedItem;

    public GameObject allTimelines;

    [HideInInspector] public List<Quest> allQuests;
    [HideInInspector] public List<QuestRef> activeGoals;
    [HideInInspector] public NPC currentNPC = null;

    [System.Serializable]
    public struct QuestRef {
        public int questIndex;
        public int goalIndex;
    }

    [System.Serializable]
    public struct StoryEvent {
        public UnityEvent onAwake;
        public UnityEvent onStart;
    }

    public StoryEvent optionalEvents;
   

    void Awake(){

        optionalEvents.onAwake?.Invoke();
        dialogueRunner = GetComponent<DialogueRunner>();
        currentStory = this;

        // Play Custom Cutscene
        dialogueRunner.AddCommandHandler<string>("PlayTimeline", StartTimeline);

        // Call Functions from Yarn Files
        dialogueRunner.AddCommandHandler("OpenBook_Recipe", OpenBookWithRecipe);
        dialogueRunner.AddCommandHandler("OpenBook_Compendium", OpenBookWithCompendium);
        dialogueRunner.AddCommandHandler("EndGame", StartEnd);

    }

    void Start(){

        optionalEvents.onStart?.Invoke();

        allQuests = questMasterlist.allQuests;
        inventory = new Inventory(customInventory);

        ResetQuests();
        ActivateQuest();
        UpdateGoal();
    }

    void OnApplicationQuit(){
        inventory.OnGameEnd();
        ResetQuestOnGameEnd();        
    }

    // ---------------------------------------------------------- 
    // Talking with NPCs 

    public void PlayerIsNear(){
        isPlayerNearNPC = true;
    }

    public void PlayerIsNotNear(){
        isPlayerNearNPC = false;
    }

    public void SetNPC(NPC npc){
        currentNPC = npc;
    }

    public void RemoveTalkPrompts(){
        currentNPC.HideTalkPrompt();
    }

    public void AttemptDialogue(){
        Debug.Log("Attempting OUT");
        if(isPlayerNearNPC==true){
            Debug.Log("Attempting TRUE");
            RemoveTalkPrompts();
            CheckDialogue();
        }
    }

    public void StartDialogue(string yarnNode){
        dialogueRunner.StartDialogue(yarnNode);
    }

    // ---------------------------------------------------------- 
    // Picking up items

    public void SetItem(ItemWorld temp){
        isPlayerNearObject = true;
        currentItem = temp;
    }

    public void RemoveItem(){
        isPlayerNearObject = false;
        currentItem = null;
    }

    // function disabled (initially used keyboard input to call this function but now done automatically in ItemWorld.cs)
    public void AttemptPickUp(){
        // if(isPlayerNearObject==true){
        //     CustomGameManager.gameManager.ItemPickedUp();
        //     currentItem.PickUpItem();
        // }
    }

    public void SetPickedUpItem(CustomItem item){
        lastPickedItem = item;
    }

    public void RemoveItem(CustomItem item){
        Item temp = new Item{customItem = item, amount = 1};
        inventory.RemoveItem(temp);
    }

    // ---------------------------------------------------------- 
    // MISC (needs to be properly implemented)

    public void ActiavteBookFollow(){
        CustomGameManager.gameManager.canOpenBook = true;
    }

    public void CheckUnlocks(){
        CustomGameManager.gameManager.UnlockedRecipe();
    }

    public void StartEnd(){
        CustomGameManager.gameManager.EndingGame();
    }

    // ---------------------------------------------------------- 
    // Compendium
    // Unlocks compendium entry after player picks up an item

    public void UnlockCompendium(CustomItem unlockItem){

        bool alreadyExists = false;

        foreach(CustomItem item in customInventory.discoveredInventory){
            if(item == unlockItem){
                alreadyExists = true;
                break;
            }
        }

        if (alreadyExists == false){
            customInventory.discoveredInventory.Add(unlockItem);
            UI_Book.currentBookUI.PopulateCompendium();
        }

        SetPickedUpItem(unlockItem);
    }

    // ---------------------------------------------------------- 
    // Recipe

    public void UnlockRecipe(Recipe unlockRecipe){

        bool alreadyExists = false;

        foreach(Recipe recipe in customInventory.discoveredRecipes){
            if(recipe == unlockRecipe){
                alreadyExists = true;
                break;
            }
        }

        if (alreadyExists == false){
            customInventory.discoveredRecipes.Add(unlockRecipe);
            UI_Book.currentBookUI.PopulateRecipes();
        }

    }

    // ---------------------------------------------------------- 
    // Quest System 

    // Go through each active quests and its quest goals in "allQuests" and adds the currently active goals to "activeGoals"
    // Function is called after goal complete

    public void UpdateGoal(){

        activeGoals.Clear();
        int completedGoalCount = 0;

        foreach(Quest quest in allQuests){

            if (quest.isQuestActive == true){
                
                bool prevSkip = true;

                foreach (QuestGoal questGoal in quest.allGoals){

                    if(questGoal.debugSkip == true && questGoal.isGoalCompleted == false){

                        if(prevSkip == true){
                            QuestRef temp = new QuestRef();
                            temp.questIndex = allQuests.IndexOf(quest);
                            temp.goalIndex = quest.allGoals.IndexOf(questGoal);
                            
                            questGoal.GoalSkip();
                            ActivateNextGoal_Debug(temp);
                        }
                        else{
                            Debug.Log(questGoal.goalName + " CANNOT be skipped cause previous goal(s) were not skipped");
                        }
                    }

                    if(questGoal.isGoalActive == true){

                        prevSkip = false;

                        QuestRef temp = new QuestRef();
                        temp.questIndex = allQuests.IndexOf(quest);
                        temp.goalIndex = quest.allGoals.IndexOf(questGoal);

                        activeGoals.Add(temp);
                    }

                    if(questGoal.isGoalCompleted == true){
                        completedGoalCount += 1;
                    }
                }

                if (completedGoalCount == quest.allGoals.Count){
                    quest.QuestComplete();
                    ActivateNextQuest(allQuests.IndexOf(quest));
                }
            }
        }

        // CHECK IF GOAL ALREADY COMPLETED
        CheckGoalOnStart();

        // Update Quest UI
        UI_Book.currentBookUI.PopulateQuests();
    }


    public void CheckGoalOnStart(){

        foreach(QuestRef goalRef in activeGoals){

            QuestGoal currentGoal = allQuests[goalRef.questIndex].allGoals[goalRef.goalIndex];
            currentGoal.CheckGoalConditionOnStart();

            if (currentGoal.isGoalCompleted == true){
                ActivateNextGoal(goalRef);
            }

        }

    }

    public void ActivateNextQuest(int completedQuestIndex){
        if(allQuests.Count > completedQuestIndex + 1){
            Quest nextQuest = allQuests[completedQuestIndex + 1];
            nextQuest.isQuestActive = true;
            nextQuest.allGoals[0].isGoalActive = true;
        }
    }

    public void ActivateNextGoal(QuestRef completedGoalRef){
        
        if(allQuests[completedGoalRef.questIndex].allGoals.Count > completedGoalRef.goalIndex+1){
            QuestGoal nextGoal = allQuests[completedGoalRef.questIndex].allGoals[completedGoalRef.goalIndex+1];
            nextGoal.isGoalActive = true;
        }

        // Updates quest UI
        Quest updateQuestUI = allQuests[completedGoalRef.questIndex];
        UI_Book.currentBookUI.UpdateQuestSteps(updateQuestUI);

        UpdateGoal();
    }

    public void ActivateNextGoal_Debug(QuestRef completedGoalRef){
        if(allQuests[completedGoalRef.questIndex].allGoals.Count > completedGoalRef.goalIndex+1){
            QuestGoal nextGoal = allQuests[completedGoalRef.questIndex].allGoals[completedGoalRef.goalIndex+1];
            nextGoal.isGoalActive = true;
        }
    }


    // Called in Inventory.cs
    public void CheckItem(Item item){

        foreach(QuestRef goalRef in activeGoals){

            QuestGoal currentGoal = allQuests[goalRef.questIndex].allGoals[goalRef.goalIndex];

            if(currentGoal.questParameters.GetParameterType() == QuestParameters.ParameterType.GetItem){
                
                currentGoal.CheckGoalCondition(item);
                if (currentGoal.isGoalCompleted == true){
                    ActivateNextGoal(goalRef);
                    break;
                }
            }

        }
            
    }

    public void CheckDialogue(){

        foreach(QuestRef goalRef in activeGoals){

            QuestGoal currentGoal = allQuests[goalRef.questIndex].allGoals[goalRef.goalIndex];

            if(currentGoal.questParameters.GetParameterType() == QuestParameters.ParameterType.Talk){
                
                currentGoal.CheckGoalCondition(currentNPC);
                if (currentGoal.isGoalCompleted == true){
                    ActivateNextGoal(goalRef);
                    break;
                }
            }

        }
    }

    // Called in QuestTrigger.cs
    public void CheckTrigger(string triggerName){

        foreach(QuestRef goalRef in activeGoals){

            QuestGoal currentGoal = allQuests[goalRef.questIndex].allGoals[goalRef.goalIndex];

            if(currentGoal.questParameters.GetParameterType() == QuestParameters.ParameterType.Trigger){
                
                currentGoal.CheckGoalCondition(triggerName);
                if (currentGoal.isGoalCompleted == true){
                    ActivateNextGoal(goalRef);
                    break;
                }
            }

        }
    }

    public void ActivateQuest(){

        foreach(Quest quest in allQuests){
            if(quest.isQuestActive == true){
                break;
            }
            else if (quest.isQuestActive == false && quest.isQuestCompleted == false){
                ActivateNextQuest(allQuests.IndexOf(quest)-1);      // Activate current quest
                break;
            }
        }
    }

    public void ResetQuests(){
        foreach(Quest quest in allQuests){
            if(quest.resetOnce == false){
                quest.Reset();
                foreach (QuestGoal questGoal in quest.allGoals){
                    questGoal.Reset();
                }
            }
        }
    }

    public void ResetQuestOnGameEnd(){
        foreach(Quest quest in allQuests){
            quest.OnGameEnd();
        }
    }


    // ---------------------------------------------------------- 
    // Coroutine Code  


    public Coroutine StartTimeline(string testing){
        Transform getTimeline = allTimelines.transform.Find(testing);

        if(getTimeline!=null){
            PlayableDirector timeline = getTimeline.GetComponent<PlayableDirector>();
            Coroutine temp = StartCoroutine(PlatTimeline(timeline));
            return temp;
        }
        else{
            Debug.Log("TIME LINE "+ testing +" NOT FOUND");
            return null;
        }
    }

    public IEnumerator PlatTimeline(PlayableDirector timeline)
    {
        timeline.Play();
        yield return new WaitForSeconds((float)timeline.duration);
        timeline.Stop();
    }
    

    public Coroutine OpenBookWithRecipe(){

        CustomGameManager.gameManager.BookToggle();
        bookTabGroup.ChangeTab(3);

        Coroutine temp = StartCoroutine(WaitForBookClose());
        return temp;
    }

    public Coroutine OpenBookWithCompendium(){


        CustomGameManager.gameManager.BookToggle();
        bookTabGroup.ChangeTab(1);

        if(lastPickedItem!=null){
            UI_Book.currentBookUI.CompendiumEnter(lastPickedItem);
        }

        Coroutine temp = StartCoroutine(WaitForBookClose());
        return temp;
    }

    // [YarnCommand("OpenBook")]
    static IEnumerator WaitForBookClose(){
       
        PlayerController2D.playerController.OnEnable();

        while(CustomGameManager.gameManager.isBookOpen == true){
            yield return null;
        }

        PlayerController2D.playerController.OnDisable();
    }

}
