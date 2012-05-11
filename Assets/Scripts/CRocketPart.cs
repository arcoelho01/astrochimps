using UnityEngine;
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

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(isCaptured) {

		}
	}
}
