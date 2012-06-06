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

				CheckForBestCourseOfAction();
			}
			else
				currentTarget = null;

			if(currentTarget) {

				// DEBUG
				Debug.Log("Current target is: " + currentTarget.transform);
			}
			else {

				// DEBUG
				Debug.Log("No current target");
			}
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
	void CheckForBestCourseOfAction() {

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

			// Let's not attack the same target if we already attacking it
			if(candidateToBeEngaged != currentTarget) {

				// Well, the candidate monkey is the new target!!!
				currentTarget = candidateToBeEngaged;
			}
			return;
		}

		candidateToBeEngaged = PickClosestTargetFromTheList(ltDronesInSight);
		// Let's not attack the same target if we already attacking it
		if(candidateToBeEngaged != currentTarget) {

			// Well, the candidate monkey is the new target!!!
			currentTarget = candidateToBeEngaged;
		}
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
			Debug.Log("Checking target " + tempTarget.transform + " distance (squared)" + fDistance );

			if(fDistance < fCloserDistance) {

				fCloserDistance = fDistance;
				tCloserTarget = tempTarget;
			}	
		}

		return tCloserTarget;
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
