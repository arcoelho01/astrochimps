using UnityEngine;
using System.Collections;
using Pathfinding;

/// <summary>
/// Script to filter all the mouse actions, including moving and clicking the buttons.
/// Right now, we use left click to 'select' units and buildings, and right click to issue 'action' commands
/// </summary>
public class MouseWorldPosition : MonoBehaviour {

	// PUBLIC
	// For the selected object
	public Transform selectedObject = null;	// Selected object itself (any)
	public Transform cursorObject = null;
	public enum eMouseStates { Hover, Walking, SelectingPosition }; 

	// Mouse cursor
	public Texture2D cursorNormal;	// regular cursor
	public Texture2D cursorAttack;	// used to indicate that we can attack this unit
	public Texture2D cursorGetInside;	// used to indicate that we can enter this building or vehicle
	public Texture2D cursorGetOut;	// used to indicate that we can exit this building or vehicle
	public Texture2D cursorWalk;	// normal cursor to select where we want the unit to move
	public Texture2D cursorWalkNotOk;	// Walk cursor, but indicating we can't walk here
	public Texture2D cursorBuild;	// cursor to show that we can build something
	Texture2D cursorCurrent;	// pointer to the current cursor texture

	// PRIVATE
	bool bnPositionAlreadySelected = false;	// Is the object already selected?
	bool bnNodeStatus = false;	// keep if current node at the mouse cursor is walkable or not
	Vector3 mouseNow;	// Mouse position now
	Vector3 mouseBefore;	// Previous mouse position
	GUIBottomMenu infoPanel = null;	// Pointer to the bottom menu bar
	Transform projectorSelectTargetPosition = null;	// Pointer to the cursor, when some object is selected
	CBaseEntity selectedBaseEntity = null;	// CBaseEntity stuff for the selected object, if exists
	Vector3 targetPosition;	// Target to the pathfinding
	eMouseStates MouseState;	// Current mouse state
	MainScript mainScript;
	
	// If we show the regular mouse cursor or not. We might want not to show it when using projectors
	bool bnShowMouseCursor = true;	

	// Defines a 'bar' in the screen. Mouse clicks outside this bar are ignored
	public float gameBarTop	=	80;	// From the top o the screen		
	public float gameBarBottom = 60 ;	// From the bottom of the screen. Usually the height of the bottom menu bar

	/*
	 * ===========================================================================================================
	 * UNITY STUFF
	 * ===========================================================================================================
	 */

	//
	void Awake() {

		// Gets the script of the GUI stuff
		infoPanel = gameObject.GetComponent<GUIBottomMenu>();

		if(infoPanel == null) {

			// DEBUG
			Debug.LogError("GUIBottomRuler not found in this object.");
		}

		if(cursorObject == null) {

			// DEBUG
			Debug.LogError("CursorObject not defined.");
		}
	}

	// Use this for initialization
	void Start () {
	
		// Cache the main script
		mainScript = GameObject.Find("/Codigo").GetComponent<MainScript>();

		if(!mainScript) {

			// DEBUG
			Debug.LogError("MainScript object not found. Check the code.");
		}

		mouseBefore = mouseNow = Input.mousePosition;
		MouseState = eMouseStates.Hover;

		// Adjust the gameBarBottom to pixels
		gameBarBottom = Screen.height - gameBarBottom;

		// Hides the OS cursor
		Screen.showCursor = false;
		cursorCurrent = cursorNormal;
	}
	
	// Update for physics stuff
	void Update () {
		
		// Check what the mouse is pointing
		MouseOver();

		// MOUSE LEFT CLICK
		if(Input.GetMouseButton(0)) {

			CheckLeftMouseClick();
		}// END OF LEFT CLICK

		// RIGHT CLICK
		// Check the right mouse button
		// The right mouse button selects the target position to move the currently selected unit
		if(Input.GetMouseButton(1)) {

			CheckRightMouseClick();
		} // END RIGHT CLICK
	}

	/*
	 * ===========================================================================================================
	 * ===========================================================================================================
	 */

	/// <summary> 
	/// When the player presses the mouse button, check whatever is under the mouse cursor through a raycast
	/// </summary>
	/// <returns>The Transform of the object clicked, if any. Otherwise, returns null</returns>
	Transform GetWhatIClicked() {

		RaycastHit hit;

		// Converts the mouse position to world position
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		// Creates a raycast from the mouse position in the screen. Ignores the minimap
		if(Physics.Raycast(ray.origin, ray.direction, out hit, 
					Mathf.Infinity, ~(1 << MainScript.minimapLayer))) {

			targetPosition = hit.point; // Keeps the position where the player clicked
			return hit.transform;	// Return the object
		}

		return null;
	}


