using UnityEngine;
using System.Collections;

/// <summary>
/// Class to show the player the main menu screen and the menu itself
/// </summary>
public class MainMenuScreen : MonoBehaviour {

	public GUISkin skin;

	//< The background image for this screen
	public Texture2D texBGImage;
	//< Background logo size
	private Rect rectBGScreen = new Rect();

	float fMenuHeight = 100;
	float fMenuWidth = 400;
	float fScreenX;
	float fScreenY;

	private delegate void GUIMethod();
	private GUIMethod currentMenu;

	//< The scene to load when the player hit new game
	public string sceneNewGame = "";

	void Awake() {

		// Background stuff
		rectBGScreen.x = 0;
		rectBGScreen.y = 0;

		rectBGScreen.width = Screen.width;
		rectBGScreen.height = Screen.height;
		
		// Put the menu on the center of the screen
		fScreenX = Screen.width * 0.5f - fMenuWidth * 0.5f;
		//fScreenY = Screen.height * 0.5f - fMenuHeight * 0.5f;
		fScreenY = 240;

		currentMenu = MainMenu;
	}

	// Use this for initialization
	void Start () {
	
		Screen.showCursor = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {

		GUI.skin = skin;

		// Draws the background image
		if(texBGImage) {

			GUI.DrawTexture(rectBGScreen, texBGImage);
		}

		currentMenu();
	}

	/// <summary>
	/// The main menu. For now, only shows "New Game" and "Quit" options
	/// </summary>
	void MainMenu() {

		GUILayout.BeginArea(new Rect(fScreenX, fScreenY, fMenuWidth, fMenuHeight));
		{
			
			GUILayout.BeginHorizontal();
			if(GUILayout.Button("Novo Jogo")) {

				if(sceneNewGame != "") {

					Application.LoadLevel(sceneNewGame);
				}
			}
			else if(GUILayout.Button("Sair")) {

				Application.Quit();
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();
	}
}
