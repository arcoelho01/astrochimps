using UnityEngine;
using System.Collections;

/// <summary>
/// This class will create an object and put it as a child from another. It's show a text in the screen,
/// right above the parent object, and then move it towards the top of the screen (like a score info from an
/// old-school game). Useful, for instance, to show a player what resource an extractor is extracting and it's
/// amount
/// </summary>
public class ShowInfoPanel : MonoBehaviour {

	// Parent for this object
	Transform myParent;
	// Vertical offset, to make the text floats towards the top of the screen 
	float offsetY;
	// Text to be shown
	string infoText = "";

	// Use this for initialization
	void Start () {
	
		myParent = transform.parent;
		offsetY = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {

		offsetY += Time.deltaTime * 20;
	}

	/// <summary>
	/// Actually draws the text on screen
	/// </summary>
	void OnGUI() {

		// Only show something if the object is actually on the screen
		if(myParent.renderer.isVisible) {
			// Find my position on the screen
			Vector3 position = Camera.main.WorldToScreenPoint(myParent.transform.position + 
					new Vector3(0, myParent.transform.localScale.y,0));

			GUI.Label(new Rect(position.x, Screen.height - position.y - offsetY,100,50), infoText);
		}
	}

	/// <summary>
	/// Set the text to be shown
	/// </summary>
	public void SetInfoText(string newText) {

		infoText = newText;
	}
}
