using UnityEngine;
using System.Collections;

public class DronePatrol : MonoBehaviour {

  public enum eAlertLevel{PATROL,ALERT,DETECT};

  private CDrone cDroneScript;

  private int myEnemyLayer; //The other side
  private Collider[] scannedColliders;

  private float viewRadius = 50.0f; // Larger because this is an initial check
  private float fCheckViewTimer = 1.6f; // So they dont all make checks at exaclty the same time
  private float fAlertViewTimer = 5.0f;
  private float fDetectViewTimer = 3.0f;
  private float fTimer = 0.0f;
  //private bool enemyInRange = false;

  Transform currentTarget = null;

  //Field of View
  private float detectionDistance = 14.0f;
  private float detectionRadius = 25.0f;
  private float alertRadius = 60.0f;

  //Field of View Alternate
  private SightTrigger mySightTrigger;

  //Patrol list variables
  public Vector3[] patrolTarget;
  public Vector3[] previousPatrolTarget;
  public GameObject[] patrolMarkers;

  //Patrol Variables
  public int patrolIndex;
  public bool bSetNewPatrol;
  public bool bStartMoving;
  public bool bPatrolPointsValid;
  public bool bGoToNextMarker = false;
  public GameObject markerPatrol;

  public eAlertLevel alertLevel = eAlertLevel.PATROL;


  public GameObject detectAlert; //Prefab to instantiate once enemy has been found, set in Inspector.
                                 //If its an enemy drone well just set an empty object to show last know position

  private GameObject existingAlert; // This is to avoid destroy problems

  public delegate void EnemyDiscoveryHandler(Transform eventRaiser, Transform target);
  public static event EnemyDiscoveryHandler OnEnemyFound;

  void Awake () {

    // Get the CDrone script
    cDroneScript = this.GetComponent<CDrone>();

    mySightTrigger = this.GetComponentInChildren<SightTrigger>();

    // Who are our enemies? //Thanks Alexandre =D
   if(gameObject.layer == MainScript.enemyLayer)
     myEnemyLayer = MainScript.alliedLayer;
   else{
     myEnemyLayer = MainScript.enemyLayer;

    }

    mySightTrigger.myEnemyLayer = myEnemyLayer;

    scannedColliders = new Collider[0];

  }


  // Use this for initialization
  void Start () {

    if(myEnemyLayer == MainScript.enemyLayer)
      AllyStart();
    else
      EnemyStart();

  }

  // Update is called once per frame
  void Update () {

    if(cDroneScript == null)
      Debug.LogError("My Cdrone script is empty go back, error");

    if(cDroneScript.isStunned())
      return;

    if(myEnemyLayer == MainScript.enemyLayer)
      AllyUpdate();
    else EnemyUpdate();

// Patrol status should be when theres nothing in colliders, Alert status when there is someone but not inside sight and Detect status once its detected
    if(alertLevel == eAlertLevel.PATROL)
      this.Patrol();
    else if(alertLevel == eAlertLevel.ALERT)
      this.Alert();
    else if(alertLevel == eAlertLevel.DETECT)
      this.Detect();
//*/

  }


  void Patrol(){
    //
    fTimer += Time.deltaTime;

    if(fTimer > fCheckViewTimer){

      scannedColliders = Physics.OverlapSphere(this.transform.position,viewRadius,1<<myEnemyLayer);
      if(scannedColliders.Length > 0)
        foreach(Collider colliderScanned in scannedColliders){
          if(colliderScanned.tag != "Building"){
            Debug.LogWarning("Alert Mode !");
            alertLevel = eAlertLevel.ALERT;
            break;
          }
        }
      fTimer = 0;
    }
    //*/
  }

