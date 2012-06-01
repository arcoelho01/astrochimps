using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainScript : MonoBehaviour {

	// PUBLIC
	public Transform prefabExtractor;
	public Transform prefabFazenda;
	public Transform prefabLaboratorio;
	public Transform prefabGaragem;

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
	/// Build a extractor and associate it to the caller (a resource site)
	/// </summary>
	/// <param name="position">A Vector3 with the position to build the extractor</param>
	/// <returns> Transform of the instantiated extractor </returns>
	public Transform BuildExtractor(Vector3 position) {

		// DEBUG
		Debug.Log("Starting building extractor");

		position.y += prefabExtractor.transform.localScale.y;

		Transform extractorClone = Instantiate(prefabExtractor, position, Quaternion.identity) 
			as Transform;

		return extractorClone;
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

}
