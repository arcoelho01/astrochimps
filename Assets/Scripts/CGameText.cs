using UnityEngine;
using System.Collections;

/// <summary>
/// Class made to hold all text that will be displayed in the game
/// </summary>
public class CGameText : MonoBehaviour {

	// All the possible text messagens in the game
	public enum eTextMessages {

		MONKEY_ASTRONAUT,
		MONKEY_CIENTIST,
		MONKEY_ENGINEER,
		MONKEY_SABOTEUR,
		RESOURCE_METAL,
		RESOURCE_OXYGEN,
		BUTTON_BUILD_COMMAND_CENTER,
		BUTTON_BUILD_RESOURCE_EXTRACTOR,
		BUTTON_BUILD_FARM,
		BUTTON_BUILD_SECURITY_CENTER,
		BUTTON_BUILD_DRONE_FACTORY,
		BUTTON_BUILD_RESEARCH_LAB,
		NONE
	}

	// Available languages
	public enum eTextLanguages {

		EN,			// English
		PT_BR		// Portugues brasileiro
	}

	public static CGameText Script;

	// The language of the game
	public eTextLanguages textLanguage;

	/*
	 * ===========================================================================================================
	 * UNITY'S STUFF
	 * ===========================================================================================================
	 */

	void Awake() {

		Script = this;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Search and returns a string in the selected language
	/// </summary>
	/// <param name="eString"> A eTextMessages enum with the type of message to be searched </param>
	public string GetText(eTextMessages eString) {

		string returnString = "";

		switch(eString) {

			case eTextMessages.MONKEY_ASTRONAUT:
				{
					// EN - PT_BR
					string[] text = { "Astronaut", "Astronauta" };
					returnString = text[(int)textLanguage];
				}
				break;

			case eTextMessages.MONKEY_CIENTIST:
				{
					// EN - PT_BR
					string[] text = { "Cientist", "Cientista" };
					returnString = text[(int)textLanguage];
				}
				break;

			case eTextMessages.MONKEY_ENGINEER:
				{
					// EN - PT_BR
					string[] text = { "Engineer", "Engenheiro" };
					returnString = text[(int)textLanguage];
				}
				break;

			case eTextMessages.MONKEY_SABOTEUR:
				{
					// EN - PT_BR
					string[] text = { "Saboteur", "Sabotador" };
					returnString = text[(int)textLanguage];
				}
				break;

			case eTextMessages.RESOURCE_METAL:
				{
					// EN - PT_BR
					string[] text = { "METAL", "METAL" };
					returnString = text[(int)textLanguage];
				}
				break;

			case eTextMessages.RESOURCE_OXYGEN:
				{
					// EN - PT_BR
					string[] text = { "OXYGEN", "OXIGÊNIO" };
					returnString = text[(int)textLanguage];
				}
				break;

			// BUILDINGS
			case eTextMessages.BUTTON_BUILD_COMMAND_CENTER:
				{
					// EN - PT_BR
					string[] text = { "Command Center", "Centro de Comando" };
					returnString = text[(int)textLanguage];
				}
				break;

			case eTextMessages.BUTTON_BUILD_RESOURCE_EXTRACTOR:
				{
					// EN - PT_BR
					string[] text = { "Resorce Extractor", "Extrator de Recursos" };
					returnString = text[(int)textLanguage];
				}
				break;

			case eTextMessages.BUTTON_BUILD_FARM:
				{
					// EN - PT_BR
					string[] text = { "Hydroponic Farm", "Fazenda Hidropônica" };
					returnString = text[(int)textLanguage];
				}
				break;

			case eTextMessages.BUTTON_BUILD_SECURITY_CENTER:
				{
					// EN - PT_BR
					string[] text = { "Security Center", "Centro de Segurança" };
					returnString = text[(int)textLanguage];
				}
				break;

			case eTextMessages.BUTTON_BUILD_DRONE_FACTORY:
				{
					// EN - PT_BR
					string[] text = { "Drone Factory", "Fábrica de Drones" };
					returnString = text[(int)textLanguage];
				}
				break;

			case eTextMessages.BUTTON_BUILD_RESEARCH_LAB:
				{
					// EN - PT_BR
					string[] text = { "Research Lab", "Laboratório de Pesquisa" };
					returnString = text[(int)textLanguage];
				}
				break;




			default:
				break;
		}

		return returnString;
	}
}
