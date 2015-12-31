using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Map.Events;

public class ArmyStats : MonoBehaviour {

	public int commanderID;
	public string commanderName;
	public int commanderLevel;
	public bool commanderIsActive = false;
	public Sprite commanderButtonSprite;

	public bool actionIdle = true;
	public bool actionMoving = false;
	public bool actionBattleAttacking = false;
	public bool actionBattleDefending = false;
	public bool actionDefending = false;
	public bool actionHarvesting = false;

	public RectTransform button;
	public SelectionRingAnim selectionRing;
	public ProgressLine progressLine;
	public ArmyCollision armyCol;

	private GameObject home;
	public Transform armyPos;
	private ArmyStats stats;

	public float healthMax = 100;
	public float healthCurrent = 100;
	public float attackPoints = 10;
	public float attackRange = 20;
	public float armourPoints = 0;
	public float LoS = 25;
	public float speed = 1f;
	public RectTransform health;
	public RectTransform damage;
	public GameObject healthbar;
	public float healthbarWidth = 54;
	public float healthbarFadeTime = 3;
	public Animator anim;
	public float wait = 0.5f;
	public Vector2 currentSize;
	private Vector2 endSize;
	public Vector2 resetSize;
	public float currentTime = 0f;
	public float damageDuration = 2f;
	public float damageDurationWait = 3f;
	public bool animateDamage = false;

	private float startTime;

	//Event Trigger nn Click but not after Drag
	public bool onUp = false;
	public bool onDrag = false;
	
	public void OnDragEnd() {
		onDrag = true;
	}
	
	public void OnPointerUp() {
		if (!onDrag) {
			onUp = true;
		}
		progressLine.Selected ();
	}

	//Healbar
	public IEnumerator HealthbarFade() {
		anim = healthbar.GetComponent<Animator>();
		yield return new WaitForSeconds (healthbarFadeTime);
		anim.SetBool ("On", false);
	}

	IEnumerator AnimateDamage() {
		currentSize = damage.sizeDelta;
		yield return new WaitForSeconds (damageDurationWait);
		animateDamage = true;
	}
	
	/*public void OnPointerUp(){
		clickToPoint.OnPointerUp ();
		if (progressLine != null) {
			progressLine.Selected ();
		}
	}*/

	public void CommanderIsSelected(int ID){
		if (ID == commanderID) {		
			InteractionEvents.BroadcastSelected (commanderID, commanderButtonSprite, armyPos.transform, stats);
			if (selectionRing != null) {
				selectionRing.Pressed ();
				selectionRing.Selected ();
			}
			progressLine._selected = true;
			progressLine.AllowLineEditing ();
		} else {
			if (progressLine != null) {
				progressLine.UnSelect ();
			}
			if (selectionRing != null) {
				selectionRing.Normal ();
				selectionRing.Deselect ();
			}
		}
	}
		
	public void Damage (float value) {
		anim = healthbar.GetComponent<Animator>();
		anim.SetBool ("On", true);
		healthCurrent -= value;
		//endSize = new Vector2 ((healthCurrent/healthMax) * healthbarWidth, health.sizeDelta.y);
		health.sizeDelta = new Vector2 ((healthCurrent/healthMax) * healthbarWidth, health.sizeDelta.y);
		StopAllCoroutines ();
		StartCoroutine(AnimateDamage());
		StartCoroutine(HealthbarFade());
	}

	public void IsSelected(int ID, Sprite commanderSprite, Transform commanderPos, ArmyStats commanderStats){
		//Debug.LogError ("Recieved at ArmyStats. Commander ID " + ID);
		if (ID != commanderID) {
			//Debug.LogError ("Comparing Commander ID " + ID + " with " + commanderID);
			if (progressLine != null) {
				progressLine.UnSelect ();
			}
			if (selectionRing != null) {
				selectionRing.Normal ();
				selectionRing.Deselect ();
			}
		}
	}

	// Use this for initialization
	void Start () {
		// Setting Health
		healthCurrent = healthMax;
		resetSize = health.sizeDelta;
		anim = healthbar.GetComponent<Animator>();
		
		InteractionEvents.OnBroadcastSelected += IsSelected;
		InteractionEvents.OnBroadcastSwitch += CommanderIsSelected;

		// Select First Commander		
		stats = this.transform.GetComponent<ArmyStats> ();

		if (commanderID == 1) {
			CommanderIsSelected(commanderID);
			IsSelected(commanderID,commanderButtonSprite,this.armyPos,stats);
		}

		if (progressLine != null) {
			progressLine.travelSpeed = speed;
		}
	}


	void Update(){

		if (animateDamage) {
			if (currentTime <= damageDuration) {
				currentTime += Time.deltaTime;
				damage.sizeDelta = Vector2.Lerp (currentSize, health.sizeDelta, currentTime / damageDuration);
			} else {
				//damage.sizeDelta.y = currentSize;
				currentTime = 0f;
			}			
			if (damage.sizeDelta == health.sizeDelta) {			
				animateDamage = false;
			}
		}

		//Event Trigger nn Click but not after Drag
		if (onUp && !onDrag) {
			Camera_pan_RTS[] cp_arr = FindObjectsOfType<Camera_pan_RTS> ();
			foreach(Camera_pan_RTS cp in cp_arr) {
				cp.AnimateToWorldPos(armyPos.position);
			}
			
			Camera_zoom_RTS[] cz_arr = FindObjectsOfType<Camera_zoom_RTS> ();
			foreach(Camera_zoom_RTS cz in cz_arr) {
				cz.IconZoomCamera();
			}
		}
		if (onUp || onDrag) {
			onDrag = false;
			onUp = false;
		}
	}

	void OnDestroy(){			
		InteractionEvents.OnBroadcastSelected -= IsSelected;
		InteractionEvents.OnBroadcastSwitch -= CommanderIsSelected;
	}
}