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

		public string stTaskDescription = "";
		public EQuestEvents eEvent = EQuestEvents.EVENT_NULL;
		public bool bnDone = false;
	};

	/// <summary>
	/// A quest entry. A quest is a series of tasks performed in a certain order (or not?)
	/// </summary>
	class QuestEntry {

		public string stName = "";
		public bool bnDone = false;
		public List<TaskEntry> lstTasks;
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
		CreateNewQuest("Teste");
		// Add a task to that quest
		CreateTaskInQuest("Teste", "This is the task description", EQuestEvents.BUILT_METAL_EXTRACTOR);
		
		// Print all Quests
		PrintQuestsInfo();
	}
	
	// Update is called once per frame
	void Update () {
	
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

					Debug.Log("+ Task " + taskEntry.stTaskDescription);
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

		switch(eEvent) {

			case EQuestEvents.BUILT_METAL_EXTRACTOR:
				// DEBUG
				Debug.Log(this.transform + " Received a BUILT_METAL_EXTRACTOR event");
				break;

			case EQuestEvents.BUILT_OXYGEN_EXTRACTOR:
				// DEBUG
				Debug.Log(this.transform + " Received a BUILT_OXYGEN_EXTRACTOR event");
				break;

			// TODO: add other events here...

			default:
				break;
																						
		}
	}

	/// <summary>
	/// Creates a task and add it to corresponding quest
	/// </summary>
	/// <param name="stQuestName"> The name of the quest to add this task. The quest is identified by it's name </param>
	/// <param name="stTaskDescription"> A string with the description of this task </param>
	/// <param name="eEvent"> A EQuestEvents enum with the type of event to perform this task </param>
	public void CreateTaskInQuest(string stQuestName, string stTaskDescription, EQuestEvents eEvent) {

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
			// ... and add it to the quest
			questEntry.lstTasks.Add(taskEntry);
		}
	}
}
