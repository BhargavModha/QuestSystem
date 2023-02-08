using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "QuestMasterlist", menuName = "Custom/New Quest Masterlist")]
public class QuestMasterlist : ScriptableObject
{
    public List<Quest> allQuests;
}
