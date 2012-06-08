using UnityEngine;
using System.Collections;

public class GUIControl : MonoBehaviour {
	
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
	
	//Macaco 2 - Engenheiro
	public Texture2D ReciclarTextureON;
	public Texture2D RepararTextureON;
	public Texture2D ReciclarTextureOFF;
	public Texture2D RepararTextureOFF;
	
	
	//Macaco 3 - Sabotador
	public Texture2D SabotarTextureON;
	public Texture2D SabotarTextureOFF;
	
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
		Debug.LogError("Chamou o set slot com slotnumber " + slotNumber + " e predio " + predio);
		SlotPredios[slotNumber] = predio;
	
		
		
	}
	
	private void drawSlots(){
		int predio = 0;
		int slot = 0;
		while(predio != null && slot != 9){
			slot++;
			predio = SlotPredios[slot];
			if (predio != null){
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
		int boxY = 642;
		int boxYDistance = 172;
		int box5x = 826;
		
		
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
			
			
		GUI.Label (new Rect (box1x,boxY,BoxMacacoTexture.width,BoxMacacoTexture.height), BoxMacacoTexture);
		GUI.Label (new Rect (box1x + boxYDistance, boxY,BoxMacacoTexture.width,BoxMacacoTexture.height), BoxMacacoTexture);
		GUI.Label (new Rect (box1x + boxYDistance * 2 ,boxY,BoxMacacoTexture.width,BoxMacacoTexture.height), BoxMacacoTexture);
		GUI.Label (new Rect (box1x + boxYDistance * 3,boxY,BoxMacacoTexture.width,BoxMacacoTexture.height), BoxMacacoTexture);
		GUI.Label (new Rect (box5x,boxY,BoxMacacoTexture.width,BoxMacacoTexture.height), BoxMacacoTexture);
		
		
		GUI.Label (new Rect (box1x +15,boxY +11,MonkeyTexture.width,MonkeyTexture.height), MonkeyTexture);
		GUI.Label (new Rect (box1x +15 + boxYDistance ,boxY +11,MonkeyTexture.width,MonkeyTexture.height), MonkeyTexture);
		GUI.Label (new Rect (box1x +15 + boxYDistance *2,boxY +11,MonkeyTexture.width,MonkeyTexture.height), MonkeyTexture);
		GUI.Label (new Rect (box1x +15 + boxYDistance *3 ,boxY +11,MonkeyTexture.width,MonkeyTexture.height), MonkeyTexture);
		
		int barraY = 642 + 7;
		
		GUI.Label (new Rect (box1x +79  ,barraY , 84,10), BarraVaziaTexture);
		GUI.Label (new Rect (box1x +79  + boxYDistance,barraY , BarraVaziaTexture.width,BarraVaziaTexture.height), BarraVaziaTexture);
		GUI.Label (new Rect (box1x +79  + boxYDistance *2,barraY , BarraVaziaTexture.width,BarraVaziaTexture.height), BarraVaziaTexture);
		GUI.Label (new Rect (box1x +79  + boxYDistance *3,barraY , BarraVaziaTexture.width,BarraVaziaTexture.height), BarraVaziaTexture);
		
		// Macaco 1 - ASTRONAUTA
		GUI.Label (new Rect (box1x + 18 , boxY + 71, AtaqueTextureON.width,AtaqueTextureON.height), AtaqueTextureON);
		GUI.Label (new Rect (box1x + 18 + 45 , boxY + 71, MoverTextureON.width,MoverTextureON.height), MoverTextureON);
		
		// Macaco 2 - ENGENHEIRO
		GUI.Label (new Rect (box1x + 18 +boxYDistance, boxY + 71, ReciclarTextureON.width,ReciclarTextureON.height), ReciclarTextureON);
		GUI.Label (new Rect (box1x + 18 + 45 +boxYDistance, boxY + 71, RepararTextureON.width,RepararTextureON.height), RepararTextureON);
		GUI.Label (new Rect (box1x + 18 + 45 +45+boxYDistance, boxY + 71, MoverTextureON.width,MoverTextureON.height), MoverTextureON);
		
		// Macaco 3 - ENGENHEIRO
		GUI.Label (new Rect (box1x + 18 + boxYDistance*2, boxY + 71, SabotarTextureON.width,SabotarTextureON.height), SabotarTextureON);
		GUI.Label (new Rect (box1x + 18 + 45 +boxYDistance*2, boxY + 71, RepararTextureON.width,RepararTextureON.height), RepararTextureON);
		GUI.Label (new Rect (box1x + 18 + 45 + 45+boxYDistance*2, boxY + 71, MoverTextureON.width,MoverTextureON.height), MoverTextureON);
		
		
		// Macaco 4 - Cientista
		GUI.Label (new Rect (box1x + 18 + boxYDistance*3, boxY + 71, ProcurarTextureON.width,ProcurarTextureON.height), ProcurarTextureON);
		GUI.Label (new Rect (box1x + 18 + 45+boxYDistance*3, boxY + 71, MoverTextureON.width,MoverTextureON.height), MoverTextureON);
		
		// Fabrica de Drones
		GUI.Label (new Rect (box1x +15 + boxYDistance *3 ,boxY +11,MonkeyTexture.width,MonkeyTexture.height), MonkeyTexture);
		
		GUI.Label (new Rect (box5x + 18 , boxY + 71, DroneCacadorTexture.width,DroneCacadorTexture.height), DroneCacadorTexture);
		GUI.Label (new Rect (box5x + 18 +45, boxY + 71, DroneSabotadorTexture.width,DroneSabotadorTexture.height), DroneSabotadorTexture);
		GUI.Label (new Rect (box5x + 18 +45+45, boxY + 71, DronePatrulhaTexture.width,DronePatrulhaTexture.height), DronePatrulhaTexture);
		
		//GUI.
		
		
		
}
	
	// Use this for initialization
	void Start () {
		setSlot(1,1);
		nextSlotPosition++;
	}
	
	// Update is called once per frame
	void Update () {

	}
}
