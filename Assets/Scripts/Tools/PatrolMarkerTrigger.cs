using UnityEngine;
using System.Collections;

public class PatrolMarkerTrigger : MonoBehaviour {

  public CDrone droneScript;

	// Use this for initialization
	//void Start () {
	//}
	
	// Update is called once per frame
	//void Update () {
	//
	//}


  void OnTriggerEnter (Collider other){

    if(other.tag != "Monkey" && other.tag != "Drone" && other.tag != "Building" && other.tag != "Resource" && other.tag != "RocketPart")
      return;

    //Debug.Log("Triggered by" + this.transform.name + " and " + other.transform.name);
    if(droneScript) droneScript.patrolScript.patrolPointsValid = false;
    this.transform.renderer.material.color = Color.red;

  }

  void OnTriggerExit (Collider other){

    if(other.tag != "Monkey" && other.tag != "Drone" && other.tag != "Building" && other.tag != "Resource" && other.tag != "RocketPart")
      return;

    //Debug.Log("Triggered exity by" + this.transform.name);
    if(droneScript) droneScript.patrolScript.patrolPointsValid = true;
    this.transform.renderer.material.color = Color.green;

  }

}
