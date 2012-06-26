using UnityEngine;
using System.Collections;

public class GUIControl : MonoBehaviour {
	
	CPlayer playerScript;
	
	public Texture2D BoxMacacoTexture;
	public Texture2D BoxMapaTexture;
	
	public Texture2D MonkeyTexture;
	public Texture2D BarraVaziaTexture;
	public Texture2D BoxMetalTexture;
	public Texture2D BoxOxigenioTexture;
	public Texture2D ReguaOxigenioTexture;
	public Texture2D NivelOxigenioTexture;
	
	public Texture2D RadarTextureON;
	public Texture2D RadarTextureOFF;
	
	public Texture2D MenuTextureON;
	public Texture2D MenuTextureOFF;
	
	//Macaco 1 - Astronauta
	public Texture2D AstronautaNomeTexture;
	public Texture2D AtaqueTextureON;
	public Texture2D MoverTextureON;
	public Texture2D AtaqueTextureOFF;
	public Texture2D MoverTextureOFF;
	
	private ArrayList allMonkeys;

	
	//Macaco 2 - Engenheiro
	public Texture2D EngenheiroNomeTexture;
	public Texture2D ReciclarTextureON;
	public Texture2D RepararTextureON;
	public Texture2D ReciclarTextureOFF;
	public Texture2D RepararTextureOFF;
	
	
	//Macaco 3 - Sabotador
	public Texture2D SabotadorNomeTexture;
	public Texture2D SabotarTextureON;
	public Texture2D SabotarTextureOFF;
	public Texture2D ReprogramarTextureON;
	public Texture2D ReprogramarTextureOFF;
	
	//Macaco 4 - Cientista
	public Texture2D CientistaNomeTexture;
	public Texture2D ProcurarTextureON;
	public Texture2D ProcurarTextureOFF;
	
	// FABRICA
	public Texture2D FabricaTexture;
	public Texture2D DroneCacadorTexture;
	public Texture2D DroneSabotadorTexture;
	public Texture2D DronePatrulhaTexture;
	
	public float NivelOxigenioFloat = 0.5f;
	private int[] SlotPredios = {0,0,0,0,0,0,0,0,0,0};

	int boxYmax;
	int boxYmin;
	ArrayList boxYposition;
	ArrayList yDirection;
	
	public Texture2D ComandoOxTexture;
	public Texture2D LabOxTexture;
	public Texture2D SegurancaOxTexture;
	public Texture2D FazendaOxTexture;
	public Texture2D FabricaOxTexture;
	
	private int nextSlotPosition = 0;
	
	private bool callUpdate = false;
	
	
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
	
	void OnGUI () {
		int box1x = 11;
		int boxXDistance = 172;
		int box5x = 826;
		NivelOxigenioFloat = playerScript.oxygenLevel / 500;

		
		GUI.Label (new Rect (8,4,BoxMapaTexture.width,BoxMapaTexture.height), BoxMapaTexture);
		GUI.Label (new Rect (150,10,RadarTextureON.width*3,RadarTextureON.height*3), RadarTextureON);
		
		GUI.Label (new Rect (950,10,MenuTextureON.width *3,MenuTextureON.height*3), MenuTextureON);
		
		GUI.Label (new Rect (210,5,BoxMetalTexture.width,BoxMetalTexture.height), BoxMetalTexture);
		GUI.Label (new Rect (380,5,BoxOxigenioTexture.width,BoxOxigenioTexture.height), BoxOxigenioTexture);
		
		
		
		for ( int x = 556 ; x < 560 + (NivelOxigenioFloat * 280); x = x +4){
			GUI.Label (new Rect (x,16,4,38), NivelOxigenioTexture);
		}
			
		GUI.Label (new Rect (550,10,ReguaOxigenioTexture.width,ReguaOxigenioTexture.height), ReguaOxigenioTexture);
		
		
		drawSlots();
			
		GUI.Label (new Rect (box5x,boxYmax,BoxMacacoTexture.width,BoxMacacoTexture.height), BoxMacacoTexture);
		
		
		int m = 0;
		foreach( CMonkey monkey in allMonkeys){
			
			int posX = box1x + boxXDistance * m;
			int posY = (int) getYposition(m);
			//Debug.Log("Macaco " + m + " Tipo " + monkey.monkeyClass);
			GUI.Label (new Rect (posX + 79, posY+7 , 	BarraVaziaTexture.width,BarraVaziaTexture.height), BarraVaziaTexture);
			GUI.Label (new Rect (posX, 		posY,		BoxMacacoTexture.width,BoxMacacoTexture.height), BoxMacacoTexture);
			GUI.Label (new Rect (posX +15,  posY +11,	MonkeyTexture.width,MonkeyTexture.height), MonkeyTexture);
				
			if(monkey.monkeyClass == CMonkey.eMonkeyType.Astronaut){
				GUI.Label (new Rect (posX + 70 , posY + 40, AstronautaNomeTexture.width,AstronautaNomeTexture.height), AstronautaNomeTexture);
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
		
	boxYmax = 600;
	boxYmin = 670;
			
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
	
	private void sobeSlot(int slotN){
		yDirection[slotN] = -1;
		
	}
	private void desceSlot(int slotN){
		yDirection[slotN] = 1;
		
	}
		
}
