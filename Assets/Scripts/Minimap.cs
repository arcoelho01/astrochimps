using UnityEngine;
using System.Collections;

public class Minimap : MonoBehaviour {

	MainScript mainScript;
	Camera minimapCam;
	
	Vector3 bottomLeft;
	Vector3 centerScreen;
	Vector3 mmDirection;

	float focusRadius;

	public Transform mmCubeFocus;

	public Rect minimapPosOnScreen;

	public float x = 0.03f;
	public float y = 0.78f;
	public float xsize = 0.15f;
	public float ysize = 0.15f;

	// Use this for initialization
	void Start () {
	
		// Cache the main script
		mainScript = GameObject.Find("/Codigo").GetComponent<MainScript>();

		if(!mainScript) {

			// DEBUG
			Debug.LogError("MainScript object not found. Check the code.");
		}

		minimapCam = this.camera;

		if(!minimapCam) {

			// DEBUG
			Debug.LogError("Camera object not found for the minimap.");
		}

		if(!mmCubeFocus) {

			// DEBUG
			Debug.LogError("Set a cube object to show the focus on the minimap");
		}

		minimapPosOnScreen = new Rect(x,y,xsize, ysize);;
	}
	
	// Update is called once per frame
	void Update () {

		CheckMainCameraProjectionInTheWorld();

		//minimapCam.rect = new Rect(x,y,xsize, ysize);
	}

	/// <summary>
	/// Casts a ray in each corner, so we know what part of the terrain is in view. With this, we can draw a
	/// rectangle on the minimap showing to the player what portion he is seeing
	/// </summary>
	void CheckMainCameraProjectionInTheWorld() {

		RaycastHit hit;

		// First, let's find the bottom left point of the screen projected in the fake ground (we do not use the
		// ground layer, because it will affect the raycast positions on the depressions or mountains
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(0,0,0));
		
		if(Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, 1 << MainScript.minimapGroundLayer)) {

			bottomLeft = hit.point;
		}
		else
			bottomLeft = Vector3.zero;

		// Now the center of the screen projected in the fake ground
		ray = Camera.main.ScreenPointToRay(new Vector3(Camera.main.pixelWidth*0.5f,Camera.main.pixelHeight*0.5f,0));
		
		if(Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, 1 << MainScript.minimapGroundLayer)) {

			centerScreen = hit.point;
		}
		else
			centerScreen = Vector3.zero;

		// Ignores the height component
		centerScreen.y = 0;
		bottomLeft.y = 0;

		// Center to the bottom left, then invert it
		mmDirection = bottomLeft - centerScreen;
		mmDirection *= -1;

		// Distance from the center to the bottom of the screen (in the world)
		focusRadius = mmDirection.magnitude;
		// Rotate to where the camera is pointing
		Quaternion rotation = Quaternion.LookRotation(mmDirection, Vector3.up);

		// The scale is set to a minimum of 20.0f so it can always be visible in the minimap
		Vector3 focusCubeSize = new Vector3((focusRadius < 20.0f) ? 20.0f:focusRadius, 1, 
				(focusRadius < 20.0f) ? 20.0f : focusRadius);

		// Position the cube
		Vector3 focusCubePosition = new Vector3(centerScreen.x, 60, centerScreen.z);

		// Applies the modifications to the cube
		mmCubeFocus.position = focusCubePosition;
		mmCubeFocus.localScale = focusCubeSize;
		mmCubeFocus.rotation = rotation;
	}

	void OnDrawGizmos() {
		/*
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(bottomLeft, 10.0f);

		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(centerScreen, 10.0f);

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(centerScreen, focusRadius);
		Gizmos.DrawRay(centerScreen, mmDirection * focusRadius);
		//*/
	}
}
