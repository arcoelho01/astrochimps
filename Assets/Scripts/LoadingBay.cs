using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This will enable a object to receive a rocket part and enable it in the launching platform. With this, we
/// don't to check the presence of parts in the launching platform itself
/// </summary>
public class LoadingBay : MonoBehaviour {

	//< We can load the rocket parts. When the rocket is complete, we can load the monkeys at the same platform
	public enum ELoadingType { RocketPart, Monkey};

	BuildingLaunchingPlatform scriptLaunchingPlatform;

	public ELoadingType eLoadingType;

	// Use this for initialization
	void Start () {
	
		// Get the Launching Platform script
		scriptLaunchingPlatform = MainScript.tLaunchingPlatform.GetComponent<BuildingLaunchingPlatform>();

		if(!scriptLaunchingPlatform) {

			// DEBUG
			Debug.LogError(this.transform + " could not find the BuildingLaunchingPlatform script.");
		}

		// Always start as a cargo loading platform
		//eLoadingType = ELoadingType.RocketPart;

		// Register myself with the BuildingLaunchingPlatform script
		scriptLaunchingPlatform.AddLoadingBay(this.transform);
	}

	/// <summary>
	/// Sense when an object with a rigidbody attached enters the collider. We're using 
	/// </summary>
	void OnTriggerEnter(Collider hit) {

		// Are we expecting parts and the player brought one?
		if(hit.gameObject.tag == "RocketPart" && eLoadingType == ELoadingType.RocketPart) {

			// 1 - Detach the rocket part from the monkey
			Transform rocketPart = hit.gameObject.transform;

			// 2 - Add it to the parts list
			scriptLaunchingPlatform.PlayerBroughtAPart(rocketPart);
		}
		else if(hit.gameObject.tag == "Monkey" && eLoadingType == ELoadingType.Monkey) {

			// DEBUG
			Debug.Log(this.transform + " monkey at bay: " + hit.gameObject.name);
		}
	}

	/// <summary>
	/// Switches the loading mode to monkey loading. This happens when the rocket is complete
	/// </summary>
	public void EnableMonkeyLoading() {

		eLoadingType = ELoadingType.Monkey;

		// DEBUG
		Debug.Log(this.transform + " switching to monkey loading");
	}
}
