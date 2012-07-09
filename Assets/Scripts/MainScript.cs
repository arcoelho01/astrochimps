using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainScript : MonoBehaviour {

	// PUBLIC
	public Transform prefabExtractor;
	public Transform prefabFazenda;
	public Transform prefabLaboratorio;
	public Transform prefabUnderConstructionBox;
	public Transform prefabForceField;
	public Transform prefabCaptureRay;

	//< Prefab to instantiate when te player wins
	public Transform prefabVictorySequence;
	//< Prefab to instantiate when the player loses by depleting his oxygen reserve
	public Transform prefabDefeatByDeath;

	public GUIBottomMenu bottomMenu;	// Pointer to the bottom menu bar
	public Transform playerObject;	// Pointer to the player object
	public CPlayer player;	// Pointer to the player itself

	public bool isMinimapEnabled = true;	// Starts with the minimap enabled

	// Index of all layers
	public static int groundLayer = 8;
	public static int alliedLayer = 10;	
	public static int enemyLayer = 11;	
	public static int neutralLayer = 12;	
	public static int minimapLayer = 13;	
	public static int minimapGroundLayer = 14;	

	// List to hold all games units and resources
	List<Transform> alliedMonkeys = new List<Transform>();
	List<Transform> alliedDrones = new List<Transform>();
	List<Transform> alliedBuildings = new List<Transform>();

	List<Transform> opponentDrones = new List<Transform>();
	List<Transform> opponentBuildings = new List<Transform>();

	List<Transform> neutralResources = new List<Transform>();
	List<Transform> neutralRocketParts = new List<Transform>();

	//< Keeps a list of all the rocket parts CRocketPart components
	public static List<CRocketPart> lcRocketParts = new List<CRocketPart>();

	// Shortcut for some scripts
	public static MouseWorldPosition mouseInputScript = null;

	// Shortcut for the four monkeys
	public static Transform MonkeyAstronaut;
	public static Transform MonkeyCientist;
	public static Transform MonkeyEngineer;
	public static Transform MonkeySaboteur;

	//< Shortcut for the Command Center
	public static CBuilding cbCommandCenter;
	//< Shortcut for the Research Lab
	public static CBuilding cbResearchLab;
	//< Shortcut for the Launching Platform object
	public static Transform tLaunchingPlatform;

	// Shortcut to this script
	public static MainScript Script;

	//< Keeps a list of all boarded monkeys
	public List<Transform> lBoardedMonkeys = new List<Transform>();
	
	//< Pause status
	public bool bnIsTheGamePaused;

	//< Signal if we are in a cutscene to ignore player inputs
	public bool bnOnCutscene;

	/*
	 * ===========================================================================================================
	 * UNITY'S STUFF
	 * ===========================================================================================================
	 */
	void Awake() {

		Script = this;
	}

	// Use this for initialization
	void Start () {
	
		bottomMenu = GetComponent<GUIBottomMenu>();
		if(!bottomMenu) {

			// DEBUG
			Debug.LogError("Bottom menu object not found!");
		}

		player = playerObject.GetComponent<CPlayer>();
		if(!player) {

			// DEBUG
			Debug.LogError("Player object not found!");
		}

		GetCurrentUnitsInScene();

		mouseInputScript = gameObject.GetComponent<MouseWorldPosition>();
		if(!mouseInputScript) {

			// DEBUG
			Debug.LogError("Cannot find the MouseWorldPosition component. Please check.");
		}
		
		//Instantiate(GameObject.("Monkey"), new Vector3(79.07609f,33.39249f,13.41692f), Quaternion.identity);

		// Get all the monkeys in the scene and assign their objects to some variables, so we have shortcuts to them
		GetMonkeysObjects();
	}
	
	// Update is called once per frame
	void Update () {
	
		CheckInputKeys();
	}

	/*
	 * ===========================================================================================================
	 * SCRIPT STUFF
	 * ===========================================================================================================
	 */

	/// <summary>
	/// Check for user input
	/// </summary>
	void CheckInputKeys() {

		// Pause
		if(Input.GetKeyDown(KeyCode.P)) {

			bnIsTheGamePaused = !bnIsTheGamePaused;

			if(bnIsTheGamePaused) {

				Time.timeScale = 0.0f; // Freezes the game
			}
			else {

				Time.timeScale = 1.0f; // Restore the game to full speed
			}

		}

		//*
		DoCheatingStuff();
		//*/
	}

	/// <summary>
	/// Get all units in the scene, filters them and add them to the corresponding list
	/// </summary>
	void GetCurrentUnitsInScene() {

		// Monkeys
		GameObject[] goMonkeys;
		goMonkeys = GameObject.FindGameObjectsWithTag("Monkey");
		foreach(GameObject oneMonkey in goMonkeys) {

			if(oneMonkey.layer == alliedLayer) {
				
				alliedMonkeys.Add(oneMonkey.transform);
			}
		}

		// Get all drones
		GetCurrentDronesInScene();

		// Buildings
		GameObject[] goBuildings;
		goBuildings = GameObject.FindGameObjectsWithTag("Building");
		foreach(GameObject oneBuilding in goBuildings) {

			if(oneBuilding.layer == alliedLayer) {
				
				alliedBuildings.Add(oneBuilding.transform);
			}
			else if(oneBuilding.layer == enemyLayer) {
				
				opponentBuildings.Add(oneBuilding.transform);
			}
		}

		GetCurrentRocketPartsInScene();

		// Resources
		GameObject[] goResources;
		goResources = GameObject.FindGameObjectsWithTag("Resource");
		foreach(GameObject oneResource in goResources) {

			neutralResources.Add(oneResource.transform);
		}
		


		// 
		Debug.Log("Resumo: macacos aliados: " + alliedMonkeys.Count);

		Debug.Log("Resumo: predios aliados: " + alliedBuildings.Count);
		Debug.Log("Resumo: predios inimigos: " + opponentBuildings.Count);

		Debug.Log("Resumo: recursos: " + neutralResources.Count);
	}

	/// <summary>
	/// Get all drones in the scene
	/// </summary>
	void GetCurrentDronesInScene() {

		GameObject[] goDrones;
		goDrones = GameObject.FindGameObjectsWithTag("Drone");

		foreach(GameObject oneDrone in goDrones) {

			if(oneDrone.layer == alliedLayer) {
				
				alliedDrones.Add(oneDrone.transform);
			}
			else if(oneDrone.layer == enemyLayer) {
				
				opponentDrones.Add(oneDrone.transform);
			}
		}

		// DEBUG
		// Prints a summary
		Debug.Log("Summary - Allied drones " + alliedDrones.Count);
		Debug.Log("Summary - Enemy drones " + opponentDrones.Count);
	}

	/// <summary>
	/// Get all the rocket parts currently in the game
	/// </summary>
	void GetCurrentRocketPartsInScene() {

		GameObject[] goRocketParts;
		goRocketParts = GameObject.FindGameObjectsWithTag("RocketPart");

		foreach(GameObject oneRocketPart in goRocketParts) {

			neutralRocketParts.Add(oneRocketPart.transform);

			CRocketPart cOneRocketPart = oneRocketPart.GetComponent<CRocketPart>();
			lcRocketParts.Add(cOneRocketPart);
		}

		// DEBUG
		// Prints a summary
		Debug.Log("Summary - Rocket Parts " + neutralRocketParts.Count);
	}

	/// <summary>
	/// Get all monkeys in the scene, sort them by class and add their transforms to the variables declared
	/// up in this script. With this, we get shortcuts to them
	/// </summary>
	void GetMonkeysObjects() {

		GameObject[] goMonkeys;

		goMonkeys = GameObject.FindGameObjectsWithTag("Monkey");
		
		foreach(GameObject oneMonkey in goMonkeys) {

			CMonkey monkeyScript = oneMonkey.GetComponent<CMonkey>();

			if(!monkeyScript)
				continue;

			switch(monkeyScript.monkeyClass) {

				case CMonkey.eMonkeyType.Astronaut:
					if(!MonkeyAstronaut)
						MonkeyAstronaut = oneMonkey.transform;
					break;

				case CMonkey.eMonkeyType.Cientist:
					if(!MonkeyCientist)
						MonkeyCientist = oneMonkey.transform;
					break;

				case CMonkey.eMonkeyType.Engineer:
					if(!MonkeyEngineer)
						MonkeyEngineer = oneMonkey.transform;
					break;

				case CMonkey.eMonkeyType.Saboteur:
					if(!MonkeySaboteur)
						MonkeySaboteur = oneMonkey.transform;
					break;

				default:
					break;
			}
		}
	}

	/// <summary> 
	/// Updates the drone's list when one of them change teams
	/// </summary>
	public void ChangeDroneTeamAndUpdateList(Transform droneObject, int droneLayer) {

		if(droneLayer == alliedLayer) {

			alliedDrones.Add(droneObject);
			opponentDrones.Remove(droneObject);
		}
		else {

			opponentDrones.Add(droneObject);
			alliedDrones.Remove(droneObject);
		}

		// DEBUG
		// Prints a summary
		Debug.Log("Summary - Allied drones " + alliedDrones.Count);
		Debug.Log("Summary - Enemy drones " + opponentDrones.Count);
	}

	/// <summary>
	/// Overloaded method: only deploys the construction box, wait the time and then destroy it. Doesn't 
	/// instantiate the new building
	/// </summary>
	/// <param name="position"> A Vector3 with the position to create the box </param>
	/// <param name="buildTime"> Time to build the building. When it's over, will show the complete animation 
	/// and will destroy itself </param>
	public void DeployUnderConstructionBox(Vector3 position, float buildTime) {

		DeployUnderConstructionBox(null, null, position, buildTime);
	}

	/// <summary>
	/// Create an instance of an 'Under Construction Box'
	/// </summary>
	/// <param name="builtOver"> Transform of the object where this Box is built. With this, we can pass a 
	/// Resource back to a built extractor, for instance. We can call it as null </param>
	/// <param name="prefabToBuild"> Prefab to the building which will be built we te timer is over.</param>
	/// <param name="position"> A Vector3 with the position to create the box and instantiate the new building,
	/// if any </param>
	/// <param name="buildTime"> Time to build the building. When it's over, it will instantiate a new building,
	/// show the complete animation and will destroy itself </param>
	public void DeployUnderConstructionBox(Transform builtOver, Transform prefabToBuild, 
			Vector3 position, float buildTime) {

		// Create a new box
		Transform myUnderConstructionBox = Instantiate(prefabUnderConstructionBox, position, Quaternion.identity)
			as Transform;

		UnderConstructionBox myUCBoxComponent = 
			myUnderConstructionBox.gameObject.GetComponent<UnderConstructionBox>();

		// Set the prefab
		myUCBoxComponent.SetPrefabToBuild(prefabToBuild);

		// Set it's timer
		myUCBoxComponent.SetBuildingTime(buildTime);

		// Set the object where this will be built over
		myUCBoxComponent.SetBuiltOverObject(builtOver);
	}


	/// <summary>
	/// Create a version of Freddy's construction box
	/// </summary>
	/// <param name="position"> Where to create the construction box </param>
	/// <param name="timeToBuild">Time taken to build this building. Actually, it's how long the construction box
	/// will be shown</param>
	IEnumerator DeployConstructionBox(Vector3 position, float timeToBuild) {

		Transform underConstructionBox = Instantiate(prefabUnderConstructionBox, position, Quaternion.identity)
			as Transform;

		yield return new WaitForSeconds(timeToBuild);
		
		// Completed? Call the completed animation
		underConstructionBox.animation.Play("Complete");

		// Wait for the animation finish
		yield return new WaitForSeconds(underConstructionBox.animation["Complete"].clip.length);

		// Destroy the object
		Destroy(underConstructionBox.gameObject);
	}

	/// <summary>
	/// Just check if we have enough resources to build this unit/structure
	/// </summary>
	/// <returns> true if we have, false if we don't </returns>
	public bool CheckIfAreEnoughResourcesToBuild(Transform objectToBuild) {

		bool rv = false;
		CBaseEntity myEntity = objectToBuild.GetComponent<CBaseEntity>();

		// Check the costs
		if(player.metalLevel >= myEntity.costMetal) 
			rv = true;

		return rv;
	}

	/// <summary>
	/// Check if a target is in the range of this unit, projecting it's position in the 2D floor plane (only uses
	/// x and z coordinates)
	/// </summary>
	/// <param name="tAttacker"> Transform of the attacker </param>
	/// <param name="tTarget"> Transform of the target </param>
	/// <param name="fAttackRange"> A float with the range of the attack </param>
	/// <returns> True if the target is in the specified range, false otherwise </returns>
	public bool CheckIfTargetIsInRange(Transform tAttacker, Transform tTarget, float fAttackRange) {

		bool rv = false;

		// No target? Get out
		if(!tTarget)
			return false;

		// Projects the position in 2D, using only the x and z axis
		Vector2 v2TargetPosition = new Vector2(tTarget.transform.position.x, tTarget.transform.position.z);
		Vector2 v2AttackerPosition = new Vector2(tAttacker.transform.position.x, tAttacker.transform.position.z);

		Vector2 v2Diff = v2TargetPosition - v2AttackerPosition;

		if(v2Diff.magnitude <= (fAttackRange * fAttackRange)) {

			rv = true;
		}

		return rv;
	}

	/// <summary>
	/// Check for enemies (collider, actually) inside a radius
	/// </summary>
	/// <param name="callerPosition"> The position to be the sphere center. Usually is the center of the caller's
	/// collider </param>
	/// <param name="tTarget"> Target's transform, so we can filter the colliders inside the radius and know for
	/// sure if we touched the target </param>
	/// <param name="targetLayer"> Target's layer, used for filtering colliders </param>
	/// <param name="fRadius"> Radius of the sphere where we will check for other objects presence. Usually the 
	/// caller's attack radius </param>
	/// <returns> True if the target is inside the radius, false otherwise </returns>
	public bool CheckIfTargetColliderIsInRange(Vector3 callerPosition, Transform tTarget, int targetLayer, float fRadius) {

		bool rv = false;

		// Get all enemies in the radius
		Collider[] scannedColliders = Physics.OverlapSphere(callerPosition, fRadius, 1<<targetLayer);

		if(scannedColliders.Length > 0) {

			foreach(Collider tempCollider in scannedColliders) {

				if(tempCollider.transform == tTarget) {

					rv = true;
					break;
				}
			}
		}

		return rv;
	}

	/// <summary>
	/// Returns the list of all rocket parts transforms found in the scene
	/// </summary>
	public List<Transform> GetListOfAllRocketParts() {

		return neutralRocketParts;
	}

	/// <summary>
	/// Returns the list of all rocket parts CRocketPart components for the parts found in the scene
	/// </summary>
	public List<CRocketPart> GetListOfAllRocketPartsComponents() {

		return lcRocketParts;
	}

	/// <summary>
	/// Returns the list of all monkeys currently in the game
	/// </summary>
	public List<Transform> GetListOfAllMonkeys() {

		return alliedMonkeys;
	}

	/*
	 * ===========================================================================================================
	 * GAME END STUFF
	 * ===========================================================================================================
	 */

	/// <summary>
	/// Check if we have all the monkeys boarded in the rocket. If so, the game was won!
	/// </summary>
	public void CheckIfAllMonkeysAreBoarded() {

		bool rv;

		/// FIXME: this sucks! Shouldn't use a number, must be another way to know how many monkeys are in the
		/// game
		if(lBoardedMonkeys.Count == 4) 
			rv = true;
		else 
			rv = false;

		if(rv) {

			// Start the victory sequence
			GameOverStartVictorySequence();
		}
	}

	/// <summary>
	/// Check if we all our monkeys are not under prison. If so, we lost the game.
	/// Usually, this function is called when a drone delivers a monkey to a prison
	/// </summary>
	public void CheckIfAllMonkeysAreConvicts() {

		// Let's assume that all monkeys are "inside"
		bool rv = true;

		foreach(Transform tMonkey in alliedMonkeys) {

			CMonkey scriptMonkey = tMonkey.GetComponent<CMonkey>();

			if(!scriptMonkey.IsInsideAPrison()) {
				
				// Monkey is free!!!!
				rv = false;
			}
		}

		// Ok, we have checked the status of all monkeys. What is the veredict?
		if(rv) {

			// Start the victory sequence
			GameOverStartJailedSequence();
		}
	}

	/// <summary>
	/// Check if we have enough oxygen to keep the monkeys alive
	/// </summary>
	public void CheckIfWeHaveOxygen() {

		if(player.oxygenLevel >= 0.1f) {

			return;
		}

		// Oops, not enough oxygen. Kill the monkeys!
		GameOverStartDeathByAsphyxiationSequence();
	}

	/// <summary>
	/// What happens when the game is lost
	/// For now this happens when all player's monkeys are in jail
	/// </summary>
	void GameOverStartJailedSequence() {

		// DEBUG
		Debug.LogWarning(this.transform + " G A M E  O V E R. Outcome: all in prison!");
	}

	/// <summary>
	/// Call the scenes when the player has no oxygen left, so the monkeys die asphyxiated
	/// </summary>
	void GameOverStartDeathByAsphyxiationSequence() {

		// DEBUG
		Debug.LogWarning(this.transform + " G A M E  O V E R. Outcome: all monkeys dead!");

		// First thing to do: ignore player inputs
		bnOnCutscene = true;

		if(prefabDefeatByDeath) {

			Instantiate(prefabDefeatByDeath, Vector3.zero, Quaternion.identity);
		}

	}

	/// <summary>
	/// What happens when the game is won
	/// For now, this happens when all monkey are boarded in the rocket ship
	/// <summary>
	void GameOverStartVictorySequence() {

		// DEBUG
		Debug.LogWarning(this.transform + " G A M E  O V E R. Outcome: victory!");

		// First thing to do: ignore player inputs
		bnOnCutscene = true;

		if(prefabVictorySequence) {

			Instantiate(prefabVictorySequence, Vector3.zero, Quaternion.identity);
		}

	}

	/*
	 * ===========================================================================================================
	 * CHEATING STUFF
	 * ===========================================================================================================
	 */

	/// <summary>
	/// Do some cheats, for development purposes only
	/// </summary>
	void DoCheatingStuff() {

		// Mess with the oxygen levels
		if(Input.GetKeyDown(KeyCode.Minus)){

			player.SubResourceOxygen(15.0f);
		}
		if(Input.GetKeyDown(KeyCode.Equals)){

			player.AddResourceOxygen(15.0f);
		}

	}
}
