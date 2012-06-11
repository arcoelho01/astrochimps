using UnityEngine;
using System.Collections;


enum eAlertLevel{PATROL,ALERT,DETECT};

public class Patrol : MonoBehaviour {

  private Transform myTransform;

  private Transform target;
  private CBaseEntity targetEntity;
  private Vector3 targetVector;
  
  private float pulseTime;
  
  //Check enemy around
  public bool enemyAround;
  private Collider[] scannedColliders;
  private int enemyMask = 1 << 11;

  private eAlertLevel status = eAlertLevel.PATROL;
  
  //Field of View
  private float detectionDistance = 14.0f;
  private float detectionRadius = 25.0f;
  private float alertRadius = 60.0f;

  public Vector3[] patrolTarget;
  public GameObject[] patrolMarkers;

  //Patrol Variables
  public bool setNewPatrol;
  public bool patrolSet;
  public int patrolIndex;
  public bool patrolPointsValid;
  public GameObject markerPatrol;
  
  public GameObject detectAlert; //Prefab to instantiate once enemy has been found, set in Inspector
  private GameObject existingAlert;

  private CDrone cdroneScript;

  void Awake () {

  cdroneScript = this.GetComponent<CDrone>();
  myTransform = this.transform;
  enemyAround = false;
  setNewPatrol = false;
  patrolSet = false;
  patrolPointsValid = false;
  scannedColliders = new Collider[0];

  }

  // Use this for initialization
  void Start () {

    pulseTime = 2.0f;

  }

  // Overlap sphere pulse
  void Update () {

  if(cdroneScript == null)
      Debug.LogError("My script is empty go back");

  if(setNewPatrol)
	PatrolSet();
		
  if(patrolSet)
  	StartPatrol();
	

  if(cdroneScript.isStunned())
      return;

  if(status == eAlertLevel.PATROL)
      Patrulha();
    else if(status == eAlertLevel.ALERT)
            Alerta();
         else if(status == eAlertLevel.DETECT)
                 Detecta();

  }

  void Patrulha(){

    pulseTime -= Time.deltaTime;


    if(pulseTime < 0){
     scannedColliders = Physics.OverlapSphere(myTransform.position,16,enemyMask);//
       if(scannedColliders.Length > 0)
         enemyAround = true;
       else enemyAround = false;
      pulseTime = 2.0f;
    }

    if(enemyAround){

      // Fase de teste
      if(scannedColliders.Length > 1){
        float tempdistance = Vector3.Distance(myTransform.position,scannedColliders[0].transform.position);
        for(int i = 0; i < scannedColliders.Length; i ++ ){
          target = scannedColliders[i].transform;
          targetEntity = target.GetComponent<CBaseEntity>();
          Debug.Log("targetEntity: " + targetEntity);
          if(!targetEntity) return;
          if(targetEntity.Type == CBaseEntity.eObjType.Building) continue;
          float tempdistanceindex = Vector3.Distance(myTransform.position,target.position);
          if(tempdistanceindex < tempdistance){
            tempdistance = tempdistanceindex;
            break;
          }else{
            target = scannedColliders[0].transform;
            targetEntity = target.GetComponent<CBaseEntity>();
          }
        }

      }else{
        target = scannedColliders[0].transform;
        targetEntity = target.GetComponent<CBaseEntity>();
      }

      targetVector = target.position - myTransform.position;

       if(Vector3.Distance(myTransform.position,target.position) < detectionDistance  && Vector3.Angle(targetVector,myTransform.forward) < detectionRadius){
        if(targetEntity.Type == CBaseEntity.eObjType.Building) return;
          pulseTime = 2.0f;
          status = eAlertLevel.DETECT;
          existingAlert = GameObject.Instantiate(detectAlert,new Vector3(target.position.x,target.position.y + 15, target.position.z), Quaternion.identity) as GameObject;
       }else if(Vector3.Distance(myTransform.position,target.position) < detectionDistance && Vector3.Angle(targetVector,myTransform.forward) < alertRadius){
          status = eAlertLevel.ALERT;
       }

    }else{
      target = null;
    }

    Debug.DrawRay(myTransform.position,myTransform.forward * 10);

  }


void Alerta () {

  targetVector = target.position - myTransform.position;

  Debug.DrawRay(myTransform.position,myTransform.forward * 10,Color.yellow); // Smelled ya !

  if(Vector3.Distance(myTransform.position,target.position) < detectionDistance){
    if(Vector3.Angle(targetVector,myTransform.forward) < detectionRadius ){
        if(targetEntity.Type == CBaseEntity.eObjType.Building) return;
        pulseTime = 2.0f;
        status = eAlertLevel.DETECT;
        existingAlert = GameObject.Instantiate(detectAlert,new Vector3(target.position.x,myTransform.position.y + 15,myTransform.position.z), Quaternion.identity) as GameObject;
    }
  }else status = eAlertLevel.PATROL;

}

