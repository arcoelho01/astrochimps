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
	public GameObject prefabUnidadeDrone;
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

		//Eventos de Menu dos Slots
		if(tipoObj == DefinicaoEstrutura.TipoEstrutura.SLOT)
		{
			if(objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao == DefinicaoEstrutura.StatusProgresso.LIBERADO)
			{
				if(GUI.Button(new Rect(5 * 25,Screen.height - 44,70 * 1.15f,70 * 0.53f),"Fazenda")){
					
					if((prefabFazenda.GetComponent<DefinicaoEstrutura>().custoMetal <= GameObject.Find("Player").GetComponent<CPlayer>().metalLevel)&&
						(prefabFazenda.GetComponent<DefinicaoEstrutura>().custoOxigenio <= GameObject.Find("Player").GetComponent<CPlayer>().oxygenLevel))
					{
						GameObject.Find("Player").GetComponent<CPlayer>().SubResourceMetal(prefabFazenda.GetComponent<DefinicaoEstrutura>().custoMetal);
						GameObject.Find("Player").GetComponent<CPlayer>().SubResourceOxygen(prefabFazenda.GetComponent<DefinicaoEstrutura>().custoOxigenio);
						
						objetoSelecionado.GetComponent<DefinicaoEstrutura>().objetoAConstruir = DefinicaoEstrutura.TipoEstrutura.FAZENDA;
						objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao = DefinicaoEstrutura.StatusProgresso.EM_PROGRESSO;
						objetoSelecionado.GetComponent<DefinicaoEstrutura>().tempoConstrucao = prefabFazenda.GetComponent<DefinicaoEstrutura>().tempoConstrucao;
					}
					//Construir(DefinicaoEstrutura.TipoEstrutura.FAZENDA);		
					//objetoSelecionado.GetComponent<DefinicaoEstrutura>().construido = true;
				}
				
				if(GUI.Button(new Rect(80* 2.65f,Screen.height - 44,70 * 1.15f,70 * 0.53f),"Garagem")){
					
					if((prefabGaragem.GetComponent<DefinicaoEstrutura>().custoMetal <= GameObject.Find("Player").GetComponent<CPlayer>().metalLevel)&&
						(prefabGaragem.GetComponent<DefinicaoEstrutura>().custoOxigenio <= GameObject.Find("Player").GetComponent<CPlayer>().oxygenLevel))
					{						
						GameObject.Find("Player").GetComponent<CPlayer>().SubResourceMetal(prefabGaragem.GetComponent<DefinicaoEstrutura>().custoMetal);
						GameObject.Find("Player").GetComponent<CPlayer>().SubResourceOxygen(prefabGaragem.GetComponent<DefinicaoEstrutura>().custoOxigenio);
						
						objetoSelecionado.GetComponent<DefinicaoEstrutura>().objetoAConstruir = DefinicaoEstrutura.TipoEstrutura.GARAGEM;
						objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao = DefinicaoEstrutura.StatusProgresso.EM_PROGRESSO;
						objetoSelecionado.GetComponent<DefinicaoEstrutura>().tempoConstrucao = prefabGaragem.GetComponent<DefinicaoEstrutura>().tempoConstrucao;
					}
					//Construir(DefinicaoEstrutura.TipoEstrutura.GARAGEM);
					//objetoSelecionado.GetComponent<DefinicaoEstrutura>().construido = true;
				}
				
				if(GUI.Button(new Rect(155* 2,Screen.height -44,70 * 1.25f,70 * 0.53f),"Laboratorio")){
					
					if((prefabLaboratorio.GetComponent<DefinicaoEstrutura>().custoMetal <= GameObject.Find("Player").GetComponent<CPlayer>().metalLevel)&&
						(prefabLaboratorio.GetComponent<DefinicaoEstrutura>().custoOxigenio <= GameObject.Find("Player").GetComponent<CPlayer>().oxygenLevel))
					{
						GameObject.Find("Player").GetComponent<CPlayer>().SubResourceMetal(prefabLaboratorio.GetComponent<DefinicaoEstrutura>().custoMetal);
						GameObject.Find("Player").GetComponent<CPlayer>().SubResourceOxygen(prefabLaboratorio.GetComponent<DefinicaoEstrutura>().custoOxigenio);
						
						objetoSelecionado.GetComponent<DefinicaoEstrutura>().objetoAConstruir = DefinicaoEstrutura.TipoEstrutura.LABORATORIO;
						objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao = DefinicaoEstrutura.StatusProgresso.EM_PROGRESSO;
						objetoSelecionado.GetComponent<DefinicaoEstrutura>().tempoConstrucao = prefabLaboratorio.GetComponent<DefinicaoEstrutura>().tempoConstrucao;
					}
					//Construir(DefinicaoEstrutura.TipoEstrutura.LABORATORIO);
					//objetoSelecionado.GetComponent<DefinicaoEstrutura>().construido = true;
				}	

				if(GUI.Button(new Rect(230 * 1.78f,Screen.height - 44,70 * 1.74f,70 * 0.53f),"Fabrica de Drones")){
					
					if((prefabFabricaDrones.GetComponent<DefinicaoEstrutura>().custoMetal <= GameObject.Find("Player").GetComponent<CPlayer>().metalLevel)&&
						(prefabFabricaDrones.GetComponent<DefinicaoEstrutura>().custoOxigenio <= GameObject.Find("Player").GetComponent<CPlayer>().oxygenLevel))
					{
						GameObject.Find("Player").GetComponent<CPlayer>().SubResourceMetal(prefabFabricaDrones.GetComponent<DefinicaoEstrutura>().custoMetal);
						GameObject.Find("Player").GetComponent<CPlayer>().SubResourceOxygen(prefabFabricaDrones.GetComponent<DefinicaoEstrutura>().custoOxigenio);
						
						objetoSelecionado.GetComponent<DefinicaoEstrutura>().objetoAConstruir = DefinicaoEstrutura.TipoEstrutura.FABRICA_DRONES;
						objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao = DefinicaoEstrutura.StatusProgresso.EM_PROGRESSO;
						objetoSelecionado.GetComponent<DefinicaoEstrutura>().tempoConstrucao = prefabFabricaDrones.GetComponent<DefinicaoEstrutura>().tempoConstrucao;
					}
					//Construir(DefinicaoEstrutura.TipoEstrutura.FABRICA_DRONES);
					//objetoSelecionado.GetComponent<DefinicaoEstrutura>().construido = true;
				}	
			}
			if(objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao == DefinicaoEstrutura.StatusProgresso.EM_PROGRESSO)
			{
				
				GUI.Label(new Rect(5 * 25,Screen.height - 44,70 * 1.15f,70 * 0.53f), "Progress: " + objetoSelecionado.GetComponent<DefinicaoEstrutura>().tempoAtualConstrucao + "%");
			}
			
		}
		//Fim eventos de menu dos  Slots
		
		//Eventos de menu das fabricas de drones
		if(tipoObj == DefinicaoEstrutura.TipoEstrutura.FABRICA_DRONES){
			if(objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao == DefinicaoEstrutura.StatusProgresso.LIBERADO){
				if(GUI.Button(new Rect(5 * 25,Screen.height - 44,70 * 1.15f,70 * 0.53f),"Drone")){
				
					objetoSelecionado.GetComponent<DefinicaoEstrutura>().objetoAConstruir = DefinicaoEstrutura.TipoEstrutura.DRONE_NORMAL;
					objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao = DefinicaoEstrutura.StatusProgresso.EM_PROGRESSO;
					objetoSelecionado.GetComponent<DefinicaoEstrutura>().tempoConstrucao = 5;
				}
			}
			if(objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao == DefinicaoEstrutura.StatusProgresso.EM_PROGRESSO){
				GUI.Label(new Rect(5 * 25,Screen.height - 44,70 * 1.15f,70 * 0.53f), "Progress: " + objetoSelecionado.GetComponent<DefinicaoEstrutura>().tempoAtualConstrucao + "%");
			}
		}
		//Fim eventos de menu das fabricas de drones
		
		//Eventos de menu das fazendas
		if(tipoObj == DefinicaoEstrutura.TipoEstrutura.FAZENDA)
		{
			if((objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao == DefinicaoEstrutura.StatusProgresso.LIBERADO)&&(objetoSelecionado.GetComponent<DefinicaoEstrutura>().nivelEstrutura < 3))
			{
				if(GUI.Button(new Rect(5 * 25,Screen.height - 44,70 * 1.15f,70 * 0.53f),"Atualizar")){
					objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao = DefinicaoEstrutura.StatusProgresso.EM_PROGRESSO;
					//objetoSelecionado.GetComponent<DefinicaoEstrutura>().IncrementaNivel();
					
				}
			}
			if(objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao == DefinicaoEstrutura.StatusProgresso.EM_PROGRESSO){
				GUI.Label(new Rect(5 * 25,Screen.height - 44,70 * 1.15f,70 * 0.53f), "Progress: " + objetoSelecionado.GetComponent<DefinicaoEstrutura>().tempoAtualConstrucao + "%");
			}
		}
		//Fim ventos de menu das fazendas
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
