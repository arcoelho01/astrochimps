using UnityEngine;
using System.Collections;

/// <summary>
/// This script handles the animation and other stuff to the launching sequence in the game. This sequence
/// is presented when the player win the game
/// </summary>
public class LaunchingSequenceControl : MonoBehaviour {

	MainScript scriptMainScript;
	Camera camLaunchCam;
	Minimap scriptMinimap;
	Transform tHud;
	Transform tRocket;

	// Use this for initialization
	void Start () {
	
		// Cache the main script
		scriptMainScript = GameObject.Find("/Codigo").GetComponent<MainScript>();

		// Get all needed objects
		camLaunchCam = this.camera;

		// Minimap script
		GameObject goMinimapCam = GameObject.FindWithTag("MinimapCam");
		scriptMinimap = goMinimapCam.GetComponent<Minimap>();

		// Hud object
		tHud = GameObject.Find("/HUD").transform;

		// Rocket Object
		GameObject[] goRockets = GameObject.FindGameObjectsWithTag("Rocket");
		foreach(GameObject goRocket in goRockets) {

			if(goRocket.name == "Rocket") {

				tRocket = goRocket.transform;
				break;
			}
		}

	}
	
	// Update is called once per frame
	void Update () {
	

		if(Input.GetKeyDown(KeyCode.Semicolon)) {

			LaunchingSequence();
		}
	}

	/// <summary>
	/// </summary>
	void LaunchingSequence() {

		// Play the countdown
		// Add white smoke
		// When the countdown is over, start the engine with an explosion
		// Animate the rocket launch 

		// Disable the player's control
		scriptMainScript.bnOnCutscene = true;
		
		// Disable the minimap
		scriptMinimap.DisableMinimap();

		// Disable the game camera
		GameObject goMainCamera = GameObject.FindWithTag("MainCamera");
		goMainCamera.GetComponent<Camera>().enabled = false;

		// Disable the HUD
		tHud.gameObject.SetActiveRecursively(false);

		// Change the view to our camera
		camLaunchCam.enabled = true;

		// Play the launching animation
		tRocket.animation.Play("RocketLaunching");
		// For the camera too
		camLaunchCam.animation.Play("LaunchingCamAtLaunch");
		
	}
}
