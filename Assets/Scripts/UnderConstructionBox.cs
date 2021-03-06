using UnityEngine;
using System.Collections;
using Pathfinding;

public class UnderConstructionBox : MonoBehaviour {

	bool bnStart = false;
	bool playedAudio = false;
	float buildTime = 0.0f;
	Transform prefabToBuild;
	Transform builtOver;
	Transform newBuilding;
	float fTimer = 0.0f;
	Transform ProgressIcon;
	public	AudioClip sfxConstrucaoConcluida;


	// Use this for initialization
	void Start () {
	
		// Get the progress icon, bar, or whatever
		ProgressIcon = this.transform.FindChild("ProgressBar");
		
	}
	
	// Update is called once per frame
	void Update () {
	
		if(!bnStart)
			return;
		if (builtOver.GetComponent<CBuilding>() != null)
			builtOver.GetComponent<CBuilding>().Selectable = false;
		
		fTimer += Time.deltaTime;

		// Timer running
		if(fTimer < buildTime) {

			if(ProgressIcon) {


				ProgressIcon.renderer.material.SetFloat("_Cutoff", 1-Mathf.InverseLerp(0, buildTime, fTimer));
			}

			return;
		}
		
		// Ok, timer is over, build complete

		// 1 - Play the final animation
		StartCoroutine(DestroyBox());
		
		// 2 - And in parallel, instantiate the new building, if any
		if(prefabToBuild && !newBuilding) {
			
			CreateNewBuilding();
			
			
		}
		if (!playedAudio){
			AudioSource.PlayClipAtPoint(sfxConstrucaoConcluida, transform.position);
			playedAudio = true;
		}
		
		
	}

	/// <summary>
	/// Sets the building time for this construction box
	/// </summary>
	public void SetBuildingTime(float buildTime) {

		this.buildTime = buildTime;
		bnStart = true;	// Starts the timer
	}

	/// <summary>
	/// Set the prefab object to build
	/// </summary>
	public void SetPrefabToBuild(Transform prefabToBuild) {

		this.prefabToBuild = prefabToBuild;
	}

	/// <summary>
	/// Routine to play the complete animation and, after it's done, destroy this object
	/// </summary>
	IEnumerator DestroyBox() {

		animation.Play("Complete");

		// Wait for the animation to finish
		yield return new WaitForSeconds(animation["Complete"].clip.length);

		// Destroy the object
		Destroy(this.gameObject);
	}

	/// <summary>
	/// Get the object where this will be built
	/// </summary>
	public void SetBuiltOverObject(Transform builtOver) {

		this.builtOver = builtOver;
	}

	/// <summary>
	/// Actually creates a building. First, instantiate it, then do the basic setup if needed
	/// </summary>
	void CreateNewBuilding() {
		// Already instantiated? Get out of here!
		
		if(newBuilding)
			return;

		newBuilding = Instantiate(prefabToBuild, this.transform.position, Quaternion.identity) as Transform;

		// Setup the new building
		CBuilding myBuildingClass = newBuilding.GetComponent<CBuilding>();

		
		// ok, the building has the CBuilding class
		if(myBuildingClass) {


			// So check what is the type of the building, and do the setup
			switch(myBuildingClass.buildingType) {
				
				case CBuilding.eBuildingType.ResourceExtractor:
					{
			
						CResource myResourceObject = builtOver.GetComponent<CResource>();
						
						// Setup the extractor
						// 1 - link it to the resource site
						myBuildingClass.resourceSite = myResourceObject;
						// 2 - Start the extractor
						myBuildingClass.IsIdle(false);
						// 3 - Double link: associate the extractor with the Resource
						myResourceObject.associatedExtractor = newBuilding;
					}
					break;

				default:
					break;
			}
		}

		// Updates the A* graph
		UpdateAStarGraphWithNewBuilding();

	}

	/// <summary>
	/// Updates the A* graph with the recently built building, using it's collider
	/// </summary>
	void UpdateAStarGraphWithNewBuilding() {

		Collider col;

		col = newBuilding.collider; 

		if(!col) {

			// DEBUG
			Debug.LogError(this.transform + " No collider found to building " + newBuilding);
		}

		Bounds newBounds = col.bounds;
		GraphUpdateObject guo = new GraphUpdateObject(newBounds);
		AstarPath.active.UpdateGraphs(guo);
	}
}
