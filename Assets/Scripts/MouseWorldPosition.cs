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
	Transform pointedObject = null;
	public Transform cursorObject = null;
	public enum eMouseStates { 
			Hover,  //< Mouse over, nothing selected
			CanWalk,	// Movable object selected, can walk where the mouse is pointing
			CannotWalk,	// Movable object selected, cannot walk where the mouse is pointing
			SelectingPosition,	
			MonkeyCanEnterBuilding,	// Monkey selected, mouse over Command Center, no monkey already in it
			Targeting,	// Monkey select, cursor over an enemy
			CanCapture, // Cientist monkey selected, mouse over a loose rocket part
			CanReleaseCaptured, // Cientist monkey selected, mouse over a captured part
			EngineerFix, // Engineer monkey selected, mouse over a sabotaged building
			CanSabotageBuilding, 
			CanSabotageDrone,
			TargetingForRecycle,	// Engineer vs disabled drone
			TargetingForBrawl,	// Astronaut vs functional drone
			TargetingForReprogram	// Saboteur vs disabled drone
	};

	// Mouse cursor
	public Texture2D cursorNormal;	// regular cursor
	public Texture2D cursorAttack;	// used to indicate that we can attack this unit
	public Texture2D cursorGetInside;	// used to indicate that we can enter this building or vehicle
	public Texture2D cursorGetOut;	// used to indicate that we can exit this building or vehicle
	public Texture2D cursorWalk;	// normal cursor to select where we want the unit to move
	public Texture2D cursorWalkNotOk;	// Walk cursor, but indicating we can't walk here
	public Texture2D cursorBuild;	// cursor to show that we can build something
	public Texture2D cursorCaptureRay;	// cursor showing that we can use the capture ray
	public Texture2D cursorReprogramEnable;	// Saboteur can reprogram this drone
	public Texture2D cursorSabotageEnable;		// shows that we can sabotage this unit/building
	public Texture2D cursorRecycleEnable;	// Enginner can recycle a stunned drone
	Texture2D cursorCurrent;	// pointer to the current cursor texture

	// PRIVATE
	bool bnPositionAlreadySelected = false;	// Is the object already selected?
	bool bnNodeStatus = false;	// keep if current node at the mouse cursor is walkable or not
	Vector3 mouseNow;	// Mouse position now
	public static Vector3 mouseHitPointNow;
	Vector3 mouseBefore;	// Previous mouse position
	GUIBottomMenu infoPanel = null;	// Pointer to the bottom menu bar
	Transform projectorSelectTargetPosition = null;	// Pointer to the cursor, when some object is selected
	CBaseEntity selectedBaseEntity = null;	// CBaseEntity stuff for the selected object, if exists
	public static Vector3 targetPosition;	// Target to the pathfinding
	eMouseStates MouseState;	// Current mouse state
	MainScript mainScript;
	
	// If we show the regular mouse cursor or not. We might want not to show it when using projectors
	bool bnShowMouseCursor = true;	

	// Defines a 'bar' in the screen. Mouse clicks outside this bar are ignored
	public float gameBarTop;	// From the top o the screen		
	public float gameBarBottom;	// From the bottom of the screen. Usually the height of the bottom menu bar

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

		// Adjust the gameBarTop to pixels
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
		if(Input.GetMouseButtonDown(1)) {

			CheckRightMouseClick();
		} // END RIGHT CLICK

		DoCheatingStuff();
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
					Mathf.Infinity, ~(1 << MainScript.minimapLayer | 1 << MainScript.minimapGroundLayer))) {

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
	/// Actually, this is the main function of this script: it analyzes the context and set the mouse state and
	/// cursor accordingly
	/// </summary>
	void MouseOver() {
		
		string infoPanelMsg = "";

		// Something is selected?
		if(selectedObject != null) {

			infoPanelMsg = selectedObject.gameObject.name + " ";
		}

		mouseNow = Input.mousePosition;

		// Moved the cursor?
		//if(mouseNow != mouseBefore) {

			RaycastHit hit;
			Transform whatIAmPointing = null;
			pointedObject = null;

			mouseBefore = mouseNow;
			

			// Converts the mouse position to world position
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
			// Creates a raycast from the mouse position in the screen, ignoring the minimap (or not?)
			if(Physics.Raycast(ray.origin, ray.direction, out hit,
					Mathf.Infinity, ~(1 << MainScript.minimapLayer | 1 << MainScript.minimapGroundLayer))) {

				whatIAmPointing = hit.transform;
				mouseHitPointNow = hit.point;
			}

			if(whatIAmPointing == null)
				return;
			else 
				pointedObject = whatIAmPointing;

			// Check the visibility
			if(whatIAmPointing.gameObject.GetComponent<VisibilityControl>() != null) {

				if(!whatIAmPointing.gameObject.GetComponent<VisibilityControl>().IsVisible()) {
					return;
				}
			}

			// 1 - check if we have something selected or not
			if(!selectedObject) {

				if(whatIAmPointing.gameObject.layer != MainScript.groundLayer)
					infoPanelMsg = whatIAmPointing.name;

				// The cursor is the regular one
				cursorCurrent = cursorNormal;
				MouseState = eMouseStates.Hover;
				// And we show it
				bnShowMouseCursor = true;

				// Special case: hover over a resource site without an extractor
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
			} // END IF - nothing selected


			// 2 - Ok, we have an unit selected.
			CBaseEntity selectedObjectEntity = selectedObject.gameObject.GetComponent<CBaseEntity>();
			
			// 2.1 - if it's movable (a monkey or a drone, for instance), them we use mostly the 'walk' cursor
			if(selectedObjectEntity.Movable) {

				// Are we pointing at the ground? Walk!
				if(whatIAmPointing.gameObject.layer == MainScript.groundLayer) {
					
					cursorCurrent = cursorWalk;
					MouseState = eMouseStates.CanWalk;

					// Check if the node under the cursor is walkable or not
					if(AstarPath.active != null) {

						Node node = AstarPath.active.GetNearest(hit.point);
						bnNodeStatus = node.walkable;
						if(!bnNodeStatus) {

							cursorCurrent = cursorWalkNotOk;
							MouseState = eMouseStates.CannotWalk;
						}
					}

					// Ok, cursor set, get out
					return;
				}

				// are we pointing at an enemy?
				if(whatIAmPointing.gameObject.layer == MainScript.enemyLayer) {

					// We have a monkey selected?
					if(selectedObject.gameObject.tag == "Monkey") {

						CMonkey selectedMonkey = selectedObject.gameObject.GetComponent<CMonkey>();

						if(!selectedMonkey) {

							// DEBUG
							Debug.LogError("Could not find CMonkey component in " + selectedObject);
						}

						// Are we targeting a drone?
						if(whatIAmPointing.gameObject.tag == "Drone") {

							bool isThisDroneStunned = false;
							// Is this drone stunned?
							CDrone pointedDrone = whatIAmPointing.gameObject.GetComponent<CDrone>();
							if(!pointedDrone) {

								// DEBUG
								Debug.LogError("Could not find CDrone component in " + whatIAmPointing);
							}

							isThisDroneStunned = pointedDrone.isStunned();

							// Default behaviour
							cursorCurrent = cursorWalkNotOk;
							MouseState = eMouseStates.CannotWalk;

							switch(selectedMonkey.monkeyClass) {
							 
								case CMonkey.eMonkeyType.Astronaut:
									{
										// The astronaut only attack working drones
										// From the GDD:
										// Astronaut vs visible saboteur drones: OK
										// Astronaut vs hunter drones: get caught
										// Astronaut vs patrol drones: not ok
										// TODO: put this rules here!
										if(!isThisDroneStunned && pointedDrone.droneType == CDrone.eDroneType.Saboteur) {

											cursorCurrent = cursorAttack;
											MouseState = eMouseStates.TargetingForBrawl;
										}
									}
									break;

								case CMonkey.eMonkeyType.Cientist:
									break;

								case CMonkey.eMonkeyType.Engineer:
									{
										// The engineer only attack (recycle) stunned drones
										if(isThisDroneStunned) {

											cursorCurrent = cursorRecycleEnable;
											MouseState = eMouseStates.TargetingForRecycle;
										}
										else {
											// Drone is not stunned anymore
											//cursorCurrent = cursorNormal;
											//MouseState = eMouseStates.Hover;
										}
									}
									break;

								case CMonkey.eMonkeyType.Saboteur:
									{
										// Saboteur:
										// Can sabotage patrol drones and hunter drones, if not visible
										// The sabouter only attack (reprogram) stunned drones
										if(isThisDroneStunned) {

											cursorCurrent = cursorReprogramEnable;
											MouseState = eMouseStates.TargetingForReprogram;
										}
										else {

											// Drone not stunned. Well, we still can sabotage it 
											cursorCurrent = cursorSabotageEnable;
											MouseState = eMouseStates.CanSabotageDrone;
										}
									}
									break;

								default:
									// DEBUG
									Debug.LogError("I should not be here...");
									break;
							}
						} // END IF - Monkey selected - Targeting an enemy drone
						else if(whatIAmPointing.gameObject.tag == "Building") {
							// No, we are targeting an enemy building.
							// Only saboteurs can do something here?
							if(selectedMonkey.monkeyClass == CMonkey.eMonkeyType.Saboteur) {

								// Get info about the building target
								CBuilding targetBuilding = whatIAmPointing.GetComponent<CBuilding>();

								if(!targetBuilding) {

									// DEBUG
									Debug.Log("Could not find CBuilding component in " + whatIAmPointing);
								}

								if(!targetBuilding.sabotado) {

									cursorCurrent = cursorSabotageEnable;
									MouseState = eMouseStates.CanSabotageBuilding;

								}
							}
						} // END IF - Monkey selected - Targeting an enemy building
					} // END IF - Monkey selected
					else if(selectedObject.gameObject.tag == "Drone") {

						// GDD:		* hunter vs saboteur: attack (identical to the astronaut's attack)
						//				* saboteur vs patrol or hunter: sabotage
						//				* saboteur vs saboteur: reprogram
						//				saboteur vs building: sabotage

						CDrone selectedDrone = selectedObject.gameObject.GetComponent<CDrone>();

						if(!selectedDrone) {

							// DEBUG
							Debug.LogError("Could not find CDrone component in " + selectedObject.transform);
						}

						// Are we targeting another drone (an enemy one, that is)
						if(whatIAmPointing.gameObject.tag == "Drone") {

							bool isThePointedDroneStunned = false;
							// Is this drone stunned?
							CDrone pointedDrone = whatIAmPointing.gameObject.GetComponent<CDrone>();
							if(!pointedDrone) {

								// DEBUG
								Debug.LogError("Could not find CDrone component in " + whatIAmPointing);
							}

							isThePointedDroneStunned = pointedDrone.isStunned();

							switch(pointedDrone.droneType) {

								case CDrone.eDroneType.Patrol:
									{
										// Saboteur x Patrol
										if(selectedDrone.droneType == CDrone.eDroneType.Saboteur && !isThePointedDroneStunned) {
										
											cursorCurrent = cursorSabotageEnable;
											MouseState = eMouseStates.CanSabotageDrone;
										}
									}
									break;

								case CDrone.eDroneType.Saboteur:
									{
										if(selectedDrone.droneType == CDrone.eDroneType.Saboteur && isThePointedDroneStunned) {
										
											// Saboteur x Saboteur (stunned)
											cursorCurrent = cursorReprogramEnable;
											MouseState = eMouseStates.TargetingForReprogram;
										}
										else if(selectedDrone.droneType == CDrone.eDroneType.Hunter && !isThePointedDroneStunned) {
										
											// Hunter vs Saboteur
											cursorCurrent = cursorAttack;
											MouseState = eMouseStates.TargetingForBrawl;
										}
									}
									break;

								case CDrone.eDroneType.Hunter:
									{
										// Saboteur x Hunter
										if(selectedDrone.droneType == CDrone.eDroneType.Saboteur && !isThePointedDroneStunned) {
										
											cursorCurrent = cursorSabotageEnable;
											MouseState = eMouseStates.CanSabotageDrone;
										}
									}
									break;
									
								default:
									break;
							}


						} // END IF - Drone selected - targeting a drone
						else if(whatIAmPointing.gameObject.tag == "Building") {

							// Saboteur drones x building
							if(selectedDrone.droneType == CDrone.eDroneType.Saboteur) {

								CBuilding pointedBuilding = whatIAmPointing.gameObject.GetComponent<CBuilding>();

								// FIXME: Slots don't have CBuilding, so the mouseOver crash here
								if(!pointedBuilding)
									return;

								// We cannot sabotage a already sabotaged building
								if(pointedBuilding.sabotado)
									return;

								cursorCurrent = cursorSabotageEnable;
								MouseState = eMouseStates.CanSabotageBuilding;
							}
						} // END IF - Drone selected - targeting a building
					} //	END IF - Drone selected


					return;
				}

				// Not pointing at the ground and not pointing at an enemy.
				// Is a monkey selected?
				if(selectedObject.gameObject.tag == "Monkey") {

					CMonkey selectedMonkey = selectedObject.gameObject.GetComponent<CMonkey>();

					if(!selectedMonkey) {

						// DEBUG
						Debug.LogError("Cannot find CMonkey component on " + selectedObject);
						return;
					}

					// are we pointing at one of ours buildings?
					if(whatIAmPointing.tag == "Building" && whatIAmPointing.gameObject.layer == MainScript.alliedLayer)	{

						CBuilding pointedBuilding = whatIAmPointing.gameObject.GetComponent<CBuilding>();

						// FIXME: Slots don't have CBuilding, so the mouseOver crash here
						if(!pointedBuilding)
							return;

						// Is this the Command Center?
						if(pointedBuilding.buildingType == CBuilding.eBuildingType.CommandCenter &&  
								!pointedBuilding.TheresAMonkeyInside()) {

							cursorCurrent = cursorGetInside;
							MouseState = eMouseStates.MonkeyCanEnterBuilding;
						}

						// Is this a sabotaged building?
						if(pointedBuilding.sabotado) {

							// and we have the engineer selected?
							if(selectedMonkey.monkeyClass == CMonkey.eMonkeyType.Engineer) {
								
								cursorCurrent = cursorBuild;
								MouseState = eMouseStates.EngineerFix;
							}
						}

						// FIXME: check the correct cursor. Not walk, perhaps?
						//	cursorCurrent = cursorNormal;
						//	MouseState = eMouseStates.Hover;
						//
						return;
					}
				
					// No, so are we pointing at a rocket part with the cientist monkey selected?
					if(whatIAmPointing.tag == "RocketPart" && 
							selectedMonkey.monkeyClass == CMonkey.eMonkeyType.Cientist) {

						CRocketPart cRocketPart = whatIAmPointing.GetComponent<CRocketPart>();

						// Ok, but it's this part already being carried by someone in my team?
						if(cRocketPart.isCaptured) {

							// Set the cursor
							cursorCurrent = cursorCaptureRay;
							// Set the state
							MouseState = eMouseStates.CanReleaseCaptured;
							// Keeps the pointed object
							//pointedObject = whatIAmPointing;
						}
						else if(cRocketPart.isRevealed){

							// Set the cursor
							cursorCurrent = cursorCaptureRay;
							// Set the state
							MouseState = eMouseStates.CanCapture;
							// Keeps the pointed object
							//pointedObject = whatIAmPointing;
						}
					}	
				} // END IF - Monkey selected - not pointing at the ground, not pointing at an enemy
				else { // not a monkey selected

				}
			} // END IF - Movable selected
			else {

				// Object is not movable: building, resource, etc.
				cursorCurrent = cursorNormal;
				MouseState = eMouseStates.Hover;
			}
		//}
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

				// Do not let we select enemy units
				if(selectedBaseEntity.Team == CBaseEntity.eObjTeam.Opponent) {

					return;
				}

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
							MouseState = eMouseStates.CanWalk;
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
	
	// SelectObject
	public void SelectObject (Transform targetObject)
	{
		// Get the basic info on the unit
		selectedBaseEntity = targetObject.gameObject.GetComponent<CBaseEntity>();

		if(selectedBaseEntity != null) {

				// Do not let we select enemy units
				if(selectedBaseEntity.Team == CBaseEntity.eObjTeam.Opponent) {

					return;
				}

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
							MouseState = eMouseStates.CanWalk;
						}
				}
			}
		
		}
	}
	
	
	/// <summary>
	/// Execute when the player presses the right mouse button
	/// Behaviours: when a movable unit is selected, select where it should walk to
	/// </summary>
	void CheckRightMouseClick() {

    
		// Check if we're inside the game defined viewport
		if(mouseNow.y < gameBarTop || mouseNow.y > gameBarBottom) 
			return;

		// Checks if we clicked in an unit
		Transform whatIClicked = GetWhatIClicked();

		// DEBUG
		if(selectedObject)
			Debug.Log("Clicked: "+ whatIClicked.name + " RightMouseButton while having " + selectedObject.name + 
					" selected with status " + MouseState);

		// Check in which state we are
		switch(MouseState) {

			// Walk
			case eMouseStates.CanWalk:
				{
					if(selectedObject.gameObject.GetComponent<CBaseEntity>().Type == CBaseEntity.eObjType.Monkey) {

						selectedObject.gameObject.GetComponent<CMonkey>().WalkTo(WhatIsThePositionSelected());
					}
					else if(selectedObject.gameObject.GetComponent<CBaseEntity>().Type == CBaseEntity.eObjType.Drone) {

						CDrone thisSelectedDrone = selectedObject.gameObject.GetComponent<CDrone>();

						if(thisSelectedDrone.droneType == CDrone.eDroneType.Patrol)
            {
              if( thisSelectedDrone.patrolDroneScript.bSetNewPatrol && thisSelectedDrone.patrolDroneScript.bPatrolPointsValid)
							thisSelectedDrone.patrolDroneScript.bStartMoving = true;
            }
						else thisSelectedDrone.WalkTo(WhatIsThePositionSelected());
					}

					// Show where we clicked in the ground
					StartCoroutine(ShowSelectedPositionWithAProjector(WhatIsThePositionSelected(),2.0f));
				}
				break;

			// Attacking something
			case eMouseStates.Targeting:
			case eMouseStates.TargetingForRecycle:
			case eMouseStates.TargetingForBrawl:
			case eMouseStates.TargetingForReprogram:
				{
					if(selectedObject.gameObject.GetComponent<CBaseEntity>().Type == CBaseEntity.eObjType.Monkey) {
						CMonkey cMonkeyScript = selectedObject.gameObject.GetComponent<CMonkey>();

						cMonkeyScript.PerformAction(whatIClicked, MouseState);
					}
					else if(selectedObject.gameObject.GetComponent<CBaseEntity>().Type == CBaseEntity.eObjType.Drone) {
						// Get the basic info on the unit
						CDrone selectedDrone = selectedObject.gameObject.GetComponent<CDrone>();
						// TODO: Fernando will check if this is right
						selectedDrone.Attack(whatIClicked);
					}
				}
				break;

			// Cientist capture a part
			case eMouseStates.CanCapture:
				{
					CBaseEntity capturedEntity = pointedObject.GetComponent<CBaseEntity>();

					if(!capturedEntity) {

						// DEBUG
						Debug.LogError("Cannot find component CBaseEntity for this object: " + capturedEntity);
					}

					selectedObject.GetComponent<CMonkey>().PerformAction(pointedObject, MouseState);
				}
				break;

			// Cientist release a part
			case eMouseStates.CanReleaseCaptured:
				{
					CBaseEntity capturedEntity = pointedObject.GetComponent<CBaseEntity>();

					if(!capturedEntity) {

						// DEBUG
						Debug.LogError("Cannot find component CBaseEntity for this object: " + capturedEntity);
					}

					capturedEntity.ReleaseMe();
				}
				break;

			// Engineer fix a sabotaged building
			case eMouseStates.EngineerFix:
				selectedObject.GetComponent<CMonkey>().PerformAction(pointedObject, MouseState);
				break;

			// A monkey entering the command center
			case eMouseStates.MonkeyCanEnterBuilding:
				{

					CMonkey cMonkeyScript = selectedObject.gameObject.GetComponent<CMonkey>();

					// Change the Monkey FSM
					cMonkeyScript.PerformAction(whatIClicked, MouseState);
				}
				break;


			// Saboteur sabotaging a building
			case eMouseStates.CanSabotageBuilding:
				{
					if(selectedObject.gameObject.GetComponent<CBaseEntity>().Type == CBaseEntity.eObjType.Monkey) {
						CMonkey cMonkeyScript = selectedObject.gameObject.GetComponent<CMonkey>();

						// DEBUG
						Debug.Log(this.transform + " calling CanSabotageBuilding with " + whatIClicked.transform);

						cMonkeyScript.PerformAction(whatIClicked, MouseState);
					}
					else if(selectedObject.gameObject.GetComponent<CBaseEntity>().Type == CBaseEntity.eObjType.Drone) {
						// Get the basic info on the unit
						CDrone selectedDrone = selectedObject.gameObject.GetComponent<CDrone>();
						selectedDrone.Attack(whatIClicked);
					}
				}
				break;

			// Saboteur vs drone
			case eMouseStates.CanSabotageDrone:
				{
					if(selectedObject.gameObject.GetComponent<CBaseEntity>().Type == CBaseEntity.eObjType.Monkey) {

						CMonkey cMonkeyScript = selectedObject.gameObject.GetComponent<CMonkey>();
						cMonkeyScript.PerformAction(whatIClicked, MouseState);
					}
					else if(selectedObject.gameObject.GetComponent<CBaseEntity>().Type == CBaseEntity.eObjType.Drone) {
						// Get the basic info on the unit
						CDrone selectedDrone = selectedObject.gameObject.GetComponent<CDrone>();
						selectedDrone.Attack(whatIClicked);
					}
				}
				break;

			default:
				break;
		}

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

		// Instantiate a local cursor
		//Transform myProjector = Instantiate(cursorObject, position, Quaternion.Euler(90.0f, 0.0f, 0.0f)) as Transform;

		// Instantiate a cursor
		if(projectorSelectTargetPosition == null) {
			projectorSelectTargetPosition = Instantiate(cursorObject, position, 
					Quaternion.identity) as Transform;
		}
		else {

			// if a projector already exists, just move it
			projectorSelectTargetPosition.transform.position = position;
		}

		yield return new WaitForSeconds(timeToShow);

		if(projectorSelectTargetPosition != null)
			Destroy(projectorSelectTargetPosition.gameObject);
	
		//Destroy(myProjector.gameObject);
	}

	/// <summary>
	/// Called from an external script, when a unit is deselected
	/// </summary>
	public void DeselectedObject() {

		if(selectedObject)
			selectedObject = null;
	}

	/// <summary>
	/// Do some cheats, for development purposes only
	/// </summary>
	void DoCheatingStuff() {

		if(Input.GetKeyDown(KeyCode.Backslash)){

			if(selectedObject && selectedObject.tag == "Building") {

				CBuilding selectedBuilding = selectedObject.gameObject.GetComponent<CBuilding>();
				if(!selectedBuilding)
					return;

				if(selectedBuilding.sabotado)
					selectedBuilding.Desabotage();
				else
					selectedBuilding.Sabotage();

				// DEBUG
				Debug.Log("CHEAT: building " + selectedObject.name + " sabotage: " + selectedBuilding.sabotado);
			}	
		}

		if(Input.GetKeyDown(KeyCode.Period)) {

			// Change the visibility of all rocket parts
			GameObject[] goRocketParts;
			goRocketParts = GameObject.FindGameObjectsWithTag("RocketPart");

			foreach(GameObject oneRocketPart in goRocketParts) {

				CRocketPart componentRocketPart = oneRocketPart.GetComponent<CRocketPart>(); 
				if(!componentRocketPart)
					Debug.LogError("Could not find CRocketPart component in " + oneRocketPart);
				
				if(!componentRocketPart.isRevealed)
					componentRocketPart.PartIsRevealed();
				else
					componentRocketPart.PartIsUnrevealed();
			}
		}

		if(Input.GetKeyDown(KeyCode.Comma)) {


			CDrone HunterDrone = GameObject.Find("/DroneEnemyHunter").GetComponent<CDrone>();

			if(!HunterDrone) {

				// DEBUG
				Debug.LogError("Drone Hunter not found");
			}
			
			HunterDrone.Attacked();
		}
		
	}
}
