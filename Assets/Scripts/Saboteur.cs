using UnityEngine;
using System.Collections;

enum eActionState {STANDBY,WORKING};

public class Saboteur : MonoBehaviour {

  private Transform myTransform;

  private CDrone cdroneScript;

  public bool amIVisible;

  public float invisibleTimeCounter;

  void Awake () {

    cdroneScript = this.GetComponent<CDrone>();
    amIVisible = false;
    invisibleTimeCounter = 5.0f;
    myTransform = this.transform;


  }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	if(cdroneScript == null)
      Debug.LogError("My script is empty go back");

	
	}

  public void PatrolSawMe () {

    amIVisible = true;

  }


  public void SabotageBuilding (GameObject goTarget) {
		
 	if(cdroneScript.isStunned())
      return;
		
    CBuilding targetBuilding = goTarget.GetComponent<CBuilding>();

    if(!targetBuilding.sabotado)
      targetBuilding.Sabotage();
    else Debug.Log("Ja sabotado");
  }

}
