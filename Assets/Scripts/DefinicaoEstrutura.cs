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
	//public bool construido;
	public enum StatusProgresso{
		LIBERADO,
		EM_PROGRESSO,
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
	
    private float progressoAtual = 0;
	private GameObject prefabConstrucao;
	private float tempoDecorrido = 0;
	
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
	}
	
	// Update is called once per frame
	void Update () {
	
		if(statusProgressao == StatusProgresso.EM_PROGRESSO)
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
			Construir();
		}
		
		if(progressoAtual ==0)
			percentualProgresso = 0;
		else
			percentualProgresso = (progressoAtual * 100)/tempoTotal;
		
		Debug.Log("("+tempoTotal+" * 100)/"+progressoAtual+" = "+percentualProgresso+"||||"+tempoAtualConstrucao);
		return percentualProgresso;
	}
	
	void Construir()
	{
		GameObject construcaoNova;
		
		statusProgressao = StatusProgresso.CONCLUIDO;
		
		if(objetoAConstruir == TipoEstrutura.FAZENDA)
			prefabConstrucao = GameObject.Find("Codigo").GetComponent<EventosMenu>().prefabFazenda;
		if(objetoAConstruir == TipoEstrutura.FABRICA_DRONES)
			prefabConstrucao = GameObject.Find("Codigo").GetComponent<EventosMenu>().prefabFabricaDrones;
		if(objetoAConstruir == TipoEstrutura.LABORATORIO)
			prefabConstrucao = GameObject.Find("Codigo").GetComponent<EventosMenu>().prefabLaboratorio;
		if(objetoAConstruir == TipoEstrutura.GARAGEM)
			prefabConstrucao = GameObject.Find("Codigo").GetComponent<EventosMenu>().prefabGaragem;
		

		construcaoNova = (GameObject)Instantiate(prefabConstrucao,new Vector3(transform.position.x,transform.position.y + 0.7f,transform.position.z),transform.rotation);		
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
}
