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
		selector = Instantiate(selectorObject,transform.position, Quaternion.Euler(90,0,0)) as Transform;
		// 2 - Adjust the size of the selector
		Projector projector = selector.GetComponent<Projector>();
		projector.orthographicSize = objectRadius;	
		// 3 - Put the projector as a child to this object, so it moves together
		selector.transform.parent = this.transform;
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
		isCaptured = false;
	}
}
