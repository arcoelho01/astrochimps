using UnityEngine;
using System.Collections;

public class EventosMouse : MonoBehaviour {
	
	//public DefinicaoEstrutura.TipoEstrutura tipoObjeto;
	//public Transform Objeto;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Input.GetMouseButtonDown(0))
			TipoObjetoSelecionado();
	}
	
	void TipoObjetoSelecionado()
	{
		
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		if(Physics.Raycast(ray,out hit))
		{
			if(hit.transform.GetComponent<DefinicaoEstrutura>() != null)
			{
				transform.GetComponent<EventosMenu>().tipoObj = hit.transform.GetComponent<DefinicaoEstrutura>().tipo;
				transform.GetComponent<EventosMenu>().objetoSelecionado = hit.transform;
			}
		}
	}
}
