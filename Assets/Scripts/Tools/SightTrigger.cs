using UnityEngine;
using System.Collections;

public class SightTrigger : MonoBehaviour {

  public int myEnemyLayer;
  public Transform oppositeDroneSeen;
  public Transform oppositeDroneLost;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnTriggerEnter (Collider other){

    if(other.tag == "Drone" && other.gameObject.layer == myEnemyLayer){
    Debug.LogWarning("Got something: " + other.transform.name);
    oppositeDroneSeen = other.transform;
    oppositeDroneLost = null;
    }
  }

  void OnTriggerExit (Collider other){

    if(other.tag == "Drone" && other.gameObject.layer == myEnemyLayer){
      Debug.LogWarning("That something got out: " + other.transform.name);
      oppositeDroneLost = other.transform;
      oppositeDroneSeen = null;
    }

  }
}
