using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Implements the behaviour of the Launching Platform. The Launching Platform it's the place where the monkeys
/// must bring the rocket parts, so it can be built and the game can be won
/// </summary>
public class BuildingLaunchingPlatform : MonoBehaviour {

	// The building script
	CBuilding cBuildingScript;

	// Hold all the parts already brought
	List<Transform> ltPartsInThePlatform;

	/*
	 * ===========================================================================================================
	 * UNITY'S STUFF
	 * ===========================================================================================================
	 */

	// 
	void Awake() {

		// Get the building script
		cBuildingScript = this.GetComponent<CBuilding>();

		// Initializes the parts list
		ltPartsInThePlatform = new List<Transform>();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	/// <summary>
	/// Check there's a monkey inside the platform and if it brought a new part
	/// </summary>
	void CheckForPart() {

	}

	/// <summary>
	/// Sense when an object with a rigidbody attached enters the collider. We're using 
	/// </summary>
	void OnTriggerEnter(Collider hit) {

		if(hit.gameObject.tag == "RocketPart") {

			// DEBUG
			Debug.Log("Rocket part entered the launching platform: " + hit.gameObject.transform.name);

			// 1 - Detach the rocket part from the monkey
			// 2 - Add it to the parts list
		}
	}

	/// <summary>
	/// Sense when an object with a rigidbody attached to it leaves the collider. Will be used if the rocket part
	/// leave the platform (stolen, perhaps?)
	/// </summary>
	void OnTriggerExit(Collider hit) {

		if(hit.gameObject.tag == "RocketPart") {

			// DEBUG
			Debug.Log("Rocket part left the launching platfform: " + hit.gameObject.transform.name);
		}
	}
}
