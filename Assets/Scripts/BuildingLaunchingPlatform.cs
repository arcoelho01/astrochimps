using UnityEngine;
using System;
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

	// Hold the transforms for all the parts from the complete rocke
	List<Transform> ltPartsOnTheRocket;

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

		ltPartsOnTheRocket = new List<Transform>();
		// Get the rocket object
		GetRocketObject();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Gets the rocket object from the hierachy.
	/// This object have several sub-objects, one for each rocket part
	/// </summary>
	void GetRocketObject() {

		String[] partsEnumNames = Enum.GetNames(typeof(CRocketPart.eRocketPartType));

		foreach(String partName in partsEnumNames) {

			String childNameToFind = "Rocket/Mesh_" + partName;
			Transform childPart = this.transform.Find(childNameToFind);
			
			if(childPart) {

				ltPartsOnTheRocket.Add(childPart);

				// DEBUG
				Debug.Log("Part added to the rocket: " + childPart.transform);
			}

			// TODO: now that we have all the parts in the rocket, we have to decide what to do when the player
			// bring it to the platform:
			// 1 - start with all deactivated and activate the part brought by the player?
			// 2 - start all with a translucent material and then apply the correct material?
			// 3 - same as 1, but activate/deactivate the renderer of the object?

		}

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
