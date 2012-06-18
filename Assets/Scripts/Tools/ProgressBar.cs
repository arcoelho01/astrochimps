using UnityEngine;
using System.Collections;

public class ProgressBar : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Updates the bar
	/// </summary>
	public void UpdateBar(float fCurrentTime, float fTargetTime) {
		
		renderer.material.SetFloat("_Cutoff", 1-Mathf.InverseLerp(0, fTargetTime, fCurrentTime));
	}
}
