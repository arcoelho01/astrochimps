using UnityEngine;
using System.Collections;

public class ShowInfoPanel : MonoBehaviour {

	Transform myParent;
	float offsetY;
	bool showInfo = false;
	string infoText = "";

	// Use this for initialization
	void Start () {
	
		myParent = transform.parent;
		offsetY = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
	

		if(myParent.renderer.isVisible) {
		
			showInfo = true;
			offsetY += Time.deltaTime * 20;
		}
		else
			showInfo = false;
	}

	void OnGUI() {

		if(showInfo) {
			// Find my position on the screen
			Vector3 position = Camera.main.WorldToScreenPoint(myParent.transform.position + 
					new Vector3(0, myParent.transform.localScale.y,0));

			GUI.Label(new Rect(position.x, Screen.height - position.y - offsetY,100,50), infoText);
		}
	}

	public void SetInfoText(string newText) {

		infoText = newText;
	}
}
