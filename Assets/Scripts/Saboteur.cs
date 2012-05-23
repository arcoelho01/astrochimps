using UnityEngine;
using System.Collections;

enum eActionState {STANDBY,WORKING};

public class Saboteur : MonoBehaviour {

  private Transform myTransform;

  private CDrone cdroneScript;

  void Awake () {

    cdroneScript = this.GetComponent<CDrone>();
    myTransform = this.transform;


  }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


  public void SabotageBuilding (GameObject goTarget) {

    CBuilding targetBuilding = goTarget.GetComponent<CBuilding>();

    if(!targetBuilding.sabotado)
      targetBuilding.Sabotage();
    else Debug.Log("Ja sabotado");

  }

}
