using UnityEngine;
using System.Collections;

/// <summary>
/// This class implements the "capture ray". This ray will be used to retrieve and carry rocket parts, and also
/// to capture monkeys and carry them to captivity. The idea is that the captured object will float above the
/// holder or the ray, and will be immediately released if this holder is attacked
/// </summary>
public class CaptureRay : MonoBehaviour {

	public Transform capturedObj;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(!capturedObj) {

		}	
	}

	/// <summary>
	/// Capture an object
	/// </summary>
	/// <param name="targetCapture"> The transform of whom it's been captured </param>
	public void Capture(Transform targetCapture) {

		// Get the captured object
		capturedObj = targetCapture;
		// Set it to a "captured" state, so it can't move or be selected. Acessing the parent
		CBaseEntity capturedBaseEntity = capturedObj.transform.parent.GetComponent<CBaseEntity>();

		if(!capturedBaseEntity) {

			//  DEBUG
			Debug.LogError("CBaseEntity not found for object " + capturedObj);
		}

		if(capturedBaseEntity.canBeCaptured)
			capturedBaseEntity.Captured(this.transform);

		// Modify it's transform position, so it floats
	}
}
