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
<<<<<<< HEAD
 //   this.transform.localScale.x = this.transform.localScale.x * sightScaleFactor;
 //   this.transform.localScale.z = this.transform.localScale.z * sightScaleFactor;
=======
      if(sightScaleFactor > 1) this.transform.localPosition += new Vector3(0,0,2.0f);
      else this.transform.localPosition = new Vector3(0,0.9f,11.0f);

      this.transform.localScale = new Vector3(15 * sightScaleFactor,14 * sightScaleFactor,4);

>>>>>>> 857131d60309eadfa1fa2e844e04bbe7f352964a
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
