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
	// Starting position of the text in the world
	Vector3 v3Position;

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
		if(BeingRenderedOnCamera()) {
			// Find my position on the screen
			Vector3 position = Camera.main.WorldToScreenPoint(v3Position);

			GUI.Label(new Rect(position.x, Screen.height - position.y - offsetY, 150,50), infoText);
		}
	}

	/// <summary>
	/// Set the text to be shown and it's initial position
	/// </summary>
	/// <param name="newText"> String with the text to be shown </param>
	/// <param name="position"> A Vector3 with the initial position (on the world) to show the text
	public void SetInfoText(string newText, Vector3 position) {

		infoText = newText;
		v3Position = position;
	}

	/// <summary>
	/// Check if the object is being rendered in the screen. If not, there's no need to display the info text,
	/// right?
	/// </summary>
	bool BeingRenderedOnCamera() {

		bool rv = false;

		CBaseEntity baseEntity = myParent.gameObject.GetComponent<CBaseEntity>();

		if(baseEntity) {

			if(baseEntity.mainRendererObject != null) {

				Debug.Log("Checking renderer in " + baseEntity.mainRendererObject);
				rv = baseEntity.mainRendererObject.renderer.isVisible;
			}
		}
		else {
					
			rv  = myParent.renderer.isVisible;
		}

		return rv;
	}
}
