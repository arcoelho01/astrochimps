using UnityEngine;
using System.Collections;

/// <summary>
/// Class with monkey's definitions
/// </summary>
public class CMonkey : CBaseEntity {

	// PRIVATE
	
	// PUBLIC
	public enum eMonkeyType { Astronaut, Cientist, Engineer, Saboteur, NONE }; // Monkeys types
	
	public AudioClip sfxSelected; // Played when the monkey is selected by the player
	public AudioClip sfxAttacked;	// Played when attacked (by a drone, for instance)
	public AudioClip sfxAck;	// Played when the monkey received and acknowledged an order
	private Transform transTarget;   // Target Transform
	private Vector3 walkTo;
	public float attackRange;      //  Attack Range to disable drones.
	
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
	private AstarAIFollow AIScript = null; // Cache a pointer to the AI script
	private float stunnedTimeCounter;
	public eMonkeyType monkeyClass; // Which class this monkey belongs

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
		AIScript = gameObject.GetComponent<AstarAIFollow>();
		
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
	
	
	// BEING ATTACKED
	public void Attacked(){
		
		AudioSource.PlayClipAtPoint(sfxAttacked, transform.position);
			
		EnterNewState(FSMState.STATE_STUNNED);
		
	}
	
	public void WalkTo(Vector3 walkTo){
		this.walkTo = walkTo;
		EnterNewState(FSMState.STATE_WALKING);
	}
	
	// ATTACK THE TARGET
	
	public void Attack(Transform transTarget){
		 this.transTarget = transTarget;
		 EnterNewState(FSMState.STATE_ATTACKING);
	}
	
	void Update(){
		
		ExecuteCurrentState();
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
				if(sfxAck) {

					AudioSource.PlayClipAtPoint(sfxAck, transform.position);
				}
				AIScript.ClickedTargetPosition(walkTo);
			
				break;

			case FSMState.STATE_STUNNED:
				stunnedTimeCounter = 10000; // Stay stunned for 10 seconds.
				AIScript.Stop();
				break;

			case FSMState.STATE_ATTACKING:
				// SET AISCRIPT TO MOVE TO TARGET
				// FIXME: target is null here!
				// DEBUG
				Debug.Log("Entering STATE_ATTACKING with target: " + transTarget);
				AIScript.ClickedTargetPosition(transTarget.position);
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

			case FSMState.STATE_INSIDE_BUILDING:
				break;

			case FSMState.STATE_WALKING:
				break;

			case FSMState.STATE_STUNNED:
				// DO NOTHING FOR 10 SECONDS
				Debug.Log("[ExecuteCurrentState: " + GetCurrentState() + "]");
				stunnedTimeCounter = stunnedTimeCounter - Time.deltaTime;
				if ( stunnedTimeCounter <=0)
					EnterNewState(FSMState.STATE_IDLE);
				break;
			
			case FSMState.STATE_ATTACKING:
			
			
			// BY LEO: POR ENQUANTO ELE NAO ESTA SE MOVIMENTANDO EM DIRECAO AO INIMIGO POIS JA ESTA SENDO FEITO FORA DAQUI, MAS ACHO QUE DEVERIA ENTRAR NO WALKING E TB NO ATTACKING
			
			// IF BELLOW IS TO CHECK IF THE TARGET STILL EXISTS
				if (transTarget == null){
					Debug.Log("TARGET INVALID");
					EnterNewState(FSMState.STATE_IDLE);
					break;
				}
			
				Debug.Log("TARGET VALID");
			
				Vector3 diff = transTarget.transform.position - gameObject.transform.position;       
				float curDistance = diff.sqrMagnitude; 
				
				// FIXME: distance must be the radius of the monkey collider plus the radius of the target collider
				if (curDistance < 3.5f)
				{
					CDrone droneTarget = transTarget.gameObject.GetComponent<CDrone>();
					if (droneTarget != null){
					
						Debug.Log("XXXX MONKEY  attacking");
						droneTarget.Attacked();
				
					// TODO: PLAY SOME ATTACKING SOUND
				
						EnterNewState(FSMState.STATE_IDLE);
					}
					
				}
				else {

					// FIXME: create another state, like "walking_to_the_target" or something. The idea is to get closer
					// to the target (walking) and then attacking, but without changing to the "walking" state.
					// Another idea: create an "pursuit" state. When distance is less than X, change to attack 
					// automatically
					// DEBUG
					Debug.Log("Distance: " + curDistance);
					WalkTo(transTarget.transform.position);
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

			case FSMState.STATE_INSIDE_BUILDING:
				break;

			case FSMState.STATE_WALKING:
				break;

			case FSMState.STATE_STUNNED:
				AIScript.Resume();
				break;

			case FSMState.STATE_ATTACKING:
				// DEBUG
				Debug.Log("Leaving STATE_ATTACKING");
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
}
