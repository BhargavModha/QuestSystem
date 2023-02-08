using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    public string triggerName;

    private void OnTriggerEnter2D(Collider2D other) {

        if(triggerName!=null && triggerName!=""){
            StoryProgress.currentStory.CheckTrigger(triggerName);
        }
    }
}
