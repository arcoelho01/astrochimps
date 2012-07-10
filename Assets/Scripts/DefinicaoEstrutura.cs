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
		PLATAFORMA_LANCAMENTO,
		DRONE_NORMAL,
		DRONE_SABOTADOR,
		DRONE_VIGILANCIA
	};
	public bool sabotado;
	public bool conectado;
	//public bool construido;
	public enum StatusProgresso{
		LIBERADO,
		EM_PROGRESSO,
		EM_ATUALIZACAO,
		CONCLUIDO
	};
	public int vida;
	public TipoEstrutura tipo;
	public StatusProgresso statusProgressao;
	public float custoOxigenio;
	public float custoMetal;
	public float tempoConstrucao;
	public float tempoAtualConstrucao;
	public TipoEstrutura objetoAConstruir;
	public string descricao;
	public int nivelEstrutura;
	public int[] custoMetalEvoluirNivel;
	public bool periferico;
	
    private float progressoAtual = 0;
	private GameObject prefabConstrucao;
	private float tempoDecorrido = 0;
	private GameObject cano;
	private int rotacaoX;
	private Vector3 posicao1;
	private Vector3 posicao2;
	
	
	// Use this for initialization
	void Start () {
		if((tipo ==TipoEstrutura.CANO_CENTRAL) || (tipo == TipoEstrutura.SLOT))
		{
			sabotado = false;
			conectado = true;
			//construido = false;
			vida = 100;
		}
		statusProgressao = StatusProgresso.LIBERADO;
		nivelEstrutura = 1;
		
		if (periferico)
			rotacaoX = 180;
		else
			rotacaoX = 0;
		
	}
	
	// Update is called once per frame
	void Update () {
	
		if((statusProgressao == StatusProgresso.EM_PROGRESSO)||(statusProgressao == StatusProgresso.EM_ATUALIZACAO))
		{
			tempoAtualConstrucao = MedeProgresso(tempoConstrucao);
		}
		ConsumirOxigenio();
	}
	
	float MedeProgresso(float tempoTotal){
		
		float percentualProgresso;
		
		if(progressoAtual < tempoTotal)
		{
			progressoAtual += 1 *Time.deltaTime;
		}
		else 
		{
			progressoAtual = tempoTotal;
			if(statusProgressao == StatusProgresso.EM_PROGRESSO)
				Construir();
			if(statusProgressao == StatusProgresso.EM_ATUALIZACAO)
				IncrementaNivel();
		}
		
		if(progressoAtual ==0)
			percentualProgresso = 0;
		else
			percentualProgresso = (progressoAtual * 100)/tempoTotal;
		
		//Debug.Log("("+tempoTotal+" * 100)/"+progressoAtual+" = "+percentualProgresso+"||||"+tempoAtualConstrucao);
		return percentualProgresso;
	}
	
	void Construir()
	{
		GameObject construcaoNova;
		bool drone = false;
		string nomeProvisorio ="";
		
		if(tipo == TipoEstrutura.SLOT)
			statusProgressao = StatusProgresso.CONCLUIDO;
		else
			statusProgressao = StatusProgresso.LIBERADO;
		
		progressoAtual = 0;
		
		//construcoes
		if(objetoAConstruir == TipoEstrutura.FAZENDA)
		{
			prefabConstrucao = GameObject.Find("Codigo").GetComponent<EventosMenu>().prefabFazenda;
			nomeProvisorio = "HydroponicFarm";
		}
		if(objetoAConstruir == TipoEstrutura.FABRICA_DRONES)
		{
			prefabConstrucao = GameObject.Find("Codigo").GetComponent<EventosMenu>().prefabFabricaDrones;
			nomeProvisorio = "DroneFactory";
		}
		if(objetoAConstruir == TipoEstrutura.LABORATORIO)
		{
			prefabConstrucao = GameObject.Find("Codigo").GetComponent<EventosMenu>().prefabLaboratorio;
			nomeProvisorio = "ResearchLab";
		}
		if(objetoAConstruir == TipoEstrutura.CENTRAL_SEGURANCA)
		{
			prefabConstrucao = GameObject.Find("Codigo").GetComponent<EventosMenu>().prefabCentralSeguranca;
			nomeProvisorio = "SecurityCenter";
		}
		//drones
		if(objetoAConstruir == TipoEstrutura.DRONE_NORMAL){
			prefabConstrucao = GameObject.Find("Codigo").GetComponent<EventosMenu>().prefabUnidadeDrone;
			drone = true;
		}
		if(objetoAConstruir == TipoEstrutura.DRONE_SABOTADOR){
			prefabConstrucao = GameObject.Find("Codigo").GetComponent<EventosMenu>().prefabUnidadeDroneSabotador;
			drone = true;
		}
		if(objetoAConstruir == TipoEstrutura.DRONE_VIGILANCIA){
			prefabConstrucao = GameObject.Find("Codigo").GetComponent<EventosMenu>().prefabUnidadeDroneVigilancia;
			drone = true;
		}
		
		if(!drone){
			construcaoNova = (GameObject)Instantiate(prefabConstrucao,new Vector3(transform.position.x,transform.position.y + 0.7f,transform.position.z),Quaternion.Euler(0,rotacaoX,0));
			construcaoNova.name = nomeProvisorio + this.name.Substring(4,1);
			GameObject.Find("Regiao"+this.name.Substring(4,1)).GetComponent<Regiao>().setor[1] = construcaoNova;
			GameObject.Find("CommandCenter").GetComponent<GerenciadorSlots>().slots[int.Parse(this.name.Substring(4,1))-1] = construcaoNova;
			cano = GameObject.Find("CanoCentral"+this.name.Substring(4,1)+"/Mesh/(TO TEX) Tube -- Low (MAP) (ANI F)"); //.renderer.enabled = true;
			cano.renderer.enabled = true;
			cano.animation.Play("Tube_Acordeon");
		}
		else{
			posicao1 = transform.Find("Mesh/Position1").transform.position;
			Debug.Log(posicao1);
			//construcaoNova = (GameObject)Instantiate(prefabConstrucao,new Vector3(transform.position.x,transform.position.y+1,transform.position.z),Quaternion.Euler(0,0,0));
			construcaoNova = (GameObject)Instantiate(prefabConstrucao,posicao1,Quaternion.Euler(0,0,0));
			posicao2 = transform.Find("Mesh/Position2").transform.position;
			//construcaoNova.GetComponent<CDrone>().WalkTo(posicao1);
			construcaoNova.GetComponent<CDrone>().WalkTo(posicao2);
		}
			
	}
	
	void ConsumirOxigenio(){
				
		if((tipo != TipoEstrutura.CANO_CENTRAL)
			&&(tipo != TipoEstrutura.EXTRATOR)
			&&(tipo != TipoEstrutura.PLATAFORMA_LANCAMENTO)
			&&(tipo != TipoEstrutura.SLOT))
		{
			if(tempoDecorrido < 1)
				tempoDecorrido += 1 * Time.deltaTime;
			else{
				
				if(GameObject.Find("Player").GetComponent<CPlayer>().oxygenLevel > 0)
					GameObject.Find("Player").GetComponent<CPlayer>().SubResourceOxygen(1);
				
				tempoDecorrido = 0;
			}
		}
	}
	
	void IncrementaNivel()
	{
		//GameObject.Find("Player").GetComponent<CPlayer>().SubResourceMetal(custoMetalEvoluirNivel[nivelEstrutura-1]);
		nivelEstrutura++;
		statusProgressao = StatusProgresso.LIBERADO;
		progressoAtual = 0;
	}
	
}
