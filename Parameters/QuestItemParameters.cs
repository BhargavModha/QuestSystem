using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemParam", menuName = "Quests/New Item Parameter")]
public class QuestItemParameters : QuestParameters
{
    [HideInInspector] public ParameterType parameterType = QuestParameters.ParameterType.GetItem;

    [Header("Custom Settings (Optional)")] 
    public bool useCustomAmount = false;
    public int customAmount = 0;


    [Header("Items")] 

    public List<Item> itemsToCheck;
    private int singleItemTrack = 0;

    public override ParameterType GetParameterType(){
        return parameterType;
    }
    
    public override void Reset(){
        singleItemTrack = 0;
        // Debug.Log("Item Reset Complete");
    }

    public override bool CheckCondition(Item newItem){

        foreach(Item itemCheck in itemsToCheck){
            // check if same type and greater amount
            if (itemCheck.customItem.itemType == newItem.customItem.itemType && itemCheck.amount<=newItem.amount){
                singleItemTrack+=1;
                break;
            }
        }

        if (singleItemTrack == itemsToCheck.Count){
            return true;
        }

        if(useCustomAmount == true && singleItemTrack == customAmount){
            return true;
        }

        return false;

    }

    public override bool CheckConditionOnStart(){

        singleItemTrack = 0;
        List<Item> inventoryList = Inventory.instance.GetItemList();

        foreach(Item itemCheck in itemsToCheck){
            foreach(Item eachItem in inventoryList){
                if (itemCheck.customItem.itemType == eachItem.customItem.itemType && itemCheck.amount<=eachItem.amount){
                    singleItemTrack+=1;
                }
            }
        }

        if (singleItemTrack == itemsToCheck.Count){
            return true;
        }

        return false;
    }
}
