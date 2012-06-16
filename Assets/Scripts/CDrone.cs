using UnityEngine;
using System.Collections;

/// <summary>
/// Class with drones definitions
/// </summary>
public class CDrone : CBaseEntity {

	// PRIVATE
	
	// PUBLIC
	public enum eDroneType { Patrol, Saboteur, Hunter, NONE }; // Drones types
	public eDroneType droneType;
	public Transform stunnedParticleSystem;
	Transform stunnedObj = null;
	float fRecycleTimer;
	
	public AudioClip sfxSelected; // Played when the monkey is selected by the player
	public AudioClip sfxAttacked;	// Played when attacked (by a drone, for instance)
	public Transform transTarget;   // Target Transform
	public CBaseEntity.eObjType typeTarget; // Target type
	public float attackRange;      //  Attack Range to disable drones. Suggested values: hunter/2.2
	private Vector3 walkTo;
	
	public enum FSMState {
		STATE_IDLE,								// Doing nothing...
		STATE_SELECTED,						// Selected by the player
		STATE_WALKING,						// Drone walking around
		STATE_STUNNED,						// Drone stunned by an enemy, cannot move
		STATE_ATTACKING,					// Attacking an enemy
		STATE_BEING_RECYCLED,			// Being recycled by an enemy
		STATE_DESTROYED,					// Destroyed (recycled) by an enemy
    STATE_PURSUIT,            // Walk until the target is in range, then attack it // NOT USE FOR NOW
		STATE_PRISONER_MONKEY,		// Drone have a monkey as prisoner. Must head to the nearest center to leave it
		STATE_NULL								// null
	};
	
	FSMState eFSMCurrentState;	// Keeps the current FSM state
	float stunnedTimeCounter;
	public AstarAIFollow AIScript = null; // Cache a pointer to the AI script
	private float sabotageTime;

  // Sorry I know this is bad but i'm not smart enough right now to make it better
  public Saboteur saboteurScript;
  public Patrol patrolScript;
	public DroneHunter hunterAIScript;

	bool isThisAnEnemyDrone = false;

	/*
	 * ===========================================================================================================
	 * UNITY'S STUFF
	 * ===========================================================================================================
	 */

