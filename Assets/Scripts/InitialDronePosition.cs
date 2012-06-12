using UnityEngine;
using System.Collections;

public class InitialDronePosition : MonoBehaviour {
	
	public bool empty;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnCollisionStay(Collision collisionInfo) 
	{
		empty = true;
		
		foreach (ContactPoint contact in collisionInfo.contacts)
		{
			empty = false;	
		}
	}
}
