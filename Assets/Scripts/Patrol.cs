using UnityEngine;
using System.Collections;


enum eAlertLevel{PATROL,ALERT,DETECT};

public class Patrol : MonoBehaviour {

  private Transform myTransform;

  private Transform target;
  private Vector3 targetVector;
  
  private float pulseTime;
  
  //Check enemy around
  public bool enemyAround;
  private Collider[] scannedColliders;
  private int enemyMask = 1 << 11;

  private eAlertLevel status = eAlertLevel.PATROL;
  
  //Field of View
  private float detectionDistance = 14.0f;
  private float detectionRadius = 10.0f;
  private float alertRadius = 45.0f;

  public Vector3[] patrolTarget;
  public GameObject[] patrolMarkers;
	
  public bool setNewPatrol;
  public bool patrolSet;
  public int patrolIndex;
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
    scannedColliders = new Collider[0];
	patrolTarget = new Vector3[4];
	patrolMarkers = new GameObject[4];

  }

  // Use this for initialization
  void Start () {

    pulseTime = 1.0f;

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
      pulseTime = 1.0f;
    }

    if(enemyAround){

      // Fase de teste
      if(scannedColliders.Length > 1){
        float tempdistance = Vector3.Distance(myTransform.position,scannedColliders[0].transform.position);
        for(int i = 0; i < scannedColliders.Length; i ++ ){
          target = scannedColliders[i].transform;
          float tempdistanceindex = Vector3.Distance(myTransform.position,target.position);
          if(tempdistanceindex < tempdistance){
            tempdistance = tempdistanceindex;
            break;
          }else target = scannedColliders[0].transform;
        }

      }else target = scannedColliders[0].transform;

      targetVector = target.position - myTransform.position;


       if(Vector3.Distance(myTransform.position,target.position) < detectionDistance  && Vector3.Angle(targetVector,myTransform.forward) < detectionRadius){
          pulseTime = 5.0f;
          status = eAlertLevel.DETECT;
          existingAlert = GameObject.Instantiate(detectAlert,new Vector3(myTransform.position.x,myTransform.position.y + 15,myTransform.position.z), Quaternion.identity) as GameObject;
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
        pulseTime = 5.0f;
        status = eAlertLevel.DETECT;
        existingAlert = GameObject.Instantiate(detectAlert,new Vector3(myTransform.position.x,myTransform.position.y + 15,myTransform.position.z), Quaternion.identity) as GameObject;
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
		
	
	public void SquarePatrol () {
		
		Debug.Log("Set patrol:" + patrolTarget);
		
		setNewPatrol = true;
		
		patrolTarget[0] = new Vector3(20.0f,0.0f,20.0f); patrolTarget[0] += MouseWorldPosition.targetPosition;
		patrolTarget[1] = new Vector3(20.0f,0.0f,-20.0f); patrolTarget[1] += MouseWorldPosition.targetPosition;
		patrolTarget[2] = new Vector3(-20.0f,0.0f,-20.0f); patrolTarget[2] += MouseWorldPosition.targetPosition;
		patrolTarget[3] = new Vector3(-20.0f,0.0f,20.0f); patrolTarget[3] += MouseWorldPosition.targetPosition;
		
		for(int x = 0; x < patrolTarget.Length; x++){
			patrolMarkers[x] = GameObject.Instantiate(markerPatrol,patrolTarget[x],Quaternion.identity) as GameObject;	
		}
		//cdroneScript.AIScript.ClickedTargetPosition(patrolTarget[0]);
		//patrolIndex = 0;
		
	}
	
	void PatrolSet () {
		

		patrolTarget[0] = new Vector3(20.0f,0.0f,20.0f) + MouseWorldPosition.mouseHitPointNow;
		patrolTarget[1] = new Vector3(20.0f,0.0f,-20.0f) + MouseWorldPosition.mouseHitPointNow;
		patrolTarget[2] = new Vector3(-20.0f,0.0f,-20.0f) + MouseWorldPosition.mouseHitPointNow;
		patrolTarget[3] = new Vector3(-20.0f,0.0f,20.0f) + MouseWorldPosition.mouseHitPointNow;
		
		for(int x = 0; x < patrolTarget.Length; x++){
			patrolMarkers[x].transform.position = patrolTarget[x];	
		}
	}
	
	public void StartPatrol () {
		
		if(setNewPatrol){
			cdroneScript.AIScript.ClickedTargetPosition(patrolTarget[0]);
			patrolIndex = 0;
			setNewPatrol = false;
		}
		
		if(Vector3.Distance(this.transform.position,patrolTarget[patrolIndex]) < 2.0f){
			patrolIndex += 1;
			if(patrolIndex >= patrolTarget.Length) patrolIndex = 0;
			cdroneScript.AIScript.ClickedTargetPosition(patrolTarget[patrolIndex]);
		}
		
	}


/*/
	void OnDrawGizmosSelected () {
		
		Vector3 pos = transform.position;
		
		Gizmos.DrawWireSphere(pos,16);	
	
	}
//*/
}
