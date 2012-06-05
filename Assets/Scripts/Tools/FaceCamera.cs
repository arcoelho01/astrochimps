using UnityEngine;
using System.Collections;

/// <summary>
/// This simple script rotates an object so it always faced towards the camera. Useful for 2D stuff mapped into
/// 3D meshes, like a progress bar or something.
/// </summary>
public class FaceCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	
		transform.LookAt(Camera.main.transform);
	}
}
