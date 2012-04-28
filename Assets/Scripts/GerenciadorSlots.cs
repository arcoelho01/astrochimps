using UnityEngine;
using System.Collections;

public class GerenciadorSlots : MonoBehaviour {
	
	public GameObject[] canos;
	public GameObject[] slots;
	public GameObject[] regioes;
	
	// Use this for initialization
	void Start () {
		
		for(int i = 0; i < 9; i++)
		{
			canos[i] = GameObject.Find("CanoCentral"+(i+1));
			slots[i] = GameObject.Find("Slot"+(i+1));
			regioes[i] = GameObject.Find("Regiao"+(i+1));
      if(i >= 3){
  			canos[i].active = false;
  			slots[i].active = false;
  			regioes[i].active = false;
      }
		}
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
