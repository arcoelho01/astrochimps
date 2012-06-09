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

	//
	void Awake() {

		Script = this;

		//isRevealed = false;
		isRevealed = true;
		GetSweetSpotAndMeshObject();

		// Calculate the number of rocket parts from the size of the enum
		String[] tempEnum = Enum.GetNames(typeof(eRocketPartType));
		numberOfDefinedRocketParts = tempEnum.Length;
	}

	// Use this for initialization
	void Start () {

		if(meshObject)
			meshObject.gameObject.renderer.enabled = isRevealed;
	}
	
	// Update is called once per frame
	void Update () {

	
	}

	/// <summary>
	/// Reveal a part in the scene, enabling it's rendering
	/// </summary>
	public void PartIsRevealed() {

		isRevealed = true;
		meshObject.gameObject.renderer.enabled = true;
	}

	/// <summary>
	/// Hide a part in the scene, disabling it's rendering
	/// </summary>
	public void PartIsUnrevealed() {

		isRevealed = false;
		meshObject.gameObject.renderer.enabled = false;
	}
}
