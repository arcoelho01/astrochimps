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
	public void ExtractResource(float amount) {

		if(!associatedExtractor) {

			return; 
		}

		// No resource left?
		if(resourceLevel == 0.0f)
			return;

		resourceLevel -= amount;

		if(resourceLevel < 0.0f) {

			// We tried to extract more than available
			amount += resourceLevel; // Adjust the amount extracted
			resourceLevel = 0.0f;	// Clean the site
			ResourcesDrained();

			// TODO: should we disable the extractor and the resource site when it's all drained?
		}

		// Check the type of resource we're dealing here
		switch(resourceType) {

			case eResourceType.Oxygen:
				// Adds the resource extracted to the player
				mainScript.player.AddResourceOxygen(amount);
				break;

			case eResourceType.Metal:
				// Adds the resource extracted to the player
				mainScript.player.AddResourceMetal(amount);
				break;

			case eResourceType.NONE:
				break;

			default:
				break;
		}
	}

	/// <summary>
	/// Override from the CBaseEntity
	/// Build a extractor in this resource site
	/// </summary>
	public override void BuildIt() {

		if(!mainScript) {

			// DEBUG
			Debug.LogError("MainScript not set!");
		}

		associatedExtractor = mainScript.BuildExtractor(transform.position);
		// Associate the extractor with the resource site
		associatedExtractor.GetComponent<CBuilding>().resourceSite = this;
		associatedExtractor.GetComponent<CBuilding>().IsIdle(false);	// activate the extractor
		// With an extractor built, there's no need to be able to select this resource site anymore
		Deselect();
		Selectable = false;
	}

	/// <summary>
	/// What to do when all resources from this site are drained
	/// </summary>
	void ResourcesDrained() {

		resourceDrained = true;
		// TODO: disable the resource site? Change it's model or texture?
		// Disable the associated extractor?
		associatedExtractor.GetComponent<CBuilding>().IsIdle(true);
	}
}
