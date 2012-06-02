using UnityEngine;
using System.Collections;

/// <summary>
/// Defines the basic GUI information on the bottom of the screen
/// Basically, shows the name of the object where the mouse is over
/// </summary>
public class GUIBottomMenu : MonoBehaviour {

	// PRIVATE
	float posX;
	float posY;
	public float height = 50;	// Height of the menu bar
	float width = 600;	// Width of the menu bar
	float marginBottom = 0;	// Margin from the bottom of the screen
	string stLabelText = "";
	CPlayer player;
	enum eMenuType { BuildExtractor, PlayerInfo, BuildingInfo, NONE} ;
	eMenuType menuType = eMenuType.PlayerInfo;
	CBaseEntity caller;	// Caller of a menu. Can be a building, for instance
	CBuilding callerBuilding;

	// PUBLIC
	public GUISkin menuSkin;
	public Transform playerObject;

	/*
	 * ===========================================================================================================
	 * UNITY STUFF
	 * ===========================================================================================================
	 */

	//
	void Awake() {

		if(!playerObject) {

			// DEBUG
			Debug.LogError("Player object on the bottom GUI is not set. Move it on the inspector");
		}

		player = playerObject.gameObject.GetComponent<CPlayer>();
	}

	// Use this for initialization
	void Start () {

		// Position the bar
		posX = 0;
		width = Screen.width;

		// Position the bar on the bottom of the screen
		posY = Screen.height - height - marginBottom;
	}
	
	/// <summary>
	/// 
	/// </summary>
	void OnGUI() {

		GUI.skin = menuSkin;

		//
		GUI.Label(new Rect(posX, posY - 20, Screen.width, 25),stLabelText);

		// This area defines the bar on the bottom part of the screen
		GUI.Box(new Rect(posX, posY, width, height), "");

		switch(menuType) {

			case eMenuType.PlayerInfo:
				PlayerInfoMenu();
				break;
			case eMenuType.BuildExtractor:
				BuildExtractorMenu();
				break;
			case eMenuType.BuildingInfo:
				BuildingInfoMenu();
				break;
			case eMenuType.NONE:
				break;
			default:
				break;

		}
	}

	/// <summary>
	/// Changes the text of the first label. Usually, it will show the unit's name
	/// </summary>
	/// <param name="stNewLabelText">A string with the new text for the first label</param>
	public void SetInfoLabel(string stNewLabelText) {

		stLabelText = stNewLabelText;
	}

	/// <summary>
	/// Clear the text of the first label
	/// </summary>
	public void ClearInfoLabel() {

		stLabelText = "";
	}

	/*
	 * ===========================================================================================================
	 * MENU ENABLING METHODS
	 * ===========================================================================================================
	 */

	/// <summary>
	/// Changes the menu type to show the player info
	/// </summary>
	/// <param name="caller"> The CBaseEntity component of the caller of this method </param>
	public void PlayerInfoMenuEnable(CBaseEntity caller) {

		menuType = eMenuType.PlayerInfo;
		this.caller = caller;
	}

	/// <summary>
	/// Changes the menu type to show the menu for building an extractor
	/// </summary>
	/// <param name="caller"> The CBaseEntity component of the caller of this method </param>
	public void BuildExtractorMenuEnable(CBaseEntity caller) {

		menuType = eMenuType.BuildExtractor;
		this.caller = caller;
	}

	/// <summary>
	/// Shows the info of a building. If it has a monkey inside, show the GUI to get it out
	/// </summary>
	/// <param name="caller"> The CBuilding component of the caller of this method </param>
	public void BuildingInfoMenuEnable(CBuilding caller) {

		menuType = eMenuType.BuildingInfo;
		this.callerBuilding = caller;
	}
	
	/*
	 * ===========================================================================================================
	 * MENU GUI DEFINITIONS
	 * ===========================================================================================================
	 */

	/// <summary>
	/// Shows info about the player status
	/// </summary>
	void PlayerInfoMenu() {

		GUI.Label(new Rect(5, posY + 5, 90, 25), "Oxygen: " + player.oxygenLevel);
		GUI.Label(new Rect(5, posY + 30, 90, 20), "Metal: " + player.metalLevel);
	}

	/// <summary>
	/// Shows the menu bar for building an extractor in a resource site
	/// </summary>
	void BuildExtractorMenu() {

		if(GUI.Button(new Rect(5, posY + 5, 120, 25),"Build Extractor")) {

			caller.BuildIt();

			// Disable the building menu
			menuType = eMenuType.PlayerInfo;
		}
	}

	/// <summary>
	/// Shows info about the player status
	/// </summary>
	void BuildingInfoMenu() {

		if(GUI.Button(new Rect(5, posY + 5, 120, 25),"Remove monkey")) {

			callerBuilding.GetTheMonkeyOut();
			// Disable the info menu
			menuType = eMenuType.PlayerInfo;
		}
	}
}
