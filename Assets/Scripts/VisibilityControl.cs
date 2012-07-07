using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisibilityControl : MonoBehaviour {
	
	private bool visible;
	public bool keepVisible;
	public float sight_range;
	public AudioClip sfxRecursoLocalizado;
	public AudioClip  sfxPecaLocalizada;
	// Use this for initialization
	void Start () {

		if (gameObject.tag.CompareTo("Building") == 0 || gameObject.tag.CompareTo("Resource") == 0 
				|| gameObject.tag == "RocketPart") {

			// Trick stolen from Fernando: add a second bar in the line below to uncomment the command. Remove
			// it to comment the entire block
			/*
			// DEBUG
			Debug.Log("Object " + gameObject.name + " is a building. Keep visible true");
			//*/
			
			keepVisible = true;
		}
		else {
			
			/*/
			// DEBUG
			Debug.Log("Object " + gameObject.name + " not a building. Keep visible false.");
			//*/

			keepVisible = false;
		}


		// FIXME: we can simplify this! All allied Objects visible, all other not visible
	
		// Allied units
		if (gameObject.layer == MainScript.alliedLayer){
			
			/*
			// DEBUG	
			Debug.Log("Object " + gameObject.name + " is in layer Allied");
			//*/

			if(this.CompareTag("Drone") || this.CompareTag("Monkey")){

				/*
				// DEBUG	
				Debug.Log("Object " + gameObject.name + " is ALLIED and a Drone, enabling his renderer");
				//*/
				//
				SetObjectVisible();
			}
			else {
				//*
				// DEBUG	
				Debug.Log("Object " + gameObject.name + " directly renderer enable");
				//*/

				renderer.enabled = true;
				visible = true;
			}
		} // END IF - layer == allied
		else if (gameObject.layer == MainScript.enemyLayer ) {
			
			/*
			// DEBUG	
			Debug.Log("Object " + gameObject.name + " is in enemy layer.");
			//*/

			if(this.CompareTag("Drone") || this.CompareTag("Monkey") || this.CompareTag("Building")){

				SetObjectNotVisible();
			}
			else {

				// DEBUG
				Debug.LogWarning(this.transform + " still acessing the renderer directly (have no mesh object)");

				renderer.enabled = false;
			}

		} // END IF -  layer == enemy
		else if (gameObject.layer == MainScript.neutralLayer) {

			SetObjectNotVisible();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if (gameObject.layer == MainScript.alliedLayer) {

			/*
			// DEBUG	
			Debug.Log("Object " + gameObject.name + " is in layer Allied");
			//*/

			return;
		}

		if(AppearsToEnemy() && !visible){
			visible = true;
			if ( this.CompareTag("Resource"))
				if (sfxRecursoLocalizado)
					AudioSource.PlayClipAtPoint(sfxRecursoLocalizado, transform.position);
			if ( this.CompareTag("RocketPart"))
				if (sfxPecaLocalizada)
					AudioSource.PlayClipAtPoint(sfxPecaLocalizada, transform.position);
			
			
			SetObjectVisible();
		}

		if (visible && !keepVisible){
			
			visible = AppearsToEnemy();
			
			if(this.CompareTag("Drone") || this.CompareTag("Monkey")){
			
				SetObjectVisible();
			}
			else
				renderer.enabled = visible;
		}
	}
	
	
	bool AppearsToEnemy() { 

		// FIXME: we're building a list with units at each frame! Main Script already have all units lists, use from
		// there

		GameObject[] gosArray;        
		List<GameObject> gos = new List<GameObject>();

		gosArray = GameObject.FindGameObjectsWithTag("Drone");

		foreach (GameObject go in gosArray)  {

				gos.Add(go);
		}

		gosArray = GameObject.FindGameObjectsWithTag("Monkey");
		foreach (GameObject go in gosArray)  {

				gos.Add(go);
		}
   
		Vector3 position = transform.position;  

		foreach (GameObject go in gos) {
			
			// We don't need to test allied units, because they are always visible to the player
			if (gameObject.layer == MainScript.alliedLayer) {

				/*
				// DEBUG	
				Debug.Log("Object " + go.name + " is in allied layer.");
				//*/

				continue;
			}
			
			// Don't test between teams
			if (gameObject.layer == go.layer ) {

				/*
				// DEBUG	
				Debug.Log("Object " + go.name + " is in the same layer as " + gameObject.name);
				//*/

				continue;
			}

			Vector3 diff = go.transform.position - position;       
			float curDistance = diff.sqrMagnitude;  
			VisibilityControl enemyScript = go.GetComponent<VisibilityControl>();
			
			// If there is no Visibility Script attached to the object, dont do anything;
			if (enemyScript == null)
				continue;
			
			if (curDistance < enemyScript.getSightRange() ) {  

				/*
				// DEBUG	
				Debug.Log("Encontrado");
				//*/
				return true; 
			}   

			/*
			// DEBUG	
			Debug.Log("Object " + go.name + " is a Drone and is false to AppearsToEnemy()");
			//*/
		}     

			
	  return false;
	}
	
	public float getSightRange()
	{
		return sight_range;
		
	}

	/// <summary>
	/// Tells if this unit is visible or not
	/// </summary>
	/// <returns> True if the unit is visible, false otherwise </returns>
	public bool IsVisible() {

		return visible;
	}

	/// <summary>
	/// Set the object to visible 
	/// </summary>
	public void SetObjectVisible() {

		Transform mesh = this.transform.FindChild("Mesh");
		if(mesh)
			mesh.gameObject.SetActiveRecursively(true);

		// Added: now that each object hava a specific icon, it must be made visible too
		Transform icon = this.transform.FindChild("Icon");
		if(icon)
			icon.gameObject.renderer.enabled = true;

		visible = true;
	}

	/// <summary>
	/// Set object to not visible
	/// </summary>
	public void SetObjectNotVisible() {

		Transform mesh = this.transform.FindChild("Mesh");
		if(mesh)
			mesh.gameObject.SetActiveRecursively(false);

		// Added: now that each object have a specific icon, it must be made (in)visible too
		Transform icon = this.transform.FindChild("Icon");
		if(icon != null)
			icon.gameObject.renderer.enabled = false;

		visible = false;
	}
}
