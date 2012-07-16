using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class with monkey's definitions
/// </summary>
public class CMonkey : CBaseEntity {

	// PRIVATE

	// PUBLIC
	public enum eMonkeyType { Astronaut, Cientist, Engineer, Saboteur, NONE }; // Monkeys types

	public AudioClip sfxSelectedDefault; // Played when the monkey is selected by the player
	public AudioClip sfxSelectedIdle; // Played when the monkey is selected by the player when Idle
	public AudioClip sfxSelectedAstronaut; // Played when the monkey Astronaut is selected by the player
	public AudioClip sfxSelectedCientist; // Played when the monkey Cientist is selected by the player
	public AudioClip sfxSelectedEngineer; // Played when the monkey Engineer is selected by the player
	public AudioClip sfxSelectedSaboteur; // Played when the monkey Saboteur is selected by the player	
	public AudioClip sfxAttacked;	// Played when attacked (by a drone, for instance)
	public AudioClip sfxAckDefault;	// Played when the monkey received and acknowledged an order
	public AudioClip sfxAckAstronaut;	// Played when the monkey received and acknowledged an order
	public AudioClip sfxAckCientist;	// Played when the monkey received and acknowledged an order
	public AudioClip sfxAckEngineer;	// Played when the monkey received and acknowledged an order
	public AudioClip sfxAckSaboteur;	// Played when the monkey received and acknowledged an order
	public AudioClip sfxResearchComplete;	// Played when the monkey received and acknowledged an order
	public AudioClip sfxAttackAstronaut;	// Played when the monkey is attacking a target
	public AudioClip sfxAttackCientist;	// Played when the monkey is attacking a target
	public AudioClip sfxAttackEngineer;	// Played when the monkey is attacking a target
	public AudioClip sfxAttackSaboteur;	// Played when the monkey is attacking a target
	public AudioClip sfxAttackAckAstronaut;	// Played when the monkey is attacking a target
	public AudioClip sfxAttackAckCientist;	// Played when the monkey is attacking a target
	public AudioClip sfxAttackAckEngineer;	// Played when the monkey is attacking a target
	public AudioClip sfxAttackAckSaboteur;	// Played when the monkey is attacking a target
	public AudioClip sfxAttackedAstronaut;	// Played when the monkey is attacking a target
	public AudioClip sfxAttackedCientist;	// Played when the monkey is attacking a target
	public AudioClip sfxAttackedEngineer;	// Played when the monkey is attacking a target
	public AudioClip sfxAttackedSaboteur;	// Played when the monkey is attacking a target
	public AudioClip sfxReprogramming; //< Played when the Saboteur is reprogramming a drone
	public AudioClip sfxDying; //< Played when the monkey dies

	AudioClip sfxWorking; //< Wherever sound this monkey should do while working

	private Transform transTarget;   // Target Transform
	private Vector3 walkTo;
	public float attackRange;      //  Attack Range to disable drones.

	public enum FSMState {
			STATE_IDLE,							// Doing nothing...
			STATE_INSIDE_BUILDING,	// when the monkey is inside a building, cannot move
			STATE_WALKING,					// Monkey walking around
			STATE_STUNNED,					// Monkey stunned by an enemy drone, cannot move
			STATE_ATTACKING,				// Attacking an enemy drone
			STATE_PURSUIT,					// Walk until the target is in range, then attack it
			STATE_WORKING,					// Monkey working on something. Action that requires a certain time
			STATE_CAPTURED,					// Monkey was captured by an enemy
			STATE_DEAD,							// Monkey died asphyxiated
			STATE_NULL							// null
	};

	FSMState eFSMCurrentState;	// Keeps the current FSM state
	private AstarAIFollow AIScript = null; // Cache a pointer to the AI script
	private float stunnedTimeCounter;
	public eMonkeyType monkeyClass; // Which class this monkey belongs

	CharacterController myController;
	public Transform iconPrefab;