  void Alert(){

    //
    fTimer += Time.deltaTime;

      //currentTarget = mySightTrigger.oppositeDroneSeen;

      if(mySightTrigger.oppositeDroneSeen != null){
        currentTarget = mySightTrigger.oppositeDroneSeen;
        //Debug.LogError("Bumped into some enemy drone");
        alertLevel = eAlertLevel.DETECT;
        if(existingAlert) GameObject.Destroy(existingAlert);
        //existingAlert = GameObject.Instantiate(detectAlert,new Vector3(currentTarget.position.x,currentTarget.position.y + 15, currentTarget.position.z),
        //                                      Quaternion.identity) as GameObject;
        if(OnEnemyFound != null)
          OnEnemyFound(this.transform,currentTarget);
      }

    if(fTimer > fAlertViewTimer){
      fTimer = 0;
      alertLevel = eAlertLevel.PATROL;
      if(currentTarget) currentTarget = null;
      if(existingAlert) GameObject.Destroy(existingAlert);
      Debug.LogWarning("Back to Patrol mode");
    }
    //*/
  }

  void Detect(){
    //
    fTimer += Time.deltaTime;

     if(!existingAlert)
       existingAlert = GameObject.Instantiate(detectAlert,new Vector3(currentTarget.position.x,currentTarget.position.y + 15, currentTarget.position.z),
                                              Quaternion.identity) as GameObject;

   if(mySightTrigger.oppositeDroneLost != null){

        Transform targetExit = mySightTrigger.oppositeDroneLost;
        if(targetExit == currentTarget){

          alertLevel = eAlertLevel.ALERT;
          if(OnEnemyFound != null)
            OnEnemyFound(this.transform,currentTarget);

        }else{

          currentTarget = targetExit;

          if(existingAlert) GameObject.Destroy(existingAlert);

          existingAlert = GameObject.Instantiate(detectAlert,new Vector3(currentTarget.position.x,currentTarget.position.y + 15, currentTarget.position.z),
                                                  Quaternion.identity) as GameObject;
          if(OnEnemyFound != null)
            OnEnemyFound(this.transform,currentTarget);
        
          alertLevel = eAlertLevel.ALERT;
        }
      }

    //*/
  }


  public bool AllyStart () {

    //Flag for setting new patrol route
    bSetNewPatrol = false;
    //Flag for start walking patrol route
    bStartMoving = false;
    //Flag that checks if patrol points are in valid positions
    bPatrolPointsValid = false;

    return true;

  }


  public bool EnemyStart () {

    //Flag for setting new patrol route
    bSetNewPatrol = false;
    //Flag for start walking patrol route
    bStartMoving = true;
    //Flag that checks if patrol points are in valid positions
    bPatrolPointsValid = false;

    int nRandomPatrol = Random.Range(2,5);

    this.ChoicePatrol(nRandomPatrol);

    patrolIndex = GetClosetsPatrolMarker();

    cDroneScript.AIScript.ClickedTargetPosition(patrolTarget[patrolIndex]);

    //cDroneScript.Patrolling();

    return true;

  }

  void AllyUpdate () {
    //This one has to go first

    if(bStartMoving)
      StartPatrol();


    if(bSetNewPatrol)
      PatrolSet();


  }

  void EnemyUpdate () {

    if(bStartMoving)
      StartPatrol();

  }

  public void ChoicePatrol(int x){

    previousPatrolTarget = patrolTarget;

    patrolTarget = new Vector3[x];
    patrolMarkers = new GameObject[x];

    bSetNewPatrol = true;
    bPatrolPointsValid = true;

    if(myEnemyLayer == MainScript.enemyLayer){
      bStartMoving = false;
      //cDroneScript.Waiting();
    }

    if(x == 4)
      SquarePatrol();
    else if (x == 3)
           TrianglePatrol();
         else LinePatrol();

    for(int n = 0; n < patrolTarget.Length; n++){
      patrolMarkers[n] = GameObject.Instantiate(markerPatrol,patrolTarget[n],Quaternion.identity) as GameObject;
      patrolMarkers[n].GetComponent<PatrolMarkerTrigger>().droneScript = this.cDroneScript;
    }

   //if(myEnemyLayer == MainScript.alliedLayer) cDroneScript.Patrolling();

  }

