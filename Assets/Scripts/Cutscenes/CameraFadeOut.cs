using UnityEngine;
using System.Collections;

public class CameraFadeOut : MonoBehaviour {

	public GUISkin skin;

	bool bnWaitForKey;
	Rect rectEndMessage;
	string stEndMessage = "Pressione qualquer tecla";
	float fEndMessageHeight = 50;
	float fEndMessageWidth = 200;
	float fEndMessagePosY;

	//< Sound to be played on the dramatic zoom on the drone
	public AudioClip sfxDramaticDroneSound;
	//< sound to be played when the monkeys are celebrating
	public AudioClip sfxMonkeysCelebrating;

	//<
	public string sceneMainScreen = "";

	// Use this for initialization
	void Start () {
	
		// Calculate the message position on the screen
		fEndMessagePosY = Screen.height - (fEndMessageHeight * 1.5f);
		rectEndMessage = new Rect(Screen.width * 0.5f - fEndMessageWidth * 0.5f,
				fEndMessagePosY, fEndMessageWidth, fEndMessageHeight);
	}
	
	// Update is called once per frame
	void Update () {

		if(bnWaitForKey) {

			if(Input.anyKey) {

				// Back to the main menu
				Application.LoadLevel(sceneMainScreen);
			}

		}
		else if(!animation.isPlaying) {

			// Adds the camera for the fade out effect
			iTween.CameraFadeAdd();
			// Call the fade out animation
			iTween.CameraFadeTo(iTween.Hash("amount",0.5f, "time",2.0f));
			bnWaitForKey = true;
		}
	}

	void OnGUI() {

		GUI.skin = skin;

		if(!bnWaitForKey)
			return;

		GUI.Label(rectEndMessage, stEndMessage);
	}

	void CallDramaticDroneSound() {

		if(sfxDramaticDroneSound)
			audio.PlayOneShot(sfxDramaticDroneSound);
	}

	void CallMonkeysPartyingSound() {

		if(sfxMonkeysCelebrating)
			audio.PlayOneShot(sfxMonkeysCelebrating);
	}
}
