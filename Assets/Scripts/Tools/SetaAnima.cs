using UnityEngine;
using System.Collections;

public class SetaAnima : MonoBehaviour {
	
	Animation Seta_ANI;
	bool already_Started = false;
	public GameObject Seta = null; 
	
	// Use this for initialization
	void Start () {
		
		Seta_ANI = Seta.GetComponent<Animation>().animation; // Passa a pista completa de animação para a variável
		Seta_ANI.Play("ArrowAppear"); // Toca a animação I (Erguendo a caixa)
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if ( !Seta_ANI.isPlaying )
			Seta_ANI.Play("ArrowFloat");
	
	}
}
