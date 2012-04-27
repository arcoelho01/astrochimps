using UnityEngine;
using System.Collections;

/// <summary>
/// Class with monkey's definitions
/// </summary>
public class CMonkey : CBaseEntity {

	// PRIVATE
	
	// PUBLIC
	public enum eMonkeyType { Astropilot, Cientist, Engineer, Saboteur, NONE }; // Monkeys types
	
	public AudioClip sfxSelected; // Played when the monkey is selected by the player
	public AudioClip sfxAttacked;	// Played when attacked (by a drone, for instance)
	
	public enum FSMState {
		STATE_IDLE,							// Doing nothing...
		STATE_SELECTED,					// Selected by the player
		STATE_INSIDE_BUILDING,	// when the monkey is inside a building, cannot move
		STATE_WALKING,					// Monkey walking around
		STATE_STUNNED,					// Monkey stunned by an enemy drone, cannot move
		STATE_ATTACKING,				// Attacking an enemy drone
		STATE_NULL							// null
	};
	
	FSMState eFSMCurrentState;	// Keeps the current FSM state

	/*
	 * ===========================================================================================================
	 * UNITY'S STUFF
	 * ===========================================================================================================
	 */

	/// <summary>
	/// When the script is initialized
	/// </summary>
	void Awake() {

		// Set the default settings for all the buildings
		Selectable = true; // All monkeys are selectable
		Movable = true; // All monkeys are movable
		Type = eObjType.Monkey;

		// FSM setup
		eFSMCurrentState = FSMState.STATE_IDLE;
	}

	/// <summary>
	/// Selected is override to play a sound
	/// </summary>
	public override Transform Select() {

		if(sfxSelected) {
		
			AudioSource.PlayClipAtPoint(sfxSelected, transform.position);
		}
		return base.Select();
	}

	/*
	 * ===========================================================================================================
	 * FINITE STATE MACHINE BASIC FUNCTIONS
	 * ===========================================================================================================
	 */

	/// <summary>
	/// Returns the FSM current state
	/// </summary>
	/// <returns> The enum corresponding to the current FSM state </returns>
	FSMState GetCurrentState() {

		return eFSMCurrentState;
	}

	/// <summary>
	/// Changes the FSM to a new state
	/// </summary>
	/// <param name="eNewState"> The new state </param>
	void EnterNewState(FSMState eNewState) {

		// Exits the current state
		LeaveCurrentState();

		eFSMCurrentState = eNewState;

		// Selects the new machine state
		switch(GetCurrentState()) {

			case FSMState.STATE_IDLE:
				{
					// DEBUG
					Debug.Log("[EnterNewState: " + GetCurrentState() + "]");
				}
				break;
			case FSMState.STATE_SELECTED:
				break;

			case FSMState.STATE_INSIDE_BUILDING:
				break;

			case FSMState.STATE_WALKING:
				break;

			case FSMState.STATE_STUNNED:
				break;

			case FSMState.STATE_ATTACKING:
				break;

			case FSMState.STATE_NULL:
				break;

			default:
				// DEBUG
				Debug.LogError("I shouldn't be here.");
				break;
		}
	}

	/// <summary>
	/// Executes the FSM current state
	/// </summary>
	void ExecuteCurrentState() {

		switch(GetCurrentState()) {

			case FSMState.STATE_IDLE:
				{
					// DEBUG
					Debug.Log("[ExecuteCurrentState: " + GetCurrentState() + "]");
				}
				break;
			case FSMState.STATE_SELECTED:
				break;

			case FSMState.STATE_INSIDE_BUILDING:
				break;

			case FSMState.STATE_WALKING:
				break;

			case FSMState.STATE_STUNNED:
				break;

			case FSMState.STATE_ATTACKING:
				break;

			case FSMState.STATE_NULL:
				break;

			default:
				// DEBUG
				Debug.LogError("I shouldn't be here.");
				break;
		}
	}

	/// <summary>
	/// Leaves the current state. Used whenever the FSM enters a new state
	/// </summary>
	void LeaveCurrentState() {

		switch(GetCurrentState()) {

			case FSMState.STATE_IDLE:
				{
					// DEBUG
					Debug.Log("[LeaveCurrentState: " + GetCurrentState() + "]");
				}
				break;

			case FSMState.STATE_SELECTED:
				break;

			case FSMState.STATE_INSIDE_BUILDING:
				break;

			case FSMState.STATE_WALKING:
				break;

			case FSMState.STATE_STUNNED:
				break;

			case FSMState.STATE_ATTACKING:
				break;

			case FSMState.STATE_NULL:
				break;

			default:
				// DEBUG
				Debug.LogError("I shouldn't be here.");
				break;
		}
	}
}
