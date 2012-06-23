using UnityEngine;
using System.Collections;

/// <summary>
/// Class with resource site definitions
/// </summary>
public class CResource : CBaseEntity {

	// PRIVATE

	// PUBLIC
	public float resourceLevel = 5.0f;	// Amount of resource in this resource site
	public enum eResourceType { Oxygen, Metal, NONE};	// Type of resource
	public eResourceType resourceType = eResourceType.NONE;
	public Transform associatedExtractor;	// Point to the extractor built here, if any

	bool resourceDrained = false;	// Easier way to say that's resourceLevel is zero!

	public class ExtractedResource {

		public eResourceType type;	// Type o resource extracted
		public float amount;				// Amount of resource extracted
		public bool isDrained;			// This resource site is drained, i.e., have no resources left?
	}
	/*
	 * ===========================================================================================================
	 * UNITY'S STUFF
	 * ===========================================================================================================
	 */

	/// <summary>
	/// When the script is initialized
	/// </summary>
	void Awake() {

		Selectable = true; 
		Movable = false;
		Type = eObjType.Resource;
	}

	//
	void Update() {

		if(isSelected) {

			// Resource site selected.
			// 1 - Offer to the player to build a extractor
			if(!associatedExtractor) {

				mainScript.bottomMenu.BuildExtractorMenuEnable(this);
			}
		}
	}

	/// <summary>
	/// Extract a certain amount of metal
	/// </summary>
	/// <param name="amount">
	/// Float with the amount of resource to be extracted. This value depends of the extractor level 
	/// </param>
	public ExtractedResource ExtractResource(float amount) {

		ExtractedResource rv = new ExtractedResource();

		rv.type = resourceType;
		rv.amount = 0.0f;
		rv.isDrained = resourceDrained;

		if(!associatedExtractor ||  resourceLevel <= 0.0f) {

			return rv; 
		}

		// extracts an amount of resource
		resourceLevel -= amount;

		// Check if we actually have enough resources
		if(resourceLevel <= 0.0f) {

			// We tried to extract more than available
			amount += resourceLevel; // Adjust the amount extracted
			resourceLevel = 0.0f;	// Clean the site
			resourceDrained = true;
		}

		rv.isDrained = resourceDrained;
		rv.amount = amount;

		return rv;
	}

	/// <summary>
	/// Override from the CBaseEntity
	/// Build an extractor in this resource site
	/// </summary>
	public override void BuildIt() {

		Transform prefabToBuild;

		if(!mainScript) {

			// DEBUG
			Debug.LogError("MainScript not set!");
		}

		// In a resource site we can only build an extractor...
		prefabToBuild = MainScript.Script.prefabExtractor;
		CBuilding myBuildingClass = prefabToBuild.GetComponent<CBuilding>();
	
		Deselect();
		// Ok, first of all, let's check if we have the resources to actually build this
		if(mainScript.CheckIfAreEnoughResourcesToBuild(prefabToBuild)) {

			// With an extractor built, there's no need to be able to select this resource site anymore
			Selectable = false;

			// Charge the player!
			mainScript.player.SubResourceMetal(myBuildingClass.costMetal);

			mainScript.DeployUnderConstructionBox(this.transform, 
					prefabToBuild, transform.position, myBuildingClass.costTime);
		}
	}

	/// <summary>
	/// Tell if we can scavenge this resource site. To do this, it can't have an extractor yet, and must have
	/// resources left
	/// </summary>
	/// <returns> True if we can build an extractor here, false otherwise </returns>
	public bool CanWeBuildInThisResourceSite() {

		bool rv = true;

		if(resourceDrained)
			rv = false;

		if(!Selectable)
			rv = false;

		if(associatedExtractor != null)
			rv = false;

		return rv;
	}
}
