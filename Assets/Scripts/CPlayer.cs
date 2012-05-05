
using System.Collections;

/// <summary>
/// Class with player's stuff definitions
/// </summary>
public class CPlayer : CBaseEntity {

	// PRIVATE
	private float mOxygenLevel = 500.0f;
	private float mMetalLevel = 500.0f;

	// PUBLIC
	public float oxygenLevel {get{return mOxygenLevel;}}
	public float metalLevel {get{return mMetalLevel;}}
	// TODO: add other resources, like hydrogen
	
	/*
	 * ===========================================================================================================
	 * UNITY'S STUFF
	 * ===========================================================================================================
	 */

	/// <summary>
	/// When the script is initialized
	/// </summary>
	void Awake() {

		Selectable = false; // The player is virtual, not a physical object 
		Type = eObjType.Player;
	}

	/// <summary>
	/// Receive a certain amount of metal
	/// </summary>
	/// <param name="amount"> Float with the amount of metal to be added </param>
	public void AddResourceMetal(float amount) {

		mMetalLevel += amount;
	}

	/// <summary>
	/// Receive a certain amount of oxygen
	/// </summary>
	/// <param name="amount"> Float with the amount of oxygen to be added </param>
	public void AddResourceOxygen(float amount) {

		mOxygenLevel += amount;
	}
	
	/// <summary>
	/// Subs the resource metal.
	/// </summary>
	/// <param name ="amount"> Float with the amount of metal to be subtracted </param>
	public void SubResourceMetal(float amount){
		mMetalLevel -= amount;
	}
	
	/// <summary>
	/// Subs the resource metal.
	/// </summary>
	/// <param name ="amount"> Float with the amount of oxygen to be subtracted </param>	
	public void SubResourceOxygen(float amount){
		mOxygenLevel -= amount;
	}	
}
