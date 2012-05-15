using UnityEngine;
using System.Collections;


public class Regiao : MonoBehaviour {
	
	public int numeroSetor;
	public bool conectado;
	public bool expansivel;
	
	public GameObject[] setor;
	//private int tamanho;

	
	// Use this for initialization
	void Start () 
	{
		LocalizaEstruturas();
	}
	
	// Update is called once per frame
	void Update () {

		bool problema = false;
		int setorProblema;
		
		if(GameObject.Find("CanoCentral"+numeroSetor) != null)
		{
			GameObject goCheckRenderer = null;

			for(int i = 0; i <= 1;i++)
			{
				
				// Check the type of renderer
				Transform mesh = setor[i].transform.FindChild("Mesh");
				if(mesh) {

					goCheckRenderer = mesh.gameObject;
				}
				else
					goCheckRenderer = setor[i];

				if(goCheckRenderer.renderer.active)
				{
					
					if(setor[i].GetComponent<DefinicaoEstrutura>().tipo == DefinicaoEstrutura.TipoEstrutura.CANO_CENTRAL)
					{
	
						if(setor[i].GetComponent<DefinicaoEstrutura>().vida <= 0)
							problema = true;
						
						if(setor[i].GetComponent<DefinicaoEstrutura>().sabotado)
							problema = true;
						
					}
					
					if(setor[i].GetComponent<DefinicaoEstrutura>().tipo == DefinicaoEstrutura.TipoEstrutura.SLOT)
					{
	
						if(setor[i].GetComponent<DefinicaoEstrutura>().sabotado)
							problema = true;
						
					}
					
				}
	
				if(problema)
					conectado = false;
				else
					conectado = true;			
			}
		}
	}
	
	public void LocalizaEstruturas()
	{
		if(GameObject.Find("CanoCentral"+numeroSetor) != null)
		{		
			setor[0] = GameObject.Find("CanoCentral"+numeroSetor);
			setor[1] = GameObject.Find("Slot"+numeroSetor);
		}		
	}
}
