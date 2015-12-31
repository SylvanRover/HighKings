using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Map.Events;

public class TileStats : MonoBehaviour {

	public string tileType;
	public int tileLevel;
	public int tileLevelMin = 1;
	public int tileLevelMax = 35;
	public string resourceType;
	private int resourceTypeToInt;
	public Text resourceText;
	public Text levelText;
	public Image levelImage;
	public Color easyLevelImageColor;
	public Color mediumLevelImageColor;
	public Color hardLevelImageColor;
	public Text typeText;
	public Text timeText;
	public Image interactButton;
	public Color interactButtonColor;

	public MakePrefabAppear spawnUI;
	public Transform infoPanelObj;
	public GUI_Popup infoPanel;
	private bool UIspawned = false;

	private GameObject homeObj;
	private CommanderMGMT cMGMT;
	private int cLvl;
	private int cLvlMin;
	private int cLvlMax;
	public int cLvlBanding = 2;

	private FilterSystem filter;
	public Image minimapIcon;
	private float iconOff = 0f;
	private float iconOn = 1f;
	public GameObject selectionRing;
	public EnemyCampCollision eCol;

	private float timer;
	private float hours;
	private float minutes;
	private float seconds;


	void Start(){

		tileLevel = Random.Range (tileLevelMin, tileLevelMax);
		levelText.text = tileLevel.ToString();

		if (tileType == "PvE") {
			resourceTypeToInt = Random.Range (0, 3);
			typeText.text = "Orc Camp";
			interactButton.color = interactButtonColor;
		} else if (tileType == "PvP"){			
			resourceTypeToInt = Random.Range (0, 4);
			typeText.text = "Enemy Player";
			interactButton.color = interactButtonColor;
		} else if (tileType == "Resource"){			
			resourceTypeToInt = Random.Range (0, 4);
			typeText.text = "Resource Stockpile";
			interactButton.color = interactButtonColor;
		}

		if (resourceType == "Food" || resourceType == "food") {
			resourceTypeToInt = 0;
		} else if (resourceType == "Stone" || resourceType == "stone") {
			resourceTypeToInt = 1;			
		} else if (resourceType == "Ore" || resourceType == "ore") {
			resourceTypeToInt = 2;			
		} else if (resourceType == "Timber" || resourceType == "Wood" || resourceType == "timber" || resourceType == "wood") {
			resourceTypeToInt = 3;			
		} else if (resourceType == "Silver" || resourceType == "silver") {
			resourceTypeToInt = 4;			
		} else if (resourceType == "Gold" || resourceType == "Premium" || resourceType == "gold" || resourceType == "premium") {
			resourceTypeToInt = 5;		
		}

		switch (resourceTypeToInt) {
		case 5: 
			resourceType = "Gold";
			break;
		case 4: 
			resourceType = "Silver";
			break;
		case 3: 
			resourceType = "Timber";
			break;
		case 2: 
			resourceType = "Ore";
			break;
		case 1: 
			resourceType = "Stone";
			break;
		case 0: 
			resourceType = "Food";
			break;
		default:
			resourceType = "Invalid Resource Type";
			break;
		}

		if (resourceType == "Invalid Resource Type") {
			resourceText.text = resourceType;
		} else {
			resourceText.text = (resourceType + " Present");
		}

		infoPanelObj = spawnUI._where.GetChild (0);
		infoPanel = infoPanelObj.GetComponent<GUI_Popup> ();

		homeObj = GameObject.FindWithTag ("Home");
		cMGMT = homeObj.GetComponent<CommanderMGMT> ();
		filter = homeObj.GetComponent<FilterSystem> ();
		
		filter.globalFilter = false;
		filter.ghostFilter = true;

	}
	
