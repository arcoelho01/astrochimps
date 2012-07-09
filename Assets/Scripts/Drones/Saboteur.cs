using UnityEngine;
using System.Collections;
using System.Collections.Generic;

enum eActionState {STANDBY,WORKING};
public enum eSaboteurBehaviour {DEFENSIVE, OFFENSIVE};

public class Saboteur : MonoBehaviour {

  public Transform myTransform; //keep a refernce to this.transform

  private CDrone cDroneScript;

  public bool amIVisible;

  public float invisibleTimeCounter;

  public eSaboteurBehaviour AIBehaviour; //only for computer drones

  int myEnemyLayer;
  Collider[] scannedColliders;
  float viewRadius = 10.0f;
  float fCheckViewTimer = 1.5f;
  float fTimer = 0.0f;
  Transform currentTarget = null;
  bool enemyInRange = false;
  Vector3 v3LastSightedTarget;

  // List of drones in sight
  List<Transform> ltDronesInSight = new List<Transform>();



  void Awake () {

    // Get the CDrone script
    cDroneScript = this.GetComponent<CDrone>();

    amIVisible = false;
    invisibleTimeCounter = 5.0f;

    myTransform = this.transform;

    // Who are our enemies? //Thanks Alexandre =D
   if(gameObject.layer == MainScript.enemyLayer)
     myEnemyLayer = MainScript.alliedLayer;
   else{
     myEnemyLayer = MainScript.enemyLayer;

    }

    scannedColliders = new Collider[0];

  }

	// Use this for initialization
	void Start () {

  if(cDroneScript == null)
      Debug.LogError("My script is empty stop");
/*/ Starts are empty for now
  if(myEnemyLayer == MainScript.enemyLayer)
      AllyStart();
    else
      EnemyStart();
//*/
	}

  void AllyStart () {

    Debug.Log("Just testing if im in the right start, saboteur");

  }

  void EnemyStart () {

    Debug.LogWarning("Just testing if im in the right start, saboteur enemy");

  }
	
	// Update is called once per frame
	void Update () {

    if(myEnemyLayer == MainScript.enemyLayer)
      AllyUpdate();
    else if(AIBehaviour == eSaboteurBehaviour.DEFENSIVE)
          DefensiveUpdate();
         else
          OffensiveUpdate();

	}

  void AllyUpdate () {

    //Debug.Log("Ally saboteur drone updating");

  }

  void DefensiveUpdate () {

    //Debug.Log("Defensive opponent saboteur drone updating");
    fTimer += Time.deltaTime;

   // Check for targets in the current range
   if(fTimer > fCheckViewTimer) {
   
     CheckForEnemiesInView(myEnemyLayer, viewRadius);
     fTimer = 0.0f;

     if(enemyInRange) {

       CheckRadiusForAgents();
     }
     else
       currentTarget = null;
   }

  }


  void OffensiveUpdate () {

    //Debug.Log("Offensive opponent saboteur drone updating");

  }

 /// <summary>
 /// Check for any enemies inside the field of view of this drone
 /// </summary>
 /// <param name="enemyLayer"> An int with the layer of the enemy. We will use this to filter other units that
 /// we are not interested </param>
 /// <param name="fRadius"> Radius of view </param>
 void CheckForEnemiesInView(int enemyLayer, float fRadius) {

   // Get all enemies in the radius
   scannedColliders = Physics.OverlapSphere(this.transform.position, fRadius, 1<<enemyLayer);

   if(scannedColliders.Length > 0) 
     enemyInRange = true;
   else
     enemyInRange = false;
 }

  /// <summary>
 /// This method is called when there an enemy in our view. So, we must decide what is best: engage with this 
 /// enemy, proceed with our current target, if any, etc.
 /// </summary>
 /// <returns> The transform of the selected target, or null if none </returns>
 public Transform CheckRadiusForAgents() {

   // First, check if there are any enemies in the radius
   CheckForEnemiesInView(myEnemyLayer, viewRadius);
   if(!enemyInRange)
     return null;

   // Clear all candidates lists
   ltDronesInSight.Clear();

   foreach(Collider candidateCollider in scannedColliders) {

     // Ok, let's check all the enemies in range
     Transform candidateTarget = candidateCollider.transform;

     if(candidateTarget.gameObject.tag == "Drone") {

       // An enemy drone
       ltDronesInSight.Add(candidateTarget);
       continue;
     }

    }

   Transform candidateToBeEngaged = PickClosestTargetFromTheList(ltDronesInSight);

   if(candidateToBeEngaged) {
   
     v3LastSightedTarget = candidateToBeEngaged.transform.position;
     return candidateToBeEngaged;
   }

   // No target?
   return null;
 }

  /// <summary>
 /// Receive a list of possibles targets and return which one is closest to this drone
 /// </summary>
 Transform PickClosestTargetFromTheList(List<Transform> ltCandidates) {

   if(ltCandidates.Count == 0)
     return null;

   if(ltCandidates.Count == 1)
     return ltCandidates[0];

   float fCloserDistance = Mathf.Infinity;
   Transform tCloserTarget = null;

   // And we have more than one in sight! Let's choose the best to engage
   foreach(Transform tempTarget in ltCandidates) {

     float fDistance = (this.transform.position - tempTarget.transform.position).sqrMagnitude;

     // DEBUG
     //Debug.Log("Checking target " + tempTarget.transform + " distance (squared)" + fDistance );

     if(fDistance < fCloserDistance) {

       fCloserDistance = fDistance;
       tCloserTarget = tempTarget;
     } 
   }

   return tCloserTarget;
 }

  public void PatrolSawMe () {

    amIVisible = true;

  }


  public void SabotageBuilding (GameObject goTarget) {
		
 	if(cDroneScript.isStunned())
      return;
		
    CBuilding targetBuilding = goTarget.GetComponent<CBuilding>();

    if(!targetBuilding.sabotado)
      targetBuilding.Sabotage();
    else Debug.Log("Ja sabotado");
  }

  //This is only for debug and for enemies but no problem being in the allies too for now
  void OnDrawGizmos() {

   Gizmos.color = Color.red;
   Gizmos.DrawWireSphere(transform.position, viewRadius);

   if(currentTarget)
     Gizmos.DrawLine(transform.position, currentTarget.transform.position);
 }

}
