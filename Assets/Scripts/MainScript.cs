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

	List<Transform> opponentMonkeys = new List<Transform>();
	List<Transform> opponentDrones = new List<Transform>();
	List<Transform> opponentBuildings = new List<Transform>();

	List<Transform> neutralResources = new List<Transform>();
	List<Transform> neutralRocketParts = new List<Transform>();

	// Shortcut for some scripts
	public static MouseWorldPosition mouseInputScript = null;

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
	}
	
	// Update is called once per frame
	void Update () {
	
	}

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
			else if(oneMonkey.layer == enemyLayer) {
				
				opponentMonkeys.Add(oneMonkey.transform);
			}
		}

		// Drones
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
		Debug.Log("Resumo: macacos inimigos: " + opponentMonkeys.Count);

		Debug.Log("Resumo: drones aliados: " + alliedDrones.Count);
		Debug.Log("Resumo: drones inimigos: " + opponentDrones.Count);

		Debug.Log("Resumo: predios aliados: " + alliedBuildings.Count);
		Debug.Log("Resumo: predios inimigos: " + opponentBuildings.Count);

		Debug.Log("Resumo: recursos: " + neutralResources.Count);
	}
}
