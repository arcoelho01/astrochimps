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
		STATE_WORKING,					// Monkey working on something. Action that requires a certain time
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
	MouseWorldPosition.eMouseStates mouseState;	//< The mouse state when the player issued an order to the monkey
	float workingTimer = 0.0f;	//< Timer for the working state
	float workingTargetTime = 0.0f; //< Time needed to perform a task. When working timer is bigger than this, 
																	// the task is done
	
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

		// Starts the object variables, like the sweet spot and the main mesh object
		GetSweetSpotAndMeshObject();

		// Get the capture spot
		if(monkeyClass == eMonkeyType.Cientist)
			captureSpot = GetCaptureSpot();
	}

	/// <summary>
	/// Update
	/// </summary>
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
				// Clear any previous targets
				transTarget = null;
				break;
			case FSMState.STATE_SELECTED:
				break;

			case FSMState.STATE_INSIDE_BUILDING:
				break;

			case FSMState.STATE_WALKING:

				// DEBUG
				Debug.Log("Entering STATE_WALKING");
				if(sfxAck) {

					AudioSource.PlayClipAtPoint(sfxAck, transform.position);
				}
				AIScript.ClickedTargetPosition(walkTo);
				// Clear the target
				transTarget = null;

				// Plays the animation for the walk cycle
				if(meshObject) {

					meshObject.animation.Play("NormalWalk");
				}

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

			case FSMState.STATE_WORKING:
				// DEBUG
				Debug.Log("FSM entered WORKING state");
				// Resets the working timer
				workingTimer = 0.0f;
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
				MonkeyAttack();
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

					// Check if the enemy is range for be attacked
					if(CheckIfTargetIsInRange())	{

						EnterNewState(FSMState.STATE_ATTACKING);
					}
				}
				break;

			case FSMState.STATE_WORKING:
				// Updates the working timer
				workingTimer += Time.deltaTime;

				if(workingTimer >= workingTargetTime) {

					workingTimer = 0.0f;

					// TODO: call the function to perform the desired task here
					WorkIsDone();
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
				// DEBUG
				Debug.Log("Leaving STATE_WALKING");
				// FIXME: the line below is causing the game to lock. This is because we getting in here for an event
				// when the AI stops, so it make no sense to call Stop() again.
				// But when we get here by other ways, like walking and then issuing an attack command?
				//AIScript.Stop();
				
				// Stops the walk cycle
				if(meshObject) {

					//meshObject.animation.Stop("Walk");
					meshObject.animation.CrossFade("idle");
				}
			
				break;

			case FSMState.STATE_STUNNED:
				AIScript.Resume();
				break;

			case FSMState.STATE_ATTACKING:
				break;

			case FSMState.STATE_PURSUIT:
				// Stop the 'walk to the target'
				AIScript.Stop();
				break;

			case FSMState.STATE_WORKING:
				// DEBUG
				Debug.Log("FSM leaving WORKING state");
				break;

			case FSMState.STATE_NULL:
				break;

			default:
				// DEBUG
				Debug.LogError("I shouldn't be here.");
				break;
		}
	}

	/*
	 * ===========================================================================================================
	 * FSM HELPER FUNCTIONS
	 * ===========================================================================================================
	 */

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
		
		if(sfxAttacked)
			AudioSource.PlayClipAtPoint(sfxAttacked, transform.position);
			
		EnterNewState(FSMState.STATE_STUNNED);
	}
	
	public void WalkTo(Vector3 walkTo){
		this.walkTo = walkTo;
		EnterNewState(FSMState.STATE_WALKING);
	}
	
	/// <summary>
	/// Get info about the collider in this object. This is needed so we know what are the boundaries of the
	/// object. With this, we can calculate the distance of the object from others, knowing if we can attack 
	/// them, for instance
	/// </summary>
	void GetColliderInfo() {

		// Get the collider - actually a CharacterController
		myController = gameObject.GetComponent<CharacterController>();
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
	/// Throws a raycast to check if the target is in range
	/// </summary>
	/// <returns> True if the target is in range, false otherwise </returns>
	bool CheckIfTargetIsInRange() {

		bool rv = false;

		// No target? Get out!
		if(!transTarget)
			return rv;
	
		RaycastHit hit;
		fAttackRange = 3.0f; // FIXME: change this arbitrary value for something more meaningful

		// Direction from here to the target
		v3Direction = transTarget.transform.position - transform.position;

		if(Physics.Raycast(transform.position, v3Direction, out hit, fAttackRange)) {

			Debug.Log("Monkey hit in " + hit.transform.name);

			if(hit.transform == transTarget)
				rv = true;
		}

		return rv;
	}

	/// <summary>
	/// Attack a target. Actually, 'attack' could be replaced by 'perform some action on...', because we using
	/// this function also to repair buildings, capture rocket parts, etc.
	/// </summary>
	/// <param name="transTarget"> Transform of the targeted object </param>
	/// <param name="currentMouseState"> A enum with the mouse state when the player issued an order </param>
	/// <summary>
	/// Performs the 'attack' of the monkey
	/// </summary>
	public void PerformAction(Transform transTarget, MouseWorldPosition.eMouseStates currentMouseState){

		 this.transTarget = transTarget;

		 mouseState = currentMouseState;
		 
		 // Go into pursuit mode -> walk to the target. When it is in range, perform the action
		 EnterNewState(FSMState.STATE_PURSUIT);
	}
	
	void MonkeyAttack() {

		// TODO here applies the following rules:
		// Astronaut: disable only the patrol drone
		// Saboteur:	attack a building, disabling it;
		//						attack a disabled drone, reprogramming it
		//						attack any drone, temporarily disabling it (CHECK)
		// Cientist:	get rocket parts
		// Engineer:	attack a disabled drone, turning it in metal
		//						'attack' a saboutaged building, repairing it 
		// All monkeys: 'attack' the command center, entering in it

		// NEW CODE
		// Perform the action according to the mouse state
		switch(mouseState) {
			
			// Astronaut monkey
			case MouseWorldPosition.eMouseStates.TargetingForBrawl:
				{

					CDrone droneTarget = transTarget.gameObject.GetComponent<CDrone>();
					if(droneTarget != null) {

						if(sfxAttack) 
							AudioSource.PlayClipAtPoint(sfxAttack, transform.position);

						droneTarget.Attacked();
					}
					EnterNewState(FSMState.STATE_IDLE);
				}
				break;
			
			// Engineer monkey
			case MouseWorldPosition.eMouseStates.EngineerFix:
				{

					// Sets the time need to fix this building
					workingTargetTime = 3.0f; // FIXME: arbitrary value! Fix!
					EnterNewState(FSMState.STATE_WORKING);
				}
				break;

			case MouseWorldPosition.eMouseStates.TargetingForRecycle:
				{

					CDrone droneTarget = transTarget.gameObject.GetComponent<CDrone>();
					if(sfxAttack) {

						AudioSource.PlayClipAtPoint(sfxAttack, transform.position);
					}

					droneTarget.Recycled();
					EnterNewState(FSMState.STATE_IDLE);
				}
				break;

			// Cientist monkey
			case MouseWorldPosition.eMouseStates.CanCapture:
				{
					// Cientist monkey capturing a RocketPart
					CBaseEntity capturedEntity = transTarget.GetComponent<CBaseEntity>();

					if(!capturedEntity) {

						// DEBUG
						Debug.LogError("Cannot find component CBaseEntity for this object: " + transTarget.name);
					}

					capturedEntity.CapturedBy(this.transform, this.captureSpot);
					EnterNewState(FSMState.STATE_IDLE);
				}
				break;

			// Saboteur monkey
			case MouseWorldPosition.eMouseStates.TargetingForReprogram:
				{
					CDrone droneTarget = transTarget.gameObject.GetComponent<CDrone>();
					droneTarget.Reprogrammed();
					EnterNewState(FSMState.STATE_IDLE);
				}
				break;

			case MouseWorldPosition.eMouseStates.CanSabotageBuilding:
				{

					// Sets the time needed to sabotage a building
					workingTargetTime = 3.0f; // FIXME: arbitratry value!
					EnterNewState(FSMState.STATE_WORKING);
				}
				break;

			// All monkeys
			case MouseWorldPosition.eMouseStates.MonkeyCanEnterBuilding:
				{
					CBuilding attackedBuilding = transTarget.gameObject.GetComponent<CBuilding>();

					if(!attackedBuilding) {

						// DEBUG
						Debug.LogError("CBuilding component not found for " + transTarget.name);
						return;
					}

					attackedBuilding.PutAMonkeyInside(this.transform);

					// DEBUG
					Debug.Log("MouseState for this action " + mouseState);
					EnterNewState(FSMState.STATE_IDLE);

					// Deselect this object
					this.Deselect();
				}
				break;

			default:
				// DEBUG
				Debug.LogError("I shouldn't be here...");
				break;
		}
	}

	/// <summary>
	/// The monkey is doing some task, and it's now finished. Check what task it is and call the appropriate
	/// function
	/// </summary>
	void WorkIsDone() {

		// DEBUG
		Debug.Log("Entering WorkIsDone with mouseState " + mouseState);
		if(!transTarget)
			Debug.LogError("Target is null! Check");

		switch(mouseState) {

			case MouseWorldPosition.eMouseStates.EngineerFix:
				{
					// DEBUG
					Debug.Log("Building fixed by the Engineer");

					CBuilding attackedBuilding = transTarget.gameObject.GetComponent<CBuilding>();
					if(!attackedBuilding) {

						// DEBUG
						Debug.LogError("CBuilding component not found for " + transTarget.name);
						return;
					}
					// Fix the broken building
					attackedBuilding.FixByEngineer();
					// Do nothing afterwards
					EnterNewState(FSMState.STATE_IDLE);
				}
				break;

			case MouseWorldPosition.eMouseStates.CanSabotageBuilding:
				{
					CBuilding buildingTarget = transTarget.gameObject.GetComponent<CBuilding>();

					// TODO: the sabotage time could be taken from the building itself
					buildingTarget.TemporarySabotage(8.0f);

					EnterNewState(FSMState.STATE_IDLE);
				}
				break;

			default:
				break;
		}
	}

	/*
	 * ===========================================================================================================
	 * EVENTS STUFF
	 * ===========================================================================================================
	 */
	/// <summary>
	/// What to do when this object is enabled
	/// </summary>
	void OnEnable() {

		AstarAIFollow.onAIMovingChange += onAstarMovingChange;
	}

	/// <summary>
	/// What to do when this object is disabled
	/// </summary>
	void OnDisable() {

		AstarAIFollow.onAIMovingChange -= onAstarMovingChange;
	}

	void onAstarMovingChange(bool isMoving) {

		// AI gets to the end of the path and we were walking
		if(!isMoving && GetCurrentState() == FSMState.STATE_WALKING) {

			// Change the current state to IDLE then
			EnterNewState(FSMState.STATE_IDLE);
			Debug.LogWarning("Should be changing FSM state");
		}

	}

	/*
	 * ===========================================================================================================
	 * HELPERS
	 * ===========================================================================================================
	 */

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
