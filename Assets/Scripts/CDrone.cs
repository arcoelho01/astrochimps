using UnityEngine;
using System.Collections;

/// <summary>
/// Class with drones definitions
/// </summary>
public class CDrone : CBaseEntity {

	// PRIVATE
	
	// PUBLIC
	public enum eDroneType { Patrol, Saboteur, Hunter, NONE }; // Drones types

	/*
	 * ===========================================================================================================
	 * UNITY'S STUFF
	 * ===========================================================================================================
	 */

	/// <summary>
	/// When the script is initialized
	/// </summary>
	void Awake() {

		// Set the default settings for all the buildings
		Selectable = true; // All monkeys are selectable
		Movable = true; // All monkeys are movable
		Type = eObjType.Drone;
	}
}
