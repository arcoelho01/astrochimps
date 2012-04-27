using UnityEngine;
using System.Collections;

/// <summary>
/// This scripts change between the Unity's made terrain for the moon or a simple plane, so we can see how
/// heavy is to process a terrain
/// </summary>
public class ToolTerrainSelector : MonoBehaviour {

	public bool useUnityTerrain = true;
	public Transform moonTerrainObject;
	public Transform moonPlaneObject;

	void Awake() {

		if(!moonTerrainObject || !moonPlaneObject) {

			// DEBUG
			Debug.LogError("Terrains for moon not set, move the objects to the inspector!");
		}

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(useUnityTerrain) {

			if(moonTerrainObject.active != true) {

				moonTerrainObject.active = true;
				moonPlaneObject.active = false;
			}
		}
		else {

			if(moonTerrainObject.active) {

				moonTerrainObject.active = false;
				moonPlaneObject.active = true;
			}
		}
	
	}

	void OnGUI() {

		useUnityTerrain = GUI.Toggle(new Rect(300, 40, 200, 70), useUnityTerrain, "Use Unity's terrain?");
	}
}