  void Detecta () {

    Debug.DrawRay(myTransform.position,myTransform.forward * 10,Color.red); // Found ya !
    pulseTime -= Time.deltaTime;

    if(pulseTime < 0){
    status = eAlertLevel.PATROL;
    GameObject.Destroy(existingAlert);
    }
  }

  public void ChoicePatrol(int x){

  patrolTarget = new Vector3[x];
  patrolMarkers = new GameObject[x];

    if(x == 4)
      SquarePatrol();
    else if (x == 3)
           TrianglePatrol();
         else LinePatrol();

  }

  private void LinePatrol() {

   //Debug.Log("Set patrol:" + patrolTarget);
   patrolSet = false;
   setNewPatrol = true;
   patrolPointsValid = true;

   patrolTarget[0] = new Vector3(20.0f,0.0f,0.0f); patrolTarget[0] += MouseWorldPosition.targetPosition;
   patrolTarget[1] = new Vector3(-20.0f,0.0f,0.0f); patrolTarget[1] += MouseWorldPosition.targetPosition;

   for(int x = 0; x < patrolTarget.Length; x++){
     patrolMarkers[x] = GameObject.Instantiate(markerPatrol,patrolTarget[x],Quaternion.identity) as GameObject;
      patrolMarkers[x].GetComponent<PatrolMarkerTrigger>().droneScript = this.cdroneScript;
   }
   //cdroneScript.AIScript.ClickedTargetPosition(patrolTarget[0]);
   //patrolIndex = 0;

 }

  private void TrianglePatrol() {

   //Debug.Log("Set patrol:" + patrolTarget);
   patrolSet = false;
   setNewPatrol = true;
   patrolPointsValid = true;

   patrolTarget[0] = new Vector3(0.0f,0.0f,20.0f); patrolTarget[0] += MouseWorldPosition.targetPosition;
   patrolTarget[1] = new Vector3(20.0f,0.0f,-20.0f); patrolTarget[1] += MouseWorldPosition.targetPosition;
   patrolTarget[2] = new Vector3(-20.0f,0.0f,-20.0f); patrolTarget[2] += MouseWorldPosition.targetPosition;

   for(int x = 0; x < patrolTarget.Length; x++){
     patrolMarkers[x] = GameObject.Instantiate(markerPatrol,patrolTarget[x],Quaternion.identity) as GameObject;
      patrolMarkers[x].GetComponent<PatrolMarkerTrigger>().droneScript = this.cdroneScript;
   }
   //cdroneScript.AIScript.ClickedTargetPosition(patrolTarget[0]);
   //patrolIndex = 0;

 }

