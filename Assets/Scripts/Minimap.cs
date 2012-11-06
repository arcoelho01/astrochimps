using UnityEngine;
using System.Collections;

public class Minimap : MonoBehaviour {

	MainScript mainScript;
	Camera minimapCam;
	Camera mainCamera;
	
	Vector3 bottomLeft;
	Vector3 centerScreen;
	Vector3 mmDirection;
	Vector3 focusCubeOriginalSize;

	float focusRadius;
	bool showMinimap;

	public Transform mmCubeFocus;

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

		focusCubeOriginalSize = mmCubeFocus.localScale;

		// Gets the main camera
		mainCamera = Camera.main;

		//
		EnableMinimap();
	}
	
	// Update is called once per frame
	void Update () {

		// FIXME: find a better place to put this. The ideal setting is all input code in one script only
		if(Input.GetKeyDown(KeyCode.Tab)) {

			if(showMinimap)
				DisableMinimap();
			else
				EnableMinimap();
		}

		// Only check the routine if we need to show the minicam
		if(showMinimap) {
		
			CheckMainCameraProjectionInTheWorld();
		}
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
//		Vector3 focusCubeSize = new Vector3((focusRadius < 20.0f) ? 20.0f:focusRadius, 1, 
//				(focusRadius < 20.0f) ? 20.0f : focusRadius);
		Vector3 focusCubeSize = focusCubeOriginalSize + new Vector3(focusRadius, 0, focusRadius);	

		// Position the cube
		Vector3 focusCubePosition = new Vector3(centerScreen.x, 60, centerScreen.z);

		// Applies the modifications to the cube
		mmCubeFocus.position = focusCubePosition;
		mmCubeFocus.localScale = focusCubeSize;
		mmCubeFocus.rotation = rotation;
	}


	/// <summary>
	/// Disable the viewing of the minimap on the screen, disabling it's camera's rendering
	/// </summary>
	public void DisableMinimap() {

		// Disables the camera
		minimapCam.enabled = false;
		showMinimap = false;
	}

	/// <summary>
	/// Enable the viewing of the minimap on the screen, enabling back the camera rendering
	/// </summary>
	public void EnableMinimap() {

		// Enables the camera
		minimapCam.enabled = true;
		showMinimap = true;
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