	Vector3 v3Direction;
	float fAttackRange = 2.2f;// FIXME
	MouseWorldPosition.eMouseStates mouseState;	//< The mouse state when the player issued an order to the monkey
	public MouseWorldPosition.eMouseStates workingMouseState;
	float fWorkingTimer = 0.0f;	//< Timer for the working state
	float fWorkingTargetTime = 0.0f; //< Time needed to perform a task. When working timer is bigger than this, 
	// the task is done
	
	float fResearchTimer;	//< Timer to research something, like the cientist trying to find the rocket parts
	float fResearchTargetTime; //< Time needed to complete the research above
	bool bnResearchIsComplete; //< Is the research complete already?
	bool bnCanResearchForRocketParts; //< The cientist can research only when the Command Center and the Research Lab are fully functional
	private GUIControl GUIScript;
	Transform attackSpot;

	//< Progress bar showed in the Monkey Panel.
	Transform tProgressBar;

	//< Time needed to sabotage a drone
	float fDroneSabotageTime = 1.5f;
	//< Time needed to sabotage a building
	float fBuildingSabotageTime = 3.5f;
	//< Time needed to fix a sabotaged building
	float fBuildingFixTime = 5.0f;
	//< Time needed to recycle a stunned drone
	float fDroneRecycleTime = 4.0f;
	//< Time needed to reprogram a stunned drone
	float fDroneReprogramTime = 5.0f;

	//< ANIMATIONS

	//< Name of the animation to play in loop when this monkey is working is something that takes time
	string stAnimWalk = "walk";
	string stAnimIdle = "idle";
	string stAnimationWorking = "repairing";
	string stDyingAnimation = "dying";
	string stAnimTargetingForBrawl = "punching";
	string stAnimEngineerFix = "repairing";
	string stAnimTargetingForRecycle = "repairing";
//	string stAnimCanCapture = "";
	string stAnimTargetingForReprogram = "reprogramming";
	string stAnimCanSabotageBuilding = "repairing";
	string stAnimCanSabotageDrone = "repairing";

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
		if(monkeyClass == eMonkeyType.Cientist) {
		
			captureSpot = GetCaptureSpot();
			captureRaySpot = GetCaptureRaySpot();

		}

		// Get's the spot where we will test for collisions used in the attack
		attackSpot = GetAttackSpot();
		
