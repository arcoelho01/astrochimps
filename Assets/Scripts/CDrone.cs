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
	public float attackRange;      //  Attack Range to disable drones.
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
		STATE_NULL								// null
	};
	
	FSMState eFSMCurrentState;	// Keeps the current FSM state
	float stunnedTimeCounter;
	private AstarAIFollow AIScript = null; // Cache a pointer to the AI script
	private float sabotageTime;

  // Sorry I know this is bad but i'm not smart enough right now to make it better
  private Saboteur saboteurScript;
  private Patrol patrolScript;

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

		sabotageTime = 2.0f;
		// Set the default settings for all the buildings
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
  /// Attack the specified transTarget. Considering all the tests we need to do it's probably best just to send the gameobject. Or not gotta discuss
  /// </summary>
  /// <param name='transTarget'>
  /// Transform transTarget.
  /// </param>
  	public void Attack(Transform transTarget){

    this.transTarget = transTarget;
    this.typeTarget = transTarget.gameObject.GetComponent<CBaseEntity>().Type;
    //EnterNewState(FSMState.STATE_PURSUIT);
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
				Destroy(this.gameObject);
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
     			//Go to target position and start attack
				if (transTarget == null){
					Debug.Log("TARGET INVALID");
					EnterNewState(FSMState.STATE_IDLE);
					break;
				}

				// DEBUG
				//Debug.Log("TARGET VALID");

				Vector3 diffPursuit = transTarget.transform.position - gameObject.transform.position;
				float curDistancePursuit = diffPursuit.sqrMagnitude; 

				// FIXME: distance must be at least the radius of the monkey collider plus the radius of the target 
				// collider
				if (curDistancePursuit < 50.0f)
				{
					EnterNewState(FSMState.STATE_ATTACKING);
				}
				else {
					// FIXME: it's working for a stationary target. But if the targets moves away? I guess we should
					// keep walking to the new target position
					// DEBUG
					Debug.Log("Distance from target: " + curDistancePursuit);
				}
     			break;
			case FSMState.STATE_ATTACKING:
				// LEO: POR ENQUANTO ELE NAO ESTA SE MOVIMENTANDO EM DIRECAO AO INIMIGO POIS JA ESTA SENDO FEITO FORA DAQUI, MAS ACHO QUE DEVERIA ENTRAR NO WALKING E TB NO ATTACKING
				if (transTarget == null){
					EnterNewState(FSMState.STATE_IDLE);
					break;
				}
				Vector3 diffAttack = transTarget.transform.position - gameObject.transform.position;       
				float curDistanceAttack = diffAttack.sqrMagnitude; 
				
				if (curDistanceAttack < 20.0f)
				{
					CDrone droneTarget = transTarget.gameObject.GetComponent<CDrone>();
					if (droneTarget != null){
						droneTarget.EnterNewState(CDrone.FSMState.STATE_STUNNED);
						EnterNewState(FSMState.STATE_IDLE);
					}
					
				}//*/
        		if(this.droneType == eDroneType.Saboteur && this.typeTarget == CBaseEntity.eObjType.Building){
					sabotageTime = sabotageTime - Time.deltaTime;
					if(sabotageTime < 0){
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
			case FSMState.STATE_PURSUIT:
     			//Go to target position and start attack
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