  private void SquarePatrol () {

  //Debug.Log("Set patrol:" + patrolTarget);
  patrolSet = false;
  setNewPatrol = true;
  patrolPointsValid = true;

  patrolTarget[0] = new Vector3(20.0f,0.0f,20.0f); patrolTarget[0] += MouseWorldPosition.targetPosition;
  patrolTarget[1] = new Vector3(20.0f,0.0f,-20.0f); patrolTarget[1] += MouseWorldPosition.targetPosition;
  patrolTarget[2] = new Vector3(-20.0f,0.0f,-20.0f); patrolTarget[2] += MouseWorldPosition.targetPosition;
  patrolTarget[3] = new Vector3(-20.0f,0.0f,20.0f); patrolTarget[3] += MouseWorldPosition.targetPosition;


    for(int x = 0; x < patrolTarget.Length; x++){
      patrolMarkers[x] = GameObject.Instantiate(markerPatrol,patrolTarget[x],Quaternion.identity) as GameObject;
      patrolMarkers[x].GetComponent<PatrolMarkerTrigger>().droneScript = this.cdroneScript;
    }

  }

	void PatrolSet () {

    if(patrolTarget.Length == 4){
      patrolTarget[0] = new Vector3(20.0f,0.0f,20.0f) + MouseWorldPosition.mouseHitPointNow;
      patrolTarget[1] = new Vector3(20.0f,0.0f,-20.0f) + MouseWorldPosition.mouseHitPointNow;
      patrolTarget[2] = new Vector3(-20.0f,0.0f,-20.0f) + MouseWorldPosition.mouseHitPointNow;
      patrolTarget[3] = new Vector3(-20.0f,0.0f,20.0f) + MouseWorldPosition.mouseHitPointNow;
    }else if(patrolTarget.Length == 3){
      patrolTarget[0] = new Vector3(0.0f,0.0f,20.0f); patrolTarget[0] += MouseWorldPosition.mouseHitPointNow;
      patrolTarget[1] = new Vector3(20.0f,0.0f,-20.0f); patrolTarget[1] += MouseWorldPosition.mouseHitPointNow;
      patrolTarget[2] = new Vector3(-20.0f,0.0f,-20.0f); patrolTarget[2] += MouseWorldPosition.mouseHitPointNow;
    }else if(patrolTarget.Length == 2){
      patrolTarget[0] = new Vector3(20.0f,0.0f,0.0f); patrolTarget[0] += MouseWorldPosition.mouseHitPointNow;
      patrolTarget[1] = new Vector3(-20.0f,0.0f,0.0f); patrolTarget[1] += MouseWorldPosition.mouseHitPointNow;
    }
		
		for(int x = 0; x < patrolTarget.Length; x++){
			patrolMarkers[x].transform.position = patrolTarget[x];	
		}
	}
	
	public void StartPatrol () {
		
		for(int x = 0; x < patrolTarget.Length; x++){
			GameObject.Destroy(patrolMarkers[x]);
		}
		
		if(setNewPatrol){
      float closestPoint = Mathf.Infinity;
      int startPoint = 0;

      for(int y = 0; y < patrolTarget.Length; y++){
        float fDistance = (myTransform.position - patrolTarget[y]).sqrMagnitude;

        if(fDistance < closestPoint){
          closestPoint = fDistance;
          startPoint = y;
        }

      }

			cdroneScript.AIScript.ClickedTargetPosition(patrolTarget[startPoint]);
			patrolIndex = startPoint;
			setNewPatrol = false;
		}
		
		if(Vector3.Distance(this.transform.position,patrolTarget[patrolIndex]) < 5.0f){
    //if(!AstarAIFollow.onAIMovingChange) {
			patrolIndex += 1;
			if(patrolIndex >= patrolTarget.Length) patrolIndex = 0;
			cdroneScript.AIScript.ClickedTargetPosition(patrolTarget[patrolIndex]);
    }
		//}
	}
  /*/
  void onAstarMovingChange(bool isMoving) {

   // AI gets to the end of the path and we were walking
   if(!isMoving && cdroneScript.GetCurrentState() == FSMState.STATE_WALKING) {


   }

 }
 // */

/*/
	void OnDrawGizmosSelected () {
		
		Vector3 pos = transform.position;
		
		Gizmos.DrawWireSphere(pos,16);	
	
	}
//*/
}