	/// <summary>
	/// For now, the OnGUI only draws the custom mouse cursor
	/// </summary>
	void OnGUI() {

		int cursorHeight = 32;
		int cursorWidth = 32;

		if(bnShowMouseCursor) {

			if(cursorCurrent == cursorNormal) {

				// Is a pointer, so we draw it at the start of the texture (left-top)
				GUI.DrawTexture(new Rect(mouseNow.x, Screen.height - mouseNow.y, 32, 32), cursorCurrent);
			}
			else {

				// It's a target type of cursor, so the pointer is in the center of the texture
				GUI.DrawTexture(new Rect(mouseNow.x - cursorWidth/2, Screen.height - mouseNow.y - cursorHeight/2, 
							32, 32), cursorCurrent);
			}
		}
	}

	/// <summary>
	/// Checks the objects under the mouse cursor, showing its info in the info panel
	/// </summary>
	void MouseOver() {
		
		string infoPanelMsg = "";

		// Something is selected?
		if(selectedObject != null) {

			infoPanelMsg = selectedObject.gameObject.name + " ";
		}

		mouseNow = Input.mousePosition;

		// Moved the cursor?
		if(mouseNow != mouseBefore) {

			RaycastHit hit;
			Transform whatIAmPointing = null;

			mouseBefore = mouseNow;

			// Converts the mouse position to world position
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
			// Creates a raycast from the mouse position in the screen, ignoring the minimap (or not?)
			if(Physics.Raycast(ray.origin, ray.direction, out hit,
					Mathf.Infinity, ~(1 << MainScript.minimapLayer))) {

				whatIAmPointing = hit.transform;
			}

			if(whatIAmPointing == null)
				return;

			// Check the visibility
			if(whatIAmPointing.gameObject.GetComponent<VisibilityControl>() != null) {

				if(!whatIAmPointing.gameObject.GetComponent<VisibilityControl>().IsVisible()) {
					return;
				}
			}

			// 1 - check if we have something select or not
			if(!selectedObject) {

				if(whatIAmPointing.gameObject.layer != MainScript.groundLayer)
					infoPanelMsg = whatIAmPointing.name;

				// The cursor is the regular one
				cursorCurrent = cursorNormal;
				// And we show it
				bnShowMouseCursor = true;

				// Special case: hover over an resource site without an extractor
				if(whatIAmPointing.tag == "Resource") {

					CResource resourceExtractor = whatIAmPointing.gameObject.GetComponent<CResource>();

					if(resourceExtractor.CanWeBuildInThisResourceSite()) {

						infoPanelMsg += " You can build a resource extractor here";
						cursorCurrent = cursorBuild;
					}
				}

				infoPanel.SetInfoLabel(infoPanelMsg);
				// And we're off!
				return;
			}

			// 2 - Ok, we have an unit selected.
			CBaseEntity selectedObjectEntity = selectedObject.gameObject.GetComponent<CBaseEntity>();
			
			// 2.1 - if it's movable (a monkey or a drone, for instance), them we use mostly the 'walk' cursor
			if(selectedObjectEntity.Movable) {

				// we're pointing at the ground? Walk!
				if(whatIAmPointing.gameObject.layer == MainScript.groundLayer) {
					
					cursorCurrent = cursorWalk;

					// Check if the node under the cursor is walkable or not
					if(AstarPath.active != null) {

						Node node = AstarPath.active.GetNearest(hit.point);
						bnNodeStatus = node.walkable;
						if(!bnNodeStatus) {

							cursorCurrent = cursorWalkNotOk;
						}
					}

					// Ok, cursor set, get out
					return;
				}

				// are we pointing at an enemy?
				if(whatIAmPointing.gameObject.layer == MainScript.enemyLayer) {

					// TODO AND FIXME: it's not that easy. Must check first if the monkey or drone can actually attack
					// what we are pointing
					cursorCurrent = cursorAttack;
					return;
				}

				// Not pointing at the ground and not pointing at an enemy.
				// Is a monkey selected?
				if(selectedObject.gameObject.tag == "Monkey") {

					// are we pointing at one of ours buildings?
					if(whatIAmPointing.tag == "Building" && whatIAmPointing.gameObject.layer == MainScript.alliedLayer)	{

						// Is this building clear?
						if(!whatIAmPointing.gameObject.GetComponent<CBuilding>().TheresAMonkeyInside()) {

							// TODO: actually, only the engineer monkey can get inside a building
							cursorCurrent = cursorGetInside;
						}
						else { // Already have a monkey
							
							cursorCurrent = cursorNormal;
						}
					}
				}
				else { // not a monkey

				}
			}
			else {

				// Object is not movable: building, resource, etc.
				cursorCurrent = cursorNormal;
			}
		}
	}

