using UnityEngine;
using System.Collections;

/// <summary>
/// Class with building definitions
/// </summary>
public class CBuilding : CBaseEntity {

	// PRIVATE
	//Material myMaterial;
	
	// PUBLIC
	public enum eBuildingType { CommandCenter, ResourceExtractor, Farm, SecurityCenter, 
		DroneFactory, ResearchLab }; // Building type
	public eBuildingType buildingType;
	
	public AudioClip sfxLoadMonkey;
	public AudioClip sfxSelected;
	
	public float costTime; // Seconds needed to build this structure
	public float costOxygen;	// Amount of oxygen units needed to build this structure
	public float costMetal;	// Amount of metal resources needed to build this structure
	public int level = 1;	// Level of this building
	public float workTime = 1.0f;
	public float myTimer = 0.0f;

	public Transform monkeyInside; // There's a monkey inside this building?
	public bool idleStatus = false;	// The building is operational and fully functional?
	public CBaseEntity resourceSite; // If it's an extractor, from which resource site it's extracting
	public Transform showInfoObject;

	public Transform sabotagedParticleSystem;
	Transform sabotagedPSObj = null;
	Vector3 v3ControlSpot;
	Vector3 v3ExitSpot;

	// ================== MERGE

	public enum TipoEstrutura{
		CANO_CENTRAL,
		CANO_EXTRATOR,
		CENTRO_COMANDO,
		FAZENDA,
		GARAGEM,
		CENTRAL_SEGURANCA,
		FABRICA_DRONES,
		SLOT,
		EXTRATOR,
		LABORATORIO,
		PLATAFORMA_LANCAMENTO
	} ;
	public bool sabotado;
	public bool conectado;
	public bool construido;
	public int vida;
	public TipoEstrutura tipo;
	// ================== MERGE


	/*
	 * ===========================================================================================================
	 * UNITY'S STUFF
	 * ===========================================================================================================
	 */
	
	public override Transform Select() {

		if(sfxSelected) {
		
			AudioSource.PlayClipAtPoint(sfxSelected, transform.position);
		}
		return base.Select();
	}

	/// IMPORTANT! NEVER DEFINE A START FUNCTION HERE!

	/// <summary>
	/// When the script is initialized
	/// </summary>
	void Awake() {

		// Set the default settings for all the buildings
		Selectable = true; // All building are selectable
		Type = eObjType.Building;

		// ================== MERGE
		if((tipo ==TipoEstrutura.CANO_CENTRAL) || (tipo == TipoEstrutura.SLOT))
		{
				sabotado = false;
				conectado = true;
				construido = false;
				vida = 100;
		}
		
		// Get the position of some helpers objects
		GetSweetSpotAndMeshObject();

		// if is the Command Center, get the position of the monkey when inside the building
		if(buildingType == eBuildingType.CommandCenter) {

			GetControlRoomSpot();
			GetExitSpot();
		}
	}

	/// <summary>
	/// In this update, we do whatever the building is supposed to do
	/// </summary>
	void Update() {

		// Only update the timer if the building is working
		if(!idleStatus && !sabotado)
			myTimer += Time.deltaTime;

		// Check if we need to show a menu
		if(isSelected && monkeyInside != null) {

			mainScript.bottomMenu.BuildingInfoMenuEnable(this);
		}

		if(buildingType == eBuildingType.ResourceExtractor) {

			if(!resourceSite) {

				// DEBUG
				Debug.LogError("Extractor without an associated resource site");
			}

			if(sabotado) {
	
				//if(meshObject.renderer)
				//	meshObject.renderer.material = materialDisabled;
				
				//foreach(Transform child in meshObject) {
				//	if(child.renderer)
				//		child.renderer.material = materialDisabled;
				//	//Debug.Log(child.name);
				//}

				return;
			}

			// Adding animation
			if(meshObject && !idleStatus && !sabotado)
				meshObject.animation.Play("Cycle");

			if(myTimer >= workTime) {

				ExtractResource();

				myTimer = 0.0f;
			}
		}
	}

	/// <summary>
	/// Change the status of this building.
	/// </summary>
	/// <param name="bnIdleStatus"> A boolean. True indicates that the building is temporaly idle, so it not
	/// produces anything. False the building is functional.
	/// </param>
	public void IsIdle(bool bnIdleStatus) {

		idleStatus = bnIdleStatus;
	}

	/// <summary>
	/// Create an instance of the selected building, place it and activate it
	/// </summary>
	public override void BuildIt() {

		// Run the BuildIt from CBaseEntity
		base.BuildIt();
	}

	/// <summary>
	/// Do whatever this building is suppose to do, according to his type
	/// </summary>
	public override void Work() {

	}

	/// <summary>
	/// Put a monkey inside this building. Building controller directly by monkey are more efficient somehow
	/// </summary>
	/// <param name ="monkeyIn">Transform of the monkey that will be inside the building</param>
	public void PutAMonkeyInside(Transform monkeyIn) {

		if(monkeyInside != null) {
			
			// DEBUG
			Debug.Log("There's already a monkey inside this building!");
			return;
		}
		
		monkeyInside = monkeyIn;
		monkeyInside.gameObject.GetComponent<CBaseEntity>().Deselect();
		// While inside the building, the monkey is not selectable anymore
		monkeyInside.gameObject.GetComponent<CBaseEntity>().Selectable = false;	
		AudioSource.PlayClipAtPoint(sfxLoadMonkey, transform.position);
		
		//this.Select();//gameObject.GetComponent<CBaseEntity>().Select();
		
		// TODO: for now we put the monkey model on top of the building :P
		monkeyInside.transform.position =	v3ControlSpot;
		monkeyInside.transform.rotation = Quaternion.identity;
	}

