using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIControl : MonoBehaviour {
	
	CPlayer playerScript;
	MainScript mainScript;
	EventosMenu eventosMenu;
	MouseWorldPosition mouseWorld;
	
	public Texture2D BoxMacacoTexture;
	public Texture2D BoxCentroComandoTexture;
	public Texture2D BoxPredioTexture;
	public Texture2D BoxSlotTexture;
	public Texture2D BoxMapaTexture;
	
	public GUISkin skin;
	
	
	public Texture2D CentroComandoTexture;
	public Texture2D LaboratorioTexture;
	public Texture2D LaboratorioTextureOFF;
	public Texture2D FabricaTexture;
	public Texture2D FabricaTextureOFF;
	public Texture2D FazendaTexture;
	public Texture2D FazendaTextureOFF;
	public Texture2D ExtratorTexture;
	public Texture2D ExtratorTextureOff;
	public Texture2D SegurancaTexture;
	public Texture2D SegurancaTextureOFF;
	public Texture2D SlotTexture;
	
	public Texture2D AguaTexture;
	public Texture2D MetalTexture;
	
	
	public Texture2D MonkeyTexture;
	public Texture2D MonkeyTextureOFF;
	public Texture2D BarraVaziaTexture;
	public Texture2D BoxMetalTexture;
	public Texture2D BoxOxigenioTexture;
	public Texture2D ReguaOxigenioTexture;
	public Texture2D NivelOxigenioTexture;
	
	public Texture2D RadarTextureON;
	public Texture2D RadarTextureOFF;
	
	public Texture2D MenuTextureON;
	public Texture2D MenuTextureOFF;
	
	public Texture2D NivelMetal;
	
	//Macaco 1 - Astronauta
	public Texture2D AstronautaNomeTexture;
	public Texture2D AstronautaMonkeyTexture;
	public Texture2D AstronautaMonkeyTexturePreso;
	public Texture2D AstronautaMonkeyTextureDown;
	public Texture2D AtaqueTextureON;
	public Texture2D MoverTextureON;
	public Texture2D AtaqueTextureOFF;
	public Texture2D MoverTextureOFF;
	
	private ArrayList allMonkeys;
	
	private ArrayList allRects;
	
	//Macaco 2 - Engenheiro
	public Texture2D EngenheiroNomeTexture;
	public Texture2D EngenheiroMonkeyTexture;
	public Texture2D EngenheiroMonkeyTexturePreso;
	public Texture2D EngenheiroMonkeyDown;
	public Texture2D ReciclarTextureON;
	public Texture2D RepararTextureON;
	public Texture2D ReciclarTextureOFF;
	public Texture2D RepararTextureOFF;
	
	
	//Macaco 3 - Sabotador
	public Texture2D SabotadorNomeTexture;
	public Texture2D SabotadorMonkeyTexture;
	public Texture2D SabotadorMonkeyTexturePreso;
	public Texture2D SabotadorMonkeyTextureDown;
	public Texture2D SabotarTextureON;
	public Texture2D SabotarTextureOFF;
	public Texture2D ReprogramarTextureON;
	public Texture2D ReprogramarTextureOFF;
	
	//Macaco 4 - Cientista
	public Texture2D CientistaNomeTexture;
	public Texture2D CientistaMonkeyTexture;
	public Texture2D CientistaMonkeyTexturePreso;
	public Texture2D CientistaMonkeyTextureDown;
	public Texture2D ProcurarTextureON;
	public Texture2D ProcurarTextureOFF;
	
	// FABRICA
	public Texture2D DroneCacadorTexture;
	public Texture2D DroneCacadorTextureOff;
	public Texture2D DroneSabotadorTexture;
	public Texture2D DroneSabotadorTextureOff;
	public Texture2D DronePatrulhaTexture;
	public Texture2D DronePatrulhaTextureOff;
	
	//FAZENDA E CENTRAL DE SEGURANÇA
	public Texture2D BtAtualizar;
	
	//Barra de Progresso
	public Texture2D BarraProgressoCheia;
	public Texture2D BarraProgressoVazia;
	public Texture2D MolduraBarraProgresso;
	
	public Texture2D removerMacaco;	
	
	public float NivelOxigenioFloat = 0.5f;
	private int[] SlotPredios = {0,0,0,0,0,0,0,0,0,0};

	int boxYmax;
	int boxYmin;
	ArrayList boxYposition;
	ArrayList yDirection;
	int xDirection;
	
	int box1x = 11;
	int boxXDistance = 172;
	int box5x = 826;
	
	public Texture2D ComandoOxTexture;
	public Texture2D LabOxTexture;
	public Texture2D SegurancaOxTexture;
	public Texture2D FazendaOxTexture;
	public Texture2D FabricaOxTexture;
	
	private int nextSlotPosition = 0;
	
	private bool callUpdate = false;
	
	
	List<Transform> lAllBuilding = new List<Transform>();
	List<Transform> lAllResources = new List<Transform>();
	
	
	public void nextSlot(int predio){
		nextSlotPosition++;
		setSlot(nextSlotPosition,  predio);
	}
	// DEFINE - tem que fazer
	// Centro de Comando - 1
	// Laboratorio - 2
	// Seguranca - 3
	// Fazenda - 4
	// Fabricao - 5
	public void setSlot(int slotNumber, int predio)
	{
		//Debug.LogError("Chamou o set slot com slotnumber " + slotNumber + " e predio " + predio);
		SlotPredios[slotNumber] = predio;
	
		
		
	}
	
	public void addMonkey(CMonkey newMonkey){
		if (allMonkeys == null)
			allMonkeys = new ArrayList();
		allMonkeys.Add(newMonkey);
		if (boxYposition == null)
			boxYposition = new ArrayList();
		boxYposition.Add(642);
		if (yDirection == null)
			yDirection = new ArrayList();
		yDirection.Add(0);
	}
	
	private void drawSlots(){
		int predio = 0;
		int slot = 0;
		while(predio != 0 && slot != 9){ // 0 == NULL WARNING
			slot++;
			predio = SlotPredios[slot];
			if (predio != 0){ // 0 == NULL WARNING
				if (predio == 1)
					GUI.Label (new Rect (527 + 33*slot,16,ComandoOxTexture.width,ComandoOxTexture.height), ComandoOxTexture);
				if (predio == 2)
					GUI.Label (new Rect (527 + 33*slot,16,LabOxTexture.width,LabOxTexture.height), LabOxTexture);
				if (predio == 3)
					GUI.Label (new Rect (527 + 33*slot,16,SegurancaOxTexture.width,SegurancaOxTexture.height), SegurancaOxTexture);
				if (predio == 4)
					GUI.Label (new Rect (527+ 33*slot,16,FazendaOxTexture.width,FazendaOxTexture.height), FazendaOxTexture);
				if (predio == 5)
					GUI.Label (new Rect (527 + 33*slot,16,FabricaOxTexture.width,FabricaOxTexture.height), FabricaOxTexture);
			}
		}
	}
	
	
	//FALTA FAZER OS SLOTS DOS PREDIOS
	int getYposition(int m){
		return (int) boxYposition[m];
	}
	
	Rect AddRect(Rect r){
		allRects.Add(r);
		return r;
	}
	void OnGUI () {
		allRects = new ArrayList();
		
		NivelOxigenioFloat = playerScript.oxygenLevel / 500;

		
		GUI.Label (AddRect(new Rect (8,4,BoxMapaTexture.width,BoxMapaTexture.height)), BoxMapaTexture);
		GUI.Label (AddRect(new Rect (150,10,RadarTextureON.width*3,RadarTextureON.height*3)), RadarTextureON);
		
		GUI.Label (AddRect(new Rect (950,10,MenuTextureON.width *3,MenuTextureON.height*3)), MenuTextureON);
		
		GUI.Label (AddRect(new Rect (210,5,BoxMetalTexture.width,BoxMetalTexture.height)), BoxMetalTexture);
		GUI.skin = skin;
		GUI.Label(new Rect (280, 22,120,100),playerScript.metalLevel.ToString());
		GUI.skin = null;
		GUI.Label (AddRect(new Rect (380,5,BoxOxigenioTexture.width,BoxOxigenioTexture.height)), BoxOxigenioTexture);
				
		
		
		for ( int x = 556 ; x < 560 + (NivelOxigenioFloat * 280); x = x +4){
			GUI.Label (new Rect (x,16,4,38), NivelOxigenioTexture);
		}
			
		GUI.Label (new Rect (550,10,ReguaOxigenioTexture.width,ReguaOxigenioTexture.height), ReguaOxigenioTexture);
		
		
		drawSlots();
			
		drawMonkeys();
		
		drawBuilding();
		
		
}
	public bool CheckClickOnRects( Vector2 point){
		
		//Debug.Log("allRects.Count "+ allRects.Count);
		if (allRects.Count != 0)
		foreach ( Rect r  in allRects ){
			Rect temp = new Rect(r.x,r.y,r.width,r.height);
			if (temp.Contains(point)){
				//Debug.Log("Clicou SOBRE");
				return true;
			}
		}
		//Debug.Log("Clicou FORA");
		return false;
	} 
	
	void drawBuilding(){
		
		int baseYSide = 300;
		int baseXSide = 760;
		CBuilding building;
		DefinicaoEstrutura estrutura;
		lAllBuilding = mainScript.GetListOfAllAlliedBuildings();
		
		foreach ( Transform predio in lAllBuilding){ 
			building = predio.GetComponent<CBuilding>();
			estrutura = predio.GetComponent<DefinicaoEstrutura>();
			if (building == null)
				continue;
			
			GUI.skin = skin;
			if (building.isSelected && building.tipo == CBuilding.TipoEstrutura.CENTRO_COMANDO){
				
				GUI.Label (AddRect(new Rect (baseXSide, baseYSide,BoxCentroComandoTexture.width,BoxCentroComandoTexture.height)), BoxCentroComandoTexture);
				GUI.Label(new Rect (baseXSide+70, baseYSide+32,120,100),"CENTRAL DE COMANDO");
				GUI.Label (new Rect (baseXSide + 10, baseYSide + 10,CentroComandoTexture.width,CentroComandoTexture.height), CentroComandoTexture);
				GUI.Label (new Rect (baseXSide + 20, baseYSide + 115,EngenheiroMonkeyTexture.width,EngenheiroMonkeyTexture.height), EngenheiroMonkeyTexture);
				GUI.Label (new Rect (baseXSide + 70 , baseYSide + 115,CientistaMonkeyTexture.width,CientistaMonkeyTexture.height), CientistaMonkeyTexture);
				GUI.Label (new Rect (baseXSide + 120 , baseYSide + 115 ,SabotadorMonkeyTexture.width,SabotadorMonkeyTexture.height), SabotadorMonkeyTexture);
				if (building.TheresAMonkeyInside() != null)
					if(GUI.Button(new Rect (baseXSide + 10 , baseYSide + 60 ,SabotadorMonkeyTexture.width,SabotadorMonkeyTexture.height),removerMacaco, "RemoverMacaco")){
						Transform monkey = building.TheresAMonkeyInside();
						building.GetTheMonkeyOut();
						building.Deselect();
						mouseWorld.SelectObject(monkey);
						
					}
				return;
			}
		    GUI.skin = skin;
			if (building.isSelected && building.tipo == CBuilding.TipoEstrutura.EXTRATOR){
				
				GUI.Label (AddRect(new Rect (baseXSide, baseYSide,BoxCentroComandoTexture.width,BoxCentroComandoTexture.height)), BoxCentroComandoTexture);
				if (building.resourceSite.GetComponent<CResource>().resourceType == CResource.eResourceType.Metal && building.resourceSite.GetComponent<CResource>().resourceLevel != 0 )
					GUI.Label(new Rect (baseXSide+70, baseYSide+32,120,100),"EXTRATOR DE METAL");
				if (building.resourceSite.GetComponent<CResource>().resourceType == CResource.eResourceType.Metal && building.resourceSite.GetComponent<CResource>().resourceLevel <= 0 )
					GUI.Label(new Rect (baseXSide+70, baseYSide+32,120,100),"EXTRATOR DE METAL DESATIVADO");
				
				if (building.resourceSite.GetComponent<CResource>().resourceType == CResource.eResourceType.Oxygen && building.resourceSite.GetComponent<CResource>().resourceLevel != 0 )
					GUI.Label(new Rect (baseXSide+70, baseYSide+32,120,100),"EXTRATOR DE OXIGENIO");
				if (building.resourceSite.GetComponent<CResource>().resourceType == CResource.eResourceType.Oxygen && building.resourceSite.GetComponent<CResource>().resourceLevel <= 0 )
					GUI.Label(new Rect (baseXSide+70, baseYSide+32,120,100),"EXTRATOR DE OXIGENIO DESATIVADO");
				
				GUI.Label (new Rect (baseXSide + 10, baseYSide + 10,ExtratorTexture.width,ExtratorTexture.height), ExtratorTexture);
			}
			
			if (building.isSelected && building.tipo == CBuilding.TipoEstrutura.CENTRAL_SEGURANCA){
			
				GUI.Label (AddRect(new Rect (baseXSide, baseYSide,BoxCentroComandoTexture.width,BoxCentroComandoTexture.height)), BoxCentroComandoTexture);
				GUI.Label(new Rect (baseXSide+70, baseYSide+32,120,100),"CENTRAL DE SEGURANCßA");
				GUI.Label (new Rect (baseXSide + 10, baseYSide + 10,SegurancaTexture.width,SegurancaTexture.height), SegurancaTexture);
				
				if(estrutura.statusProgressao == DefinicaoEstrutura.StatusProgresso.EM_ATUALIZACAO){
					GUI.DrawTexture(new Rect (baseXSide + 30, baseYSide + 70,(LaboratorioTexture.width + 60) * (estrutura.tempoAtualConstrucao/100),20),BarraProgressoCheia);
					GUI.DrawTexture(new Rect (baseXSide + 30, baseYSide + 70,LaboratorioTexture.width + 60,20),MolduraBarraProgresso);
				}
				else{
					if (GUI.Button(new Rect (baseXSide + 30, baseYSide + 70,LaboratorioTexture.width,LaboratorioTexture.height),BtAtualizar,"Atualizar")){
						if(!eventosMenu.lastLevel())
							if(eventosMenu.canUpgrade())
								eventosMenu.AtualizarCentralSeguranca();
					}
				}
				
				GUI.Label(new Rect (baseXSide+30, baseYSide+150,120,100),"NIVEL: "+estrutura.nivelEstrutura);
			}
			
			if (building.isSelected && building.tipo == CBuilding.TipoEstrutura.FAZENDA){
			
				GUI.Label (AddRect(new Rect (baseXSide, baseYSide,BoxCentroComandoTexture.width,BoxCentroComandoTexture.height)), BoxCentroComandoTexture);
				GUI.Label(new Rect (baseXSide+70, baseYSide+32,120,100),"FAZENDA HIDROPONICA");
				GUI.Label (new Rect (baseXSide + 10, baseYSide + 10,FazendaTexture.width,FazendaTexture.height), FazendaTexture);
				
				if(estrutura.statusProgressao == DefinicaoEstrutura.StatusProgresso.EM_ATUALIZACAO){
					GUI.DrawTexture(new Rect (baseXSide + 30, baseYSide + 70,(LaboratorioTexture.width + 60) * (estrutura.tempoAtualConstrucao/100),20),BarraProgressoCheia);
					GUI.DrawTexture(new Rect (baseXSide + 30, baseYSide + 70,LaboratorioTexture.width + 60,20),MolduraBarraProgresso);
				}
				else{
					if (GUI.Button(new Rect (baseXSide + 30, baseYSide + 70,LaboratorioTexture.width,LaboratorioTexture.height),BtAtualizar,"Atualizar")){
						if(!eventosMenu.lastLevel())
							if(eventosMenu.canUpgrade())
								eventosMenu.AtualizarFazenda();
					}
				}
				
				GUI.Label(new Rect (baseXSide+30, baseYSide+150,120,100),"NIVEL: "+estrutura.nivelEstrutura);
			}
			if (building.isSelected && building.tipo == CBuilding.TipoEstrutura.FABRICA_DRONES){
			
				GUI.Label (AddRect(new Rect (baseXSide, baseYSide,BoxCentroComandoTexture.width,BoxCentroComandoTexture.height)), BoxCentroComandoTexture);
				GUI.Label(new Rect (baseXSide+70, baseYSide+32,120,100),"FABRICA DE DRONES");
				GUI.Label (new Rect (baseXSide + 10, baseYSide + 10,FabricaTexture.width,FabricaTexture.height), FabricaTexture);
				
				if(estrutura.statusProgressao == DefinicaoEstrutura.StatusProgresso.EM_PROGRESSO){
					GUI.DrawTexture(new Rect (baseXSide + 30, baseYSide + 70,(LaboratorioTexture.width + 60) * (estrutura.tempoAtualConstrucao/100),20),BarraProgressoCheia);
					GUI.DrawTexture(new Rect (baseXSide + 30, baseYSide + 70,LaboratorioTexture.width + 60,20),MolduraBarraProgresso);
				}
				else{
					if (GUI.Button(new Rect (baseXSide + 30, baseYSide + 70,LaboratorioTexture.width,LaboratorioTexture.height),(eventosMenu.canDronesabotador()?DroneSabotadorTexture:DroneSabotadorTextureOff),"DroneSabotador")){
						if(eventosMenu.canDronesabotador())
							eventosMenu.FabricarDroneSabotador();
					}
					if (GUI.Button(new Rect (baseXSide + 30, baseYSide + 120,SegurancaTexture.width,SegurancaTexture.height), (eventosMenu.canDroneVigia()?DronePatrulhaTexture:DronePatrulhaTextureOff), "DroneVigia")){
						if(eventosMenu.canDroneVigia())
							eventosMenu.FabricarDroneVigia();
					}
					if(GUI.Button(new Rect (baseXSide + 90, baseYSide + 70,FazendaTexture.width,FazendaTexture.height),(eventosMenu.canDroneCacador()?DroneCacadorTexture:DroneCacadorTextureOff), "DroneCacador")){
						if(eventosMenu.canDroneCacador())
							eventosMenu.FabricarDroneCacador();
					}
				}
				/*if(GUI.Button(new Rect (baseXSide + 90, baseYSide + 120,FabricaTexture.width,FabricaTexture.height), "", "DroneCacador")){
					eventosMenu.ConstruirFabrica();
				}*/
			}
			if (building.isSelected && building.tipo == CBuilding.TipoEstrutura.LABORATORIO){
			
				GUI.Label (AddRect(new Rect (baseXSide, baseYSide,BoxCentroComandoTexture.width,BoxCentroComandoTexture.height)), BoxCentroComandoTexture);
				GUI.Label(new Rect (baseXSide+70, baseYSide+32,120,100),"LABORATORIO");
				GUI.Label (new Rect (baseXSide + 10, baseYSide + 10,LaboratorioTexture.width,LaboratorioTexture.height), LaboratorioTexture);
			}
						
			if (eventosMenu.isSlot() && building.Selectable){
				
				Rect area;
				
				GUI.Label (AddRect(new Rect (baseXSide, baseYSide,BoxSlotTexture.width,BoxSlotTexture.height)), BoxSlotTexture);
				GUI.Label(new Rect (baseXSide+70, baseYSide+32,120,100),"AREA DE CONSTRUCAO");
				GUI.Label (new Rect (baseXSide + 10, baseYSide + 10,SlotTexture.width,SlotTexture.height), SlotTexture);
				
				//Botão do laboratorio
				if (GUI.Button(new Rect (baseXSide + 30, baseYSide + 70,LaboratorioTexture.width,LaboratorioTexture.height), (eventosMenu.canLaboratorio()?LaboratorioTexture:LaboratorioTextureOFF),"Laboratorio")){
					if(eventosMenu.canLaboratorio())
						eventosMenu.ConstruirLaboratorio();
				}
				
				area = new Rect (baseXSide + 30, baseYSide + 70,LaboratorioTexture.width,LaboratorioTexture.height);
				if(area.Contains(Event.current.mousePosition)){
					GUI.Box (new Rect (baseXSide+2, baseYSide+181,BoxMetalTexture.width+12,BoxMetalTexture.height+20),"");
					GUI.skin = skin;
					GUI.Label(new Rect (baseXSide + 5, baseYSide+180,120,100),"LABORATORIO");
					GUI.Label(new Rect (baseXSide + 70, baseYSide+210,120,100),eventosMenu.getDadosLaboratorio("CUSTO"));
					GUI.Label(new Rect (baseXSide + 5, baseYSide+220,120,100),eventosMenu.getDadosLaboratorio("DESCRICAO"));
					GUI.skin = null;					
				}
				
				//Botão da central de segurança
				if (GUI.Button(new Rect (baseXSide + 30, baseYSide + 120,SegurancaTexture.width,SegurancaTexture.height), (eventosMenu.canSeguranca()?SegurancaTexture:SegurancaTextureOFF),"Seguranca")){
					if(eventosMenu.canSeguranca())
						eventosMenu.ConstruirCentralSeguranca();
				}
				
				area = new Rect (baseXSide + 30, baseYSide + 120,SegurancaTexture.width,SegurancaTexture.height);
				if(area.Contains(Event.current.mousePosition)){
					GUI.Box (new Rect (baseXSide+2, baseYSide+181,BoxMetalTexture.width+12,BoxMetalTexture.height+20),"");
					GUI.skin = skin;
					GUI.Label(new Rect (baseXSide + 5, baseYSide+180,180,100),"CENTRAL DE SEGURANCA");
					GUI.Label(new Rect (baseXSide + 70, baseYSide+210,120,100),eventosMenu.getDadosCentralSeguranca("CUSTO"));
					GUI.skin = null;
				}
				
				//Botão da fazenda
				if(GUI.Button(new Rect (baseXSide + 90, baseYSide + 70,FazendaTexture.width,FazendaTexture.height), (eventosMenu.canFazenda()?FazendaTexture:FazendaTextureOFF),"Fazenda")){
					if(eventosMenu.canFazenda())
						eventosMenu.ConstruirFazenda();
				}
				
				area = new Rect (baseXSide + 90, baseYSide + 70,FazendaTexture.width,FazendaTexture.height);
				if(area.Contains(Event.current.mousePosition)){
					GUI.Box (new Rect (baseXSide+2, baseYSide+181,BoxMetalTexture.width+12,BoxMetalTexture.height+20),"");
					GUI.skin = skin;
					GUI.Label(new Rect (baseXSide + 5, baseYSide+180,180,100),"FAZENDA HIDROPONICA");
					GUI.Label(new Rect (baseXSide + 70, baseYSide+210,120,100),eventosMenu.getDadosFazenda("CUSTO"));
					GUI.skin = null;
				}
				
				//Botão da fabrica de drones
				if(GUI.Button(new Rect (baseXSide + 90, baseYSide + 120,FabricaTexture.width,FabricaTexture.height), (eventosMenu.canFabrica()?FabricaTexture:FabricaTextureOFF),"Fabrica")){
					if(eventosMenu.canFabrica())
						eventosMenu.ConstruirFabrica();
				}
				
				area = new Rect (baseXSide + 90, baseYSide + 120,FabricaTexture.width,FabricaTexture.height);
				if(area.Contains(Event.current.mousePosition)){
					GUI.Box (new Rect (baseXSide+2, baseYSide+181,BoxMetalTexture.width+12,BoxMetalTexture.height+20),"");
					GUI.skin = skin;
					GUI.Label(new Rect (baseXSide + 5, baseYSide+180,180,100),"FABRICA DE DRONES");
					GUI.Label(new Rect (baseXSide + 70, baseYSide+210,120,100),eventosMenu.getDadosFabricaDrones("CUSTO"));
					GUI.skin = null;
				}
				
				return;
			}
		}
			
		lAllResources = mainScript.GetListOfAllNeutralResources();
		foreach ( Transform recurso in lAllResources){
			CResource r = recurso.GetComponent<CResource>();
			if (r.isSelected)
			{
				GUI.Label (AddRect(new Rect (baseXSide, baseYSide,BoxCentroComandoTexture.width,BoxCentroComandoTexture.height)), BoxCentroComandoTexture);
				if (r.resourceType == CResource.eResourceType.Metal){
					GUI.Label(new Rect (baseXSide+70, baseYSide+32,120,100),"METAL");
					GUI.Label (new Rect (baseXSide + 10, baseYSide + 10,MetalTexture.width,MetalTexture.height), MetalTexture);
				}
				if (r.resourceType == CResource.eResourceType.Oxygen){
					GUI.Label(new Rect (baseXSide+70, baseYSide+32,120,100),"FONTE DE H2O");
					GUI.Label (new Rect (baseXSide + 10, baseYSide + 10,AguaTexture.width,AguaTexture.height), MetalTexture);
				}
				
				if (GUI.Button(new Rect (baseXSide + 40, baseYSide + 80, ExtratorTexture.width,ExtratorTexture.height), (mainScript.CheckIfAreEnoughResourcesToBuild(MainScript.Script.prefabExtractor)?ExtratorTexture:ExtratorTextureOff),"Laboratorio")){
					recurso.GetComponent<CBaseEntity>().BuildIt();
				}
			}
				
		}
	}
	
	
	
	void drawMonkeys(){
		
		int m = 0;
		foreach( CMonkey monkey in allMonkeys){
			
			int posX = box1x + boxXDistance * m;
			int posY = (int) getYposition(m);
			//Debug.Log("Macaco " + m + " Tipo " + monkey.monkeyClass);
			GUI.Label (new Rect (posX + 79, posY+7 , 	BarraVaziaTexture.width,BarraVaziaTexture.height), BarraVaziaTexture);
			GUI.Label (AddRect(new Rect (posX, 		posY,		BoxMacacoTexture.width,BoxMacacoTexture.height)), BoxMacacoTexture);
			
				
			if(monkey.monkeyClass == CMonkey.eMonkeyType.Astronaut){
				GUI.Label (new Rect (posX + 70 , posY + 40, AstronautaNomeTexture.width,AstronautaNomeTexture.height), AstronautaNomeTexture);
				if (monkey.getFSMCurrentState() == CMonkey.FSMState.STATE_CAPTURED)
					GUI.Label (new Rect (posX +15,  posY +11,	AstronautaMonkeyTexturePreso.width,AstronautaMonkeyTexturePreso.height), AstronautaMonkeyTexturePreso);
				else
					GUI.Label (new Rect (posX +15,  posY +11,	AstronautaMonkeyTexture.width,AstronautaMonkeyTexture.height), AstronautaMonkeyTexture);
				
				if (monkey.getFSMCurrentState() == CMonkey.FSMState.STATE_IDLE){
					GUI.Label (new Rect (posX + 18 , posY + 71, AtaqueTextureOFF.width,AtaqueTextureOFF.height), AtaqueTextureOFF);
					GUI.Label (new Rect (posX + 18 + 45 , getYposition(m)  + 71, MoverTextureON.width,MoverTextureOFF.height), MoverTextureOFF);
					//desceSlot(m);
				}else if(monkey.getFSMCurrentState() == CMonkey.FSMState.STATE_WALKING){
					GUI.Label (new Rect (posX + 18 , posY + 71, AtaqueTextureOFF.width,AtaqueTextureOFF.height), AtaqueTextureOFF);
					GUI.Label (new Rect (posX + 18 + 45 , posY  + 71, MoverTextureON.width,MoverTextureON.height), MoverTextureON);
					//sobeSlot(m);
				}else if(monkey.getFSMCurrentState() == CMonkey.FSMState.STATE_ATTACKING || monkey.getFSMCurrentState() == CMonkey.FSMState.STATE_PURSUIT){
					GUI.Label (new Rect (posX + 18 , posY + 71, AtaqueTextureON.width,AtaqueTextureON.height), AtaqueTextureON);
					GUI.Label (new Rect (posX + 18 + 45 , posY  + 71, MoverTextureON.width,MoverTextureOFF.height), MoverTextureOFF);
					//sobeSlot(m);
				} else {
					GUI.Label (new Rect (posX + 18, getYposition(m) + 71, AtaqueTextureOFF.width,AtaqueTextureOFF.height), AtaqueTextureOFF);
					GUI.Label (new Rect (posX + 18 + 45 , posY + 71, MoverTextureON.width,MoverTextureOFF.height), MoverTextureOFF);
					//desceSlot(m);
				}
			} else 	if(monkey.monkeyClass == CMonkey.eMonkeyType.Engineer){
				GUI.Label (new Rect (posX + 70 , posY + 40, EngenheiroNomeTexture.width,EngenheiroNomeTexture.height), EngenheiroNomeTexture);
				if (monkey.getFSMCurrentState() == CMonkey.FSMState.STATE_CAPTURED)
					GUI.Label (new Rect (posX +15,  posY +11,	EngenheiroMonkeyTexturePreso.width,EngenheiroMonkeyTexturePreso.height), EngenheiroMonkeyTexturePreso);
				else
					GUI.Label (new Rect (posX +15,  posY +11,	EngenheiroMonkeyTexture.width,EngenheiroMonkeyTexture.height), EngenheiroMonkeyTexture);
				if (monkey.getFSMCurrentState() == CMonkey.FSMState.STATE_IDLE){
					GUI.Label (new Rect (posX + 18 , posY + 71, ReciclarTextureON.width,ReciclarTextureON.height), ReciclarTextureOFF);
					GUI.Label (new Rect (posX + 18 + 45 , posY  + 71, MoverTextureON.width,MoverTextureOFF.height), MoverTextureOFF);
					GUI.Label (new Rect (posX + 18 + 45 + 45, posY + 71, RepararTextureON.width,RepararTextureON.height), RepararTextureOFF);
				}else if(monkey.getFSMCurrentState() == CMonkey.FSMState.STATE_WALKING){
					GUI.Label (new Rect (posX + 18 , posY + 71, ReciclarTextureON.width,ReciclarTextureON.height), ReciclarTextureOFF);
					GUI.Label (new Rect (posX + 18 + 45 , posY + 71, MoverTextureON.width,MoverTextureON.height), MoverTextureON);
					GUI.Label (new Rect (posX + 18 + 45 + 45, posY + 71, RepararTextureON.width,RepararTextureON.height), RepararTextureOFF);
				}else if(monkey.getFSMCurrentState() == CMonkey.FSMState.STATE_WORKING){
					GUI.Label (new Rect (posX + 18 , posY + 71, ReciclarTextureON.width,ReciclarTextureON.height), ReciclarTextureON);
					GUI.Label (new Rect (posX + 18 + 45 , posY + 71, MoverTextureON.width,MoverTextureOFF.height), MoverTextureOFF);
					GUI.Label (new Rect (posX + 18 + 45 + 45, posY + 71, RepararTextureOFF.width,RepararTextureOFF.height), RepararTextureOFF);
				}else if(monkey.getFSMCurrentState() == CMonkey.FSMState.STATE_ATTACKING){
					GUI.Label (new Rect (posX + 18, posY + 71, ReciclarTextureOFF.width,ReciclarTextureOFF.height), ReciclarTextureOFF);
					GUI.Label (new Rect (posX + 18 + 45 , posY + 71, MoverTextureOFF.width,MoverTextureOFF.height), MoverTextureOFF);
					GUI.Label (new Rect (posX + 18 + 45 + 45, posY + 71, RepararTextureON.width,RepararTextureON.height), RepararTextureON);
				} else 
				{
					GUI.Label (new Rect (posX + 18 , posY + 71, ReciclarTextureON.width,ReciclarTextureON.height), ReciclarTextureOFF);
					GUI.Label (new Rect (posX + 18 + 45 , posY  + 71, MoverTextureON.width,MoverTextureOFF.height), MoverTextureOFF);
					GUI.Label (new Rect (posX + 18 + 45 + 45, posY + 71, RepararTextureON.width,RepararTextureON.height), RepararTextureOFF);
				}
			} else 	if(monkey.monkeyClass == CMonkey.eMonkeyType.Cientist){
				GUI.Label (new Rect (posX + 70 , posY + 40, CientistaNomeTexture.width,CientistaNomeTexture.height), CientistaNomeTexture);
				if (monkey.getFSMCurrentState() == CMonkey.FSMState.STATE_CAPTURED)
					GUI.Label (new Rect (posX +15,  posY +11,	CientistaMonkeyTexturePreso.width,CientistaMonkeyTexturePreso.height), CientistaMonkeyTexturePreso);
				else
					GUI.Label (new Rect (posX +15,  posY +11,	CientistaMonkeyTexture.width,CientistaMonkeyTexture.height), CientistaMonkeyTexture);
				if (monkey.getFSMCurrentState() == CMonkey.FSMState.STATE_IDLE){
					GUI.Label (new Rect (posX + 18 , posY + 71, ProcurarTextureOFF.width,ProcurarTextureOFF.height), ProcurarTextureOFF);
					GUI.Label (new Rect (posX + 18 + 45 , posY  + 71, MoverTextureON.width,MoverTextureOFF.height), MoverTextureOFF);
				}else if(monkey.getFSMCurrentState() == CMonkey.FSMState.STATE_WALKING){
					GUI.Label (new Rect (posX + 18 , posY + 71, ProcurarTextureOFF.width,ProcurarTextureOFF.height), ProcurarTextureOFF);
					GUI.Label (new Rect (posX + 18 + 45 , posY + 71, MoverTextureON.width,MoverTextureON.height), MoverTextureON);
				}else if(monkey.getFSMCurrentState() == CMonkey.FSMState.STATE_WORKING){
					GUI.Label (new Rect (posX + 18 , posY + 71, ProcurarTextureON.width,ProcurarTextureON.height), ProcurarTextureON);
					GUI.Label (new Rect (posX + 18 + 45 , posY + 71, MoverTextureOFF.width,MoverTextureOFF.height), MoverTextureOFF);
				} else 
				{
					GUI.Label (new Rect (posX + 18 , posY + 71, ProcurarTextureON.width,ProcurarTextureON.height), ProcurarTextureON);
					GUI.Label (new Rect (posX + 18 + 45 , posY  + 71, MoverTextureON.width,MoverTextureOFF.height), MoverTextureOFF);
				}
			} else 	if(monkey.monkeyClass == CMonkey.eMonkeyType.Saboteur){
				GUI.Label (new Rect (posX + 70 , posY + 40, SabotadorNomeTexture.width,SabotadorNomeTexture.height), SabotadorNomeTexture);
				if (monkey.getFSMCurrentState() == CMonkey.FSMState.STATE_CAPTURED)
					GUI.Label (new Rect (posX +15,  posY +11,	SabotadorMonkeyTexturePreso.width,SabotadorMonkeyTexturePreso.height), SabotadorMonkeyTexturePreso);
				else
					GUI.Label (new Rect (posX +15,  posY +11,	SabotadorMonkeyTexture.width,SabotadorMonkeyTexture.height), SabotadorMonkeyTexture);
				if (monkey.getFSMCurrentState() == CMonkey.FSMState.STATE_IDLE){
					GUI.Label (new Rect (posX + 18 , posY + 71, SabotarTextureOFF.width,SabotarTextureOFF.height), SabotarTextureOFF);
					GUI.Label (new Rect (posX + 18 + 45 , posY  + 71, MoverTextureON.width,MoverTextureOFF.height), MoverTextureOFF);
					GUI.Label (new Rect (posX + 18 + 45 + 45, posY + 71, ReprogramarTextureOFF.width,ReprogramarTextureOFF.height), ReprogramarTextureOFF);
				}else if(monkey.getFSMCurrentState() == CMonkey.FSMState.STATE_WALKING){
					GUI.Label (new Rect (posX + 18 , posY + 71, SabotarTextureOFF.width,SabotarTextureOFF.height), SabotarTextureOFF);
					GUI.Label (new Rect (posX + 18 + 45 , posY  + 71, MoverTextureON.width,MoverTextureON.height), MoverTextureON);
					GUI.Label (new Rect (posX + 18 + 45 + 45, posY + 71, ReprogramarTextureOFF.width,ReprogramarTextureOFF.height), ReprogramarTextureOFF);
				}else if(monkey.getFSMCurrentState() == CMonkey.FSMState.STATE_WORKING && monkey.workingMouseState == MouseWorldPosition.eMouseStates.TargetingForReprogram){
					GUI.Label (new Rect (posX + 18 , posY + 71, SabotarTextureOFF.width,SabotarTextureOFF.height), SabotarTextureOFF);
					GUI.Label (new Rect (posX + 18 + 45 , posY  + 71, MoverTextureOFF.width,MoverTextureOFF.height), MoverTextureOFF);
					GUI.Label (new Rect (posX + 18 + 45 + 45, posY + 71, ReprogramarTextureON.width,ReprogramarTextureON.height), ReprogramarTextureON);
				} else if(monkey.getFSMCurrentState() == CMonkey.FSMState.STATE_WORKING && ( monkey.workingMouseState == MouseWorldPosition.eMouseStates.CanSabotageBuilding || monkey.workingMouseState == MouseWorldPosition.eMouseStates.CanSabotageDrone)){
					GUI.Label (new Rect (posX + 18 , posY + 71, SabotarTextureON.width,SabotarTextureON.height), SabotarTextureON);
					GUI.Label (new Rect (posX + 18 + 45 , posY  + 71, MoverTextureON.width,MoverTextureON.height), MoverTextureOFF);
					GUI.Label (new Rect (posX + 18 + 45 + 45, posY + 71, ReprogramarTextureOFF.width,ReprogramarTextureOFF.height), ReprogramarTextureOFF);
				} else 
				{
					GUI.Label (new Rect (posX + 18 , posY + 71, SabotarTextureON.width,SabotarTextureON.height), SabotarTextureOFF);
					GUI.Label (new Rect (posX + 18 + 45 , posY  + 71, MoverTextureON.width,MoverTextureON.height), MoverTextureOFF);
					GUI.Label (new Rect (posX + 18 + 45 + 45, posY + 71, ReprogramarTextureOFF.width,ReprogramarTextureOFF.height), ReprogramarTextureOFF);
				}
			}
			m++;
		}
	}
	
	// Use this for initialization
	void Start () {
		setSlot(1,1);
		nextSlotPosition++;
		
		if (allMonkeys == null)
			allMonkeys = new ArrayList();
		if (boxYposition == null)
			boxYposition = new ArrayList();
		if (yDirection == null)
			yDirection = new ArrayList();
		
		playerScript = GameObject.Find("Player").GetComponent<CPlayer>();
		mainScript = GameObject.Find("Codigo").GetComponent<MainScript>();
		eventosMenu = GameObject.Find("Codigo").GetComponent<EventosMenu>();
		
		mouseWorld = GameObject.Find("Codigo").GetComponent<MouseWorldPosition>();
		
		boxYmax = 600;
		boxYmin = 670;
		
		xDirection = 0;
		allRects = new ArrayList();
	}
	
	// Update is called once per frame
	void Update () {
		show();
		int i = 0;
		foreach( int m in yDirection){
			
			if (   (int) yDirection[i] == 1 && (int) boxYposition[i] < 670){
				boxYposition[i] = (int) boxYposition[i] + (int) yDirection[i] *4;
			}else if ((int) yDirection[i] == -1 && (int) boxYposition[i] > 600){
				boxYposition[i] = (int) boxYposition[i] + (int) yDirection[i] *4;
			}
			i++;
		}
		
		

	}
	
	public void show(){
		int i = 0;
		foreach( CMonkey monkey in allMonkeys){
			if( (monkey.getFSMCurrentState() == CMonkey.FSMState.STATE_IDLE && !monkey.isSelected) || 
				monkey.getFSMCurrentState() == CMonkey.FSMState.STATE_NULL ||
				monkey.getFSMCurrentState() == CMonkey.FSMState.STATE_CAPTURED ||
				(monkey.getFSMCurrentState() == CMonkey.FSMState.STATE_INSIDE_BUILDING 
				&& monkey.getResearchIsComplete()))
				desceSlot(i);
			else 
				sobeSlot(i);
			i++;
		}
	}
	
	private void direitaCentroComando(){
		xDirection = 1;
	}
	private void esquerdaCentroComando(){
		xDirection = -1;
	}
	
	private void sobeSlot(int slotN){
		yDirection[slotN] = -1;
		
	}
	private void desceSlot(int slotN){
		yDirection[slotN] = 1;
		
	}
		
}
