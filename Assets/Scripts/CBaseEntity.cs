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
	// Can this unit be moved? (monkeys, drones, vehicles)
	public bool Movable;
	public eObjTeam Team;
	public eObjType Type;
	public string Name;
	public string objectName {get{return mName;} set{mName = value;}}
	public bool isSelected {get{return mIsSelected;} set{mIsSelected = value;}}

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

	// Only a shortcut. When the object has multiple renderes in the hierarchy, we only verify on to know if 
	// whether is being rendered or not
	public Transform mainRendererObject = null;	
	
	protected Transform meshObject = null;
	protected Vector3 sweetSpot;

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

		if(Selectable) {

			isSelected = false;

			if(selector) {

				// Destroy this instance of the projector
				Destroy(selector.gameObject);
			}
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
	/// Instantiate the selector object, showing the user what object he selected
	/// </summary>
	void CreateSelector() {

		// New code with instantiate
		float objectRadius = Mathf.Max(transform.localScale.x, transform.localScale.z);

		// 1 - Instantiate the selector
		selector = Instantiate(selectorObject,transform.position, Quaternion.identity) as Transform;
		// 2 - Adjust the size of the selector
		Projector projector = selector.GetComponent<Projector>();
		if (projector != null ){
			
			selector.rotation = Quaternion.Euler(90,0,0);
			projector.orthographicSize = objectRadius;	
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
	public virtual void CapturedBy(Transform capturer) {

		isCaptured = true;

		capturedFormerParent = this.transform.parent;
		capturedFormerPosition = transform.position;

		// Move it as child of the capturer. This way, it will move together
		this.transform.parent = capturer.transform;

		Vector3 newPosition = new Vector3(0, 3, -3);
		transform.position = capturer.transform.position + newPosition;
	}

	/// <summary>
	/// Release the captured object
	/// </summary>
	public virtual void ReleaseMe() {

		this.transform.parent = capturedFormerParent;

		// FIXME: find a better way to put the captured object back to the ground
		Vector3 putMeBackInTheGround = new Vector3(transform.position.x, capturedFormerPosition.y, 
				transform.position.z);
		
		transform.position = putMeBackInTheGround;

		isCaptured = false;
	}

	/// <summary>
	/// Get the sweet spot position, if this object is defined. Otherwise, will return this object position
	/// </summar>
	protected Vector3 GetSweetSpotPosition() {

		Transform sweetSpotObj = transform.Find("SweetSpot");

		if(sweetSpotObj) {
			Debug.Log("SIM! Achei o sweetspot");
			return sweetSpotObj.transform.position;
		}
		else {
			Debug.Log("Nao achei o sweetspot");
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
}

