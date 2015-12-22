using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Map.Events;

public class UnitStats : MonoBehaviour {

	public int unitID;
	public string unitName;
    public bool unitIsActive = false;
	public Sprite unitButtonSprite;

	public RectTransform button;
	public SelectionRingAnim selectionRing;

	public Transform unitPos;
	private ArmyStats stats;

    public string unitType;
    public int unitCost = 1;
    public int movementCost = 1;
    public int speed = 1;
    public int unitBuildTime = 1;
    public int unitPop = 1;
    public float healthMax = 3;
	public float healthCurrent = 3;
	public float damage = 1;
	public float attackRange = 1;
	public float armourPoints = 0;
	public float lineOfSight = 2;
	public RectTransform healthRect;
	public RectTransform damageRect;
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
	}

	//Healbar
	public IEnumerator HealthbarFade() {
		anim = healthbar.GetComponent<Animator>();
		yield return new WaitForSeconds (healthbarFadeTime);
		anim.SetBool ("On", false);
	}

	IEnumerator AnimateDamage() {
		currentSize = damageRect.sizeDelta;
		yield return new WaitForSeconds (damageDurationWait);
		animateDamage = true;
	}
	
	/*public void OnPointerUp(){
		clickToPoint.OnPointerUp ();
		if (progressLine != null) {
			progressLine.Selected ();
		}
	}*/

	public void UnitIsSelected(int ID){
		if (ID == unitID) {		
			InteractionEvents.BroadcastSelected (unitID, unitButtonSprite, unitPos.transform, stats);
			if (selectionRing != null) {
				selectionRing.Pressed ();
				selectionRing.Selected ();
			}
		} else {
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
		healthRect.sizeDelta = new Vector2 ((healthCurrent/healthMax) * healthbarWidth, healthRect.sizeDelta.y);
		StopAllCoroutines ();
		StartCoroutine(AnimateDamage());
		StartCoroutine(HealthbarFade());
	}

	public void IsSelected(int ID, Sprite commanderSprite, Transform commanderPos, ArmyStats commanderStats){
		//Debug.LogError ("Recieved at ArmyStats. Commander ID " + ID);
		if (ID != unitID) {
			//Debug.LogError ("Comparing Commander ID " + ID + " with " + commanderID);
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
		resetSize = healthRect.sizeDelta;
		anim = healthbar.GetComponent<Animator>();
		
		InteractionEvents.OnBroadcastSelected += IsSelected;
		InteractionEvents.OnBroadcastSwitch += UnitIsSelected;

		// Select First Commander		
		stats = this.transform.GetComponent<ArmyStats> ();

		if (unitID == 1) {
			UnitIsSelected(unitID);
			IsSelected(unitID, unitButtonSprite, this.unitPos, stats);
		}
	}


	void Update(){

		if (animateDamage) {
			if (currentTime <= damageDuration) {
				currentTime += Time.deltaTime;
				damageRect.sizeDelta = Vector2.Lerp (currentSize, healthRect.sizeDelta, currentTime / damageDuration);
			} else {
				//damage.sizeDelta.y = currentSize;
				currentTime = 0f;
			}			
			if (damageRect.sizeDelta == healthRect.sizeDelta) {			
				animateDamage = false;
			}
		}

		//Event Trigger nn Click but not after Drag
		if (onUp && !onDrag) {
			Camera_pan_RTS[] cp_arr = FindObjectsOfType<Camera_pan_RTS> ();
			foreach(Camera_pan_RTS cp in cp_arr) {
				cp.AnimateToWorldPos(unitPos.position);
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

        if (healthCurrent <= 0) {
            Destroy(this.gameObject);
        }
	}

	void OnDestroy(){			
		InteractionEvents.OnBroadcastSelected -= IsSelected;
		InteractionEvents.OnBroadcastSwitch -= UnitIsSelected;
	}
}