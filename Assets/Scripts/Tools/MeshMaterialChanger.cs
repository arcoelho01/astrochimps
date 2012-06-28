using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// This class will hold the mesh original materials and change it for other. Also provides a method to restore
/// the object's original materials
/// </summary>
public class MeshMaterialChanger : MonoBehaviour {

	// Number of materials in this object
	int materialsNumber;
	// Array to hold all original materials in the object
	Material[] originalMaterials;
	// Transform of the model itself (the first child in the hierarchy, usually)
	private Transform myObject;
	// Material with disabled aspect to be set in the inspector
	public Material disabledMaterial;

	//
	void Awake() {

		// First, we access the mesh object inside this hierarchy
		foreach(Transform child in this.transform) {

			myObject = child;
			break;
		}

		if(!myObject) {

			// DEBUG
			Debug.LogError("Could not find the object in this hierarchy: " + this.transform);
		}

		// Ok, now we save the materials
		originalMaterials = new Material[myObject.renderer.materials.Length];
		Array.Copy(myObject.renderer.materials, 0, originalMaterials, 0, myObject.renderer.materials.Length);

		// and keep the materials count
		materialsNumber = originalMaterials.Length;
	}


	// Use this for initialization
	void Start () {

		Material[] disabledMaterials = new Material[materialsNumber];

		// start with the disabled material
		if(disabledMaterial) {

			for(int nIdx = 0; nIdx < materialsNumber; nIdx++) {
			
				// Create an array with disabled materials only
				disabledMaterials[nIdx] = disabledMaterial;
			}

			// And apply them to the object
			myObject.renderer.materials = disabledMaterials;
		}
	}

	/// <summary>
	/// Restores the saved materials
	/// </summary>
	public void RestoreMaterials() {

		myObject.renderer.materials = originalMaterials;
	}
}
