using UnityEngine;
using System.Collections;

public class EventosMenu : MonoBehaviour {
	
	public GUISkin menuSkin;
	public float altura;
	public float largura;
	public GameObject prefabFazenda;
	public GameObject prefabLaboratorio;
	public GameObject prefabCentralSeguranca;
	public GameObject prefabFabricaDrones;
	public GameObject prefabUnidadeDrone;
	public GameObject prefabUnidadeDroneSabotador;
	public GameObject prefabUnidadeDroneVigilancia;
	public int precoMetalDrone;
	public int precoMetalDroneSabotador;
	public int precoMetalDroneVigilancia;
	public Transform objetoSelecionado;
   	public DefinicaoEstrutura.TipoEstrutura tipoObj;
	public int quantidadeMaximaMacacos = 1;
	public int quantidadeMaximaDrones = 1;
	public int quantidadeDrones;
	public int quantidadeMacacos;
	
	private bool slotsPosicionados = false;
	private bool fazendaConstruida = false;
	private bool centralSegurancaConstruida = false;
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
		if (objetoSelecionado!= null)
			if (!objetoSelecionado.GetComponent<CBuilding>().isSelected)
				objetoSelecionado = null;
	
	}
	
	//Construções
	public void ConstruirFazenda(){
		GameObject.Find("Player").GetComponent<CPlayer>().SubResourceMetal(prefabFazenda.GetComponent<DefinicaoEstrutura>().custoMetal);
		GameObject.Find("Player").GetComponent<CPlayer>().SubResourceOxygen(prefabFazenda.GetComponent<DefinicaoEstrutura>().custoOxigenio);
		objetoSelecionado.GetComponent<CBuilding>().Deselect();
		
		GameObject.Find("HUD-Objects").GetComponent<GUIControl>().nextSlot(4);
						
		objetoSelecionado.GetComponent<DefinicaoEstrutura>().objetoAConstruir = DefinicaoEstrutura.TipoEstrutura.FAZENDA;
		objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao = DefinicaoEstrutura.StatusProgresso.EM_PROGRESSO;
		objetoSelecionado.GetComponent<DefinicaoEstrutura>().tempoConstrucao = prefabFazenda.GetComponent<DefinicaoEstrutura>().tempoConstrucao;
		MainScript.Script.DeployUnderConstructionBox(objetoSelecionado.transform,null,objetoSelecionado.transform.position,prefabFazenda.GetComponent<DefinicaoEstrutura>().tempoConstrucao);
		fazendaConstruida = true;
	}
	
	public void ConstruirCentralSeguranca(){
		GameObject.Find("Player").GetComponent<CPlayer>().SubResourceMetal(prefabCentralSeguranca.GetComponent<DefinicaoEstrutura>().custoMetal);
		GameObject.Find("Player").GetComponent<CPlayer>().SubResourceOxygen(prefabCentralSeguranca.GetComponent<DefinicaoEstrutura>().custoOxigenio);
		GameObject.Find("HUD-Objects").GetComponent<GUIControl>().nextSlot(3);
		objetoSelecionado.GetComponent<CBuilding>().Deselect();
		objetoSelecionado.GetComponent<DefinicaoEstrutura>().objetoAConstruir = DefinicaoEstrutura.TipoEstrutura.CENTRAL_SEGURANCA;
		objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao = DefinicaoEstrutura.StatusProgresso.EM_PROGRESSO;
		objetoSelecionado.GetComponent<DefinicaoEstrutura>().tempoConstrucao = prefabCentralSeguranca.GetComponent<DefinicaoEstrutura>().tempoConstrucao;
		MainScript.Script.DeployUnderConstructionBox(objetoSelecionado.transform,null,objetoSelecionado.transform.position,prefabCentralSeguranca.GetComponent<DefinicaoEstrutura>().tempoConstrucao);
		centralSegurancaConstruida = true;
	}
	
	public void ConstruirLaboratorio(){
		GameObject.Find("Player").GetComponent<CPlayer>().SubResourceMetal(prefabLaboratorio.GetComponent<DefinicaoEstrutura>().custoMetal);
		GameObject.Find("Player").GetComponent<CPlayer>().SubResourceOxygen(prefabLaboratorio.GetComponent<DefinicaoEstrutura>().custoOxigenio);
		objetoSelecionado.GetComponent<CBuilding>().Deselect();
		GameObject.Find("HUD-Objects").GetComponent<GUIControl>().nextSlot(2);
		objetoSelecionado.GetComponent<DefinicaoEstrutura>().objetoAConstruir = DefinicaoEstrutura.TipoEstrutura.LABORATORIO;
		objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao = DefinicaoEstrutura.StatusProgresso.EM_PROGRESSO;
		objetoSelecionado.GetComponent<DefinicaoEstrutura>().tempoConstrucao = prefabLaboratorio.GetComponent<DefinicaoEstrutura>().tempoConstrucao;
		MainScript.Script.DeployUnderConstructionBox(objetoSelecionado.transform,null,objetoSelecionado.transform.position,prefabLaboratorio.GetComponent<DefinicaoEstrutura>().tempoConstrucao);
	}
	
	public void ConstruirFabrica(){
		
		GameObject.Find("Player").GetComponent<CPlayer>().SubResourceMetal(prefabFabricaDrones.GetComponent<DefinicaoEstrutura>().custoMetal);
		GameObject.Find("Player").GetComponent<CPlayer>().SubResourceOxygen(prefabFabricaDrones.GetComponent<DefinicaoEstrutura>().custoOxigenio);
		objetoSelecionado.GetComponent<CBuilding>().Deselect();
		GameObject.Find("HUD-Objects").GetComponent<GUIControl>().nextSlot(5);
		objetoSelecionado.GetComponent<DefinicaoEstrutura>().objetoAConstruir = DefinicaoEstrutura.TipoEstrutura.FABRICA_DRONES;
		objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao = DefinicaoEstrutura.StatusProgresso.EM_PROGRESSO;
		objetoSelecionado.GetComponent<DefinicaoEstrutura>().tempoConstrucao = prefabFabricaDrones.GetComponent<DefinicaoEstrutura>().tempoConstrucao;
		MainScript.Script.DeployUnderConstructionBox(objetoSelecionado.transform,null,objetoSelecionado.transform.position,prefabFabricaDrones.GetComponent<DefinicaoEstrutura>().tempoConstrucao);
		
	}
	//Drones
	public void FabricarDroneCacador(){
		GameObject.Find("Player").GetComponent<CPlayer>().SubResourceMetal(precoMetalDrone);
		objetoSelecionado.GetComponent<DefinicaoEstrutura>().objetoAConstruir = DefinicaoEstrutura.TipoEstrutura.DRONE_NORMAL;
		objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao = DefinicaoEstrutura.StatusProgresso.EM_PROGRESSO;
		objetoSelecionado.GetComponent<DefinicaoEstrutura>().tempoConstrucao = 15 / MainScript.getFabricRate();		
	}
	
	public void FabricarDroneVigia(){
		GameObject.Find("Player").GetComponent<CPlayer>().SubResourceMetal(precoMetalDroneVigilancia);
		objetoSelecionado.GetComponent<DefinicaoEstrutura>().objetoAConstruir = DefinicaoEstrutura.TipoEstrutura.DRONE_VIGILANCIA;
		objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao = DefinicaoEstrutura.StatusProgresso.EM_PROGRESSO;
		objetoSelecionado.GetComponent<DefinicaoEstrutura>().tempoConstrucao = 15 / MainScript.getFabricRate();
	}
	
	public void FabricarDroneSabotador(){
		GameObject.Find("Player").GetComponent<CPlayer>().SubResourceMetal(precoMetalDroneSabotador);
		objetoSelecionado.GetComponent<DefinicaoEstrutura>().objetoAConstruir = DefinicaoEstrutura.TipoEstrutura.DRONE_SABOTADOR;
		objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao = DefinicaoEstrutura.StatusProgresso.EM_PROGRESSO;
		objetoSelecionado.GetComponent<DefinicaoEstrutura>().tempoConstrucao = 15 / MainScript.getFabricRate();		
	}
	//Fim Drones
	
	//Métodos de Atualização
	public void AtualizarFazenda(){
		objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao = DefinicaoEstrutura.StatusProgresso.EM_ATUALIZACAO;
		GameObject.Find("Player").GetComponent<CPlayer>().SubResourceMetal(objetoSelecionado.GetComponent<DefinicaoEstrutura>().custoMetalEvoluirNivel[objetoSelecionado.GetComponent<DefinicaoEstrutura>().nivelEstrutura-1]);		
	}
	
	public void AtualizarCentralSeguranca(){
		objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao = DefinicaoEstrutura.StatusProgresso.EM_ATUALIZACAO;
		GameObject.Find("Player").GetComponent<CPlayer>().SubResourceMetal(objetoSelecionado.GetComponent<DefinicaoEstrutura>().custoMetalEvoluirNivel[objetoSelecionado.GetComponent<DefinicaoEstrutura>().nivelEstrutura-1]);		
	}
	
	public bool canUpgrade(){
		metal = objetoSelecionado.GetComponent<DefinicaoEstrutura>().custoMetalEvoluirNivel[objetoSelecionado.GetComponent<DefinicaoEstrutura>().nivelEstrutura-1];
		return (metal <= GameObject.Find("Player").GetComponent<CPlayer>().metalLevel);
	}
	
	public bool lastLevel(){
		return (objetoSelecionado.GetComponent<DefinicaoEstrutura>().nivelEstrutura == 3);
	}
	//Fim Métodos de Atualização
	
	public bool isFazendaConstruida(){
		return fazendaConstruida;
	}
	
	public bool isCentralSegurancaConstruida(){
		return centralSegurancaConstruida;
	}
	
	
	public bool canFazenda(){
		return ((prefabFazenda.GetComponent<DefinicaoEstrutura>().custoMetal <= GameObject.Find("Player").GetComponent<CPlayer>().metalLevel) &&(!fazendaConstruida));
	}
	
	public bool canSeguranca(){
		return ((prefabCentralSeguranca.GetComponent<DefinicaoEstrutura>().custoMetal <= GameObject.Find("Player").GetComponent<CPlayer>().metalLevel)&&(!centralSegurancaConstruida));
	}
	
	public bool canLaboratorio(){
		return (prefabLaboratorio.GetComponent<DefinicaoEstrutura>().custoMetal <= GameObject.Find("Player").GetComponent<CPlayer>().metalLevel);
	}
	
	public bool canFabrica(){
		return (prefabFabricaDrones.GetComponent<DefinicaoEstrutura>().custoMetal <= GameObject.Find("Player").GetComponent<CPlayer>().metalLevel);
	}
	
	public bool isSlot(){
		if (objetoSelecionado != null)
			return (objetoSelecionado.GetComponent<DefinicaoEstrutura>().tipo == DefinicaoEstrutura.TipoEstrutura.SLOT);
		return false;
	}
	//Drones
	public bool canDroneCacador(){
		bool retorno = true;
		
		if(precoMetalDrone > GameObject.Find("Player").GetComponent<CPlayer>().metalLevel)
			retorno = false;
		
		if(quantidadeMaximaDrones <= quantidadeDrones)
			retorno = false;
		
		return retorno;
	}
	
	public bool canDroneVigia(){
		bool retorno = true;
		
		if(precoMetalDroneVigilancia > GameObject.Find("Player").GetComponent<CPlayer>().metalLevel)
			retorno = false;
		
		if(quantidadeMaximaDrones <= quantidadeDrones)
			retorno = false;
		
		return retorno;
	}
	
	public bool canDronesabotador(){
		bool retorno = true;
		
		if(precoMetalDroneSabotador > GameObject.Find("Player").GetComponent<CPlayer>().metalLevel)
			retorno = false;
		
		if(quantidadeMaximaDrones <= quantidadeDrones)
			retorno = false;
		
		return retorno;
	}
	//Fim Drone
		
	void OnGUI(){
		GUI.skin = menuSkin;
		
		//float screenX = Screen.width * 0.5f - largura * 0.5f; // NOT USED WARNING
		//float screenY = Screen.height * 0.5f - altura * 0.5f; // NOT USED WARNING
		
		//GUI.Box(new Rect(0,Screen.height - 100,Screen.width,100),"Menu");

		//Eventos de Menu dos Slots
		//	if(objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao == DefinicaoEstrutura.StatusProgresso.LIBERADO)

//			if(objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao == DefinicaoEstrutura.StatusProgresso.EM_PROGRESSO)
//			{
				
//				GUI.Label(new Rect(5 * 25,Screen.height - 44,70 * 1.15f,70 * 0.53f), "Progress: " + objetoSelecionado.GetComponent<DefinicaoEstrutura>().tempoAtualConstrucao + "%");
//			}
			
		//Fim eventos de menu dos  Slots
		
		//Eventos de menu das fabricas de drones
		if(tipoObj == DefinicaoEstrutura.TipoEstrutura.FABRICA_DRONES){
			if(objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao == DefinicaoEstrutura.StatusProgresso.LIBERADO){
				if(GUI.Button(new Rect(5 * 25,Screen.height - 44,70 * 1.15f,70 * 0.53f),"Drone")){
				
					objetoSelecionado.GetComponent<DefinicaoEstrutura>().objetoAConstruir = DefinicaoEstrutura.TipoEstrutura.DRONE_NORMAL;
					objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao = DefinicaoEstrutura.StatusProgresso.EM_PROGRESSO;
					objetoSelecionado.GetComponent<DefinicaoEstrutura>().tempoConstrucao = 5;
				}
				
				if(GUI.Button(new Rect(80* 2.65f,Screen.height - 44,70 * 1.80f,70 * 0.53f),"Drone de Vigilancia")){
				
					objetoSelecionado.GetComponent<DefinicaoEstrutura>().objetoAConstruir = DefinicaoEstrutura.TipoEstrutura.DRONE_VIGILANCIA;
					objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao = DefinicaoEstrutura.StatusProgresso.EM_PROGRESSO;
					objetoSelecionado.GetComponent<DefinicaoEstrutura>().tempoConstrucao = 5;
				}
				if(GUI.Button(new Rect(155* 2.65f,Screen.height - 44,70 * 1.80f,70 * 0.53f),"Drone Sabotador")){
				
					objetoSelecionado.GetComponent<DefinicaoEstrutura>().objetoAConstruir = DefinicaoEstrutura.TipoEstrutura.DRONE_SABOTADOR;
					objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao = DefinicaoEstrutura.StatusProgresso.EM_PROGRESSO;
					objetoSelecionado.GetComponent<DefinicaoEstrutura>().tempoConstrucao = 5;
				}		
			}
			if(objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao != DefinicaoEstrutura.StatusProgresso.LIBERADO){
				GUI.Label(new Rect(5 * 25,Screen.height - 44,70 * 1.15f,70 * 0.53f), "Progress: " + objetoSelecionado.GetComponent<DefinicaoEstrutura>().tempoAtualConstrucao + "%");
			}
		}
		//Fim eventos de menu das fabricas de drones
		
		//Eventos de menu das fazendas
		if(tipoObj == DefinicaoEstrutura.TipoEstrutura.FAZENDA)
		{
			//metal = objetoSelecionado.GetComponent<DefinicaoEstrutura>().custoMetalEvoluirNivel[objetoSelecionado.GetComponent<DefinicaoEstrutura>().nivelEstrutura-1];
			
			if((objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao == DefinicaoEstrutura.StatusProgresso.LIBERADO)&&(objetoSelecionado.GetComponent<DefinicaoEstrutura>().nivelEstrutura < 3))
			{
				metal = objetoSelecionado.GetComponent<DefinicaoEstrutura>().custoMetalEvoluirNivel[objetoSelecionado.GetComponent<DefinicaoEstrutura>().nivelEstrutura-1];
				if(GUI.Button(new Rect(5 * 25,Screen.height - 44,70 * 1.15f,70 * 0.53f),"Atualizar")){
					if(metal <= GameObject.Find("Player").GetComponent<CPlayer>().metalLevel){
						objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao = DefinicaoEstrutura.StatusProgresso.EM_ATUALIZACAO;
						GameObject.Find("Player").GetComponent<CPlayer>().SubResourceMetal(objetoSelecionado.GetComponent<DefinicaoEstrutura>().custoMetalEvoluirNivel[objetoSelecionado.GetComponent<DefinicaoEstrutura>().nivelEstrutura-1]);
					}
				}
			}
			if(objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao == DefinicaoEstrutura.StatusProgresso.EM_ATUALIZACAO){
				GUI.Label(new Rect(5 * 25,Screen.height - 44,70 * 1.15f,70 * 0.53f), "Progress: " + objetoSelecionado.GetComponent<DefinicaoEstrutura>().tempoAtualConstrucao + "%");
			}
		}
		//Fim eventos de menu das fazendas
		
		//Eventos de menu da central de seguranca
		if(tipoObj == DefinicaoEstrutura.TipoEstrutura.CENTRAL_SEGURANCA)
		{
			//metal = objetoSelecionado.GetComponent<DefinicaoEstrutura>().custoMetalEvoluirNivel[objetoSelecionado.GetComponent<DefinicaoEstrutura>().nivelEstrutura-1];
			
			if((objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao == DefinicaoEstrutura.StatusProgresso.LIBERADO)&&(objetoSelecionado.GetComponent<DefinicaoEstrutura>().nivelEstrutura < 3)){
				metal = objetoSelecionado.GetComponent<DefinicaoEstrutura>().custoMetalEvoluirNivel[objetoSelecionado.GetComponent<DefinicaoEstrutura>().nivelEstrutura-1];
				if(GUI.Button(new Rect(5 * 25,Screen.height - 44,70 * 1.15f,70 * 0.53f),"Atualizar")){
					if(metal <= GameObject.Find("Player").GetComponent<CPlayer>().metalLevel){
						objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao = DefinicaoEstrutura.StatusProgresso.EM_ATUALIZACAO;
						GameObject.Find("Player").GetComponent<CPlayer>().SubResourceMetal(objetoSelecionado.GetComponent<DefinicaoEstrutura>().custoMetalEvoluirNivel[objetoSelecionado.GetComponent<DefinicaoEstrutura>().nivelEstrutura-1]);
					}
				}
			}
			
			if(objetoSelecionado.GetComponent<DefinicaoEstrutura>().statusProgressao == DefinicaoEstrutura.StatusProgresso.EM_ATUALIZACAO){
				GUI.Label(new Rect(5 * 25,Screen.height - 44,70 * 1.15f,70 * 0.53f), "Progress: " + objetoSelecionado.GetComponent<DefinicaoEstrutura>().tempoAtualConstrucao + "%");
			}
		}
		//Fim eventos de menu da central de seguranca
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
			case DefinicaoEstrutura.TipoEstrutura.CENTRAL_SEGURANCA:
				rv = prefabCentralSeguranca;
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
