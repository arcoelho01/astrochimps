using UnityEngine;
using System.Collections;

public class OpponentDroneFactory : MonoBehaviour {
	
	public GameObject commanderCenter;
	public GameObject drone;
	public int timeCreateDrone;
	public int metalCost;
	private float currentTime = 0;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		CreateDrone();
	}
	
	void CreateDrone()
	{
		GameObject newDrone;
		
		if(commanderCenter.GetComponent<OponentManager>().maxDrones > commanderCenter.GetComponent<OponentManager>().amountDrones)
		{
			if(ProgressCompleted()){
				commanderCenter.GetComponent<OponentManager>().amountDrones++;
				commanderCenter.GetComponent<OponentManager>().metalResource -= metalCost;
				newDrone = (GameObject)Instantiate(drone,new Vector3(transform.position.x+4,transform.position.y+1,transform.position.z+3),transform.rotation);
				currentTime = 0;
			}
		}
	}
	
	bool ProgressCompleted()
	{
		bool unlocked = false;
		
		if(currentTime < timeCreateDrone)
		{
			currentTime += 1 * Time.deltaTime; 
		}
		else
		{
			currentTime = timeCreateDrone;
			unlocked = true;
		}
		
		return unlocked;
	}
}
