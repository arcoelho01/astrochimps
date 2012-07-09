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
	//< Prefab to a sparks particle system. Will be used when the drone is reprogrammed
	public Transform prefabSparks;
	//< Object to keep the sparks
	Transform sparksObj;

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
    STATE_DISABLED,            // Drone sabotaged by an enemy, cannot move
		STATE_NULL								// null
	};
	
	FSMState eFSMCurrentState;	// Keeps the current FSM state
	float stunnedTimeCounter;
  float disabledTimeCounter;
	public AstarAIFollow AIScript = null; // Cache a pointer to the AI script
	private float sabotageTime;

  // Sorry I know this is bad but i'm not smart enough right now to make it better
  public Saboteur saboteurScript;
  public Patrol patrolScript;
  public DronePatrol patrolDroneScript;
	public DroneHunter hunterAIScript;
  public EnemyPatrol enemyPatrolScript;

	bool isThisAnEnemyDrone = false;

	//< This drone was reprogrammed?
	bool bnWasReprogrammed;
	//< If so, this will keep it's life time
	float fWasReprogrammedTimer = 0.0f;
	//< Time that the drone will last after being reprogrammed
	float fWasReprogrammedTargetTime = 5.0f;

	//< The character controller for this drone
	CharacterController myController;

	/*
	 * ===========================================================================================================
	 * UNITY'S STUFF
	 * ===========================================================================================================
	 */

	/// <summary>
	/// When the script is initialized
	/// </summary>
	void Awake() {

    if(this.droneType == eDroneType.Patrol){
      patrolDroneScript  = this.gameObject.GetComponent<DronePatrol>();
      //patrolScript = this.gameObject.GetComponent<Patrol>();
    }
    else if(this.droneType == eDroneType.Saboteur) saboteurScript = this.gameObject.GetComponent<Saboteur>();

		// Check if it is a CPU controlled drone (or opponent drones, for that matter)
		if(this.gameObject.layer == MainScript.enemyLayer) {

        // Set a flag to make easier for us
       isThisAnEnemyDrone = true;

			// Check the type and get the component
			// AI hunter drone
			if(this.droneType == eDroneType.Hunter) {

				hunterAIScript = this.gameObject.GetComponent<DroneHunter>();

				captureSpot = GetCaptureSpot();
				captureRaySpot = GetCaptureRaySpot();

				if(!hunterAIScript) {

					// DEBUG
					Debug.LogError("DroneHunter component not found in " + this.transform);
				}
			}

      if(this.droneType == eDroneType.Patrol) {

        enemyPatrolScript = this.gameObject.GetComponent<EnemyPatrol>();
        // Set a flag to make easier for us
       //isThisAnEnemyDrone = true;

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

		// Get info about the collider
		GetColliderInfo();

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
		
		selectorRadius = myController.radius;
		return base.Select();
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
		
		// FIXME: Updates the Reprogrammed Timer here. Is this the best place?
		if(bnWasReprogrammed)
			fWasReprogrammedTimer += Time.deltaTime;
		
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

  public void Patrolling (){
    EnterNewState(FSMState.STATE_WALKING);
  }

  public void Waiting (){
    EnterNewState(FSMState.STATE_IDLE);
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
				//fRecycleTimer = 0.0f;
				break;

			case FSMState.STATE_DESTROYED:
				{

					// Play the deactivation animation and then fade out the drone
					StartCoroutine(DestroyDrone());
				}
				break;

			case FSMState.STATE_PRISONER_MONKEY:
				{

					// DEBUG
					//Debug.Log(this.transform + " entering STATE_PRISONER_MONKEY");

					// Find the nearest prison for the captured monkey
					transTarget = hunterAIScript.GetNearestPrison();
			
					if(transTarget) {

						CBuilding targetBuildingScript = transTarget.GetComponent<CBuilding>();

						if(!targetBuildingScript) {

							// DEBUG
							Debug.LogError(this.transform + " Could not find CBuilding component.");
						}

						// Move to the building
						AIScript.ClickedTargetPosition(targetBuildingScript.GetExitSpotPosition());
					}
				}
				break;

    case FSMState.STATE_DISABLED:

       if(sfxAttacked) {

         AudioSource.PlayClipAtPoint(sfxAttacked, transform.position);
       }

       // Do the animation
       //if(meshObject){

         //meshObject.animation.Play("Deactivate");
       //}

       // If have somebody captured, release it
       if(capturedEntity != null) {

         if(capturedEntity.Type == eObjType.Monkey) {

           // Cast CBaseEntity to CMonkey (it is actually a CMonkey instance, anyway)
           CMonkey monkeyEntity = capturedEntity as CMonkey;
           monkeyEntity.ReleaseMe();
         }
       }

      disabledTimeCounter = 8; // Stay disabled for 8 seconds.

      this.Deselect();
      this.Selectable = false;

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
	/// Executes the FSM current state
	/// </summary>
  void ExecuteCurrentState() {

		// Reprogrammed and timer is over?
		if(bnWasReprogrammed && fWasReprogrammedTimer >= fWasReprogrammedTargetTime 
				&& GetCurrentState() != FSMState.STATE_DESTROYED) {

			EnterNewState(FSMState.STATE_DESTROYED);
		}

		switch(GetCurrentState()) {

			case FSMState.STATE_IDLE:
				{
					// Do the floating animation
					if(meshObject) {
						//meshObject.animation.PlayQueued("Walk", QueueMode.CompleteOthers);
					}

					// CPU controlled enemy drones
					if(isThisAnEnemyDrone && this.droneType == eDroneType.Hunter) {

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
          if(isThisAnEnemyDrone && this.droneType == eDroneType.Saboteur) {

           // Search for targets
           Transform tempTarget = saboteurScript.CheckRadiusForAgents();

           if(tempTarget) {

             CDrone droneTempTarget = tempTarget.gameObject.GetComponent<CDrone>();
             // There's a target...
             if(!(droneTempTarget.GetCurrentState() == FSMState.STATE_DISABLED) && (tempTarget != transTarget) ) {
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
          // Is an AI Saboteur Drone attacking?
         if(isThisAnEnemyDrone && this.droneType == eDroneType.Saboteur) {

           // Check if the target is in range
           if(mainScript.CheckIfTargetIsInRange(this.transform, transTarget, this.attackRange)) {
             //Debug.LogWarning("Entering Attack state from Pursuit state");
             // Target in range, switching to attack
             EnterNewState(FSMState.STATE_ATTACKING);
           }

           return;
         }
					

          if(!isThisAnEnemyDrone && this.droneType == eDroneType.Saboteur) {

            if(mainScript.CheckIfTargetIsInRange(this.transform, transTarget, this.attackRange)) {
             // Target in range, switching to attack
             EnterNewState(FSMState.STATE_ATTACKING);
           }

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
						targetBaseEntity.CapturedBy(this.transform, this.captureSpot, this.captureRaySpot);
						this.capturedEntity = targetBaseEntity;

						// Change state
						EnterNewState(FSMState.STATE_PRISONER_MONKEY);
					}

					return;
				}

        if(this.droneType == eDroneType.Saboteur){

          if(isThisAnEnemyDrone) {

           CDrone droneTarget = transTarget.gameObject.GetComponent<CDrone>();

           if (droneTarget != null){

              droneTarget.EnterNewState(CDrone.FSMState.STATE_DISABLED);

              EnterNewState(FSMState.STATE_IDLE);
           }
          }else{
            if(this.typeTarget == CBaseEntity.eObjType.Building){
              //sabotageTime = sabotageTime - Time.deltaTime;

              //if(sabotageTime < 0){
                saboteurScript.SabotageBuilding(transTarget.gameObject);
                //sabotageTime = 2.0f;
                EnterNewState(FSMState.STATE_IDLE);
              //}
            }
           if(this.typeTarget == CBaseEntity.eObjType.Drone){
              CDrone droneTarget = transTarget.gameObject.GetComponent<CDrone>();
              if (droneTarget != null){

                droneTarget.EnterNewState(CDrone.FSMState.STATE_DISABLED);

                EnterNewState(FSMState.STATE_IDLE);
              }
           }
          }

      }

			break;

			case FSMState.STATE_BEING_RECYCLED:
				{
					// Give the metal to the player
					mainScript.player.AddResourceMetal(5.0f);	// FIXME: each drone must have a resource value

					// And vanishes with the drone
					EnterNewState(FSMState.STATE_DESTROYED);
				}
				break;

			case FSMState.STATE_DESTROYED:
				break;

			case FSMState.STATE_PRISONER_MONKEY:
				{

					// DEBUG
					//Debug.Log(this.transform + " executing STATE_PRISONER_MONKEY");

					// Check if we have reached the prison already
					
				}
				break;

    case FSMState.STATE_DISABLED:

       disabledTimeCounter = disabledTimeCounter - Time.deltaTime;

       if ( disabledTimeCounter <= 0)
         EnterNewState(FSMState.STATE_IDLE);

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

					// Do the reactivate animation, in case the drone is not being recycled already
					if(meshObject && GetCurrentState() != FSMState.STATE_BEING_RECYCLED) {

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

      case FSMState.STATE_DISABLED:

         this.Selectable = true;

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

		return (GetCurrentState() == FSMState.STATE_STUNNED);
	}
	

	/// <summary>
	/// What to do when the drone is being recycled
	/// </summary>
	public void Recycled() {

		// DEBUG
		//Debug.Log("Drone being recycled");

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

		// Starts the 'time bomb' for this drone. Reprogrammed drones only lasts a few seconds in player's control,
		// then self destruct
		bnWasReprogrammed = true;
		fWasReprogrammedTimer = 0.0f;

		// Add a visual aid
		if(prefabSparks) {

			// Instantiate the particle system
			sparksObj = Instantiate(prefabSparks, this.transform.position + Vector3.up, 
					Quaternion.Euler(-90,0,00)) as Transform;

			// Put it as child
			sparksObj.transform.parent = this.transform;
		}
	}

	/// <summary>
	/// Plays a final animation and, when it's over, destroy the drone object
	/// </summary>
	IEnumerator DestroyDrone() {

		// debug
		Debug.LogWarning(this.transform + " playing animation");
		meshObject.animation.Play("Deactivate");

		// Wait for the animation to finish
		yield return new WaitForSeconds(meshObject.animation["Deactivate"].clip.length);

		// Fade out the drone object
		FadeOutDrone();
	}

	/// <summary>
	/// Uses iTween to fade out the destroyed drone
	/// </summary>
	void FadeOutDrone() {

		// Creates a new hashtable
		Hashtable ht = new Hashtable();

		// Alpha should be 0
		ht.Add("alpha",0);
		// What to call when the animation is over
		ht.Add("oncomplete","DestroyThisDrone");
		// Where is the function to call when the animation is over. We need this because the animation is
		// over the meshObject, not this object
		ht.Add("oncompletetarget",this.gameObject);
		// Call the iTween
		iTween.FadeTo(meshObject.gameObject, ht);
	}

	/// <summary>
	/// Actually destroy the drone game object
	/// </summary>
	void DestroyThisDrone() {

		// Destroy the object
		Destroy(this.gameObject);
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

		AstarAIFollow.OnReachedEndOfPath += OnAStarReachedEndOfPath;
	}

	/// <summary>
	/// What to do when this object is disabled
	/// </summary>
	void OnDisable() {

		AstarAIFollow.OnReachedEndOfPath -= OnAStarReachedEndOfPath;
	}

	/// <summary>
	/// What to do when this object is destroyed
	/// </summary>
	void OnDestroy() {

		AstarAIFollow.OnReachedEndOfPath -= OnAStarReachedEndOfPath;
	}

	/// <summary>
	/// This method is called when the Astar reaches the end of the path
	/// </summary>
	void OnAStarReachedEndOfPath(Transform eventRaiser, bool isMoving) {

		// Ignores if the stop event wasn't generate by the monkey itself
		if(eventRaiser != this.transform)
			return;

		// FIXME: remove this filtering, it's here so it doesn't affects other drones
		if(this.droneType == eDroneType.Hunter && isThisAnEnemyDrone ) {

			if(!isMoving && GetCurrentState() == FSMState.STATE_WALKING) {

				// Reached the target position
				EnterNewState(FSMState.STATE_IDLE);
			}

			// HUNTER DRONE BEHAVIOUR - Prisoner being delivered
			if(!isMoving && GetCurrentState() == FSMState.STATE_PRISONER_MONKEY) {

				Debug.LogWarning(this.transform + " reached prisoner delivery position " + eventRaiser);
				// Incarcerated the prisoner in the building
				hunterAIScript.DeliverPrisoner(transTarget, this.capturedEntity);
				// Back to idle
				EnterNewState(FSMState.STATE_IDLE);
			}
		}else if(this.droneType == eDroneType.Patrol){

       patrolDroneScript.bGoToNextMarker = true;

    }
	}

  public override void Deselect() {

   if((this.droneType == eDroneType.Patrol) && patrolDroneScript.bSetNewPatrol)
      patrolDroneScript.RevertPatrol();

   base.Deselect();

 }
}