	/// <summary>
	/// Return the transform of the object currently selected, if any
	/// </summary>
	/// <returns>The Transform of the currently selected object</returns>
	public Transform GetSelectedObject() {

		return selectedObject;
	}

	/// <summary>
	/// Executed when a script wants to receive a position when a mouse button is clicked
	/// </summary>
	public void IWantToSelectAPosition() {

		MouseState = eMouseStates.SelectingPosition;
		bnPositionAlreadySelected = false;

		// Instantiate a cursor
		projectorSelectTargetPosition = Instantiate(cursorObject, 
				transform.position, Quaternion.Euler(90.0f, 0.0f, 0.0f)) as Transform;

	}

	/// <summary>
	/// Used after IWantToSelectAPosition by another script, so it will know when something is clicked and a 
	/// position is selected
	/// </summary>
	/// <returns> True if we already select a position, false otherwise </returns>
	public bool PositionIsAlreadySelected() {
		
		return bnPositionAlreadySelected;
	}

	/// <summary>
	/// Used in conjuction with IWantToSelectAPosition and PositionIsAlreadySelected. If the position is already
	/// select, them polls this script to know what is this position
	/// </summary>
	/// <returns> A Vector3 with the position selected </returns>
	public Vector3 WhatIsThePositionSelected() {

		// DEBUG
		return targetPosition; 
	}

	/// <summary>
	/// Execute when the player presses the left mouse button
	/// Behaviours: select an unit, deselect-it if clicked in the terrain
	/// </summary>
	void CheckLeftMouseClick() {

		// Check if we're inside the game defined viewport
		if(mouseNow.y < gameBarTop || mouseNow.y > gameBarBottom)
			return;

		// Checks if we clicked in an unit
		Transform whatIClicked = GetWhatIClicked();

		// Possibilities:
		// 1-I don't have anything selected. In this case, select the object under the mouse cursor, if possible
		// 2-I already have select an object. In this case, the click will point where it should move
		if(whatIClicked != null) {

			// Get the basic info on the unit
			selectedBaseEntity = whatIClicked.gameObject.GetComponent<CBaseEntity>();

			if(selectedBaseEntity != null) {

				if(selectedBaseEntity.Selectable) {

					// Unit not selected?
					if(!selectedBaseEntity.isSelected) {

						// Unselect previous units
						if(selectedObject != null) {

							selectedObject.gameObject.GetComponent<CBaseEntity>().Deselect();
							RemoveCursor();
						}

						// Select this unit
						selectedObject = selectedBaseEntity.Select();

						infoPanel.SetInfoLabel(selectedObject.gameObject.name);

						if(selectedBaseEntity.Movable) {

							// Change the mouse state
							MouseState = eMouseStates.Walking;
						}
						else {

							// FIXME: to avoid a previously selected drone to walk, even not being selected
							MouseState = eMouseStates.Hover;
						}
					}
				}
			}
			else {

				// Unselect current object
				if(selectedObject != null) {

					// Deselect the current selected object
					selectedObject.gameObject.GetComponent<CBaseEntity>().Deselect();
					selectedObject = null;
					// Change the mouse state
					MouseState = eMouseStates.Hover;
					RemoveCursor();

					// Reset the menu panel
					mainScript.bottomMenu.PlayerInfoMenuEnable(null);
				}
			}
		}
	}

