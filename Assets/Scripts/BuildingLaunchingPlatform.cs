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

	// PUBLIC
	public AudioClip sfxPartAdded;

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
	///
	/// </summary>
	void AddRocketPartToTheList(Transform rocketPartToBeAdded) {

		if(sfxPartAdded) {

			AudioSource.PlayClipAtPoint(sfxPartAdded, transform.position);
		}

		// Add to the list
		ltPartsInThePlatform.Add(rocketPartToBeAdded);

		// TODO: tell the player this in some way, like in a text on the screen, etc
		Debug.Log("Rocket part " + rocketPartToBeAdded + "delivered in the platform");
	}

	/// <summary>
	/// Sense when an object with a rigidbody attached enters the collider. We're using 
	/// </summary>
	void OnTriggerEnter(Collider hit) {

		if(hit.gameObject.tag == "RocketPart") {

			// DEBUG
			Debug.Log("Rocket part entered the launching platform: " + hit.gameObject.transform.name);

			// 1 - Detach the rocket part from the monkey
			Transform rocketPart = hit.gameObject.transform;
			CRocketPart rocketPartEntity = rocketPart.GetComponent<CRocketPart>();
			rocketPartEntity.ReleaseMe();

			// 2 - Add it to the parts list
			AddRocketPartToTheList(rocketPart);
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

			Transform rocketPart = hit.gameObject.transform;
			ltPartsInThePlatform.Remove(rocketPart);
		}
	}
}
