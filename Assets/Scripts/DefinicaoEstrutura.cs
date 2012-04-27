using UnityEngine;
using System.Collections;

public class DefinicaoEstrutura : MonoBehaviour {
	
	public enum TipoEstrutura{
		CANO_CENTRAL,
		CANO_EXTRATOR,
		CENTRO_COMANDO,
		FAZENDA,
		GARAGEM,
		CENTRAL_SEGURANCA,
		FABRICA_DRONES,
		SLOT,
		EXTRATOR,
		LABORATORIO,
		PLATAFORMA_LANCAMENTO
	};
	public bool sabotado;
	public bool conectado;
	public bool construido;
	public int vida;
	public TipoEstrutura tipo;
	public float custoOxigenio;
	public float custoMetal;
	
	// Use this for initialization
	void Start () {
		if((tipo ==TipoEstrutura.CANO_CENTRAL) || (tipo == TipoEstrutura.SLOT))
		{
			sabotado = false;
			conectado = true;
			construido = false;
			vida = 100;
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
