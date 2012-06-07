using UnityEngine;
using System.Collections;

public class GUIControl : MonoBehaviour {
	
	public Texture2D BoxTexture;
	public Texture2D MonkeyTexture;
	public Texture2D BarraVaziaTexture;
	public Texture2D AtaqueTextureON;
	public Texture2D MoverTextureON;
	
	
	void OnGUI () {
		int box1x = 11;
		int boxY = 642;
		int boxYDistance = 172;
		int box5x = 826;
		
		GUI.Label (new Rect (box1x,boxY,BoxTexture.width,BoxTexture.height), BoxTexture);
		GUI.Label (new Rect (box1x + boxYDistance, boxY,BoxTexture.width,BoxTexture.height), BoxTexture);
		GUI.Label (new Rect (box1x + boxYDistance * 2 ,boxY,BoxTexture.width,BoxTexture.height), BoxTexture);
		GUI.Label (new Rect (box1x + boxYDistance * 3,boxY,BoxTexture.width,BoxTexture.height), BoxTexture);
		GUI.Label (new Rect (box5x,boxY,BoxTexture.width,BoxTexture.height), BoxTexture);
		
		
		GUI.Label (new Rect (box1x +15,boxY +11,MonkeyTexture.width,MonkeyTexture.height), MonkeyTexture);
		GUI.Label (new Rect (box1x +15 + boxYDistance ,boxY +11,MonkeyTexture.width,MonkeyTexture.height), MonkeyTexture);
		GUI.Label (new Rect (box1x +15 + boxYDistance *2,boxY +11,MonkeyTexture.width,MonkeyTexture.height), MonkeyTexture);
		GUI.Label (new Rect (box1x +15 + boxYDistance *3 ,boxY +11,MonkeyTexture.width,MonkeyTexture.height), MonkeyTexture);
		
		int barraY = 642 + 7;
		
		GUI.Label (new Rect (box1x +79  ,barraY , 84,10), BarraVaziaTexture);
		GUI.Label (new Rect (box1x +79  + boxYDistance,barraY , BarraVaziaTexture.width,BarraVaziaTexture.height), BarraVaziaTexture);
		GUI.Label (new Rect (box1x +79  + boxYDistance *2,barraY , BarraVaziaTexture.width,BarraVaziaTexture.height), BarraVaziaTexture);
		GUI.Label (new Rect (box1x +79  + boxYDistance *3,barraY , BarraVaziaTexture.width,BarraVaziaTexture.height), BarraVaziaTexture);
		
		
		GUI.Label (new Rect (box1x + 18 , boxY + 71, AtaqueTextureON.width,AtaqueTextureON.height), AtaqueTextureON);
		GUI.Label (new Rect (box1x + 18 + 45 , boxY + 71, MoverTextureON.width,MoverTextureON.height), MoverTextureON);
}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}