	/// <summary>
	/// Get the monkey out of the building, so it will be free to roam the terrain. The building is no more
	/// affected by his bonus
	/// </summary>
	public void GetTheMonkeyOut() {
		
		if(!monkeyInside) { // There's no monkey here!
			
			return;
		}
		
		if(sfxLoadMonkey)
			AudioSource.PlayClipAtPoint(sfxLoadMonkey, transform.position);

		monkeyInside.transform.position = v3ExitSpot;
		monkeyInside.gameObject.GetComponent<CBaseEntity>().Selectable = true;	
		monkeyInside = null;
	}

	/// <summary>
	/// Check if there's a monkey inside this building
	/// </summary>
	/// <returns> The Transform of the monkey inside this building, if any </returns>
	public Transform TheresAMonkeyInside() {

		return monkeyInside;
	}

	/// <summary>
	/// Extract resource from the resource site
	/// </summary>
	void ExtractResource() {

		CResource.ExtractedResource extractedResource;
		float extractionAmount = level * 1.5f;
		string resourceString = "";

		// Try to extract resources
		extractedResource = resourceSite.GetComponent<CResource>().ExtractResource(extractionAmount);

		// Check if there's any resource left in the resource site. If not, disable the building
		if(extractedResource.isDrained) {

			IsIdle(true);
		}

		// Check what we got
		switch(extractedResource.type) {

			case CResource.eResourceType.Oxygen:
				// Adds the resource extracted to the player
				mainScript.player.AddResourceOxygen(extractedResource.amount);
				resourceString = "Oxygen ";
				break;

			case CResource.eResourceType.Metal:
				// Adds the resource extracted to the player
				mainScript.player.AddResourceMetal(extractedResource.amount);
				resourceString = "Metal ";
				break;

			case CResource.eResourceType.NONE:
					break;

				default:
				break;
		}

		// Instantiate a info text
		StartCoroutine(ShowInfoForExtractedResource(resourceString, extractedResource.amount));
	}

	/// <summary>
	/// Show a text tag with the resource extracted and it's amount. Floats up the screen and then vanishes.
	/// Useful to show the player what the extractor is doing
	/// </summary>
	IEnumerator ShowInfoForExtractedResource(string stResource, float amountExtracted) {

		Transform myInfo = Instantiate(showInfoObject, this.transform.position, Quaternion.identity) as Transform;

		myInfo.transform.parent = this.transform;
		string infoText = stResource + " " + amountExtracted;
		myInfo.GetComponent<ShowInfoPanel>().SetInfoText(infoText, sweetSpot);

		yield return new WaitForSeconds(2.0f);

		if(myInfo)
			Destroy(myInfo.gameObject);
	}

	/// <summary>
	/// Fixing the building by an engineer. For now, it only changes the state from "sabotaged" to "not 
	/// sabotaged"
	/// TODO: add some cost in resources (metal?) when this is done. And check if we can afford it before fixing
	/// </summary>
	public void FixByEngineer() {

		// FIXME: Cost of repairing: say, 20% of the building cost?
		float fRepairCost = costMetal * 0.2f;

		if(mainScript.player.metalLevel >= fRepairCost && sabotado) {
		
			mainScript.player.SubResourceMetal(fRepairCost);

			Desabotage();	
			// Instantiate a info text
			StartCoroutine(ShowInfoForExtractedResource("Metal ", -fRepairCost));
		}
	}

	/// <summary>
	/// Sabotage this building, adding a visual aid to the player
	/// </summary>
	public void Sabotage() {

		sabotado = true;

		// Add a visual aid
		if(sabotagedParticleSystem) {

			// Instantiate the particle system
			sabotagedPSObj = Instantiate(sabotagedParticleSystem, sweetSpot, 
						Quaternion.Euler(-90,0,0)) as Transform;

			// Put it as child
			sabotagedPSObj.transform.parent = this.transform;
		}
	}

	/// <summary>
	/// "Desabotage" (is this a real word?) this building, removing the visual aid to the player
	/// </summary>
	public void Desabotage() {
	
		sabotado = false;
		//renderer.material = myMaterial;

		if(sabotagedPSObj)
			Destroy(sabotagedPSObj.gameObject);
	}

	/// <summary>
	/// Find the object "ControlSpot", which designates where the monkey should be when inside the building
	/// </summary>
	void GetControlRoomSpot() {

			GameObject controlRoomSpotObj = GameObject.Find("ControlSpot");

			if(!controlRoomSpotObj) {

				// DEBUG
				Debug.LogError("Object 'ControlSpot' not found for this Command Center");
			}
			else {
			
				v3ControlSpot = controlRoomSpotObj.transform.position;
			}
	}
	
	/// <summary>
	///	Find the coordinates for the object 'ExitSpot', which designates where an unit should be when exiting
	///	the building
	/// </summary>
	void GetExitSpot() {

			GameObject exitSpotObj = GameObject.Find("ExitSpot");

			if(!exitSpotObj) {

				// DEBUG
				Debug.LogError("Object 'ControlSpot' not found for this Command Center");
			}
			else {
			
				v3ExitSpot = exitSpotObj.transform.position;
			}
	}

	/// <summary>
	/// Returns the position of the exit spot of this building
	/// </summary>
	/// <returns> A Vector3 with the position of the Exit Spot object </returns>
	public Vector3 GetExitSpotPosition() {

		return v3ExitSpot;
	}
}