	/// <summary>
	/// Execute when the player presses the right mouse button
	/// Behaviours: when a le unit is selected, select where it should walk to
	/// </summary>
	void CheckRightMouseClick() {

		// Check if we're inside the game defined viewport
		if(mouseNow.y < gameBarTop || mouseNow.y > gameBarBottom)
			return;

		// Checks if we clicked in an unit
		Transform whatIClicked = GetWhatIClicked();

		// FIXME: add monkey select, I clicked in a building
		if(selectedObject != null) {

			if(selectedObject.gameObject.GetComponent<CBaseEntity>().Type == CBaseEntity.eObjType.Monkey) {

				// Get the basic info on the unit
				selectedBaseEntity = whatIClicked.gameObject.GetComponent<CBaseEntity>();

				if(selectedBaseEntity != null) {

					if(selectedBaseEntity.Type == CBaseEntity.eObjType.Building) {

						// Unit not selected?
						if(!selectedBaseEntity.isSelected) {

							// After all that, check if the clicked object is a building, so the monkey can get inside
							if(selectedBaseEntity.Type == CBaseEntity.eObjType.Building) {
								
								if (whatIClicked.gameObject.layer != MainScript.enemyLayer){

									CBuilding selectedBuilding = whatIClicked.gameObject.GetComponent<CBuilding>();
									// Puts the monkey inside the building. Actually, the building get the monkey
									selectedBuilding.PutAMonkeyInside(selectedObject);
									// Deselect the monkey
									selectedObject.gameObject.GetComponent<CBaseEntity>().Deselect();
									selectedObject = null;
								}
							}
						}
					}
					// ITS A DRONE
					if(selectedBaseEntity.Type == CBaseEntity.eObjType.Drone) {
						// ITS AN ENEMY
						if (whatIClicked.gameObject.layer == MainScript.enemyLayer){
							CMonkey cMonkeyScript = selectedObject.gameObject.GetComponent<CMonkey>();
							// MONKEY ATTACKs DRONE
							if (cMonkeyScript != null){
								cMonkeyScript.Attack(whatIClicked);
							}
						}
					}
					
				}

			}

			// Only walk if the place we clicked is allowed to walk
			if(bnNodeStatus) {

				// IF CLICK WASNT ON UNITS OR BUILDINGS
				if(whatIClicked.gameObject.GetComponent<CBaseEntity>() == null) {
					// WALK THE MONKEY
					if(selectedObject.gameObject.GetComponent<CBaseEntity>().Type == CBaseEntity.eObjType.Monkey) {
						selectedObject.gameObject.GetComponent<CMonkey>().WalkTo(WhatIsThePositionSelected());

						// Show where we clicked in the ground
						StartCoroutine(ShowSelectedPositionWithAProjector(WhatIsThePositionSelected(),2.0f));
					} 
					// WALK THE DRONE
					else if(selectedObject.gameObject.GetComponent<CBaseEntity>().Type == CBaseEntity.eObjType.Drone) {
						selectedObject.gameObject.GetComponent<CDrone>().WalkTo(WhatIsThePositionSelected());

						// Show where we clicked in the ground
						StartCoroutine(ShowSelectedPositionWithAProjector(WhatIsThePositionSelected(),2.0f));
					}
				}
			}
		}

		/*/
		// FIXME: guess we don't need this anymore. Maybe checking if selectedObject != null is enough
		if(MouseState == eMouseStates.Walking) {

			if(!bnNodeStatus) {

				return;
			}

			GetWhatIClicked();
		}
		else if(MouseState == eMouseStates.SelectingPosition) {

			if(!bnNodeStatus) {

				return;
			}

			// This will store the clicked position
			GetWhatIClicked();
			MouseState = eMouseStates.Hover;
			bnPositionAlreadySelected = true; // This will let other scripts know

			RemoveCursor();
		}
		//*/
	}

	/// <summary>
	/// Removes the cursor projector, destroying its object
	/// Used when we have deselected a unit
	/// </summary>
	public void RemoveCursor() {

		// Destroys the cursor's instance
		if(projectorSelectTargetPosition != null) {

			Destroy(projectorSelectTargetPosition.gameObject);
		}
	}

	/// <summary>
	/// When the player select a position to walk to, we show a cursor in the ground for visual aid
	/// </summary>
	/// <param name="position"> Position where to show the cursor (usually where we clicked)</param>
	/// <param name="timeToShow"> A float with the amount of time to show the projector in the ground</param>
	IEnumerator ShowSelectedPositionWithAProjector(Vector3 position, float timeToShow) {

		// Instantiate a cursor
		if(projectorSelectTargetPosition == null) {
			projectorSelectTargetPosition = Instantiate(cursorObject, position, 
					Quaternion.Euler(90.0f, 0.0f, 0.0f)) as Transform;
		}

		yield return new WaitForSeconds(timeToShow);

		if(projectorSelectTargetPosition != null)
			Destroy(projectorSelectTargetPosition.gameObject);
	}
}
