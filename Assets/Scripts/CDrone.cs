using UnityEngine;
using System.Collections;

/// <summary>
/// Class with drones definitions
/// </summary>
public class CDrone : CBaseEntity {

	// PRIVATE
	
	// PUBLIC
	public enum eDroneType { Patrol, Saboteur, Hunter, NONE }; // Drones types
	public Transform stunnedParticleSystem;
	Transform stunnedObj = null;

	/*
	 * ===========================================================================================================
	 * UNITY'S STUFF
	 * ===========================================================================================================
	 */
	
	
	
	public AudioClip sfxSelected; // Played when the monkey is selected by the player
	public AudioClip sfxAttacked;	// Played when attacked (by a drone, for instance)
	public Transform transTarget;   // Target Transform
	public float attackRange;      //  Attack Range to disable drones.
	private Vector3 walkTo;
	
	public enum FSMState {
		STATE_IDLE,							// Doing nothing...
		STATE_SELECTED,						// Selected by the player
		STATE_WALKING,						// Drone walking around
		STATE_STUNNED,						// Drone stunned by an enemy, cannot move
		STATE_ATTACKING,					// Attacking an enemy
		STATE_NULL							// null
	};
	
	FSMState eFSMCurrentState;	// Keeps the current FSM state
	float stunnedTimeCounter;
	private AstarAIFollow AIScript = null; // Cache a pointer to the AI script

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
		Selectable = true; // All drones are selectable
		Movable = true; // All drones are movable
		Type = eObjType.Drone;
		
		AIScript = gameObject.GetComponent<AstarAIFollow>();
		
		// FSM setup
		eFSMCurrentState = FSMState.STATE_IDLE;
	}
	
	public void WalkTo(Vector3 walkTo){
		this.walkTo = walkTo;
		EnterNewState(FSMState.STATE_WALKING);
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
	
	public bool isStunned(){
		return (eFSMCurrentState == FSMState.STATE_STUNNED);
	}
	
	
	public void Update(){
		
		ExecuteCurrentState();
	}
	
	/// <summary>
	/// What to do when this drone is attacked?
	/// </summary>
	public void Attacked(){
		
		Debug.Log("XXXX BEING ATTACKED");
		
		// TODO check who is attacking this drone
		// If is a monkey, check in CMonkey the rules; some monkeys cause the drone to be disabled, others to 
		// recycle it, etc
		EnterNewState(FSMState.STATE_STUNNED);
		
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
				//	Debug.Log("[EnterNewState: " + GetCurrentState() + "]");
				}
				break;
			case FSMState.STATE_SELECTED:
				break;
			
			case FSMState.STATE_WALKING:
				// SET AISCRIPT TO MOVE TO TARGET
				Debug.Log("XXXXXX Drone walking");
				AIScript.ClickedTargetPosition(walkTo);

				break;

			case FSMState.STATE_STUNNED:
				// Play a sound effect
				if(sfxAttacked) {

					AudioSource.PlayClipAtPoint(sfxAttacked, transform.position);
				}

				// Add a visual aid
				if(stunnedParticleSystem) {
				
					// Instantiate the particle system
					stunnedObj = Instantiate(stunnedParticleSystem, this.transform.position + Vector3.up, 
							Quaternion.Euler(-90,0,0)) as Transform;
					
					// Put it as child
					stunnedObj.transform.parent = this.transform;
				}

				stunnedTimeCounter = 10; // Stay stunned for 10 seconds.
				AIScript.Stop();
				break;

			case FSMState.STATE_ATTACKING:
				// Get the target to attack
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
					//Debug.Log("[ExecuteCurrentState: " + GetCurrentState() + "]");
				}
				break;
			case FSMState.STATE_SELECTED:
				break;
			case FSMState.STATE_WALKING:
				break;

			case FSMState.STATE_STUNNED:
				//Debug.Log("[ExecuteCurrentState: " + GetCurrentState() + "]");
				// DO NOTHING FOR 10 SECONDS
				stunnedTimeCounter = stunnedTimeCounter - Time.deltaTime;
				if ( stunnedTimeCounter <=0)
					EnterNewState(FSMState.STATE_IDLE);
				break;

			case FSMState.STATE_ATTACKING:
				// LEO: POR ENQUANTO ELE NAO ESTA SE MOVIMENTANDO EM DIRECAO AO INIMIGO POIS JA ESTA SENDO FEITO FORA DAQUI, MAS ACHO QUE DEVERIA ENTRAR NO WALKING E TB NO ATTACKING
				if (transTarget == null){
					EnterNewState(FSMState.STATE_IDLE);
					break;
				}
				
				Vector3 diff = transTarget.transform.position - gameObject.transform.position;       
				float curDistance = diff.sqrMagnitude; 
				
				if (curDistance < 500)
				{
					CDrone droneTarget = transTarget.gameObject.GetComponent<CDrone>();
					if (droneTarget != null)
						droneTarget.EnterNewState(CDrone.FSMState.STATE_STUNNED);
				
					EnterNewState(FSMState.STATE_IDLE);
					
				}
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
			
			case FSMState.STATE_WALKING:
				break;

			case FSMState.STATE_STUNNED:
				{

					// Destroys the 'stunned particle system', if exists
					if(stunnedObj)
						Destroy(stunnedObj.gameObject);
				}
				break;

			case FSMState.STATE_ATTACKING:
				transTarget = null;
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
	/// Helper function to tell if a drone is in the STATE_STUNNED or no. Useful for monkeys attacking this drone
	/// </summary>
	public bool IsThisDroneStunned() {

		if(GetCurrentState() == FSMState.STATE_STUNNED)
			return true;
		else
			return false;
	}
}
