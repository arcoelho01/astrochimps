using UnityEngine;
using System.Collections;

/// <summary>
/// Displays the end game credits
/// </summary>
public class EndCredits : MonoBehaviour {

	string stMessage = "";
	int nIdx = 0;
	float fTimerCredit = 0.0f;
	public float fTimeCredit = 4.0f;

	string stTitle;
	string stText;

	Rect rectMessage;
	Rect rectTitleMessage;
	float fMessageWidth = 300;
	float fMessageHeight = 400;
	float fMessagePosY = 100;

	public GUISkin skin; 

	bool bnEndOfCredits;

	string[] stEndCredits = new string[] {
		"Game Design", "Alexandre Ramos Coelho", "Aurélio Athayde Rauber", "Fernando Leite", 
		"Freddy Christian Gund", "Leonardo Schenkel", "Márcio Lobato", "Rodrigo Ribeiro","\n",
		"Lead Animator","Márcio Lobato","\n",
		"Lead Artist", "Aurélio Rauber","\n",
		"Lead Modeller", "Freddy Gund","\n",
		"Lead Sound Designer", "Leonardo Schenkel", "\n",
		"Programming", "Alexandre Ramos", "Fernando Leite", "Rodrigo Ribeiro", "Leonardo Schenkel", "\n",
		"Additional Programming", "Freddy Gund", "\n",
		"Voices", "Alexandre Ramos", "\n",
		"Music", "West Noorwod Cassete Library - Flashlight","by Adam Marshall","\n",
		"Additional Music", "Road To Hell", "by Kevin MacLeod", "incompetech.com","\n",
		"Using", "iTween", "itween.pixelplacement.com","\n",
		"Using", "Detonator for Unity", "Ben Throop","\n"
	};

	//<
	public string nextScene = "";

	// Use this for initialization
	void Start () {

		// DEBUG
		Debug.Log(this.transform + " " + stEndCredits.Length);

		//rectMessage = new Rect(Screen.width * 0.5f - fMessageWidth * 0.5f,
		//		fMessagePosY, fMessageWidth, fMessageHeight);
		//rectTitleMessage = new Rect(Screen.width * 0.5f - fMessageWidth * 0.5f,
		//		fMessagePosY-50, fMessageWidth, fMessageHeight);
		ShowNextCredit();
	}
	
	// Update is called once per frame
	void Update () {
	
		if(bnEndOfCredits && Input.anyKey) {

				LoadNextCutscene();
		}

		fTimerCredit += Time.deltaTime;

		if(fTimerCredit > fTimeCredit)
			ShowNextCredit();

	}

	void OnGUI() {

		GUI.skin = skin;

		GUI.Label(rectTitleMessage, stTitle);
		GUI.Label(rectMessage, stText);
	}

	/// <summary>
	/// Get 
	/// </summary>
	void ShowNextCredit() {

		int nLines = 0;

		if(nIdx >= stEndCredits.Length) {

			bnEndOfCredits = true;
			stTitle = "";
			stText = "";
			return;
		}

		string stRead = "";

		stTitle = "";
		stText = "";
		fTimerCredit = 0.0f;

		stTitle = stEndCredits[nIdx++];

		while(stRead != "\n" && nIdx < stEndCredits.Length) {

			stRead = stEndCredits[nIdx];
			stText += stRead + "\n";
			nLines++;
			nIdx++;
		}

		fMessagePosY = Screen.height - (nLines * 25);
		rectMessage = new Rect(Screen.width - fMessageWidth,
				fMessagePosY, fMessageWidth, fMessageHeight);

		rectTitleMessage = new Rect(Screen.width - fMessageWidth,
				fMessagePosY-50, fMessageWidth, fMessageHeight);
	}

	/// <summary>
	/// Loads the next cutscene
	/// </summary>
	void LoadNextCutscene() {

		iTween.CameraFadeAdd();
		iTween.CameraFadeTo(0.5f, 3.0f);
		Application.LoadLevel(nextScene);
	}
}
