using UnityEngine;
using System.Collections;

public class SightTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnTriggerEnter (){

    Debug.LogWarning("Got something");

  }

  void OnTriggerExit (){

    Debug.LogWarning("That something got out");

  }
}
