using UnityEngine;
using System.Collections;

public class EnemyPatrol : MonoBehaviour {

  public int patrolIndex;

  public Vector3[] patrolTarget;
  //public Vector3[] previousPatrolTarget; // This probably wont be necessary but lets leave it for now
  //public GameObject[] patrolMarkers;

  private CDrone cdroneScript;

	// Use this for initialization
	void Start () {

    cdroneScript = this.GetComponent<CDrone>();
    patrolIndex = 0;
    ChoicePatrol(4);
    cdroneScript.AIScript.ClickedTargetPosition(patrolTarget[patrolIndex]);

	}
	
	// Update is called once per frame
	void Update () {

    StartPatrol();

	}

  public void ChoicePatrol(int x){

  patrolTarget = new Vector3[x];
  //patrolMarkers = new GameObject[x];

    if(x == 4)
      SquarePatrol();
    else if (x == 3)
           TrianglePatrol();
         else LinePatrol();

  }

  private void LinePatrol() {

   //Debug.Log("Set patrol:" + patrolTarget);
   //patrolSet = false;
   //setNewPatrol = true;
   //patrolPointsValid = true;

   patrolTarget[0] = new Vector3(20.0f,0.0f,0.0f) + this.transform.position;
   patrolTarget[1] = new Vector3(-20.0f,0.0f,0.0f) + this.transform.position;

 }

  private void TrianglePatrol() {

   //Debug.Log("Set patrol:" + patrolTarget);
   //patrolSet = false;
   //setNewPatrol = true;
   //patrolPointsValid = true;

   patrolTarget[0] = new Vector3(0.0f,0.0f,20.0f) + this.transform.position;
   patrolTarget[1] = new Vector3(20.0f,0.0f,-20.0f) + this.transform.position;
   patrolTarget[2] = new Vector3(-20.0f,0.0f,-20.0f) + this.transform.position;


 }

  private void SquarePatrol () {

  //Debug.Log("Set patrol:" + patrolTarget);
  //patrolSet = false;
  //setNewPatrol = true;
  //patrolPointsValid = true;

  patrolTarget[0] = new Vector3(20.0f,0.0f,20.0f) + this.transform.position;
  patrolTarget[1] = new Vector3(20.0f,0.0f,-20.0f) + this.transform.position;
  patrolTarget[2] = new Vector3(-20.0f,0.0f,-20.0f) + this.transform.position;
  patrolTarget[3] = new Vector3(-20.0f,0.0f,20.0f) + this.transform.position;

  }

  public void StartPatrol () {

    //if(cdroneScript.GetCurrentState() == CDrone.FSMState.STATE_IDLE)
    //  cdroneScript.AIScript.ClickedTargetPosition(patrolTarget[patrolIndex]);

    if(Vector3.Distance(this.transform.position,patrolTarget[patrolIndex]) < 5.0f){
      //if(!AstarAIFollow.onAIMovingChange) {
      patrolIndex += 1;
      if(patrolIndex >= patrolTarget.Length) patrolIndex = 0;
      cdroneScript.AIScript.ClickedTargetPosition(patrolTarget[patrolIndex]);
    }
   //}
 }

}
