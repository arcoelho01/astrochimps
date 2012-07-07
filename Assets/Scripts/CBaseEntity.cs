using UnityEngine;
using System.Collections;

/// <summary>
/// Basic class for monkeys and drones
/// </summary>
public class CBaseEntity : MonoBehaviour {

	// PRIVATE
	private string mName = string.Empty;	// Object name in the game
	private bool mIsSelected = false;			// The object is selected?

	// PUBLIC
	// Possible teams for an object
	public enum eObjTeam { Allied, Opponent, Neutral };		
	// Types of objects in the game
	public enum eObjType { Monkey, Drone, Resource, Building, Player, RocketPart };

	// This object can be selected?
	public bool Selectable;	

	protected float selectorRadius = 0.0f;

	// Can this unit be moved? (monkeys, drones, vehicles)
	public bool Movable;
	public eObjTeam Team;
	public eObjType Type;
	public string Name;
	public string objectName {get{return mName;} set{mName = value;}}
	public bool isSelected {get{return mIsSelected;} set{mIsSelected = value;}}

	public float costTime; // Seconds needed to build this structure
	public float costMetal;	// Amount of metal resources needed to build this structure

	// FIXME: put in the appropriate class!
	public Transform selectorObject = null;
	Transform selector;
	public MainScript mainScript;	// Pointer to the main game script

	public Material materialDisabled;

	// This object can be captured?
	public bool canBeCaptured;
	public bool isCaptured;

	Transform capturedFormerParent;
	Vector3 capturedFormerPosition;
	protected Transform capturer;
	protected CBaseEntity capturedEntity;

	// Only a shortcut. When the object has multiple renderes in the hierarchy, we only verify on to know if 
	// whether is being rendered or not
	public Transform mainRendererObject = null;	
	
	protected Transform meshObject = null;
	protected Transform sweetSpotObj;
	protected Vector3 sweetSpot;
	// The object to whom captured objects will be parented, if applicable
	protected Transform captureSpot;
	// Where the capture ray should start
	protected Transform captureRaySpot;

	// Visual aid to show that object was captured
	Transform captureForceField;
	Transform captureRay;

	//< Prefab to the progress bar
	public Transform progressBarPrefab;
	/*
	 * ===========================================================================================================
	 * UNITY'S STUFF
	 * ===========================================================================================================
	 */

	//
	void Start() {

		// Cache the main script
		mainScript = GameObject.Find("/Codigo").GetComponent<MainScript>();

		if(!mainScript) {

			// DEBUG
			Debug.LogError("MainScript object not found. Check the code.");
		}
	}

	/*
	 * ===========================================================================================================
	 * CLASS' STUFF
	 * ===========================================================================================================
	 */

	/// <summary>
	/// Runs whenever this object is selected in the game. Different objects have different behaviours. Monkeys
	/// and drones, when selected, have a cursor added to themselves. 
	/// </summary>
	/// <returns>The Transform of the selected object</returns>
	public virtual Transform Select() {

		Transform rv = null;

		if(Selectable) {
	
			isSelected = true;

			CreateSelector();
			rv = this.transform;
		}

		return rv;
	}

	/// <summary>
	/// Deselect the current object, if possible
	/// </summary>
	public virtual void Deselect() {

		if(Selectable && isSelected) {

			isSelected = false;

			if(selector) {

				// Destroy this instance of the projector
				Destroy(selector.gameObject);
			}

			MainScript.mouseInputScript.DeselectedObject();
		}
	}

	/// <summary>
	/// Declaration of the build function
	/// </summary>
	public virtual void BuildIt() {

		// DEBUG
		Debug.Log("CBaseEntity BuildIt");
	}

	/// <summary>
	/// Do whatever this building is supposed to do
	/// </summary>
	public virtual void Work() {

		// DEBUG
		Debug.Log("CBaseEntity Work");
	}

	/// <summary>
	/// What to do when this entity is attacked
	/// </summary>
	public virtual void Attacked() {

		// DEBUG
		Debug.Log("CBaseEntity Attacked");
	}

	/// <summary>
	/// Instantiate the selector object, showing the user what object he selected
	/// </summary>
	void CreateSelector() {

		// New code with instantiate
		if(selectorRadius == 0.0f) {
		
			selectorRadius = Mathf.Max(transform.localScale.x, transform.localScale.z);
		}

		// 1 - Instantiate the selector
		selector = Instantiate(selectorObject,transform.position, Quaternion.identity) as Transform;
		// 2 - Adjust the size of the selector
		Projector projector = selector.GetComponent<Projector>();
		if (projector != null ){
			
			selector.rotation = Quaternion.Euler(90,0,0);
			projector.orthographicSize = selectorRadius * 2;	
			// 3 - Put the projector as a child to this object, so it moves together
			selector.transform.parent = this.transform;
		}
		
		else{ // IN CASE ITS NOT A PROJECTOR, Example: AN ARROW 
			
			//selector = Instantiate(selectorObject,sweetSpot, Quaternion.Euler(90,0,0)) as Transform;
			selector.transform.parent = this.transform;
			selector.transform.position = sweetSpot;
			selector.rotation = Quaternion.identity;
		}
	}

