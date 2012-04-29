using UnityEngine;
using System.Collections;

public class EventosMenu : MonoBehaviour {
	
	public GUISkin menuSkin;
	public float altura;
	public float largura;
	public GameObject prefabFazenda;
	public GameObject prefabLaboratorio;
	public GameObject prefabGaragem;
	public GameObject prefabFabricaDrones;
	public Transform objetoSelecionado;
   	public DefinicaoEstrutura.TipoEstrutura tipoObj;
	
	private bool slotsPosicionados = false;
	private float oxigenio;
	private float metal;
	CPlayer playerScript;

 /* private int i_Inicial = 0;
  private int i_Final = 3;*/

  private int quantidadeConstrucoes = 0;

	// Use this for initialization
	void Start () {
		
		playerScript = GameObject.Find("Player").GetComponent<CPlayer>();
		if(!playerScript) {

			// DEBUG
			Debug.LogError("Player script not found.");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI(){
		GUI.skin = menuSkin;
		
		float screenX = Screen.width * 0.5f - largura * 0.5f;
		float screenY = Screen.height * 0.5f - altura * 0.5f;
		
		//GUI.Box(new Rect(0,Screen.height - 100,Screen.width,100),"Menu");

		
		if((tipoObj == DefinicaoEstrutura.TipoEstrutura.SLOT) && (!objetoSelecionado.GetComponent<DefinicaoEstrutura>().construido))
		{
			if(GUI.Button(new Rect(5 * 25,Screen.height - 44,70 * 1.15f,70 * 0.53f),"Fazenda")){
				Construir(DefinicaoEstrutura.TipoEstrutura.FAZENDA);
				objetoSelecionado.GetComponent<DefinicaoEstrutura>().construido = true;
			}
			
			if(GUI.Button(new Rect(80* 2.65f,Screen.height - 44,70 * 1.15f,70 * 0.53f),"Garagem")){
				Construir(DefinicaoEstrutura.TipoEstrutura.GARAGEM);
				objetoSelecionado.GetComponent<DefinicaoEstrutura>().construido = true;
			}
			
			if(GUI.Button(new Rect(155* 2,Screen.height -44,70 * 1.25f,70 * 0.53f),"Laboratorio")){
				Construir(DefinicaoEstrutura.TipoEstrutura.LABORATORIO);
				objetoSelecionado.GetComponent<DefinicaoEstrutura>().construido = true;
			}	
			
			if(GUI.Button(new Rect(230 * 1.78f,Screen.height - 44,70 * 1.74f,70 * 0.53f),"Fabrica de Drones")){
				Construir(DefinicaoEstrutura.TipoEstrutura.FABRICA_DRONES);
				objetoSelecionado.GetComponent<DefinicaoEstrutura>().construido = true;
			}	
			
		}
		/*
		if((tipoObj == DefinicaoEstrutura.TipoEstrutura.CENTRO_COMANDO) && (!slotsPosicionados))
			if(GUI.Button(new Rect(5* 25,Screen.height - 44,70 * 1.3f,70 * 0.53f),"Expandir")){
				HabilitarSlot();
			}
			*/
		if(tipoObj == DefinicaoEstrutura.TipoEstrutura.FABRICA_DRONES)
			if(GUI.Button(new Rect(5,Screen.height,70,70),"Drone")){
				
			}		
	}
	
	void Construir(DefinicaoEstrutura.TipoEstrutura tipoConstrucao)
	{
		GameObject novaConstrucao;
		oxigenio = playerScript.oxygenLevel;
		metal = playerScript.metalLevel;
		
		// Check Cost
		GameObject prefabIWantToBuild = CheckCostAndSelectMyPrefab(tipoConstrucao);

		if(!prefabIWantToBuild) {

			// DEBUG
			Debug.Log("Not enough resources to build this.");
		}
		else {
				
			novaConstrucao = (GameObject)Instantiate(prefabIWantToBuild,
				                                         new Vector3(objetoSelecionado.position.x,objetoSelecionado.position.y + 0.7f,
				                                                     objetoSelecionado.position.z),objetoSelecionado.rotation);
			// Pay for it!
			playerScript.SubResourceMetal(prefabIWantToBuild.GetComponent<DefinicaoEstrutura>().custoMetal);
			playerScript.SubResourceOxygen(prefabIWantToBuild.GetComponent<DefinicaoEstrutura>().custoOxigenio);
      quantidadeConstrucoes++;
      if(quantidadeConstrucoes == 3)
        HabilitarSlot();
		}

		/*
		if((objetoSelecionado.GetComponent<DefinicaoEstrutura>().custoOxigenio <= oxigenio)
			&& (objetoSelecionado.GetComponent<DefinicaoEstrutura>().custoMetal <= metal))
		{
			if(tipoConstrucao == DefinicaoEstrutura.TipoEstrutura.FAZENDA)
				novaConstrucao = (GameObject)Instantiate(prefabFazenda,
				                                         new Vector3(objetoSelecionado.position.x,objetoSelecionado.position.y + 0.7f,
				                                                     objetoSelecionado.position.z),objetoSelecionado.rotation);
	
			if(tipoConstrucao == DefinicaoEstrutura.TipoEstrutura.GARAGEM)
				novaConstrucao = (GameObject)Instantiate(prefabGaragem,
				                                         new Vector3(objetoSelecionado.position.x,objetoSelecionado.position.y + 0.7f,
				                                                     objetoSelecionado.position.z),objetoSelecionado.rotation);
			
			if(tipoConstrucao == DefinicaoEstrutura.TipoEstrutura.LABORATORIO)
				novaConstrucao = (GameObject)Instantiate(prefabLaboratorio,
				                                         new Vector3(objetoSelecionado.position.x,objetoSelecionado.position.y + 0.7f,
				                                                     objetoSelecionado.position.z),objetoSelecionado.rotation);
			
			if(tipoConstrucao == DefinicaoEstrutura.TipoEstrutura.FABRICA_DRONES)
				novaConstrucao = (GameObject)Instantiate(prefabFabricaDrones,
				                                         new Vector3(objetoSelecionado.position.x,objetoSelecionado.position.y + 0.7f,
				                                                     objetoSelecionado.position.z),objetoSelecionado.rotation);
		
			GameObject.Find("Player").GetComponent<CPlayer>().SubResourceMetal(objetoSelecionado.GetComponent<DefinicaoEstrutura>().custoMetal);
			GameObject.Find("Player").GetComponent<CPlayer>().SubResourceOxygen(objetoSelecionado.GetComponent<DefinicaoEstrutura>().custoOxigenio);		
		}
		else
		{
			// FIXME: GUI stuff will only show in the OnGUI() or methods called by it
			//GUI.TextArea(new Rect(310,Screen.height - 75,70,70),"Faltam Recursos!");
		}
		//*/		
	}
	
	void HabilitarSlot()
	{
		for(int i = 3; i < 9; i++){


			GameObject.Find("CentroComando").GetComponent<GerenciadorSlots>().canos[i].active = true;
			GameObject.Find("CentroComando").GetComponent<GerenciadorSlots>().slots[i].active = true;
			GameObject.Find("CentroComando").GetComponent<GerenciadorSlots>().regioes[i].active = true;

			//Debug.Log(objetoSelecionado.GetComponent<GerenciadorSlots>().canos[i].name+" "+i+objetoSelecionado.GetComponent<GerenciadorSlots>().canos[i].active);
		}
		/*if(i_Final == 9)
			slotsPosicionados = true;
			playerScript.SubResourceMetal(objetoSelecionado.GetComponent<DefinicaoEstrutura>().custoMetal);
			playerScript.SubResourceOxygen(objetoSelecionado.GetComponent<DefinicaoEstrutura>().custoOxigenio);*/

	}


	/// <summary>
	/// Select the prefab accordingly to the type of building we want to build. Also, check if we have enough
	/// resources to build it.
	/// </summary>
	/// <param name="buildingType"> The building type, as defined in DefinicaoEstrutura.TipoEstrutura </param>
	/// <returns> 
	/// The game object of the selected prefab, or null in case we don't have enough resources to build it
	/// </returns>
	GameObject CheckCostAndSelectMyPrefab(DefinicaoEstrutura.TipoEstrutura buildingType) {

		GameObject rv = null;

		switch(buildingType) {

			case DefinicaoEstrutura.TipoEstrutura.FAZENDA:
				rv = prefabFazenda;
				break;
			case DefinicaoEstrutura.TipoEstrutura.GARAGEM:
				rv = prefabGaragem;
				break;
			case DefinicaoEstrutura.TipoEstrutura.LABORATORIO:
				rv = prefabLaboratorio;
				break;
			case DefinicaoEstrutura.TipoEstrutura.FABRICA_DRONES:
				rv = prefabFabricaDrones;
				break;
		}

		oxigenio = playerScript.oxygenLevel;
		metal = playerScript.metalLevel;

		// Ok, prefabs selected. Now we check the costs
		if((rv.GetComponent<DefinicaoEstrutura>().custoOxigenio > oxigenio)
			|| (rv.GetComponent<DefinicaoEstrutura>().custoMetal > metal)) {

			rv = null;
		}

		return rv;
	}	
}
