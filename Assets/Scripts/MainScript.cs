using UnityEngine;
using System.Collections;

public class MainScript : MonoBehaviour {

	// PUBLIC
	public Transform prefabExtractor;
	public Transform prefabFazenda;
	public Transform prefabLaboratorio;
	public Transform prefabGaragem;

	public GUIBottomMenu bottomMenu;	// Pointer to the bottom menu bar
	public Transform playerObject;	// Pointer to the player object
	public CPlayer player;

	public bool isMinimapEnabled = true;	// Starts with the minimap enabled

	// Index of all layers
	public static int groundLayer = 8;
	public static int alliedLayer = 10;	
	public static int enemyLayer = 11;	
	public static int neutralLayer = 12;	
	public static int minimapLayer = 13;	
	public static int minimapGroundLayer = 14;	

	// Use this for initialization
	void Start () {
	
		bottomMenu = GetComponent<GUIBottomMenu>();
		if(!bottomMenu) {

			// DEBUG
			Debug.LogError("Bottom menu object not found!");
		}

		player = playerObject.GetComponent<CPlayer>();
		if(!player) {

			// DEBUG
			Debug.LogError("Player object not found!");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Build a extractor and associate it to the caller (a resource site)
	/// </summary>
	/// <param name="position">A Vector3 with the position to build the extractor</param>
	/// <returns> Transform of the instantiated extractor </returns>
	public Transform BuildExtractor(Vector3 position) {

		// DEBUG
		Debug.Log("Starting building extractor");

		position.y += prefabExtractor.transform.localScale.y;

		Transform extractorClone = Instantiate(prefabExtractor, position, Quaternion.identity) 
			as Transform;

		return extractorClone;
	}
}
