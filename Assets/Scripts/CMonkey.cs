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
	public AudioClip sfxAttack;	// Played when the monkey is attacking a target
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
		STATE_PURSUIT,					// Walk until the target is in range, then attack it
		STATE_NULL							// null
	};
	
	FSMState eFSMCurrentState;	// Keeps the current FSM state
	private AstarAIFollow AIScript = null; // Cache a pointer to the AI script
	private float stunnedTimeCounter;
	public eMonkeyType monkeyClass; // Which class this monkey belongs

	CharacterController myController;
	public Transform iconPrefab;

	Vector3 v3Direction;
	float fAttackRange;
	
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

		// Get info about the collider
		GetColliderInfo();

		// Load the icon for this unit
		LoadMinimapIcon();
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
		 //EnterNewState(FSMState.STATE_ATTACKING);
		 // Changed by Alexandre: instead of attacking, we first enter the pursuit mode. If the target is already 
		 // at range, the FSM will change to ATTACKING
		 EnterNewState(FSMState.STATE_PURSUIT);
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
				break;

			case FSMState.STATE_PURSUIT:
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
			
				// DEBUG
				Debug.Log("Executing attack");

				// TODO here applies the following rules:
				// Astronaut: disable only the patrol drone
				// Saboteur:	attack a building, disabling it;
				//						attack a disabled drone, reprogramming it
				//						attack any drone, temporarily disabling it (CHECK)
				// Cientist:	get rocket parts
				// Engineer:	attack a disabled drone, turning it in metal
				//						'attack' a saboutaged building, repairing it 
			
				CDrone droneTarget = transTarget.gameObject.GetComponent<CDrone>();
				if (droneTarget != null){

					Debug.Log("XXXX MONKEY  attacking");

					// Astronaut only attack drones that are not stunned
					if(monkeyClass == eMonkeyType.Astronaut && !droneTarget.isStunned()) {

						if(sfxAttack) {

							AudioSource.PlayClipAtPoint(sfxAttack, transform.position);
						}

						droneTarget.Attacked();
					}
					else if(monkeyClass == eMonkeyType.Engineer && droneTarget.isStunned()) {
					
						if(sfxAttack) {

							AudioSource.PlayClipAtPoint(sfxAttack, transform.position);
						}

						droneTarget.Recycled();
					}
					else if(monkeyClass == eMonkeyType.Saboteur && droneTarget.isStunned()) {

						// DEBUG
						Debug.Log("Drone being attacked by a Saboteur. Should be reprogrammed");
					}
					
					// TODO: PLAY SOME ATTACKING SOUND

					EnterNewState(FSMState.STATE_IDLE);
				}
				break;

			case FSMState.STATE_PURSUIT:
				{
					// ADDED by Alexandre: pursuit is the combination of walk and attack modes. First, walk to the target. 
					// Here, test if the target is in range, then switch to the ATTACK state
					// IF BELLOW IS TO CHECK IF THE TARGET STILL EXISTS
					if (transTarget == null){
						// DEBUG
						Debug.Log("TARGET INVALID");

						EnterNewState(FSMState.STATE_IDLE);
						break;
					}

					// DEBUG
					Debug.Log("TARGET VALID");

					Vector3 diff = transTarget.transform.position - gameObject.transform.position;       
					float curDistance = diff.sqrMagnitude; 

					// Check if the enemy is range for be attacked
					if(CheckIfEnemyIsInRange())	{

						EnterNewState(FSMState.STATE_ATTACKING);
					}
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
				AIScript.Stop();
				break;

			case FSMState.STATE_STUNNED:
				AIScript.Resume();
				break;

			case FSMState.STATE_ATTACKING:
				// DEBUG
				Debug.Log("Leaving STATE_ATTACKING");
				transTarget = null;
				break;

			case FSMState.STATE_PURSUIT:
				// Stop the 'walk to the target'
				AIScript.Stop();
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
	/// Get info about the collider in this object. This is needed so we know what are the boundaries of the
	/// object. With this, we can calculate the distance of the object from others, knowing if we can attack 
	/// them, for instance
	void GetColliderInfo() {

		// Get the collider - actually a CharacterController
		myController = gameObject.GetComponent<CharacterController>();

		// DEBUG: display it's info
		Debug.Log("Object position: " + this.transform.position + " Center: " + myController.center);
	}

	/// <summary>
	/// Loads the appropriate icon to display in the minimap. It should depends on the side (allied or opponent),
	/// type of object and visibility
	/// </summary>
	void LoadMinimapIcon() {

		Transform myIcon = Instantiate(iconPrefab, this.transform.position, Quaternion.identity) as Transform;

		myIcon.transform.parent = this.transform;
		myIcon.name = "Icon";

	}

	/// <summary>
	/// Throws a raycast to check if the enemy is in range
	/// </summary>
	/// <returns> A boolean if the enemy is in range </returns>
	bool CheckIfEnemyIsInRange() {

		bool rv = false;

		// No target? Get out!
		if(!transTarget)
			return rv;
	
		RaycastHit hit;
		fAttackRange = 3.0f;
		// Direction from here to the target
		v3Direction = transTarget.transform.position - transform.position;

		if(Physics.Raycast(transform.position, v3Direction, out hit, fAttackRange, 1 << MainScript.enemyLayer)) {

			Debug.Log("Monkey hit in " + hit.transform.name);

			rv = true;
		}

		return rv;
	}

	/// <summary>
	/// Draw some helpers on screen
	/// </summary>
	void OnDrawGizmos() {

		if(!transTarget)
			return;

		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, v3Direction);
		Gizmos.DrawWireSphere(transform.position, fAttackRange);
	}
}