		GUIScript = GameObject.Find("HUD-Objects").GetComponent<GUIControl>();
		GUIScript.addMonkey(this);
		
	}

	/// <summary>
	/// Update
	/// </summary>
	void Update(){

		ExecuteCurrentState();
	}
	
	
	public FSMState getFSMCurrentState(){
		return eFSMCurrentState;
	}
	
	public bool getResearchIsComplete(){
		return bnResearchIsComplete;
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
				// DEBUG
				//Debug.LogWarning(this.transform + " clearing target on entering STATE_IDLE");
				break;

			case FSMState.STATE_INSIDE_BUILDING:
				{
					// Cientist monkey only
					if(monkeyClass == eMonkeyType.Cientist) {

						// Resets the timer
						fResearchTimer = 0.0f;
						fResearchTargetTime = 10.0f;
					}
				}
				break;

			case FSMState.STATE_WALKING:
				// DEBUG
				Debug.Log(this.transform + " Entering STATE_WALKING");
				if(sfxAckDefault) {

					AudioSource.PlayClipAtPoint(sfxAckDefault, transform.position);
				}
				AIScript.ClickedTargetPosition(walkTo);

				// DEBUG
				//Debug.LogWarning(this.transform + " clearing target on STATE_WALKING");

				// Clear the target
				transTarget = null;

				// Plays the animation for the walk cycle
				if(meshObject) {

					meshObject.animation.Play(stAnimWalk);
				}

				break;

			case FSMState.STATE_STUNNED:
				stunnedTimeCounter = 10; // Stay stunned for 10 seconds.
				AIScript.Stop();
				break;

			case FSMState.STATE_ATTACKING:
				// SET AISCRIPT TO MOVE TO TARGET
				break;

			case FSMState.STATE_PURSUIT:
				{
					
					if(!transTarget) {

						// DEBUG
						Debug.LogWarning(this.transform + " STATE_PURSUIT received a null target!");
					}
					else {
						
						AIScript.ClickedTargetPosition(transTarget.position);
					}

					// Plays the animation for the walk cycle
					if(meshObject) {

						meshObject.animation.Play(stAnimWalk);
					}
				}
				break;

			case FSMState.STATE_WORKING:
				{
					// DEBUG
					Debug.Log(this.transform + " FSM entered WORKING state");
					// Resets the working timer
					fWorkingTimer = 0.0f;
					// Instantiate a new progress bar
					if(!tProgressBar) {

						tProgressBar = Instantiate(progressBarPrefab, sweetSpotObj.transform.position, 
								Quaternion.identity) as Transform;
					}

					if(sfxWorking) {

						AudioSource.PlayClipAtPoint(sfxWorking, transform.position);
					}
				}
				break;

			case FSMState.STATE_CAPTURED:
				{
					// DEBUG
					//Debug.Log(this.transform + " [Entering STATE_CAPTURED]");
					this.Deselect();
					this.Selectable = false;
				}
				break;

			case FSMState.STATE_DEAD:
				{

					// Play the dying animation
					if(meshObject) {

						if(meshObject.animation.isPlaying)
							meshObject.animation.CrossFade(stDyingAnimation);
						else
							meshObject.animation.Play(stDyingAnimation);
					}
					
					if(sfxDying) {

						AudioSource.PlayClipAtPoint(sfxDying, this.transform.position);
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
	/// Executes the FSM current state
	/// </summary>
	void ExecuteCurrentState() {

		switch(GetCurrentState()) {

			case FSMState.STATE_IDLE:
				{
					// DEBUG
					//Debug.Log("[ExecuteCurrentState: " + GetCurrentState() + "]");
					//if(!meshObject.animation.IsPlaying(stAnimIdle)) {
					if(!meshObject.animation.isPlaying) {

						meshObject.animation.Play(stAnimIdle);
					}
				}
				break;

			case FSMState.STATE_INSIDE_BUILDING:
				{

					// Cientist monkey only: researching to reveal the rocket parts
					if(monkeyClass == eMonkeyType.Cientist && !bnResearchIsComplete && bnCanResearchForRocketParts) {

						// Instantiate a new progress bar
						if(!tProgressBar) {

							tProgressBar = Instantiate(progressBarPrefab, sweetSpotObj.transform.position, 
									Quaternion.identity) as Transform;
						}

						// Updates the timer
						fResearchTimer += Time.deltaTime;

						// Updates the progress bar
						if(tProgressBar) {	
							

							tProgressBar.gameObject.GetComponent<ProgressBar>().UpdateIncreaseBar(
									fResearchTimer, fResearchTargetTime);
						}

						// Is the research done?
						if(fResearchTimer >= fResearchTargetTime) {

							bnResearchIsComplete = true;
							playResearchComplete();

							//
							CientistRevealedRocketParts();

							// If we're used a progress bar, now we get rid of it
							if(tProgressBar) {
						
								Destroy(tProgressBar.gameObject);
								tProgressBar = null;
								
							}
						}
					}
				}
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
				Debug.Log(this.transform + " Executing attack");
				MonkeyAttack();
				break;

			case FSMState.STATE_PURSUIT:
				{
					// ADDED by Alexandre: pursuit is the combination of walk and attack modes. First, walk to the target. 
					// Here, test if the target is in range, then switch to the ATTACK state
					// IF BELLOW IS TO CHECK IF THE TARGET STILL EXISTS
					if (transTarget == null){
						// DEBUG
						Debug.Log(this.transform + " TARGET INVALID");
						EnterNewState(FSMState.STATE_IDLE);
						break;
					}

					if(mainScript.CheckIfTargetColliderIsInRange(attackSpot.transform.position, 
								transTarget, transTarget.gameObject.layer, fAttackRange)) {

						EnterNewState(FSMState.STATE_ATTACKING);
					}
				}
				break;

			case FSMState.STATE_WORKING:
				{
					// Updates the working timer
					fWorkingTimer += Time.deltaTime;

					// Updates the progress bar
					if(tProgressBar) {

						tProgressBar.gameObject.GetComponent<ProgressBar>().UpdateIncreaseBar(fWorkingTimer, fWorkingTargetTime);
					}

					if(fWorkingTimer >= fWorkingTargetTime) {

						fWorkingTimer = 0.0f;

						WorkIsDone();
					}

					if(!meshObject.animation.IsPlaying(stAnimationWorking)) {

						meshObject.animation.Play(stAnimationWorking);
					}
				}
				break;

			case FSMState.STATE_CAPTURED:
				break;

			case FSMState.STATE_DEAD:
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
					Debug.Log(this.transform + " [LeaveCurrentState: " + GetCurrentState() + "]");
				}
				break;

			case FSMState.STATE_INSIDE_BUILDING:
				{
					// If we're used a progress bar, now we get rid of it
					if(tProgressBar) {

						Destroy(tProgressBar.gameObject);
						tProgressBar = null;
					}
				}
				break;

			case FSMState.STATE_WALKING:
				// DEBUG
				Debug.Log(this.transform + " Leaving STATE_WALKING");
				// FIXME: the line below is causing the game to lock. This is because we getting in here for an event
				// when the AI stops, so it make no sense to call Stop() again.
				// But when we get here by other ways, like walking and then issuing an attack command?
				if(AIScript.bnIsMoving)
					AIScript.Stop();

				// Stops the walk cycle
				if(meshObject) {

					//meshObject.animation.Stop("Walk");
					meshObject.animation.CrossFade(stAnimIdle);
				}

				break;

			case FSMState.STATE_STUNNED:
				//AIScript.Resume();
				Debug.Log(this.transform + " Leaving [STATE_STUNNED]");
				break;

			case FSMState.STATE_ATTACKING:
				break;

			case FSMState.STATE_PURSUIT:
				// Stop the 'walk to the target'
				AIScript.Stop();

				// Stops the walk cycle
				if(meshObject) {

					//meshObject.animation.Stop("Walk");
					meshObject.animation.CrossFade(stAnimIdle);
				}

				break;

			case FSMState.STATE_WORKING:
				{
					// DEBUG
					Debug.Log(this.transform + " FSM leaving WORKING state");

					// If we're used a progress bar, now we get rid of it
					if(tProgressBar) {

						Destroy(tProgressBar.gameObject);
						tProgressBar = null;
					}
				}
				break;

			case FSMState.STATE_CAPTURED:
				{
					// DEBUG
					//Debug.Log(this.transform + " [Leaving STATE_CAPTURED]");

					// The monkey can be selected again
					this.Selectable = true;
				}
				break;

			case FSMState.STATE_NULL:
				break;

			case FSMState.STATE_DEAD:
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
	/// Selected is overriden to play a sound
	/// </summary>
	public override Transform Select() {

			playSelectedSound();
		

		selectorRadius = myController.radius;
		return base.Select();
	}



	/// <summary>
	/// Used to send a monkey to a point in the terrain. It will keep the target's position and change to
	/// the 'walking' state
	/// </summary>
	/// <param name="walkTo"> A Vector3 with the position to walk to </param>
	public void WalkTo(Vector3 walkTo){

		this.walkTo = walkTo;
		EnterNewState(FSMState.STATE_WALKING);
	}

	/// <summary>
	///
	/// </summary>
	public void KillMonkey() {

		EnterNewState(FSMState.STATE_DEAD);
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

		// Direction from here to the target
		v3Direction = transTarget.transform.position - transform.position;

		if(Physics.Raycast(transform.position, v3Direction, out hit, fAttackRange)) {

			Debug.Log("Monkey hit in " + hit.transform.name);

			if(!transTarget) {

				// DEBUG
				Debug.LogError(this.transform + " trying to attack without a target.");
			}

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

		// DEBUG
		Debug.LogWarning(this.transform + " performing " + mouseState);

		// Go into pursuit mode -> walk to the target. When it is in range, perform the action
		EnterNewState(FSMState.STATE_PURSUIT);
	}
	
	void playAckSound(){
		if (monkeyClass == eMonkeyType.Astronaut){
			if (sfxAckAstronaut)
				AudioSource.PlayClipAtPoint(sfxAckAstronaut, transform.position);
		}else if (monkeyClass == eMonkeyType.Cientist){
			if (sfxAckCientist)
				AudioSource.PlayClipAtPoint(sfxAckCientist, transform.position);
		}else if (monkeyClass == eMonkeyType.Engineer){
			if (sfxAckEngineer)
				AudioSource.PlayClipAtPoint(sfxAckEngineer, transform.position);
		}else if (monkeyClass == eMonkeyType.Saboteur){
			if (sfxAckSaboteur)
				AudioSource.PlayClipAtPoint(sfxAckSaboteur, transform.position);
		}else 
			if (sfxAckDefault)
				AudioSource.PlayClipAtPoint(sfxAckDefault, transform.position);			
	}
	void playSelectedSound(){
		if(eFSMCurrentState == FSMState.STATE_IDLE)
			if (sfxSelectedIdle){
				AudioSource.PlayClipAtPoint(sfxSelectedIdle, transform.position);
				return;
		}
		
		if (monkeyClass == eMonkeyType.Astronaut){
			if (sfxSelectedAstronaut)
				AudioSource.PlayClipAtPoint(sfxSelectedAstronaut, transform.position);
		}else if (monkeyClass == eMonkeyType.Cientist){
			if (sfxSelectedCientist)
				AudioSource.PlayClipAtPoint(sfxSelectedCientist, transform.position);
		}else if (monkeyClass == eMonkeyType.Engineer){
			if (sfxSelectedEngineer)
				AudioSource.PlayClipAtPoint(sfxSelectedEngineer, transform.position);
		}else if (monkeyClass == eMonkeyType.Saboteur){
			if (sfxSelectedSaboteur)
				AudioSource.PlayClipAtPoint(sfxSelectedSaboteur, transform.position);
		}		
	}
	
	void playAttackSound(){
		
		if (monkeyClass == eMonkeyType.Astronaut){
			if (sfxAttackAstronaut)
				AudioSource.PlayClipAtPoint(sfxAttackAstronaut, transform.position);
		}else if (monkeyClass == eMonkeyType.Cientist){
			if (sfxAttackCientist)
				AudioSource.PlayClipAtPoint(sfxAttackCientist, transform.position);
		}else if (monkeyClass == eMonkeyType.Engineer){
			if (sfxAttackEngineer)
				AudioSource.PlayClipAtPoint(sfxAttackEngineer, transform.position);
		}else if (monkeyClass == eMonkeyType.Saboteur){
			if (sfxAttackSaboteur)
				AudioSource.PlayClipAtPoint(sfxAttackSaboteur, transform.position);
		}		
	}
	
	void playAttackAckSound(){
		
		if (monkeyClass == eMonkeyType.Astronaut){
			if (sfxAttackAckAstronaut)
				AudioSource.PlayClipAtPoint(sfxAttackAckAstronaut, transform.position);
		}else if (monkeyClass == eMonkeyType.Cientist){
			if (sfxAttackAckCientist)
				AudioSource.PlayClipAtPoint(sfxAttackAckCientist, transform.position);
		}else if (monkeyClass == eMonkeyType.Engineer){
			if (sfxAttackAckEngineer)
				AudioSource.PlayClipAtPoint(sfxAttackAckEngineer, transform.position);
		}else if (monkeyClass == eMonkeyType.Saboteur){
			if (sfxAttackAckSaboteur)
				AudioSource.PlayClipAtPoint(sfxAttackAckSaboteur, transform.position);
		}		
	}
	
	void playCapturedSound(){
		if (monkeyClass == eMonkeyType.Astronaut){
			if (sfxAttackedAstronaut)
				AudioSource.PlayClipAtPoint(sfxAttackedAstronaut, transform.position);
		}else if (monkeyClass == eMonkeyType.Cientist){
			if (sfxAttackedCientist)
				AudioSource.PlayClipAtPoint(sfxAttackedCientist, transform.position);
		}else if (monkeyClass == eMonkeyType.Engineer){
			if (sfxAttackedEngineer)
				AudioSource.PlayClipAtPoint(sfxAttackedEngineer, transform.position);
		}else if (monkeyClass == eMonkeyType.Saboteur){
			if (sfxAttackedSaboteur)
				AudioSource.PlayClipAtPoint(sfxAttackedSaboteur, transform.position);
		}	
		
	}
	
	void playResearchComplete(){
		if (sfxResearchComplete)
				AudioSource.PlayClipAtPoint(sfxResearchComplete, transform.position);
		
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

						// Play the attack animation
						if(meshObject) {

							stAnimationWorking = stAnimTargetingForBrawl;
							meshObject.animation.Play(stAnimationWorking);
						}

						playAttackAckSound();

						droneTarget.Attacked();
					}
					EnterNewState(FSMState.STATE_IDLE);
				}
				break;

				// Engineer monkey
			case MouseWorldPosition.eMouseStates.EngineerFix:
				{

					// Sets the time need to fix this building
					fWorkingTargetTime = fBuildingFixTime;
					workingMouseState = mouseState;

					// Play the attack animation
					if(meshObject) {
						
						stAnimationWorking = stAnimEngineerFix;
						meshObject.animation.Play(stAnimationWorking);
					}

					playAttackAckSound();
					EnterNewState(FSMState.STATE_WORKING);
				}
				break;

			case MouseWorldPosition.eMouseStates.TargetingForRecycle:
				{
					fWorkingTargetTime = fDroneRecycleTime;
					workingMouseState = mouseState;

					// Play the attack animation
					if(meshObject) {

						stAnimationWorking = stAnimTargetingForRecycle;
						meshObject.animation.Play(stAnimationWorking);
					}

					playAttackAckSound();
					EnterNewState(FSMState.STATE_WORKING);
				}
				break;

				// Cientist monkey
			case MouseWorldPosition.eMouseStates.CanCapture:
				{	
					// Cientist monkey capturing a RocketPart
					CBaseEntity cCapturedEntity = transTarget.GetComponent<CBaseEntity>();
					playAttackAckSound();
					if(!cCapturedEntity) {

						// DEBUG
						Debug.LogError("Cannot find component CBaseEntity for this object: " + transTarget.name);
					}

					cCapturedEntity.CapturedBy(this.transform, this.captureSpot, this.captureRaySpot);
					this.capturedEntity = cCapturedEntity;

					EnterNewState(FSMState.STATE_IDLE);
				}
				break;

				// Saboteur monkey
			case MouseWorldPosition.eMouseStates.TargetingForReprogram:
				{
					// Sets the time needed to reprogram this drone
					fWorkingTargetTime = fDroneReprogramTime;
					sfxWorking = sfxReprogramming;
					workingMouseState = mouseState;

					// Play the attack animation
					if(meshObject) {

						stAnimationWorking = stAnimTargetingForReprogram;
						meshObject.animation.Play(stAnimationWorking);
					}

					playAttackAckSound();
					EnterNewState(FSMState.STATE_WORKING);
				}
				break;

			case MouseWorldPosition.eMouseStates.CanSabotageBuilding:
				{

					// Sets the time needed to sabotage a building
					fWorkingTargetTime = fBuildingSabotageTime;
					workingMouseState = mouseState;

					// Play the attack animation
					if(meshObject) {

						stAnimationWorking = stAnimCanSabotageBuilding;
						meshObject.animation.Play(stAnimationWorking);
					}

					playAttackAckSound();
					EnterNewState(FSMState.STATE_WORKING);
				}
				break;

			case MouseWorldPosition.eMouseStates.CanSabotageDrone:
				{
					// Sets the time needed to sabotage a building
					fWorkingTargetTime = fDroneSabotageTime;
					workingMouseState = mouseState;

					// Play the attack animation
					if(meshObject) {

						stAnimationWorking = stAnimCanSabotageDrone;
						meshObject.animation.Play(stAnimationWorking);
					}

					playAttackAckSound();
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

					// Check if we are entering the rocket
					if(attackedBuilding.buildingType == CBuilding.eBuildingType.LaunchingPlatform) {

						attackedBuilding.PutAMonkeyInsideRocket(this.transform, this.monkeyClass);
					}
					else {

						attackedBuilding.PutAMonkeyInside(this.transform);
					}

					// DEBUG
					Debug.Log("MouseState for this action " + mouseState + " in " + attackedBuilding.transform);
					EnterNewState(FSMState.STATE_INSIDE_BUILDING);

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
		Debug.Log(this.transform + " Entering WorkIsDone with mouseState " + mouseState);
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
					playAttackSound();
					// Do nothing afterwards
					EnterNewState(FSMState.STATE_IDLE);
				}
				break;

			case MouseWorldPosition.eMouseStates.CanSabotageBuilding:
				{
					CBuilding buildingTarget = transTarget.gameObject.GetComponent<CBuilding>();

					// TODO: the sabotage time could be taken from the building itself
					buildingTarget.TemporarySabotage(8.0f);
					playAttackSound();

					EnterNewState(FSMState.STATE_IDLE);
				}
				break;

			case MouseWorldPosition.eMouseStates.CanSabotageDrone:
				{
					CDrone droneTarget = transTarget.gameObject.GetComponent<CDrone>();

					// FIXME: for now, we leave the sabotage having the same effect as if the drone was attacked
					droneTarget.Attacked();
					playAttackSound();

					EnterNewState(FSMState.STATE_IDLE);
				}
				break;

			case MouseWorldPosition.eMouseStates.TargetingForRecycle:
				{
					CDrone droneTarget = transTarget.gameObject.GetComponent<CDrone>();

					// Drone recycled
					droneTarget.Recycled();
					playAttackSound();

					EnterNewState(FSMState.STATE_IDLE);
				}
				break;

			case MouseWorldPosition.eMouseStates.TargetingForReprogram:
				{
					CDrone droneTarget = transTarget.gameObject.GetComponent<CDrone>();

					// Drone reprogrammed
					droneTarget.Reprogrammed();
					playAttackSound();
					EnterNewState(FSMState.STATE_IDLE);
				}
				break;

			default:
				break;
		}
	}

	/// <summary>
	/// Tells to this monkey that it is being captured by an enemy hunter drone, setting it up (stop walking, 
	/// deselect the monkey and doesn't allow it to walk anymore until it's released
	/// </summary>
	public override void CapturedBy(Transform capturer, Transform captureSpot, Transform captureRaySpot) {

		if(isCaptured)
			return;

		// Play a sound, if any
		playCapturedSound();

		// Change the FSM State
		EnterNewState(FSMState.STATE_CAPTURED);

		// Deactivate the Character Controller, so the monkey will move together with the drone
		myController.enabled = false;

		base.CapturedBy(capturer, captureSpot, captureRaySpot);
	}

	/// <summary>
	/// When the monkey is released we need to setup a few things before call the ReleaseMe() from CBaseEntity
	/// </summary>
	public override void ReleaseMe() {

		// Reactivates the Character Controller
		myController.enabled = true;
		// Enable selection of this monkey again
		Selectable = true;

		base.ReleaseMe();
	}

	/// <summary>
	/// Find the attack spot object for the Astronaut monkey
	/// </summary>
	Transform GetAttackSpot() {

		return transform.Find("AttackSpot");
	}

	/// <summary>
	/// The Cientist have finished the researched and found all rocket parts
	/// </summary>
	void CientistRevealedRocketParts() {

		foreach(CRocketPart cRocketPart in MainScript.lcRocketParts) {

			cRocketPart.PartIsRevealed();
		}
	}

	/// <summary>
	/// Shortcut to check if a monkey is inside a prison: must be with the isCaptured flag on and be inside a 
	/// building.
	/// </summary>
	public bool IsInsideAPrison() {

		bool rv = false;

		if(!capturer || !isCaptured) {
		
			rv = false;
		}
		else if(capturer.GetComponent<CBaseEntity>().Type == CBaseEntity.eObjType.Building) {
		
			rv = true;
		}

		return rv;
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

		// Check if the IA reached the end of a path
		AstarAIFollow.OnReachedEndOfPath += OnAStarReachedEndOfPath;
		// Check if a building have been sabotaged or fixed
		CBuilding.OnSabotageStatusChange += OnSabotageStatusChange;
	}

	/// <summary>
	/// What to do when this object is disabled
	/// </summary>
	void OnDisable() {

		// Check if the IA reached the end of a path
		AstarAIFollow.OnReachedEndOfPath -= OnAStarReachedEndOfPath;
		// Check if a building have been sabotaged or fixed
		CBuilding.OnSabotageStatusChange -= OnSabotageStatusChange;
	}

	/// <summary>
	/// What to do when the event is raised.
	/// </summary>
	void OnAStarReachedEndOfPath(Transform eventRaiser, bool isMoving) {

		// Ignores if the stop event wasn't generate by the monkey itself
		if(eventRaiser != this.transform)
			return;

		// AI gets to the end of the path and we were walking
		if(!isMoving && GetCurrentState() == FSMState.STATE_WALKING) {

			// FIXME: once one monkey stopped, all will stop with this call. Must be a way to filter the event
			// caller

			// Change the current state to IDLE then
			EnterNewState(FSMState.STATE_IDLE);
			Debug.LogWarning(this.transform + " Stopped moving, changing to idle");
		}
	}

	/// <summary>
	/// Called by an event when a building is sabotaged or fixed. Useful for the Cientist, for instance, to know
	/// if it can keep researching
	/// </summary>
	/// <param name="buildingEventRaiser"> CBuilding component of whoever sended the event message </param>
	/// <param name="bnSabotageStatus"> True if the building was sabotaged, false if it is fixed </param>
	void OnSabotageStatusChange(CBuilding buildingEventRaiser, bool bnSabotageStatus) {

		if(monkeyClass == eMonkeyType.Cientist) {

			// The research only will proceed when both Command Center and the Research Lab are operational
			if(buildingEventRaiser.buildingType == CBuilding.eBuildingType.ResearchLab ||
					buildingEventRaiser.buildingType == CBuilding.eBuildingType.CommandCenter) {

				if(MainScript.cbCommandCenter && MainScript.cbResearchLab) {

					bnCanResearchForRocketParts = 
						!MainScript.cbCommandCenter.sabotado & !MainScript.cbResearchLab.sabotado;
				}

				// The Research Lab was sabotaged?
				if(buildingEventRaiser.buildingType == CBuilding.eBuildingType.ResearchLab && !bnSabotageStatus) {

					// Lost our research, must do it again
					bnResearchIsComplete = false;
					if(GetCurrentState() == FSMState.STATE_INSIDE_BUILDING) {
						// reenter the building
						EnterNewState(FSMState.STATE_INSIDE_BUILDING);
					}
				}
			}
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

		if(attackSpot)
			Gizmos.DrawWireSphere(attackSpot.transform.position, fAttackRange);

		if(!transTarget)
			return;

		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, v3Direction);
		//Gizmos.DrawWireSphere(transform.position, fAttackRange);
	}
}
