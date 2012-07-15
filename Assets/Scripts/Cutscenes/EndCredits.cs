using UnityEngine;
using System.Collections;

/// <summary>
/// Displays the end game credits
/// </summary>
public class EndCredits : MonoBehaviour {

	string stMessage = "";
	int nIdx = 0;
	float fTimerCredit = 0.0f;
	float fTimeCredit = 4.0f;

	string stTitle;
	string stText;

	Rect rectMessage;
	Rect rectTitleMessage;

	public GUISkin skin; 

	string[] stEndCredits = new string[] {
		"Game Design", "Alexandre Ramos Coelho", "Aurélio Athayde Rauber", "Fernando Leite", 
		"Freddy Christian Gund", "Leonardo Schenkel", "Márcio Lobato", "Rodrigo Ribeiro","\n",
		"Lead Animator","Márcio Lobato","\n",
		"Lead Artist", "Aurélio Rauber","\n",
		"Lead Modeller", "Freddy Gund","\n",
		"Lead Sound Designer", "Leonardo Schenkel", "\n",
		"Programming", "Alexandre Ramos", "Fernando Leite", "Rodrigo Ribeiro", "Leonardo Schenkel", "\n",
		"Additional Programming", "Freddy Gund", "\n",
		"Lead Monkey Boy", "Alexandre Ramos", "\n"
	};

	//<
	public string nextScene = "";
	GUIText gtTitle;

	// Use this for initialization
	void Start () {

		// DEBUG
		Debug.Log(this.transform + " " + stEndCredits.Length);

		float fMessageWidth = 300;
		float fMessageHeight = 400;
		float fMessagePosY = 100;
		rectMessage = new Rect(Screen.width * 0.5f - fMessageWidth * 0.5f,
				fMessagePosY, fMessageWidth, fMessageHeight);
		rectTitleMessage = new Rect(Screen.width * 0.5f - fMessageWidth * 0.5f,
				fMessagePosY-50, fMessageWidth, fMessageHeight);

		gtTitle = GetComponent<GUIText>();
		gtTitle.pixelOffset = new Vector2(200,200);
		
		ShowNextCredit();
	}
	
	// Update is called once per frame
	void Update () {
	
		fTimerCredit += Time.deltaTime;

		if(fTimerCredit > fTimeCredit)
			ShowNextCredit();
	}

	void OnGUI() {

		GUI.skin = skin;

		//GUI.Label(rectTitleMessage, stTitle);
		//GUI.Label(rectMessage, stText);
	}

	/// <summary>
	/// Get 
	/// </summary>
	void ShowNextCredit() {

		string stRead = "";

		stTitle = "";
		stText = "";
		fTimerCredit = 0.0f;

		stTitle = stEndCredits[nIdx++];


		while(stRead != "\n" && nIdx < stEndCredits.Length) {

			stRead = stEndCredits[nIdx];
			stText += stRead + "\n";
			nIdx++;
		}

		gtTitle.text = stTitle;
		// DEBUG
		//Debug.Log("Title: " + stTitle);
		//Debug.Log(stText);
	}
}
