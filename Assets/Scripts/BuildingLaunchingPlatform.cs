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

	// A checklist for all the parts already brought by the player
	bool[] aPartsAlreadyBrought;

	// Hold the transforms for all the parts from the complete rocket
	List<Transform> ltPartsOnTheRocket;

	// Just a helper for the enums of the rockets parts
	String[] partsEnumNames;

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

		// Get all the parts defined in the enum
		partsEnumNames = Enum.GetNames(typeof(CRocketPart.eRocketPartType));

		// Initializes the parts list
		aPartsAlreadyBrought = new bool[partsEnumNames.Length];

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
	void PlayerBroughtAPart(Transform rocketPartToBeAdded) {

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
		
		// DEBUG
		if(!meshOfThisRocketPart) {

			Debug.LogError(this.transform + " not found meshOfThisRocketPart, index " + nIdxOfRocketPartEnum);
		}

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
		
		// TODO: here will be a good place to check if the player have not yet assembled the rocket. To do this,
		// check aPartsAlreadyBrought for a false. If all elements are true, the rocket is complete

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
			PlayerBroughtAPart(rocketPart);
		}
	}
}
