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
	Transform tLaunchExplosion;
	Transform tRocketThrust;
	bool bnRocketLaunched;

	public Transform tPrefabLaunchExplosion;
	public Transform tLaunchExplosionSpot;
	public AudioClip sfxRocketLaunching;

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

		// Rocket engine thrust
		tRocketThrust = tRocket.transform.Find("RocketThrust");

		if(!tRocketThrust) {

			// DEBUG
			Debug.LogError(this.transform + " cannot find the rocket thrust particle system");
		}

		// Comment the line below to test the launching
		LaunchingSequence();
	}
	
	// Update is called once per frame
	void Update () {

		// Code below just for testing the launching sequence directly.
		/*
		if(Input.GetKeyDown(KeyCode.Z)) {

			// DEBUG
			Debug.Log(this.transform + " starting cutscene");

			LaunchingSequence();
		}
		//*/

		if(bnRocketLaunched)
			return;
	
	}

	/// <summary>
	/// </summary>
	void LaunchingSequence() {

		bnRocketLaunched = true;

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

		// This is for the Detonator package to work
		camLaunchCam.tag = "MainCamera";

		// Start the engine
		tRocketThrust.gameObject.active = true;

		// Play the launching animation
		tRocket.animation.Play("RocketLaunching");
		// For the camera too
		camLaunchCam.animation.Play("LaunchingCamAtLaunch");

		tLaunchExplosion = Instantiate(tPrefabLaunchExplosion, tLaunchExplosionSpot.transform.position, Quaternion.identity) as Transform;
	}

	/// <summary>
	/// If we are using the Detonator package, do the launching explosion
	/// <summary>
	void DoDetonatorLaunchingExplosion() {

		// Trigger the launching explosion
		if(tPrefabLaunchExplosion && tLaunchExplosionSpot) {

			tLaunchExplosion = Instantiate(tPrefabLaunchExplosion, tLaunchExplosionSpot.transform.position, Quaternion.identity) as Transform;

			Detonator scriptDetonator = tLaunchExplosion.GetComponent<Detonator>();

			if(!scriptDetonator)
				Debug.LogError(this.transform + " detonator script not found!");

			scriptDetonator.destroyTime = 0.0f;
			scriptDetonator.Explode();
		}
	}

	void ShowCredits() {

		// HACK!
		scriptMainScript.player.AddResourceOxygen(400.0f);

		Application.LoadLevelAdditive("credits");
	}

	void LoadNextCutscene() {

		iTween.CameraFadeAdd();
		iTween.CameraFadeTo(0.5f, 3.0f);
		Application.LoadLevel("end_victory");
	}
}