	/// <summary>
	/// When the script is initialized
	/// </summary>
	void Awake() {

    if(this.droneType == eDroneType.Patrol) patrolScript = this.gameObject.GetComponent<Patrol>();
    else if(this.droneType == eDroneType.Saboteur) saboteurScript = this.gameObject.GetComponent<Saboteur>();

		// Check if it is a CPU controlled drone (or opponent drones, for that matter)
		if(this.gameObject.layer == MainScript.enemyLayer) {

			// Check the type and get the component
			// AI hunter drone
			if(this.droneType == eDroneType.Hunter) {

				hunterAIScript = this.gameObject.GetComponent<DroneHunter>();

				// Set a flag to make easier for us
				isThisAnEnemyDrone = true;

				captureSpot = GetCaptureSpot();

				if(!hunterAIScript) {

					// DEBUG
					Debug.LogError("DroneHunter component not found in " + this.transform);
				}
			}
		}

		sabotageTime = 2.0f;
		// Set the default settings for all the drones
		if(!isThisAnEnemyDrone)
			Selectable = true; // All drones are selectable

		Movable = true; // All drones are movable
		Type = eObjType.Drone;
		
		AIScript = gameObject.GetComponent<AstarAIFollow>();
		
		// FSM setup
		eFSMCurrentState = FSMState.STATE_IDLE;

		// Starts the object variables, like the sweet spot and the main mesh object
		GetSweetSpotAndMeshObject();

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
		
		if(this.droneType == eDroneType.Patrol)
			mainScript.bottomMenu.PatrolDroneMenuEnable(this);
    else if(this.droneType == eDroneType.Saboteur)
          mainScript.bottomMenu.PlayerInfoMenuEnable(this);
		
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
	public FSMState GetCurrentState() {

		return eFSMCurrentState;
	}
	
	
	public void Update(){
		
		ExecuteCurrentState();
	}
	
	/// <summary>
	/// What to do when this drone is attacked?
	/// </summary>
	public override void Attacked(){
		
		Debug.Log("XXXX BEING ATTACKED");
		
		// TODO check who is attacking this drone
		// If is a monkey, check in CMonkey the rules; some monkeys cause the drone to be disabled, others to 
		// recycle it, etc
		EnterNewState(FSMState.STATE_STUNNED);
	}

  /// <summary>
  /// Attack the specified transTarget. Considering all the tests we need to do it's probably best just to send the gameobject. Or not gotta discuss
  /// </summary>
  /// <param name='transTarget'>
  /// Transform transTarget.
  /// </param>
  public void Attack(Transform transTarget){

    this.transTarget = transTarget;
    this.typeTarget = transTarget.gameObject.GetComponent<CBaseEntity>().Type;
    EnterNewState(FSMState.STATE_PURSUIT);
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
					if(isThisAnEnemyDrone)
						transTarget = null;
				}
				break;
			case FSMState.STATE_SELECTED:
				break;
			
			case FSMState.STATE_WALKING:
				// SET AISCRIPT TO MOVE TO TARGET
				//Debug.Log("XXXXXX Drone walking");
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

				// Do the animation
				if(meshObject) {

					meshObject.animation.Play("Deactivate");
				}

				stunnedTimeCounter = 10; // Stay stunned for 10 seconds.

				AIScript.Stop();

				// If have somebody captured, release it
				if(capturedEntity != null) {

					if(capturedEntity.Type == eObjType.Monkey) {

						// Cast CBaseEntity to CMonkey (it is actually a CMonkey instance, anyway)
						CMonkey monkeyEntity = capturedEntity as CMonkey;
						monkeyEntity.ReleaseMe();
					}
				}

				break;

			case FSMState.STATE_PURSUIT:
				AIScript.ClickedTargetPosition(transTarget.position);
				break;

			case FSMState.STATE_ATTACKING:
				// Get the target to attack
				break;

			case FSMState.STATE_BEING_RECYCLED:
				// Clears the timer
				fRecycleTimer = 0.0f;
				break;

			case FSMState.STATE_DESTROYED:
				// TODO: update the drones list before!
				Destroy(this.gameObject);
				break;

			case FSMState.STATE_PRISONER_MONKEY:
				{

					// DEBUG
					//Debug.Log(this.transform + " entering STATE_PRISONER_MONKEY");
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
	/// Executes the FSM current state
	/// </summary>
  void ExecuteCurrentState() {

		switch(GetCurrentState()) {

			case FSMState.STATE_IDLE:
				{
					// Do the floating animation
					if(meshObject) {
						//meshObject.animation.PlayQueued("Walk", QueueMode.CompleteOthers);
					}

					// CPU controlled enemy drones
					if(isThisAnEnemyDrone) {

						// Search for targets
						Transform tempTarget = hunterAIScript.CheckRadiusForAgents();

						if(tempTarget) {

							// There's a target...
							if(tempTarget != transTarget) {

								// ... and it is a new target
								transTarget = tempTarget;
								// Chase it!
								EnterNewState(FSMState.STATE_PURSUIT);
							}
						}
					}
				}
				break;

			case FSMState.STATE_SELECTED:
				break;

			case FSMState.STATE_WALKING:
				break;

			case FSMState.STATE_STUNNED:

				stunnedTimeCounter = stunnedTimeCounter - Time.deltaTime;
				if ( stunnedTimeCounter <=0)
					EnterNewState(FSMState.STATE_IDLE);
				break;

			case FSMState.STATE_PURSUIT:
				{
					//Go to target position and start attack
					if (transTarget == null){
						Debug.Log("TARGET INVALID");
						EnterNewState(FSMState.STATE_IDLE);
						break;
					}
					
					// Is an AI Hunter Drone attacking?
					if(isThisAnEnemyDrone && this.droneType == eDroneType.Hunter) {

						// Check if the target is in range
						if(mainScript.CheckIfTargetIsInRange(this.transform, transTarget, this.attackRange)) {

							// Target in range, switching to attack
							EnterNewState(FSMState.STATE_ATTACKING);
						}

						return;
					}
					
					Vector3 diffPursuit = transTarget.transform.position - gameObject.transform.position;
					float curDistancePursuit = diffPursuit.sqrMagnitude;

					// FIXME: distance must be at least the radius of the monkey collider plus the radius of the target collider
					if (curDistancePursuit < 50.0f)
					{
						EnterNewState(FSMState.STATE_ATTACKING);
					}
					else {
						// FIXME: it's working for a stationary target. But if the targets moves away? I guess we should
						// keep walking to the new target position
					}
				}
				break;

			case FSMState.STATE_ATTACKING:

				if (transTarget == null){
					EnterNewState(FSMState.STATE_IDLE);
					break;
				}

				// Enemy AI Hunter Drone behaviour
				if(isThisAnEnemyDrone && this.droneType == eDroneType.Hunter) {

					// DEBUG
					Debug.Log("Drone " + this.transform + " attacking " + transTarget.transform);

					CBaseEntity targetBaseEntity = transTarget.gameObject.GetComponent<CBaseEntity>();

					// Hunter drone vs Monkey
					if(targetBaseEntity.Type == CBaseEntity.eObjType.Monkey) {

						// Captures the monkey
						targetBaseEntity.CapturedBy(this.transform, this.captureSpot);
						this.capturedEntity = targetBaseEntity;

						// Change state
						EnterNewState(FSMState.STATE_PRISONER_MONKEY);
					}

					return;
				}

				Vector3 diffAttack = transTarget.transform.position - gameObject.transform.position;       
				float curDistanceAttack = diffAttack.sqrMagnitude; 

				// FIXME: This is just a temporary attack on drones, sabotaged and stunned should be 2 different states
				if (curDistanceAttack < 20.0f)
				{
					CDrone droneTarget = transTarget.gameObject.GetComponent<CDrone>();
					if (droneTarget != null)
					{
						droneTarget.EnterNewState(CDrone.FSMState.STATE_STUNNED);
						EnterNewState(FSMState.STATE_IDLE);
					}

				}

				if(this.droneType == eDroneType.Saboteur && this.typeTarget == CBaseEntity.eObjType.Building)
				{
					sabotageTime = sabotageTime - Time.deltaTime;
					if(sabotageTime < 0)
					{
						Debug.LogWarning("Sabotage target: " + transTarget);
						saboteurScript.SabotageBuilding(transTarget.gameObject);
						sabotageTime = 2.0f;
						EnterNewState(FSMState.STATE_IDLE);
					}
				}
				break;

			case FSMState.STATE_BEING_RECYCLED:
				fRecycleTimer += Time.deltaTime;
				if(fRecycleTimer > 5.0f) {

					// Give the metal to the player
					mainScript.player.AddResourceMetal(5.0f);	// FIXME: each drone must have a resource value

					// And vanishes with the drone
					EnterNewState(FSMState.STATE_DESTROYED);
				}
				break;

			case FSMState.STATE_PRISONER_MONKEY:
				{

					// DEBUG
					//Debug.Log(this.transform + " executing STATE_PRISONER_MONKEY");
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

					// Do the stunned animation
					if(meshObject) {

						meshObject.animation.Play("Reactivate");
					}

					// Destroys the 'stunned particle system', if exists
					if(stunnedObj)
						Destroy(stunnedObj.gameObject);
				}
				break;

			case FSMState.STATE_ATTACKING:
				transTarget = null;
				break;

			case FSMState.STATE_BEING_RECYCLED:
				break;

			case FSMState.STATE_DESTROYED:
				break;

			case FSMState.STATE_PURSUIT:
				//Go to target position and start attack
				break;

			case FSMState.STATE_PRISONER_MONKEY:
				{

					// DEBUG
					//Debug.Log(this.transform + " leaving STATE_PRISONER_MONKEY");
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
	/// Helper function to tell if a drone is in the STATE_STUNNED or no. Useful for monkeys attacking this drone
	/// </summary>
	public bool isStunned(){

		return (eFSMCurrentState == FSMState.STATE_STUNNED);
	}
	

	/// <summary>
	/// What to do when the drone is being recycled
	/// </summary>
	public void Recycled() {

		// DEBUG
		Debug.Log("Drone being recycled");

		EnterNewState(FSMState.STATE_BEING_RECYCLED);
	}

	/// <summary>
	/// This drone has been reprogrammed, so it will change it's team
	/// </summary>
	public void Reprogrammed() {

		// Change the drone layer and team
		if(this.gameObject.layer == MainScript.enemyLayer) {

			this.gameObject.layer = MainScript.alliedLayer;
			this.Team = eObjTeam.Allied;

		}
		else {

			this.gameObject.layer = MainScript.enemyLayer;
			this.Team = eObjTeam.Opponent;
		}

		// Updates the drone list
		MainScript.Script.ChangeDroneTeamAndUpdateList(this.gameObject.transform, this.gameObject.layer);

		// Change the state of the drone
		EnterNewState(FSMState.STATE_IDLE);
	}
}
