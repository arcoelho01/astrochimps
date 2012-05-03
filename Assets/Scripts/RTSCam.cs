using UnityEngine;
using System.Collections;

/// <summary>
///	RTS-like camera
///	Camera move: on the borders of the screen, or ASDW keys
/// Camera rotation: QE keys
/// Zoom: mouse wheel, button 3 resets the zoom
/// 
/// INPUT MANAGER:
/// Uses standard Horizontal and Vertical axis in Unity3D;
/// Must define CamRotation;
/// </summary>
public class RTSCam : MonoBehaviour {

	// PRIVATE
	Camera myCam;
	bool bnCamModified = false;
	bool bnCamRotateWithMouse = false;
	bool bnCamMoving = false;
	float x = 0.0f;
	float y = 0.0f;
	Vector3 screenWorldCenter = Vector3.zero;
	Vector3 camPanDirection = Vector3.zero;
	Vector3 worldLeftBoundary;
	Vector3 worldTopBoundary; 
	Vector3 screenHAxis;	// Horizontal axis on the screen
	Vector3 screenVAxis;	// Vertical axis on the screen
	Vector3 screenCenter;	// Center of the screen on the world
	Quaternion camDefaultRotation;

	// Define min and max angles for rotation, so the camera doesn't cross the ground when rotating
	float camRotationMinAngleX = 10.0f; // min angle for rotation in X
	float camRotationMaxAngleX = 80.0f; // max angle for rotation in X

	float screenCenterX;
	float screenCenterY;
	
	float panSpeed;

	Vector3 mouseNow;
	Vector3 mouseBefore;
	Vector3 mouseDelta;

	Transform objectInFocus = null;
	// PUBLIC
	public static RTSCam Script; 
	public float zoomMin = 10.0f;
	public float zoomMax = 80.0f;
	public float zoomDefault = 20.0f;
	public float zoomSpeed = 150.0f;
	public float rotateSpeed = 1.5f;
	public float panSpeedBase = 25.0f;
	public float camAngleDefault = 45.0f;
	public float cameraDistance;

	public Transform micListener = null;

	// Defines the border of the screen. When the mouse is within this amount of pixels from the border, pan
	// the camera
	public int screenPanEdge = 10;

	MouseWorldPosition inputStuffScript = null;
	/*
	 * ===========================================================================================================
	 * UNITY'S STUFF
	 * ===========================================================================================================
	 */

	/// <summary>
	/// Executed when the script is loaded
	/// </summary>	
	void Awake() {

		Script = this;

		myCam = gameObject.GetComponent<Camera>();

		inputStuffScript = GameObject.Find("Codigo").gameObject.GetComponent<MouseWorldPosition>();
		if(inputStuffScript == null) {

			// DEBUG
			Debug.LogError("Cannot find the input script");
		}

		if(micListener == null) {

			// DEBUG
			Debug.LogError("Cannot find the 'microphone' object. Set it in the inspector.");
		}
	}	

	/// <summary>
	/// Class starting
	/// </summary>
	void Start () {

		camDefaultRotation = transform.rotation;

		x = myCam.transform.eulerAngles.x;
		y = myCam.transform.eulerAngles.y;

		screenCenterX = Screen.width * 0.5f;
		screenCenterY = Screen.height * 0.5f;

		cameraDistance = zoomDefault;

		PositionCamera();
	}

	/// <summary>
	///	Update
	/// </summary>
	void Update() {
		
		// FIXME: put this in the mouse event script, not here!
		// Calculates the mouse movement
		mouseBefore = mouseNow;

		// Resets the camera movement flags
		bnCamModified = false;
		bnCamRotateWithMouse = false;

		CheckCameraRotation();
		CheckCameraZoom();

		// Only pan when not rotating with the mouse
		if(!bnCamRotateWithMouse) {

			CheckCameraPan();
		}

		if(Input.GetMouseButton(2)) {

			ResetCamera();
		}

		// FOCUS ON OBJECT
		if(Input.GetKeyDown("space")) {

			// Is a unit selected?
			objectInFocus = inputStuffScript.GetSelectedObject();
			LookAtTarget(objectInFocus);
		}

		if(bnCamMoving) {

			LookAtTarget(objectInFocus);
		}

		// Any changes to the camera? Redraw it!
		if(bnCamModified) {

			PositionCamera();
		}
	}

