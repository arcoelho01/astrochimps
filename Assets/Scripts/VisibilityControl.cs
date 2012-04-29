using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisibilityControl : MonoBehaviour {
	
	private bool visible;
	public bool keepVisible;
	public float sight_range;
	private int alliedMask = 10;
	private int enemyMask = 11;
	private int neutralMask = 12;
	
	// Use this for initialization
	void Start () {

		if (gameObject.tag.CompareTo("Building") == 0 || gameObject.tag.CompareTo("Resource") == 0) {

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
	
		if (gameObject.layer == alliedMask){
			
			/*
			// DEBUG	
			Debug.Log("Object " + gameObject.name + " is in layer Allied");
			//*/

			if(this.CompareTag("Drone") || this.CompareTag("Monkey")){

				/*
				// DEBUG	
				Debug.Log("Object " + gameObject.name + " is ALLIED and a Drone, enabling his renderer");
				//*/

				Transform mesh = this.transform.FindChild("Mesh");
				mesh.gameObject.SetActiveRecursively(true);
			}
			else {

				/*
				// DEBUG	
				Debug.Log("Object " + gameObject.name + " renderer enable");
				//*/

				renderer.enabled = true;
			}
		}
		else if (gameObject.layer == enemyMask ) {
			
			/*
			// DEBUG	
			Debug.Log("Object " + gameObject.name + " is in enemy layer.");
			//*/

			if(this.CompareTag("Drone") || this.CompareTag("Monkey")){

				Transform mesh = this.transform.FindChild("Mesh");
				mesh.gameObject.SetActiveRecursively(false);
			}
			else
				renderer.enabled = false;
		}
		else if (gameObject.layer == neutralMask) {

				renderer.enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if (gameObject.layer == alliedMask) {

			/*
			// DEBUG	
			Debug.Log("Object " + gameObject.name + " is in layer Allied");
			//*/

			return;
		}
		
		if(AppearsToEnemy() && !visible){
			visible = true;
			
			if(this.CompareTag("Drone") || this.CompareTag("Monkey")){

				Transform mesh = this.transform.FindChild("Mesh");
				mesh.gameObject.SetActiveRecursively(visible);
			}
			else 
				renderer.enabled = visible;
		}

		if (visible && !keepVisible){
			
			visible = AppearsToEnemy();
			
			if(this.CompareTag("Drone") || this.CompareTag("Monkey")){
			
				Transform mesh = this.transform.FindChild("Mesh");
				mesh.gameObject.SetActiveRecursively(visible);
  
			}
			else
				renderer.enabled = visible;
		}
		
			
	}
	
	// Only working for Drones
	
	bool AppearsToEnemy() { 
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
			
			// IF Allied dont test
			if (gameObject.layer == alliedMask) {

				/*
				// DEBUG	
				Debug.Log("Object " + go.name + " is in allied layer.");
				//*/

				continue;
			}
			
			// Dont test between teams
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
}
