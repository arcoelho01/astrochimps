using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class to manage quests inside the game
/// </summary>
public class QuestManager : MonoBehaviour {

	/// Game Events
	public enum EQuestEvents {
		// FOUND_: when the player find something in the game
		FOUND_WATER_RESOURCE, // When the player find a water resource in the map
		FOUND_METAL_RESOURCE, // When the player find a metal resource in the map
		FOUND_ROCKET_PART, // When the player find any rocket part in the map
		// BUILT_: when the player build something in the game
		BUILT_OXYGEN_EXTRACTOR, // When the player build an oxygen extractor
		BUILT_METAL_EXTRACTOR, // When the player build a metal extractor
		// TODO: add more events here! Found an enemy unit, an enemy building, sabotage something, etc
		EVENT_NULL
	}

	/// <summary>
	/// A task description.
	/// </summary>
	class TaskEntry {

		public string stTaskDescription = ""; //< Description of this task
		public EQuestEvents eEvent = EQuestEvents.EVENT_NULL; //< Type of event
		public bool bnDone = false; //< Is this task done?
		public int nEventCount = 1; //< Number of events to consider this task done
	};

	/// <summary>
	/// A quest entry. A quest is a series of tasks performed in a certain order (or not?)
	/// </summary>
	class QuestEntry {

		public string stName = ""; //< Name of the quest. Will be used to search this particular quest in the quest list
		public bool bnDone = false; //< Is this task done?
		public List<TaskEntry> lstTasks; //< List of tasks in this quest
		public int nTasksDone = 0;
	};

	//< List of all quests
	private List<QuestEntry> lstQuests;

	/*
	 * ===========================================================================================================
	 * UNITY'S STUFF
	 * ===========================================================================================================
	 */
	// Use this for initialization
	void Start () {
	
		// Initialize the quests list
		lstQuests = new List<QuestEntry>();
		
		// TEST PURPOSE ONLY
		// Create a Quest
		CreateNewQuest("Heavy Metal");
		// Add a task to that quest
		CreateTaskInQuest("Heavy Metal", "Find a metal resource.", EQuestEvents.FOUND_METAL_RESOURCE, 1);
		// Create a Quest
		CreateNewQuest("Breathe");
		// Add a task to that quest
		CreateTaskInQuest("Breathe", "Find a water resource.", EQuestEvents.FOUND_WATER_RESOURCE, 2);
		CreateTaskInQuest("Breathe", "Build an oxygen extractor", EQuestEvents.BUILT_OXYGEN_EXTRACTOR, 1);
		
		// Print all Quests
		PrintQuestsInfo();
	}
	
	/*
	 * ===========================================================================================================
	 * QUEST'S STUFF
	 * ===========================================================================================================
	 */
	/// <summary>
	/// Creates a new quest and adds it to quests list
	/// </summary>
	/// <param name="stQuestName"> A string with this quest's name</param>
	public void CreateNewQuest(string stQuestName) {

		QuestEntry entry = new QuestEntry();
		entry.stName = stQuestName;
		entry.lstTasks = new List<TaskEntry>();

		// Adds this quest to the quests list
		lstQuests.Add(entry);
	}

	/// <summary>
	/// Prints information for all quests
	/// </summary>
	public void PrintQuestsInfo() {

		int nIdx = 0;

		// DEBUG
		Debug.LogWarning("Quests info. Total quests in the list: " + lstQuests.Count);

		foreach(QuestEntry entry in lstQuests) {

			Debug.Log("Quest #" + nIdx + ": " + entry.stName);
			Debug.Log("Tasks in this quest: " + entry.lstTasks.Count);

			if(entry.lstTasks.Count > 0) {

				foreach(TaskEntry taskEntry in entry.lstTasks) {

					Debug.Log("+ Task " + taskEntry.stTaskDescription + " : " + taskEntry.nEventCount);
				}
			}
			
			nIdx++;
		}
	}

	/*
	 * ===========================================================================================================
	 * EVENTS STUFF
	 * ===========================================================================================================
	 */
	/// <summary>
	/// Add an event to the QuestManager. This will be fired by the event generator. For example, if the player
	/// builds an extractor, the class CBuilding will call this method.
	/// </summary>
	/// <param name="eEvent"> One of the EQuestEvents </param>
	public void AddEventToTheQM(EQuestEvents eEvent) {

		// FIXME: I guess this code won't be needed
		switch(eEvent) {

			case EQuestEvents.BUILT_METAL_EXTRACTOR:
				// DEBUG
				Debug.LogWarning(this.transform + " Received a BUILT_METAL_EXTRACTOR event");
				break;

			case EQuestEvents.BUILT_OXYGEN_EXTRACTOR:
				// DEBUG
				Debug.LogWarning(this.transform + " Received a BUILT_OXYGEN_EXTRACTOR event");
				break;

			case EQuestEvents.FOUND_WATER_RESOURCE:
				// DEBUG
				Debug.LogWarning(this.transform + " Received a FOUND_WATER_EXTRACTOR event");
				break;

			case EQuestEvents.FOUND_METAL_RESOURCE:
				// DEBUG
				Debug.LogWarning(this.transform + " Received a FOUND_METAL_EXTRACTOR event");
				break;

			// TODO: add other events here...
			default:
				break;
		}

		// In all quests ...
		foreach(QuestEntry questEntry in lstQuests) {

			if(questEntry.bnDone)
				continue;

			// ... in all tasks
			foreach(TaskEntry taskEntry in questEntry.lstTasks) {

				// Task already done? Next task then
				if(taskEntry.bnDone)
					continue;

				// Event Match!
				if(taskEntry.eEvent == eEvent) {

					// Decrease the event counter
					taskEntry.nEventCount--;

					// Enough of theses events? Mark this task as done!
					if(taskEntry.nEventCount	<= 0) {

						taskEntry.bnDone = true;

						// Increase the "tasks done" counter
						questEntry.nTasksDone++;

						// Check if we haven't completed the task
						if(questEntry.lstTasks.Count == questEntry.nTasksDone) {

							// Quest done!
							questEntry.bnDone = true;

							// DEBUG
							Debug.Log(this.transform + "Quest " + questEntry.stName + " done.");
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Creates a task and add it to corresponding quest
	/// </summary>
	/// <param name="stQuestName"> The name of the quest to add this task. The quest is identified by it's name </param>
	/// <param name="stTaskDescription"> A string with the description of this task </param>
	/// <param name="eEvent"> A EQuestEvents enum with the type of event to perform this task </param>
	public void CreateTaskInQuest(string stQuestName, string stTaskDescription, EQuestEvents eEvent, int nCount) {

		// First of all, let's find the quest in the list
		QuestEntry questEntry = null;

		foreach(QuestEntry entry in lstQuests) {

			if(entry.stName == stQuestName) {

				questEntry = entry;
				break;
			}
		}

		if(questEntry != null) {

			// Ok, quest found. Create the task...
			TaskEntry taskEntry = new TaskEntry();
			taskEntry.stTaskDescription = stTaskDescription;
			taskEntry.eEvent = eEvent;
			taskEntry.nEventCount = nCount;

			// ... and add it to the quest
			questEntry.lstTasks.Add(taskEntry);
		}
	}
}
