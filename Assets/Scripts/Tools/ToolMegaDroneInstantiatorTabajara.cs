using UnityEngine;
using System.Collections;

/// <summary>
/// Script only to automatically instantiate a bunch of drones, so we can stress out the machine
/// </summary>
public class ToolMegaDroneInstantiatorTabajara : MonoBehaviour {

	// PRIVATE
	
	// PUBLIC
	public Transform droneObject = null;
	Transform droneClone;
	float hSliderValue = 1.0f;
	int droneIdx = 0;
	MouseWorldPosition inputScript = null;
	bool bnSelectingAPosition = false;
	int dronesWanted;
	
	/*
	 * ===========================================================================================================
	 * UNITY'S STUFF
	 * ===========================================================================================================
	 */
	//
	void Awake() {

		// Get the script that handles input
		inputScript = GetComponent<MouseWorldPosition>();

		if(inputScript == null) {

			// DEBUG
			Debug.Log("InputScript not defined");
		}
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(bnSelectingAPosition) {

			if(inputScript.PositionIsAlreadySelected()) {

				CreateDrones(dronesWanted, inputScript.WhatIsThePositionSelected());
				bnSelectingAPosition = false;
			}
		}	
	}

	// 
	void OnGUI() {

		GUI.Label(new Rect(100,10,100,30), "Create " + Mathf.RoundToInt(hSliderValue) + " drones");
		hSliderValue = GUI.HorizontalSlider(new Rect(100,40,100,30), hSliderValue, 1.0f, 50.0f);

		if(!bnSelectingAPosition) {

			if(GUI.Button(new Rect(200,10,50,20), "Create")) {

				// Tell the input script we want to select a place
				inputScript.IWantToSelectAPosition();
				bnSelectingAPosition = true;
				dronesWanted = Mathf.RoundToInt(hSliderValue);
			}
		}
		GUI.Label(new Rect(300,10,200,30), "Drones in scene: " + droneIdx);
	}

	/// <summary>
	/// Instantiate a number of drones
	/// </summary>
	/// <param name="numDrones"> Number of drones to be instantiated </param>
	/// <param name="initialPosition"> A Vector3 with the position to start creating the drones </param>
	void CreateDrones(int numDrones, Vector3 initialPosition) {

		int droneLine = 0;
		int droneColumn = 0;

		if(droneObject == null) {

			// DEBUG
			Debug.LogError("Drone object not defined!");
			return;
		}

		for(int i = 0; i < numDrones; i++) {

			if(droneColumn >= 5) {

				droneLine++;
				droneColumn = 0;
			}

			Vector3 createPosition = new Vector3(3.0f * droneLine, droneObject.transform.position.y, 
					3.0f * droneColumn) + initialPosition;

			droneClone = Instantiate(droneObject, createPosition, Quaternion.identity) as Transform;
			droneClone.name = "Drone_" + droneLine + "_" + droneColumn;

			droneIdx++;
			droneColumn++;
		}
	}
}
