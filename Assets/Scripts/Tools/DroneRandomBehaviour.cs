using UnityEngine;
using System.Collections;

public class DroneRandomBehaviour : MonoBehaviour {

	private AstarAIFollow AIScript = null;
	public float changeTargetTimer = 4.0f;
	float currentTimer = 0.0f;
	
	//
	void Awake() {

		AIScript = GetComponent<AstarAIFollow>();

		if(AIScript == null) {

			// DEBUG
			Debug.LogError("Couldn't find AIScript for this object: " + name);
		}
	}

	// Use this for initialization
	void Start () {
	
		currentTimer = 0.0f;
		PickNewTarget();
	}
	
	// Update is called once per frame
	void Update () {
		
		if (!gameObject.GetComponent<CDrone>().isStunned())
			currentTimer += Time.deltaTime;

		if(currentTimer > changeTargetTimer) {

			currentTimer = 0.0f;
			PickNewTarget();
		}
	}

	void PickNewTarget() {

		Vector3 newPosition = new Vector3(Random.Range(-10.0F, 10.0F),0, Random.Range(-10.0F, 10.0F));
		AIScript.ClickedTargetPosition(transform.position + newPosition);	
	}
}