	/*
	 * ===========================================================================================================
	 * SCRIPT'S STUFF
	 * ===========================================================================================================
	 */

	/// <summary>
	/// Reposition the camera, according to the new rotation and pan
	/// </summary>
	void PositionCamera() {

		// Calculates the new rotation
		Quaternion rotation = Quaternion.Euler(x,y, 0);
		Vector3 position = rotation * new Vector3(0, 0, -cameraDistance) + camPanDirection;		

		// Applies the new settings
		transform.rotation = rotation; 
		transform.position = position;

		//
		RepositionMicListener();
	}

	/// <summary>
	/// Reset the camera to it's initial view angle
	/// </summary>
	void ResetCamera() {

		transform.rotation = camDefaultRotation;

		x = myCam.transform.eulerAngles.x;
		y = myCam.transform.eulerAngles.y;

		PositionCamera();
	}

	/// <summary>
	/// Adjusts the pan speed according to the amount of zoom. Closer moves slower, farther zoom moves faster
	/// </summary>
	float AdjustedPanSpeed() {

		// TODO: improve this part. Use some real mathematics, please! :P
		return (cameraDistance - zoomMin) / (zoomMax - zoomMin) * panSpeedBase + panSpeedBase;
	}

	/// <summary>
	/// Moves the camera so it focus on a specific target
	/// </summary>
	/// <param name="objectToLook">An object in the game to gain camera focus</param>
	void LookAtTarget(Transform objectToLook) {

		if(objectToLook == null)
			return;

		if(!bnCamMoving) {
			
			bnCamMoving = true;
		}

		camPanDirection = Vector3.Lerp(camPanDirection, objectToLook.transform.position, Time.deltaTime * 5.0f);
		bnCamModified = true;

		// Check
		if(Vector3.Distance(camPanDirection, objectToLook.transform.position) < 0.1f) {

			bnCamMoving = false;
			// DEBUG
			Debug.Log("Moving ended.");
		}
	}

	/// <summary>
	/// Pan the camera when the mouse cursor gets near the screen border
	/// </summary>
	void CheckCameraPan() {

		// CAMERA PAN
		// 1 - Create the horizontal e vertical axes from the camera position
		screenWorldCenter = camera.ScreenToWorldPoint(new Vector3(screenCenterX,screenCenterY, camera.nearClipPlane));
		worldLeftBoundary = camera.ScreenToWorldPoint(new Vector3(0, screenCenterY, camera.nearClipPlane)); 
		worldTopBoundary = camera.ScreenToWorldPoint(new Vector3(screenCenterX, 0, camera.nearClipPlane));
		screenHAxis = screenWorldCenter - worldLeftBoundary;
		screenVAxis = screenWorldCenter - worldTopBoundary;

		// Corrects the scroll speed, depending on how farther the camera is
		panSpeed = AdjustedPanSpeed();

		// HORIZONTAL PAN
		if(Input.GetAxis("Horizontal") < 0 || Input.mousePosition.x < screenPanEdge ) {

			camPanDirection -= screenHAxis.normalized * Time.deltaTime * panSpeed; 
			bnCamModified = true;
		}
		else if(Input.GetAxis("Horizontal") > 0 || Input.mousePosition.x > (Screen.width-screenPanEdge)) {

			camPanDirection += screenHAxis.normalized * Time.deltaTime * panSpeed; 
			bnCamModified = true;
		}

		// VERTICAL PAN
		if(Input.GetAxis("Vertical") < 0 || Input.mousePosition.y < screenPanEdge ) {

			Vector3 screenVAxisForPan = screenVAxis;
			screenVAxisForPan.y = 0.0f;

			camPanDirection -= screenVAxisForPan.normalized * Time.deltaTime * panSpeed; 
			bnCamModified = true;
		}
		else if(Input.GetAxis("Vertical") > 0 || Input.mousePosition.y > (Screen.height-screenPanEdge)) {

			Vector3 screenVAxisForPan = screenVAxis;
			screenVAxisForPan.y = 0.0f;

			camPanDirection += screenVAxisForPan.normalized * Time.deltaTime * panSpeed; 
			bnCamModified = true;
		}
	}

