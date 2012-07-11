using UnityEngine;
using System.Collections;

public class SightTrigger : MonoBehaviour {

  public int myEnemyLayer;
  public Transform oppositeDroneSeen;
  public Transform oppositeDroneLost;

  public float sightScaleFactor;
  public bool scaleChange;

	// Use this for initialization
	void Start () {

    sightScaleFactor = 1.0f;
    scaleChange = false;
	
	}
	
	// Update is called once per frame
	void Update () {

    if(scaleChange){
 //   this.transform.localScale.x = this.transform.localScale.x * sightScaleFactor;
 //   this.transform.localScale.z = this.transform.localScale.z * sightScaleFactor;
    scaleChange = false;
    }
	
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
