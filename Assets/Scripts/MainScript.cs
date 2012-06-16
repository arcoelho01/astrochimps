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

	// Shortcut for some scripts
	public static MouseWorldPosition mouseInputScript = null;

	// Shortcut for the four monkeys
	public static Transform MonkeyAstronaut;
	public static Transform MonkeyCientist;
	public static Transform MonkeyEngineer;
	public static Transform MonkeySaboteur;

	// Shortcut to this script
	public static MainScript Script;

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

		// FIXME: added only for now
		GetCurrentUnitsInScene();

		mouseInputScript = gameObject.GetComponent<MouseWorldPosition>();
		if(!mouseInputScript) {

			// DEBUG
			Debug.LogError("Cannot find the MouseWorldPosition component. Please check.");
		}

		// Get all the monkeys in the scene and assign their objects to some variables, so we have shortcuts to them
		GetMonkeysObjects();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/*
	 * ===========================================================================================================
	 * SCRIPT STUFF
	 * ===========================================================================================================
	 */

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

}
