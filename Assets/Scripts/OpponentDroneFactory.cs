using UnityEngine;
using System.Collections;

public class OpponentDroneFactory : MonoBehaviour {
	
	public Transform commanderCenter;
	public GameObject droneHunter;
	public GameObject dronePatrol;
	public GameObject droneSaboteur;
	public int timeCreateDrone;
	public int metalCost;
	private float currentTime = 0;
	private enum kindDrone{
		DRONE_HUNTER,
		DRONE_PATROL,
		DRONE_SABOTEUR
	};
	private kindDrone typeDrone;
	
	// Use this for initialization
	void Start () {
		typeDrone = kindDrone.DRONE_HUNTER;
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log(commanderCenter.name);
		Debug.Log(commanderCenter.GetComponent<OponentManager>().maxDroneHunter);
		Debug.Log(commanderCenter.GetComponent<OponentManager>().amountDroneHunter);
		CreateDroneHunter();
		CreateDronePatrol();
		CreateDroneSaboteur();
	}
	
	void CreateDroneHunter()
	{
		GameObject newDrone;
		if(typeDrone == kindDrone.DRONE_HUNTER)
		{
			if(commanderCenter.GetComponent<OponentManager>().maxDroneHunter > commanderCenter.GetComponent<OponentManager>().amountDroneHunter)
			{
				if(ProgressCompleted()){
					commanderCenter.GetComponent<OponentManager>().amountDroneHunter++;
					commanderCenter.GetComponent<OponentManager>().metalResource -= metalCost;
					newDrone = (GameObject)Instantiate(droneHunter,new Vector3(transform.position.x+4,transform.position.y+2,transform.position.z+3),transform.rotation);
					currentTime = 0;
					typeDrone = kindDrone.DRONE_PATROL;
				}
			}
			else
			{
				typeDrone = kindDrone.DRONE_PATROL;
			}
		}	
	}

	void CreateDronePatrol()
	{
		GameObject newDrone;
		
		if(typeDrone == kindDrone.DRONE_PATROL)
		{	
			if(commanderCenter.GetComponent<OponentManager>().maxDroneHunter > commanderCenter.GetComponent<OponentManager>().amountDroneHunter)
			{
				if(ProgressCompleted()){
					commanderCenter.GetComponent<OponentManager>().amountDronePatrol++;
					commanderCenter.GetComponent<OponentManager>().metalResource -= metalCost;
					newDrone = (GameObject)Instantiate(dronePatrol,new Vector3(transform.position.x+4,transform.position.y+2,transform.position.z+3),transform.rotation);
					currentTime = 0;
					typeDrone = kindDrone.DRONE_SABOTEUR;
				}
			}
			else
			{
				typeDrone = kindDrone.DRONE_SABOTEUR;
			}
		}
	}
	
	void CreateDroneSaboteur()
	{
		GameObject newDrone;
		
		if(typeDrone == kindDrone.DRONE_SABOTEUR)
		{	
			if(commanderCenter.GetComponent<OponentManager>().maxDroneHunter > commanderCenter.GetComponent<OponentManager>().amountDroneHunter)
			{
				if(ProgressCompleted()){
					commanderCenter.GetComponent<OponentManager>().amountDroneSaboteur++;
					commanderCenter.GetComponent<OponentManager>().metalResource -= metalCost;
					newDrone = (GameObject)Instantiate(dronePatrol,new Vector3(transform.position.x+4,transform.position.y+2,transform.position.z+3),transform.rotation);
					currentTime = 0;
					typeDrone = kindDrone.DRONE_HUNTER;
				}
			}
			else
			{
				typeDrone = kindDrone.DRONE_HUNTER;
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
