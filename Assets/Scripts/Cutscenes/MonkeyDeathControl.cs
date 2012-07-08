using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This script handles the animation for the death of the monkeys when they run out of oxygen
/// </summary>
public class MonkeyDeathControl : MonoBehaviour {

	MainScript scriptMainScript;
	Camera camLaunchCam;
	Minimap scriptMinimap;
	Transform tHud;
	GameObject goMainCamera;

	public Camera camPrefabCamera;

	// Four cameras, one for each monkey
	Camera[] camMonkeys = new Camera[4];
	Rect[] rectCamMonkeys = new Rect[4];

	List<Transform> lAllMonkeys = new List<Transform>();

	public AudioClip sfxDeathMusic;

	// Use this for initialization
	void Start () {
	
		// Cache the main script
		scriptMainScript = GameObject.Find("/Codigo").GetComponent<MainScript>();

		// Minimap script
		GameObject goMinimapCam = GameObject.FindWithTag("MinimapCam");
		scriptMinimap = goMinimapCam.GetComponent<Minimap>();

		// Hud object
		tHud = GameObject.Find("/HUD").transform;

		goMainCamera = GameObject.FindWithTag("MainCamera");

		// All the monkeys in the game
		lAllMonkeys = scriptMainScript.GetListOfAllMonkeys();
	}
	
	// Update is called once per frame
	void Update () {
	
		if(Input.GetKeyDown(KeyCode.X)) {

			//
			SetupCameras();
			DeathSequence();
		}
	}

	/// <summary>
	/// Split the screen in 4 rects
	/// </summary>
	void SetupCameras() {

		// Split the screen in 4 parts
		// x, y, width, height
		rectCamMonkeys[0] = new Rect(0, 0, 0.5f, 0.5f);
		rectCamMonkeys[1] = new Rect(0.5f, 0, 0.5f, 0.5f);
		rectCamMonkeys[2] = new Rect(0, 0.5f, 0.5f, 0.5f);
		rectCamMonkeys[3] = new Rect(0.5f, 0.5f, 0.5f, 0.5f);

		// Create four cameras
		for(int nIdx = 0; nIdx < 4; nIdx++) {

			// Create the camera itself
			camMonkeys[nIdx] =  Instantiate(camPrefabCamera, 
					goMainCamera.transform.position, goMainCamera.transform.rotation) as Camera;

			// Set its share of the screen
			camMonkeys[nIdx].rect = rectCamMonkeys[nIdx];
			camMonkeys[nIdx].fieldOfView = 5.0f;
			
			// Point it to one of the monkeys
			if(nIdx < lAllMonkeys.Count) {

				camMonkeys[nIdx].transform.LookAt(lAllMonkeys[nIdx].transform);
				// DEBUG
				Debug.Log(this.transform + " Cam " + nIdx + " looking at " + lAllMonkeys[nIdx].transform);
			}
		}
	}

	/// <summary>
	///
	/// </summary>
	void DeathSequence() {

		// Disable the player's control
		scriptMainScript.bnOnCutscene = true;
		
		// Disable the minimap
		scriptMinimap.DisableMinimap();

		// Disable the game camera
		goMainCamera.GetComponent<Camera>().enabled = false;

		// Disable the HUD
		tHud.gameObject.SetActiveRecursively(false);

		// Play the "death song"
		if(sfxDeathMusic)
			audio.PlayOneShot(sfxDeathMusic);

		// Play the animation
		PlayMonkeysDyingAnimation();
	}

	/// <summary>
	/// </summary>
	void PlayMonkeysDyingAnimation() {

		foreach(Transform tMonkey in lAllMonkeys) {

			CMonkey cMonkey = tMonkey.GetComponent<CMonkey>();
			if(cMonkey) {

				cMonkey.KillMonkey();
			}
		}
	}
}
