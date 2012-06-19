using UnityEngine;
using System.Collections;

/// <summary>
/// This class creates a progress bar that fills (left to right) from time to time.
/// </summary>
public class ProgressBar : MonoBehaviour {

	// For now, always face the camera
	bool bnFaceCamera = true;

	//< Sound to play when the bar completes
	public AudioClip sfxBarComplete;

	// Use this for initialization
	void Start () {
	
		FaceCamera();
	}
	
	/// <summary>
	/// Rotates the bar so it will always face the camera
	/// </summary>
	void Update() {

		if(!bnFaceCamera)
			return;

		FaceCamera();
	}

	/// <summary>
	/// Updates the bar, filling it (left to right)
	/// </summary>
	/// <param name="fCurrentTime"> Current timer (or % done until now) </param>
	/// <param name="fTargetTime"> Time needed to fill the bar (100% completion) </param>
	public void UpdateIncreaseBar(float fCurrentTime, float fTargetTime) {
		
		renderer.material.SetFloat("_Cutoff", 1-Mathf.InverseLerp(0, fTargetTime, fCurrentTime));

		// Bar completed?
		if(fCurrentTime >= fTargetTime) {

			// FIXME: the bar will soon be destroyed. Should we use PlayOneShot() instead?
			if(sfxBarComplete)
				AudioSource.PlayClipAtPoint(sfxBarComplete, transform.position);
		}
	}

	/// <summary>
	/// Updates the bar, decreasing the fill bar (right to left)
	/// </summary>
	/// <param name="fCurrentTime"> Current timer (or % done until now) </param>
	/// <param name="fTargetTime"> Time needed to fill the bar (100% completion) </param>
	public void UpdateDecreaseBar(float fCurrentTime, float fTargetTime) {
		
		renderer.material.SetFloat("_Cutoff", Mathf.InverseLerp(0, fTargetTime, fCurrentTime));

		// Bar completed?
		if(fCurrentTime >= fTargetTime) {

			// FIXME: the bar will soon be destroyed. Should we use PlayOneShot() instead?
			if(sfxBarComplete)
				AudioSource.PlayClipAtPoint(sfxBarComplete, transform.position);
		}
	}

	/// <summary>
	/// Rotates the bar so it face the camera
	/// </summary>
	void FaceCamera() {

		// The mesh is inverted. If we use LookAt() direct with the camera position, we cannot see the bar (it's
		// textures are mapped to the other side of the mesh). So, to correct this, we actually look "away" from
		// the camera
		transform.LookAt(2 * transform.position - Camera.main.transform.position);
	}

}
