using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Class with definitions for the rocket parts
/// The rocket parts are scattered in the map. The player must find it all, so it can assemble a rocket to
/// send his team to Mars
/// </summary>
public class CRocketPart : CBaseEntity {

	// PUBLIC
	// Enumeration for the rockets parts
	public enum eRocketPartType { Engine, Cockpit, FuelTank, NavSystem, SurvivalCapsule };
	public eRocketPartType rocketPartType;
	// This part was already revealed? If it was, everyone can see it
	public bool isRevealed; 
	public int numberOfDefinedRocketParts;

	public static CRocketPart Script;
	VisibilityControl visibilityScript;

	//
	void Awake() {

		Script = this;

		GetSweetSpotAndMeshObject();

		// Calculate the number of rocket parts from the size of the enum
		String[] tempEnum = Enum.GetNames(typeof(eRocketPartType));
		numberOfDefinedRocketParts = tempEnum.Length;

		// Get the visibility control script
		visibilityScript = gameObject.GetComponent<VisibilityControl>();
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	
	}

	/// <summary>
	/// Reveal a part in the scene, enabling it's rendering
	/// </summary>
	public void PartIsRevealed() {

		if(visibilityScript.IsVisible()) {

			// This part was already found by the player. Nothing to be done here
			return;
		}

		// DEBUG QQQ
		Debug.Log(this.transform + " Revealing part");
		isRevealed = true;
		//collider.enabled = true;

		// Turn off the gravity for this object
		//rigidbody.useGravity = true;

		// Extra: get the visibility control component and tells it to make this visible!
		visibilityScript.SetObjectVisible();
	}

	/// <summary>
	/// Hide a part in the scene, disabling it's rendering
	/// </summary>
	public void PartIsUnrevealed() {

		// This part was not found by research, so it keeps it visibility!
		if(!isRevealed) {

			return;
		}

		//isRevealed = false;
		//collider.enabled = false;

		// Turn off the gravity for this object
		//rigidbody.useGravity = false;

		// Extra: get the visibility control component and tells it to make this visible!
		if(visibilityScript) {

			visibilityScript.SetObjectNotVisible();
		}
	}

	/*
	 * ===========================================================================================================
	 * EVENTS STUFF
	 * ===========================================================================================================
	 */
	/// <summary>
	/// What to do when this object is enabled
	/// </summary>
	void OnEnable() {

		// Check if a building have been sabotaged or fixed
		CBuilding.OnSabotageStatusChange += OnSabotageStatusChange;
	}

	/// <summary>
	/// What to do when this object is disabled
	/// </summary>
	void OnDisable() {

		// Check if a building have been sabotaged or fixed
		CBuilding.OnSabotageStatusChange -= OnSabotageStatusChange;
	}

	/// <summary>
	/// What to do when this object is destroyed
	/// </summary>
	void OnDestroy() {

		// Check if a building have been sabotaged or fixed
		CBuilding.OnSabotageStatusChange -= OnSabotageStatusChange;
	}

	/// <summary>
	/// Called by an event when a building is sabotaged or fixed. If the Lab got sabotaged, we lost the view of
	/// the researched parts
	/// </summary>
	/// <param name="buildingEventRaiser"> CBuilding component of whoever sended the event message </param>
	/// <param name="bnSabotageStatus"> True if the building was sabotaged, false if it is fixed </param>
	void OnSabotageStatusChange(CBuilding buildingEventRaiser, bool bnSabotageStatus) {

		if(bnSabotageStatus && buildingEventRaiser.buildingType == CBuilding.eBuildingType.ResearchLab) {

			// Sorry, part unrevealed
			PartIsUnrevealed();
		}
	}
}