	/// <summary>
	/// Checks for the scroll wheel to do the zoom in and out of the camera
	/// </summary>
	void CheckCameraZoom() {

		// CAMERA ZOOM
		// Reads the mouse wheel for the camera zoom
		if(Input.GetAxis("Mouse ScrollWheel") < 0) {

			cameraDistance += zoomSpeed * Time.deltaTime;
			if(cameraDistance > zoomMax) {

				cameraDistance = zoomMax;
			}
			bnCamModified = true;
		}
		if(Input.GetAxis("Mouse ScrollWheel") > 0) {

			cameraDistance -= zoomSpeed * Time.deltaTime;
			if(cameraDistance < zoomMin) {

				cameraDistance = zoomMin;
			}
			bnCamModified = true;
		}
	}

	/// <summary>
	/// Check for camera rotation commands. One way to do this is holding the keyboard "alt" key and moving
	/// the mouse. When doing this, the panning of the camera is disabled, but the zoom function is still enabled
	/// Another way is using the Q and E keys
	/// </summary>
	void CheckCameraRotation() {

		// CAMERA ROTATION
		if(Input.GetKey("left alt")) {

			bnCamRotateWithMouse = true;
		}
		if(Input.GetKeyUp("left alt")) {

			bnCamRotateWithMouse = false;
		}

		if(bnCamRotateWithMouse) {

			mouseNow = Input.mousePosition;
			mouseDelta = mouseNow - mouseBefore;

			// Uses only the X movement
			y += mouseDelta.normalized.x * rotateSpeed;
			x += mouseDelta.normalized.y * rotateSpeed;
			x = Mathf.Clamp(x, camRotationMinAngleX, camRotationMaxAngleX);

			bnCamModified = true;
		}
		else if(Input.GetAxis("CamRotation") != 0) {

			y += Input.GetAxis("CamRotation") * rotateSpeed;
			bnCamModified = true;
		}
	}

	/// <summary>
	/// Returns the Vertical axis: the axis from the center of the screen to the top of it
	/// </summary>
	public Vector3 GetCameraVerticalAxis() {

		return screenVAxis;
	}

	/// <summary>
	/// Cast a ray through the camera to check where is the center of the screen in world coordinates
	/// </summary>
	/// <returns> A Vector3 with the world position pointed by the center of the screen </returns>
	public Vector3 GetCameraCenterOnWorld() {

		RaycastHit hit;

		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Camera.main.pixelWidth*0.5f,Camera.main.pixelHeight*0.5f,0));
		
		if(Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, 
					1 << MainScript.minimapGroundLayer)) {

			return hit.point;
		}

		return Vector3.zero;
	}

	/// <summary>
	/// Reposition the 'microphone' to the center of the screen, so we can use 3D sound in the action
	/// </summary>
	void RepositionMicListener() {

		Vector3 position = GetCameraCenterOnWorld();

		micListener.transform.position = position;
	}

	/*
	 * ===========================================================================================================
	 * HELPERS
	 * ===========================================================================================================
	 */

	/// <summary>
	/// Draws helpers on the screen
	/// </summary>
	void OnDrawGizmos() {

		/*
		Gizmos.color = Color.green;

		Gizmos.DrawRay(screenWorldCenter, screenHAxis * 100);
		Gizmos.DrawRay(screenWorldCenter, screenVAxis * 100);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(micListener.transform.position, 2.0f);
		//*/
	}

}
