using UnityEngine;
using System.Collections;

public class SightTrigger : MonoBehaviour {

  public int myEnemyLayer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnTriggerEnter (Collider other){

    if(other.tag == "Monkey" || other.tag == "Drone")
     Debug.LogWarning("Got something: " + other.transform.name);

  }

  void OnTriggerExit (Collider other){

    if(other.tag == "Monkey" || other.tag == "Drone")
      Debug.LogWarning("That something got out: " + other.transform.name);

  }
}
