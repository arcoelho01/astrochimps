using UnityEngine;
using System.Collections;

/// <summary>
/// This class will control the texture showed when the cursor is active or inactive. It will be useful when
/// showing the player were he can build a structure or not (green and red cursor, for instance)
/// </summary>
public class CursorProjectorControl : MonoBehaviour {

	// PUBLIC
	public Material selectActiveMaterial = null; // Put a material for the 'active' state when selecting a target
	public Material selectInactiveMaterial = null; // Put a material for the 'inactive' state when selecting a target
	public Material getInsideActiveMaterial = null; // Put a material for the 'active' state when is possible to
																									// get inside a building
	public Material getInsideInactiveMaterial = null; // Put a material for the 'inactive' state when is possible
																										// to get out of a building

	public enum eCursorType {	SelectTarget, GetInside};
	public eCursorType cursorType;
	
	// PRIVATE
	bool bnActive = true;
	Projector projector;

	void Awake() {

		// Just checking
		if(selectActiveMaterial == null || selectInactiveMaterial == null ||
				getInsideActiveMaterial == null || getInsideInactiveMaterial == null) {

			// DEBUG
			Debug.Log("Texture not defined. Move it to the inspector.");
		}

		projector = GetComponent<Projector>();
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Changes the projector to the 'active' state, adjusting the boolean flag and changing the material
	/// </summary>
	/// <param name="useSelectCursor"> A boolean that indicates if we use the regular select cursor in the
	/// projector (true), or use the get inside/outside building cursor (false)</param>
	/// <param name="newState"> Indicates the state of the cursor, so it's use the corresponding material in
	/// the projector. True use the active material, false use the inactive material</param>
	public void SetState(bool useSelectCursor, bool newState) {

		/*
		if(bnActive == newState)
			return;
		//*/

		bnActive = newState;

		if(useSelectCursor) {
	
			projector.material = (bnActive == true) ? selectActiveMaterial : selectInactiveMaterial;
		}
		else {

			projector.material = (bnActive == true) ? getInsideActiveMaterial : getInsideInactiveMaterial;
		}
	}
}