	/// <summary>
	/// Somebody captured this entity! 
	/// </summary>
	/// <param name="capturer"> Transform for who is capturing this object </param>
	/// <param name="captureSpot"> Transform of the capture spot (inside the hierarchy of the capturer) </param>
	/// <param name="captureRaySpot"> Transform of the spot from where the capture ray is shoot </param>
	public virtual void CapturedBy(Transform capturer, Transform captureSpot, Transform captureRaySpot) {

		if(isCaptured)
			return;

		isCaptured = true;

		// Keep track of the capturer
		this.capturer = capturer;
		capturedFormerParent = this.transform.parent;
		capturedFormerPosition = transform.position;

		// Move it as child of the capturer. This way they will move together
		this.transform.position = captureSpot.transform.position;
		this.transform.parent = captureSpot;
		this.transform.rotation = capturer.transform.rotation;

		// Check if the object have a rigidbody attached
		if(this.rigidbody) {

			if(this.rigidbody.useGravity) {

				// Turn off the gravity for this object
				this.rigidbody.useGravity = false;
			}
			if(!this.rigidbody.isKinematic) {

				this.rigidbody.isKinematic = true;
			}
		}

		if(MainScript.Script.prefabForceField && !captureForceField) {

			// TODO: add some visual aid to help the player to see that stuff floating around is actually something
			// that was captured
			captureForceField = Instantiate(MainScript.Script.prefabForceField, this.transform.position, 
					Quaternion.identity) as Transform;
			captureForceField.transform.parent = this.transform;
		}

		// Create an instance of the capture ray
		if(MainScript.Script.prefabCaptureRay && !captureRay) {

			Transform tRayOriginObject;

			if(captureRaySpot)
				tRayOriginObject = captureRaySpot;
			else
				tRayOriginObject = capturer;

			captureRay = Instantiate(MainScript.Script.prefabCaptureRay, this.transform.position, 
					Quaternion.identity) as Transform;
			captureRay.transform.parent = this.transform;

			Vector3 direction = tRayOriginObject.transform.position - this.transform.position;
			Vector3 newSize = new Vector3(captureRay.transform.localScale.x,captureRay.transform.localScale.y, 
					direction.magnitude);
			captureRay.transform.localScale = newSize;

			captureRay.transform.LookAt(tRayOriginObject);

		}

	}

	/// <summary>
	/// Release the captured object
	/// </summary>
	public virtual void ReleaseMe() {

		this.transform.parent = capturedFormerParent;

		// The capturer has lost us
		this.capturer.gameObject.GetComponent<CBaseEntity>().capturedEntity = null;
		this.capturer = null;

		// FIXME: find a better way to put the captured object back to the ground
		//Vector3 putMeBackInTheGround = new Vector3(transform.position.x, capturedFormerPosition.y, 
		//		transform.position.z);
		
		//transform.position = putMeBackInTheGround;
		
		// Check if the object have a rigidbody attached
		if(this.rigidbody) {

			if(!this.rigidbody.useGravity) {

				// Turn off the gravity for this object
				this.rigidbody.useGravity = true;
			}
			if(this.rigidbody.isKinematic) {

				this.rigidbody.isKinematic = false;
			}
		}

		isCaptured = false;

		if(captureForceField)
			Destroy(captureForceField.gameObject);

		if(captureRay)
			Destroy(captureRay.gameObject);
	}

	/// <summary>
	/// Check if this unit is carrying something or someone
	/// </summary>
	public bool CheckIfHaveSomethingCaptured() {

		if(capturedEntity == null)
			return false;
		else
			return true;
	}

	/// <summary>
	/// Get the sweet spot position, if this object is defined. Otherwise, will return this object position
	/// </summary>
	protected Vector3 GetSweetSpotPosition() {

		sweetSpotObj = transform.Find("SweetSpot");

		if(sweetSpotObj) {
			return sweetSpotObj.transform.position;
		}
		else {
			return this.transform.position;
		}
	}

	/// <summary>
	/// Get the mesh object in the hierarchy, as this:
	/// Object -> collider
	/// | - Icon
	/// | - SweetSpot
	/// | - Mesh 
	///	  | - Imported FBX -> animation
	/// </summary>
	protected Transform GetMeshObject() {

		// First, find the "Mesh" in the hierarchy
		Transform meshHierarchy = transform.Find("Mesh");

		// Now, get the children of the Mesh, This will be the real model
		if(meshHierarchy) {

			foreach(Transform child in meshHierarchy) {
				
				return child;
			}
		}

		return null;
	}

	/// <summary>
	/// Get the mesh object, the renderer and the sweet spot for this object
	/// </summary>
	protected void GetSweetSpotAndMeshObject() {

		// Get the mesh
		meshObject = GetMeshObject();
		if(!meshObject) {

			// DEBUG
			//Debug.LogError("Cannot find Mesh for " + this.transform);
		}
		else {

			// Get one object to be check the renderer
			foreach(Transform child in meshObject) {

				if(child.GetComponent<Renderer>()) {
	
					mainRendererObject = child;
					break;
				}
			}
		}
		
		sweetSpot = GetSweetSpotPosition();
	}

	/// <summary>
	/// Get the Capture Spot object from the hierarchy. Useful only for the cientist monkey and the hunter drones
	/// </summary>
	/// <returns> The transform of the Capture Spot, or null if not found </returns>
	protected Transform GetCaptureSpot() {

		Transform captureSpotObj = transform.Find("CaptureSpot");

		return captureSpotObj;
	}

	/// <summary>
	/// Get the Capture Ray Spot object from the hierarchy, which is the spot from where the capture ray is shoot.
	/// Useful only for the cientist monkey and the hunter drones
	/// </summary>
	/// <returns> The transform of the Capture Ray Spot, or null if not found </returns>
	protected Transform GetCaptureRaySpot() {

		Transform captureRaySpotObj = transform.Find("CaptureRaySpot");

		return captureRaySpotObj;
	}
}