	void Update() {

		if (cLvl != null && cMGMT != null) {
			cLvl = cMGMT.commanderButtonStats.commanderLevel;
		}

		// Time to Target
		if (cMGMT != null && timeText != null) {
			
			timer = cMGMT.commanderButtonStats.progressLine.timeToDist;
			minutes = Mathf.Floor (timer / 60);
			seconds = Mathf.RoundToInt (timer % 60);
			
			if (minutes > 0){
				timeText.text = (minutes + "m " + seconds + "s");
			} else {
				timeText.text = (seconds + "s");
			}
		}

		// Difficulty Colors
		if (tileLevel < (cLvl - cLvlBanding)) {
			levelImage.color = easyLevelImageColor;
		} else if (tileLevel > (cLvl + cLvlBanding)) {
			levelImage.color = hardLevelImageColor;			
		} else {			
			levelImage.color = mediumLevelImageColor;
		}

		// Global Filtering
		if (filter.globalFilter) {
			cLvlMin = cMGMT.cLvlGlobLow;
			cLvlMax = cMGMT.cLvlGlobHigh;
		} else {			
			cLvlMin = cLvl;
			cLvlMax = cLvl;
		}

		// PvP filtering
		if (tileType != null && tileType == "PvP" && !filter.pvpShowAll) {
			if ((cLvlMin - filter.currentPvpMin) > tileLevel) {
				Color c = minimapIcon.color;
				if (filter.ghostFilter){
					c.a = 0.5f;
					selectionRing.layer = LayerMask.NameToLayer ("UI");
				} else {					
					c.a = 0f;
					selectionRing.layer = LayerMask.NameToLayer ("UIWorld");
				}
				minimapIcon.color = c;
			}else if ((cLvlMax + filter.currentPvpMax) < tileLevel) {
				Color c = minimapIcon.color;
				if (filter.ghostFilter){
					c.a = 0.5f;
					selectionRing.layer = LayerMask.NameToLayer ("UI");
				} else {					
					c.a = 0f;
					selectionRing.layer = LayerMask.NameToLayer ("UIWorld");
				}
				minimapIcon.color = c;
			} else if (eCol != null && eCol.anim.GetBool ("Destroyed") == true) {
				Color c = minimapIcon.color;
				c.a = 0.5f;
				minimapIcon.color = c;
				selectionRing.layer = LayerMask.NameToLayer ("UI");
			} else {
				Color c = minimapIcon.color;
				c.a = 1f;
				minimapIcon.color = c;
				selectionRing.layer = LayerMask.NameToLayer ("UI");
			}
		}

		if (filter.pvpShowAll) {
			Color c = minimapIcon.color;
			c.a = 1f;
			minimapIcon.color = c;
			selectionRing.layer = LayerMask.NameToLayer ("UI");
		}

		// PvE filtering
		if (tileType != null && tileType == "PvE" && !filter.pveShowAll) {
			if ((cLvlMin - filter.currentPvpMin) > tileLevel) {
				Color c = minimapIcon.color;
				if (filter.ghostFilter) {
					c.a = 0.5f;
					selectionRing.layer = LayerMask.NameToLayer ("UI");
				} else {					
					c.a = 0f;
					selectionRing.layer = LayerMask.NameToLayer ("UIWorld");
				}
				minimapIcon.color = c;
			} else if ((cLvlMax + filter.currentPvpMax) < tileLevel) {
				Color c = minimapIcon.color;
				if (filter.ghostFilter) {
					c.a = 0.5f;
					selectionRing.layer = LayerMask.NameToLayer ("UI");
				} else {					
					c.a = 0f;
					selectionRing.layer = LayerMask.NameToLayer ("UIWorld");
				}
				minimapIcon.color = c;
			} else if (eCol != null && eCol.anim.GetBool ("Destroyed") == true) {
				Color c = minimapIcon.color;
				c.a = 0.5f;
				minimapIcon.color = c;
				selectionRing.layer = LayerMask.NameToLayer ("UI");
			} else {
				Color c = minimapIcon.color;
				c.a = 1f;
				minimapIcon.color = c;
				selectionRing.layer = LayerMask.NameToLayer ("UI");
			}
		}

		if (filter.pveShowAll) {
			Color c = minimapIcon.color;
			c.a = 1f;
			minimapIcon.color = c;
			selectionRing.layer = LayerMask.NameToLayer ("UI");
		}
	}
	
	/*void OnGUI() {
		GUI.Label (new Rect (20, 180, 220, 50), "Commander Level Min is: " + (cLvlMin));
		GUI.Label (new Rect (20, 200, 220, 50), "Commander Level Max is: " + (cLvlMax));
		GUI.Label (new Rect (20, 220, 220, 50), "Filter Minimum is: " + (cLvlMin - filter.currentPvpMin));
		GUI.Label (new Rect (20, 240, 220, 50), "Filter Maximum is: " + (cLvlMax + filter.currentPvpMax));
	}*/

}
