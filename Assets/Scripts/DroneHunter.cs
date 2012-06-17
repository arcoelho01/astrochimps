using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Implements the behaviour of an AI controlled hunter drone.
/// From the Game Design Document:
/// Capture monkeys (highest priority)
/// Attack enemy saboteur drones (when they were revealed);
/// Avoid enemy saboteur drones
/// </summary>
public class DroneHunter : MonoBehaviour {

	int myEnemyLayer;
	Collider[] scannedColliders;
	CDrone cDroneScript;
	float viewRadius = 10.0f;
	float fCheckViewTimer = 1.0f;
	float fTimer = 0.0f;
	Transform currentTarget = null;
	bool enemyInRange = false;
	Vector3 v3LastSightedTarget;

	// List of monkeys in sight
	List<Transform> ltMonkeysInSight = new List<Transform>();
	// List of drones in sight
	List<Transform> ltDronesInSight = new List<Transform>();

	/*
	 * ===========================================================================================================
	 * UNITY'S STUFF
	 * ===========================================================================================================
	 */

	//
	void Awake() {

		// Get the CDrone script
		cDroneScript = this.GetComponent<CDrone>();

		// Who are our enemies?
		if(gameObject.layer == MainScript.enemyLayer)
			myEnemyLayer = MainScript.alliedLayer;
		else
			myEnemyLayer = MainScript.enemyLayer;

		scannedColliders = new Collider[0];
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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

	/*
	 * ===========================================================================================================
	 * HUNTER DRONE BEHAVIOUR
	 * ===========================================================================================================
	 */

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
		ltMonkeysInSight.Clear();
		ltDronesInSight.Clear();

		foreach(Collider candidateCollider in scannedColliders) {

			// Ok, let's check all the enemies in range
			Transform candidateTarget = candidateCollider.transform;

			if(candidateTarget.gameObject.tag == "Monkey") {

				// Wow, we got a monkey in sight!
				ltMonkeysInSight.Add(candidateTarget);
				continue;
			}

			if(candidateTarget.gameObject.tag == "Drone") {

				// An enemy drone
				ltDronesInSight.Add(candidateTarget);
				continue;
			}
		}

		// Ok, all enemies sorted. Now let's decide what to do
		// Let's check the monkeys
		Transform candidateToBeEngaged = PickClosestTargetFromTheList(ltMonkeysInSight);

		if(candidateToBeEngaged) {

			v3LastSightedTarget = candidateToBeEngaged.transform.position;
			return candidateToBeEngaged;
		}

		// No monkey in sight, so let's check the drones
		candidateToBeEngaged = PickClosestTargetFromTheList(ltDronesInSight);

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

	/// <summamy>
	/// Find the nearest prison for the hunter drop the captured monkey
	/// </summary>
	/// <returns> The transform of the nearest prison, or null if ain't find any </returns>
	public Transform GetNearestPrison() {

		Transform tNearestPrison = null;

		{
		
			// FIXME: this is temporary. Must find all buildings, filter which doesn't have a prisoner yet and then
			// select the nearest
			GameObject tempObject = GameObject.Find("CommandCenterOponent");	
			if(tempObject)
				tNearestPrison = tempObject.transform;
		}

		return tNearestPrison;

	}

	/// <summary>
	/// Executes the delivery of the captured monkey to the prison building
	/// </summary>
	public void DeliverPrisoner(Transform tPrisonBuilding, CBaseEntity entityPrisoner) {

		if(entityPrisoner.Type == CBaseEntity.eObjType.Monkey) {

			// Cast CBaseEntity to CMonkey (it is actually a CMonkey instance, anyway)
			CMonkey monkeyEntity = entityPrisoner as CMonkey;
			monkeyEntity.ReleaseMe();

			// ... and immediately the building captures it
			CBuilding tBuildingClass = tPrisonBuilding.GetComponent<CBuilding>();
			tBuildingClass.ReceivePrisoner(monkeyEntity);
		}
	}

	/// <summary>
	/// Returns the position where the drone last saw an enemy
	/// </summary>
	public Vector3 GetLastTargetSightedPosition() {

		return v3LastSightedTarget;
	}

	/*
	 * ===========================================================================================================
	 * GIZMOS AND HELPERS
	 * ===========================================================================================================
	 */

	/// <summary>
	/// Draw some helpers on screen
	/// </summary>
	void OnDrawGizmos() {

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, viewRadius);

		if(currentTarget)
			Gizmos.DrawLine(transform.position, currentTarget.transform.position);
	}
}
