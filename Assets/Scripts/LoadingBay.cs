using UnityEngine;
using System.Collections;

/// <summary>
/// This will enable a object to receive a rocket part and enable it in the launching platform. With this, we
/// don't to check the presence of parts in the launching platform itself
/// </summary>

public class LoadingBay : MonoBehaviour {

	BuildingLaunchingPlatform scriptLaunchingPlatform;

	// Use this for initialization
	void Start () {
	
		// Get the Launching Platform script
		scriptLaunchingPlatform = MainScript.tLaunchingPlatform.GetComponent<BuildingLaunchingPlatform>();

		if(!scriptLaunchingPlatform) {

			// DEBUG
			Debug.LogError(this.transform + " could not find the BuildingLaunchingPlatform script.");
		}
	}

	//
	void Awake() {

		// Get to the Lauching Platform script


	}
	
	// Update is called once per frame
	void Update () {
	
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
			//CRocketPart rocketPartEntity = rocketPart.GetComponent<CRocketPart>();
			//rocketPartEntity.ReleaseMe();

			// 2 - Add it to the parts list
			scriptLaunchingPlatform.PlayerBroughtAPart(rocketPart);
		}
	}
}
