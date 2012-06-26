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
	public Texture2D AtaqueTextureON;
	public Texture2D MoverTextureON;
	public Texture2D AtaqueTextureOFF;
	public Texture2D MoverTextureOFF;
	
	private bool modeAtaqueM1 = false;
	private bool modeMoverM1 = false;
	private bool modeMoverM2 = false;
	private bool modeMoverM3 = false;
	private bool modeMoverM4 = false;
	private bool modeReciclarM2 = false;
	private bool modeRepararM2 = false;
	private bool modeSabotarM3 = false;
	private bool modeReprogramarM3 = false;
	private bool modeProcurarM4 = false;
	
	
	//Macaco 2 - Engenheiro
	public Texture2D ReciclarTextureON;
	public Texture2D RepararTextureON;
	public Texture2D ReciclarTextureOFF;
	public Texture2D RepararTextureOFF;
	
	
	//Macaco 3 - Sabotador
	public Texture2D SabotarTextureON;
	public Texture2D SabotarTextureOFF;
	public Texture2D ReprogramarTextureON;
	public Texture2D ReprogramarTextureOFF;
	
	//Macaco 4 - Cientista
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
	int boxY1;
	int boxY2; 
	int boxY3;
	int boxY4; 
	
	int y1dir = 0;
	int y2dir = 0;
	int y3dir = 0;
	int y4dir = 0;
	
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
		
		Debug.Log("NEXT SLOT CHAMADO");

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
	
	
	void OnGUI () {
		int box1x = 11;
		int boxYDistance = 172;
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
			
			
		GUI.Label (new Rect (box1x,boxY1,BoxMacacoTexture.width,BoxMacacoTexture.height), BoxMacacoTexture);
		GUI.Label (new Rect (box1x + boxYDistance, boxY2,BoxMacacoTexture.width,BoxMacacoTexture.height), BoxMacacoTexture);
		GUI.Label (new Rect (box1x + boxYDistance * 2 ,boxY3,BoxMacacoTexture.width,BoxMacacoTexture.height), BoxMacacoTexture);
		GUI.Label (new Rect (box1x + boxYDistance * 3,boxY4,BoxMacacoTexture.width,BoxMacacoTexture.height), BoxMacacoTexture);
		GUI.Label (new Rect (box5x,boxY4,BoxMacacoTexture.width,BoxMacacoTexture.height), BoxMacacoTexture);
		
		
		GUI.Label (new Rect (box1x +15, boxY1 +11,MonkeyTexture.width,MonkeyTexture.height), MonkeyTexture);
		GUI.Label (new Rect (box1x +15 + boxYDistance ,boxY2 +11,MonkeyTexture.width,MonkeyTexture.height), MonkeyTexture);
		GUI.Label (new Rect (box1x +15 + boxYDistance *2,boxY3 +11,MonkeyTexture.width,MonkeyTexture.height), MonkeyTexture);
		GUI.Label (new Rect (box1x +15 + boxYDistance *3 ,boxY4 +11,MonkeyTexture.width,MonkeyTexture.height), MonkeyTexture);
		
		//int barraY = 642 + 7;
		
		GUI.Label (new Rect (box1x +79  , boxY1+7 , 84,10), BarraVaziaTexture);
		GUI.Label (new Rect (box1x +79  + boxYDistance,boxY2+7 , BarraVaziaTexture.width,BarraVaziaTexture.height), BarraVaziaTexture);
		GUI.Label (new Rect (box1x +79  + boxYDistance *2,boxY3+7 , BarraVaziaTexture.width,BarraVaziaTexture.height), BarraVaziaTexture);
		GUI.Label (new Rect (box1x +79  + boxYDistance *3,boxY4+7 , BarraVaziaTexture.width,BarraVaziaTexture.height), BarraVaziaTexture);
		
		// Macaco 1 - ASTRONAUTA
		if(modeAtaqueM1)
			GUI.Label (new Rect (box1x + 18 , boxY1 + 71, AtaqueTextureON.width,AtaqueTextureON.height), AtaqueTextureON);
		else
			GUI.Label (new Rect (box1x + 18 , boxY1 + 71, AtaqueTextureON.width,AtaqueTextureON.height), AtaqueTextureOFF);
		
		if(modeMoverM1)
			GUI.Label (new Rect (box1x + 18 + 45 , boxY1 + 71, MoverTextureON.width,MoverTextureON.height), MoverTextureON);
		else
			GUI.Label (new Rect (box1x + 18 + 45 , boxY1 + 71, MoverTextureON.width,MoverTextureON.height), MoverTextureOFF);
		
		// Macaco 2 - ENGENHEIRO
		if(modeReciclarM2)
			GUI.Label (new Rect (box1x + 18 +boxYDistance, boxY2 + 71, ReciclarTextureON.width,ReciclarTextureON.height), ReciclarTextureON);
		else
			GUI.Label (new Rect (box1x + 18 +boxYDistance, boxY2 + 71, ReciclarTextureON.width,ReciclarTextureON.height), ReciclarTextureOFF);
		if(modeRepararM2)
			GUI.Label (new Rect (box1x + 18 + 45 +boxYDistance, boxY2 + 71, RepararTextureON.width,RepararTextureON.height), RepararTextureON);
		else
			GUI.Label (new Rect (box1x + 18 + 45 +boxYDistance, boxY2 + 71, RepararTextureON.width,RepararTextureON.height), RepararTextureOFF);
		if(modeMoverM2)
			GUI.Label (new Rect (box1x + 18 + 45 +45+boxYDistance, boxY2 + 71, MoverTextureON.width,MoverTextureON.height), MoverTextureON);
		else
			GUI.Label (new Rect (box1x + 18 + 45 +45+boxYDistance, boxY2 + 71, MoverTextureON.width,MoverTextureON.height), MoverTextureOFF);
		
		// Macaco 3 - SABOTADOR
		if(modeSabotarM3)
			GUI.Label (new Rect (box1x + 18 + boxYDistance*2, boxY3 + 71, SabotarTextureON.width,SabotarTextureON.height), SabotarTextureON);
		else
			GUI.Label (new Rect (box1x + 18 + boxYDistance*2, boxY3 + 71, SabotarTextureON.width,SabotarTextureON.height), SabotarTextureOFF);
		if(modeReprogramarM3)
			GUI.Label (new Rect (box1x + 18 + 45 +boxYDistance*2, boxY3 + 71, ReprogramarTextureON.width,ReprogramarTextureON.height), ReprogramarTextureON);
		else
			GUI.Label (new Rect (box1x + 18 + 45 +boxYDistance*2, boxY3 + 71, ReprogramarTextureON.width,ReprogramarTextureON.height), ReprogramarTextureOFF);
		if(modeMoverM3)
			GUI.Label (new Rect (box1x + 18 + 45 + 45+boxYDistance*2, boxY3 + 71, MoverTextureON.width,MoverTextureON.height), MoverTextureON);
		else
			GUI.Label (new Rect (box1x + 18 + 45 + 45+boxYDistance*2, boxY3 + 71, MoverTextureON.width,MoverTextureON.height), MoverTextureOFF);
		
		// Macaco 4 - Cientista
		if (modeProcurarM4)
			GUI.Label (new Rect (box1x + 18 + boxYDistance*3, boxY4 + 71, ProcurarTextureON.width,ProcurarTextureON.height), ProcurarTextureON);
		else
			GUI.Label (new Rect (box1x + 18 + boxYDistance*3, boxY4 + 71, ProcurarTextureON.width,ProcurarTextureON.height), ProcurarTextureOFF);
		if (modeMoverM4)
			GUI.Label (new Rect (box1x + 18 + 45+boxYDistance*3, boxY4 + 71, MoverTextureON.width,MoverTextureON.height), MoverTextureON);
		else
			GUI.Label (new Rect (box1x + 18 + 45+boxYDistance*3, boxY4 + 71, MoverTextureON.width,MoverTextureON.height), MoverTextureOFF);
		
		// Fabrica de Drones
		GUI.Label (new Rect (box1x +15 + boxYDistance *3 ,boxY4 +11,MonkeyTexture.width,MonkeyTexture.height), MonkeyTexture);
		
		GUI.Label (new Rect (box5x + 18 , boxY4 + 71, DroneCacadorTexture.width,DroneCacadorTexture.height), DroneCacadorTexture);
		GUI.Label (new Rect (box5x + 18 +45, boxY4 + 71, DroneSabotadorTexture.width,DroneSabotadorTexture.height), DroneSabotadorTexture);
		GUI.Label (new Rect (box5x + 18 +45+45, boxY4 + 71, DronePatrulhaTexture.width,DronePatrulhaTexture.height), DronePatrulhaTexture);
		
		
}
	
	// Use this for initialization
	void Start () {
		setSlot(1,1);
		nextSlotPosition++;
		
		playerScript = GameObject.Find("Player").GetComponent<CPlayer>();
		
	boxYmax = 542;
	boxYmin = 642;
	boxY1 = boxYmin;
	boxY2 = boxYmin; 
	boxY3 = boxYmin;
	boxY4 = boxYmin; 
	}
	
	// Update is called once per frame
	void Update () {
		
		if (y1dir != 0 && boxY1 > boxYmin && boxY1 < boxYmax)
			boxY1 = boxY1+ y1dir;
		if (y2dir != 0 && boxY2 > boxYmin && boxY2 < boxYmax)
			boxY1 = boxY2+ y2dir;
		if (y3dir != 0 && boxY3 > boxYmin && boxY3 < boxYmax)
			boxY1 = boxY3+ y3dir;
		if (y4dir != 0 && boxY4 > boxYmin && boxY4 < boxYmax)
			boxY1 = boxY4+ y4dir;

	}
	
	public void setMover(bool liga, CMonkey.eMonkeyType macaco){
		if (macaco == CMonkey.eMonkeyType.Astronaut){
			if (liga) y1dir = -1; else y1dir = 1;
			modeMoverM1 = liga;
			modeAtaqueM1 = false;
		}else if(macaco == CMonkey.eMonkeyType.Engineer){
			if (liga) y2dir = -1; else y2dir = 1;
			modeMoverM2 = liga;
			modeReciclarM2 = false;
		}else if(macaco == CMonkey.eMonkeyType.Saboteur){
			if (liga) y3dir = -1; else y3dir = 1;
			modeMoverM3 = liga;
			modeReprogramarM3 = false;
		}else if(macaco == CMonkey.eMonkeyType.Cientist){
			if (liga) y4dir = -1; else y4dir = 1;
			modeMoverM4 = liga;
			modeProcurarM4 = false;
		}
		Debug.Log("SET mover = " + liga );
	}
	
	public void setAtacar(bool liga, CMonkey.eMonkeyType macaco){
		if (macaco == CMonkey.eMonkeyType.Astronaut){
			modeMoverM1 = !liga;
			modeAtaqueM1 = liga;
		}else if (macaco == CMonkey.eMonkeyType.Engineer){
			setReparar(true);
		}else if (macaco == CMonkey.eMonkeyType.Saboteur){
			setSabotar(true);
		}
		
		
	}
	public void setReparar(bool liga){
			modeRepararM2 = liga;
			modeMoverM2 = false;
			modeReciclarM2 = false;
		
	}
	public void setReciclar(bool liga){
			modeReciclarM2 = liga;
			modeMoverM2 = false;
			modeRepararM2 = false;
	}
	
	public void setSabotar(bool liga){
			modeSabotarM3 = liga;	
			modeMoverM3 = false;
			modeSabotarM3 = false;
	}
	public void setProcurar(bool liga){
			modeProcurarM4 = liga;
			modeMoverM4 = false;
	}
}
