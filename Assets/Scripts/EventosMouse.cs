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
	
		// Do the raycast, but ignore the minimap's layers	
		if(Physics.Raycast(ray,out hit, Mathf.Infinity, 
					~(1 << MainScript.minimapLayer | 1 << MainScript.minimapGroundLayer)))
		{
			Debug.Log("hit em " + hit.transform);

			// If the mesh collider is a child object...
			if(hit.transform.parent.transform.GetComponent<DefinicaoEstrutura>() != null) {

				transform.GetComponent<EventosMenu>().tipoObj = hit.transform.parent.transform.GetComponent<DefinicaoEstrutura>().tipo;
				transform.GetComponent<EventosMenu>().objetoSelecionado = hit.transform.parent.transform;
			}
			else if(hit.transform.GetComponent<DefinicaoEstrutura>() != null)
			{
				transform.GetComponent<EventosMenu>().tipoObj = hit.transform.GetComponent<DefinicaoEstrutura>().tipo;
				transform.GetComponent<EventosMenu>().objetoSelecionado = hit.transform;
			}
		}
	}
}