  public void StartPatrol () {

    if(bSetNewPatrol){
      for(int x = 0; x < patrolMarkers.Length; x++){
        GameObject.Destroy(patrolMarkers[x]);
      }

      bSetNewPatrol = false;
      cDroneScript.AIScript.ClickedTargetPosition(patrolTarget[patrolIndex]);
      //if(myEnemyLayer == MainScript.enemyLayer) cDroneScript.Patrolling();
    }

    if(bGoToNextMarker){
      patrolIndex += 1;
      if(patrolIndex >= patrolTarget.Length) patrolIndex = 0;
        cDroneScript.AIScript.ClickedTargetPosition(patrolTarget[patrolIndex]);
        bGoToNextMarker = false;
      }
    }

  private void LinePatrol() {

   patrolTarget[0] = new Vector3(20.0f,0.0f,0.0f) + this.transform.position;
   patrolTarget[1] = new Vector3(-20.0f,0.0f,0.0f) + this.transform.position;

 }

  private void TrianglePatrol() {

   patrolTarget[0] = new Vector3(0.0f,0.0f,20.0f) + this.transform.position;
   patrolTarget[1] = new Vector3(20.0f,0.0f,-20.0f) + this.transform.position;
   patrolTarget[2] = new Vector3(-20.0f,0.0f,-20.0f) + this.transform.position;

 }

  private void SquarePatrol () {

  patrolTarget[0] = new Vector3(20.0f,0.0f,20.0f) + this.transform.position;
  patrolTarget[1] = new Vector3(20.0f,0.0f,-20.0f) + this.transform.position;
  patrolTarget[2] = new Vector3(-20.0f,0.0f,-20.0f) + this.transform.position;
  patrolTarget[3] = new Vector3(-20.0f,0.0f,20.0f) + this.transform.position;

  }

  int GetClosetsPatrolMarker () {

      float closestPoint = Mathf.Infinity;
      int startPoint = 0;

      for(int y = 0; y < patrolTarget.Length; y++){
        float fDistance = (this.transform.position - patrolTarget[y]).sqrMagnitude;

        if(fDistance < closestPoint){
          closestPoint = fDistance;
          startPoint = y;
        }

      }

    return startPoint;

  }

  //Player Drone
    void PatrolSet () {

    if(patrolTarget.Length == 4){
      patrolTarget[0] = new Vector3(20.0f,0.0f,20.0f) + MouseWorldPosition.mouseHitPointNow;
      patrolTarget[1] = new Vector3(20.0f,0.0f,-20.0f) + MouseWorldPosition.mouseHitPointNow;
      patrolTarget[2] = new Vector3(-20.0f,0.0f,-20.0f) + MouseWorldPosition.mouseHitPointNow;
      patrolTarget[3] = new Vector3(-20.0f,0.0f,20.0f) + MouseWorldPosition.mouseHitPointNow;
    }else if(patrolTarget.Length == 3){
      patrolTarget[0] = new Vector3(0.0f,0.0f,20.0f) + MouseWorldPosition.mouseHitPointNow;
      patrolTarget[1] = new Vector3(20.0f,0.0f,-20.0f) + MouseWorldPosition.mouseHitPointNow;
      patrolTarget[2] = new Vector3(-20.0f,0.0f,-20.0f) + MouseWorldPosition.mouseHitPointNow;
    }else if(patrolTarget.Length == 2){
      patrolTarget[0] = new Vector3(20.0f,0.0f,0.0f) + MouseWorldPosition.mouseHitPointNow;
      patrolTarget[1] = new Vector3(-20.0f,0.0f,0.0f) + MouseWorldPosition.mouseHitPointNow;
    }
   for(int x = 0; x < patrolTarget.Length; x++){
     patrolMarkers[x].transform.position = patrolTarget[x];
   }
  }

  public void RevertPatrol(){

    if(previousPatrolTarget.Length > 0){
      patrolTarget = previousPatrolTarget;
      //Debug.LogWarning("previousPatrolTarget: " +previousPatrolTarget[0]);
      bStartMoving = true;
      bSetNewPatrol = false;
    }else{
      bStartMoving = false;
      bSetNewPatrol = false;
      bPatrolPointsValid = false;
      for(int x = 0; x < patrolTarget.Length; x++){
        GameObject.Destroy(patrolMarkers[x]);
      }
    }
  }

  void OnDrawGizmos() {

   Gizmos.color = Color.green;
   Gizmos.DrawWireSphere(transform.position, viewRadius);

 }

}
