using UnityEngine;
using System.Collections;

public class GerenciadorSlots : MonoBehaviour {
	
	public GameObject[] canos;
	public GameObject[] slots;
	public GameObject[] regioes;
	
	private bool alteracaoBloqueada = false;
	
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
	
		HabilitarSlots();
			
	}
	
	void HabilitarSlots()
	{
		
		if(!alteracaoBloqueada)
		{
			if((slots[0].GetComponent<DefinicaoEstrutura>().tipo != DefinicaoEstrutura.TipoEstrutura.SLOT)&&
				(slots[1].GetComponent<DefinicaoEstrutura>().tipo != DefinicaoEstrutura.TipoEstrutura.SLOT)&&
				(slots[2].GetComponent<DefinicaoEstrutura>().tipo != DefinicaoEstrutura.TipoEstrutura.SLOT))
			{
				alteracaoBloqueada = true;
				for(int i = 3; i< 9; i++)
				{
					canos[i].active = true;
  					slots[i].active = true;
  					regioes[i].active = true;
				}
			}
		}
	}
}
