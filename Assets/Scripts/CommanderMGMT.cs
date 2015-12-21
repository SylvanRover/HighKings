using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Map.Events;

public class CommanderMGMT : MonoBehaviour {

	[System.Serializable]
	public class Commander {
		public int ID;
		public string Name;
		public int Level;
		public float Speed;
		public bool Active;
		public Sprite BubbleSprite;
		public Sprite ButtonSprite;
	}
	
	public List<Commander> commanderData;

	public Button commanderButton;
	public ArmyStats commanderButtonStats;
	private RadialTimer commanderButtonRadial;

	public MakePrefabAppear spawnArmies;
	public Transform bubbleHolder;	
	public Canvas bubbleHolderCanvas;
	public GameObject bubbleObj;
	private GameObject newbubble;
	private UICommanderBubble bubbleUI;
	private GameObject commanderObj;
	private ArmyStats stats;
	
	private Settlement home;	
	public EventTriggerDragIcon homeButton;

	private int currentCommanderID;
	private Transform currentCommanderPos;

	public Transform lookAt;
	public bool lockOnTarget = false;
	public float lockOnWait = 1f;

	public int cLvlGlobHigh = 0;
	public int cLvlGlobLow = 40;

	IEnumerator LockOnTargetWait(){		
		yield return new WaitForSeconds (lockOnWait);
		lockOnTarget = true;
	}

	public void CameraToCommander(){
		lockOnTarget = false;
		Camera_pan_RTS[] cp_arr = FindObjectsOfType<Camera_pan_RTS> ();
		foreach(Camera_pan_RTS cp in cp_arr) {
			cp.AnimateToWorldPos(currentCommanderPos.position);
		}
		
		Camera_zoom_RTS[] cz_arr = FindObjectsOfType<Camera_zoom_RTS> ();
		foreach(Camera_zoom_RTS cz in cz_arr) {
			cz.IconZoomCamera();
		}
		commanderButtonStats.selectionRing.Selected ();
		commanderButtonStats.selectionRing.Pressed ();		
		StartCoroutine(LockOnTargetWait());
	}
		
	public void ChangeCommanderButton(int ID, Sprite commanderSprite, Transform commanderPos, ArmyStats commanderStats){
		//Debug.LogError ("Recieved at CommanderMGMT");		
		commanderButton.image.sprite = commanderSprite;
		currentCommanderID = ID;
		currentCommanderPos = commanderPos;
		commanderButtonStats = commanderStats;
		commanderButtonRadial = currentCommanderPos.GetComponent<RadialTimer> ();
	}

	public void CommanderPrevious(){
		//Debug.LogError ("Previous Commander");
		if (currentCommanderID <= 1) {
			currentCommanderID = 7;
		}	
		//Debug.LogError ("Commander ID " + (currentCommanderID-1));
		InteractionEvents.BroadcastSwitch (currentCommanderID-1);
	}
	public void CommanderNext(){
		//Debug.LogError ("Next Commander");
		if (currentCommanderID >= 6) {
			currentCommanderID = 0;
		}	
		//Debug.LogError ("Commander ID " + (currentCommanderID+1));
		InteractionEvents.BroadcastSwitch (currentCommanderID+1);
	}
	
	void Start(){

		// Spawn Commanders Randomly
		foreach(Commander c in commanderData) {
			commanderObj = spawnArmies.SpawnAPrefab("Army", this.transform.position);
			commanderObj.transform.position = new Vector3(commanderObj.transform.position.x, 0f, commanderObj.transform.position.z);
			stats = commanderObj.GetComponent<ArmyStats>();
			stats.commanderID = c.ID;
			stats.commanderName = c.Name;
			stats.commanderLevel = c.Level;
			stats.commanderIsActive = c.Active;
			stats.commanderButtonSprite = c.ButtonSprite;
			stats.speed = c.Speed;

			newbubble = GameObject.Instantiate(bubbleObj);
			newbubble.transform.SetParent(bubbleHolder, false);
			bubbleUI = newbubble.GetComponent<UICommanderBubble>();
			bubbleUI.screenBuffer = (bubbleUI.bubbleArrow.rect.width * bubbleHolderCanvas.scaleFactor) / 2;
			bubbleUI.target = commanderObj.transform;
			bubbleUI.targetButton = stats;
			bubbleUI.commanderHeadButton.image.sprite = c.BubbleSprite;

			// Find Global lowest & highest level
			if (c.Level > cLvlGlobHigh){
				cLvlGlobHigh = c.Level;
			}
			if (c.Level < cLvlGlobLow){
				cLvlGlobLow = c.Level;
			}
		}

		// Create Bubble for home settlement		
		newbubble = GameObject.Instantiate(bubbleObj);
		newbubble.transform.SetParent(bubbleHolder, false);
		bubbleUI = newbubble.GetComponent<UICommanderBubble>();
		bubbleUI.screenBuffer = (bubbleUI.bubbleArrow.rect.width * bubbleHolderCanvas.scaleFactor) / 2;
		bubbleUI.target = this.transform;

		InteractionEvents.OnBroadcastSelected += ChangeCommanderButton;
	}

	void LateUpdate(){
		if (lockOnTarget) {
			lookAt.transform.position = currentCommanderPos.transform.position;
		}
	}

	/*void OnGUI() {
		GUI.Label (new Rect (20, 220, 200, 30), "Scale Factor is: " + bubbleHolderCanvas.scaleFactor);
	}*/

	void OnDestroy(){		
		InteractionEvents.OnBroadcastSelected -= ChangeCommanderButton;
	}
}
