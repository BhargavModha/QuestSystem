# Quest System

It keeps track of the player's current progress through the story.  
The quest system is designed to be robust and scalable (hopefully). To achieve this, almost all aspects of this system were designed using Scriptable Objects

![Quest System in The Alchemists Garden](/Images/Screenshot_1.png)

Each “**Quest**” scriptable object contains  
- Quest Name  
- Quest Desc (Not displayed in the book UI) 
- Quest Icon  
- A list of “Quest Goals”. 

The “**Quest Goal**” scriptable object contains  
- Goal Name (only used to manage the inspector)
- Goal Description (displayed in the book)
- A “Quest Parameter”
- List of “Quest Rewards” (optional)
- Boolean to skip current goal (for debugging)

The “Quest” and “Quest Goal” have two booleans, one to check if it is active and the other if it has been completed. By default, the StoryProgress.cs script will set the first goal of the first quest active.

---

To complete a quest goal, the player NEEDS to complete/satisfy the quest parameter.  
The QuestParameter.cs is an abstract class (so it's easier to add more types of parameters in the future) that requires the inherited class to override functions. 

Currently, there are three types of “**Quest Parameters**”.
- **Talk** (requires the player to talk to an NPC)
    - NPC name
    - Yarn Dialogue Node

- **GetItem** (requires the player to have item(s) in the inventory)
    - Use Custom Amount (boolean to enable a specific amount of items from the list)(optional)
    - Custom Amount (optional)
    - List of “Items”
- **Trigger** (requires the player to pass through a trigger)
    - TriggerName (NOTE: requires the trigger game object to contain QuestTrigger.cs script with the same trigger name)


To add more types of parameters, you’ll need  
- To create a new class that inherits QuestParameter  
- To add a new function in StoryProgress.cs that checks the condition (refer CheckItem() and CheckDialogue())

---

Once the player has completed the goal, “**Quest Rewards**” can be activated. There can be a list of rewards or no rewards at all for completing a goal. The QuestReward.cs is also an abstract class that requires the inherited class to override functions.

There are two types of rewards
- StartDialogue (trigger dialogue on goal completion)
    - Yarn Node
- CustomEvent (trigger UnityEvent)
    - (NOTE: This reward requires a QuestEventRewardListener.cs script in the scene that references this quest event reward scriptable object. This script has a list of UnityEvents that can be edited in the inspector. When this reward is activated, it will trigger an action to which the QuestEventRewardListener.cs is subscribed and invoke the UnityEvent.)

If the debug boolean is active on a goal, it will automatically mark the current goal as complete and set the next goal to be active. It will also trigger the custom event reward (if any) in the goal. 
