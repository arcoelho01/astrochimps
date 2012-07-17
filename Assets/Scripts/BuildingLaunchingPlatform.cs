using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Implements the behaviour of the Launching Platform. The Launching Platform it's the place where the monkeys
/// must bring the rocket parts, so it can be built and the game can be won
/// Here, we check for all parts to assemble the rocket, and enabled them as the player bring them
/// </summary>
public class BuildingLaunchingPlatform : MonoBehaviour {

	// The building script
	CBuilding cBuildingScript;

	// A checklist for all the parts already brought by the player
	bool[] aPartsAlreadyBrought;

	// Hold the transforms for all the parts from the complete rocket
	List<Transform> ltPartsOnTheRocket;

	// Just a helper for the enums of the rockets parts
	String[] partsEnumNames;

	// PUBLIC
	public AudioClip sfxPartAdded;
	
	public AudioClip sfxSpaceShipReady;
	
	public bool bnIsTheRocketCompleted;

	//< Transform for the elevator. Will be enabled when the rocket is complete
	public Transform tPlatformElevator;

	/*
	 * ===========================================================================================================
	 * UNITY'S STUFF
	 * ===========================================================================================================
	 */

	// 
	void Awake() {

		// Register itself with the main script
		MainScript.tLaunchingPlatform = this.transform;

		// Get the building script
		cBuildingScript = this.GetComponent<CBuilding>();

		// Get all the parts 8503-2229 defined in the enum
		partsEnumNames = Enum.GetNames(typeof(CRocketPart.eRocketPartType));

		// Initializes the parts list
		aPartsAlreadyBrought = new bool[partsEnumNames.Length];

		InitializeElevatorObject();
	}

	// Use this for initialization
	void Start () {

		ltPartsOnTheRocket = new List<Transform>();
		// Get the rocket object
		GetRocketObject();
	}
	
	/// <summary>
	/// Gets the rocket object from the hierachy.
	/// This object have several sub-objects, one for each rocket part. What we do is first find the object with
	/// the name 'Mesh_'+ the part name. Then, in this object, we search for the first child, which will contain
	/// the mesh itself, with its materials and renderer
	/// </summary>
	void GetRocketObject() {

		foreach(String partName in partsEnumNames) {

			String childNameToFind = "Rocket/Mesh_" + partName;
			Transform childPart = this.transform.Find(childNameToFind);
			
			if(childPart) {

				ltPartsOnTheRocket.Add(childPart);
				// DEBUG
				Debug.Log("Part added to the rocket: " + childPart.transform);
			}
		}
	}
	
	/// <summary>
	/// What to do when the player sucessfully brought a rocket part to the launching platform
	/// </summary>
	public void PlayerBroughtAPart(Transform rocketPartToBeAdded) {

		if(sfxPartAdded) {

			AudioSource.PlayClipAtPoint(sfxPartAdded, transform.position);
		}

		// Now, enable the part in the rocket blueprint
		// Get the rocket part type
		CRocketPart cRocketPart = rocketPartToBeAdded.GetComponent<CRocketPart>();
		// Get the part's type index in the enum
		int nIdxOfRocketPartEnum = (int)cRocketPart.rocketPartType;

		// Get the transform of the corresponding part in the complete rocket
		Transform meshOfThisRocketPart = ltPartsOnTheRocket[nIdxOfRocketPartEnum];
		// Get the material changer component
		MeshMaterialChanger materialChangerOfThisRocketPart = 
			meshOfThisRocketPart.GetComponent<MeshMaterialChanger>();
		// Ask it to enable the part in the complete rocket
		materialChangerOfThisRocketPart.RestoreMaterials();

		// Enable the part in the parts array
		aPartsAlreadyBrought[nIdxOfRocketPartEnum] = true;

		// Destroy the found part (not the one on the complete rocket!)
		if(rocketPartToBeAdded)
			Destroy(rocketPartToBeAdded.gameObject);
		
		// Check if the rocket was completed with this part
		if(!bnIsTheRocketCompleted)
			bnIsTheRocketCompleted = IsTheRocketCompleted();

		if(bnIsTheRocketCompleted) {
			if(sfxSpaceShipReady)
				AudioSource.PlayClipAtPoint(sfxSpaceShipReady, transform.position);
			
			// Enables the embarking of the monkeys on the rocket ship
			EnableLoadingOfMonkeys();
		}
	}

	/// <summary>
	/// Check if the player completed the completed the rocket
	/// </summary>
	/// <returns> True if all parts are present, false if any is missing </returns>
	bool IsTheRocketCompleted() {

		// Let's assume that the player already completed the rocket
		bool rv = true;

		for(int i = 0; i < aPartsAlreadyBrought.Length; i++) {

			if(!aPartsAlreadyBrought[i])
				rv = false;
		}

		return rv;
	}
	
	public int getPartsOnTheRocket(){
		// Let's assume that the player already completed the rocket

		int c = 0;
		for(int i = 0; i < aPartsAlreadyBrought.Length; i++) {

			if(aPartsAlreadyBrought[i])
				c++;
		}

		return c;
	}
	
	/// <summary>
	/// Find the elevator object and disables it when the game start
	/// </summary>
	void InitializeElevatorObject() {

		// Find the elevator object
		tPlatformElevator = this.transform.Find("RocketElevator");

		if(!tPlatformElevator) {

			// DEBUG
			Debug.LogError(this.transform + " could not find the elevator object");
		}

		// Disables the elevator
		tPlatformElevator.gameObject.SetActiveRecursively(false);
	}

	/// <summary>
	/// Tasks to be performed when the rocket is complete
	/// Enable the elevator
	/// Disable the Loading Bay for the parts
	/// Create a Loading Bay for the monkeys
	/// </summary>
	void EnableLoadingOfMonkeys() {

		// Enable the elevator
		tPlatformElevator.gameObject.SetActiveRecursively(true);
	}
}
