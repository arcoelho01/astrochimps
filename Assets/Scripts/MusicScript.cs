using UnityEngine;
using System.Collections;

public class MusicScript : MonoBehaviour {
	
	public GameObject now;
	private BuildingLaunchingPlatform launch;
	private CPlayer player;
	GameObject trilha130;
	GameObject trilha140;
	GameObject trilha150;
	GameObject oxy130;
	GameObject oxy140;
	GameObject oxy150;
	public AudioClip sfxNivelOxigenioBaixo;
	bool avisou;
	
	// Use this for initialization
	 void Start() {
       	launch = GameObject.Find("LaunchingPlatform").GetComponent<BuildingLaunchingPlatform>();
		player = GameObject.Find("Player").GetComponent<CPlayer>();
		trilha130 = GameObject.Find("trilha_simples-130BPM");
		trilha140 = GameObject.Find("trilha_simples-140BPM");
		trilha150 = GameObject.Find("trilha_simples-150BPM");
		oxy130 = GameObject.Find("OXI-130BPM");
		oxy140 = GameObject.Find("OXI-140BPM");
		oxy150 = GameObject.Find("OXI-150BPM");
		
		
		trilha130.audio.Play();
		trilha140.audio.Play();
		trilha150.audio.Play();
		oxy130.audio.Play();
		oxy140.audio.Play();
		oxy150.audio.Play();
		avisou = false;
		muteAll();
		
    }
	
	void Awake(){

		
		
	}

	void Update () {
		
		if (player.oxygenLevel < 100){
			if (!avisou){
				AudioSource.PlayClipAtPoint(sfxNivelOxigenioBaixo,new Vector3(0,0,0));
				avisou = true;
			}
			muteAll();
			if (launch.getPartsOnTheRocket() <= 1 && player.oxygenLevel < 99){
				oxy130.audio.mute = false;
			}else
			if (launch.getPartsOnTheRocket() <= 3  && player.oxygenLevel < 99){
				oxy140.audio.mute = false;
			}else
			if (launch.getPartsOnTheRocket() <= 5  && player.oxygenLevel < 99){
				oxy140.audio.mute = false;
			}
		}
		else{
			avisou = false;
			if (launch.getPartsOnTheRocket() <= 1){
				muteAll();
				trilha130.audio.mute = false;
			}else
			if (launch.getPartsOnTheRocket() <= 3){
				muteAll();
				trilha140.audio.mute = false;
			}else
			if (launch.getPartsOnTheRocket() <= 5){
				muteAll();
				trilha150.audio.mute = false;
			}
		}
	}
	
	void muteAll(){
		
		trilha130.audio.mute = true;
		trilha140.audio.mute = true;
		trilha150.audio.mute = true;
		oxy130.audio.mute = true;
		oxy140.audio.mute = true;
		oxy150.audio.mute = true;
		
	}
}
