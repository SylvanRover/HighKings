using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitStats : MonoBehaviour {

    public bool unitIsActive = false;
	public Sprite unitButtonSprite;

	public RectTransform button;
	//public SelectionRingAnim selectionRing;

	public Transform unitPos;

    public Image healthbarImage;
    public Color neutralOwned;
    public Color playerOwned;
    public Color enemyOwned;

    // Unit Variables
    public int unitID;
    public int ownership = 0;
    public string unitName;
    public string unitType;
    public int unitCost;
    public int movementCost;
    public int movementMax;
    public int speed;
    public int unitBuildTime;
    public int unitPop;
    public float healthMax;
	public float healthCurrent;
	public float damage;
	public float attackRange;
	public float armourPoints;
	public float lineOfSight;

    public int Ownership {

        get { return ownership; }
        set {
            ownership = value;
            if (ownership == 0) {
                healthbarImage.color = neutralOwned;
            }
            if (ownership == 1) {
                healthbarImage.color = playerOwned;
            }
            if (ownership == 2) {
                healthbarImage.color = enemyOwned;
            }
        }
    }

    public Text unitNameText;

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

	/*public void UnitIsSelected(int ID){
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
	}*/
		
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

	/*public void IsSelected(int ID, Sprite commanderSprite, Transform commanderPos, ArmyStats commanderStats){
		//Debug.LogError ("Recieved at ArmyStats. Commander ID " + ID);
		if (ID != unitID) {
			//Debug.LogError ("Comparing Commander ID " + ID + " with " + commanderID);
			if (selectionRing != null) {
				selectionRing.Normal ();
				selectionRing.Deselect ();
			}
		}
	}*/

	void Start () {
		// Setting Health
		healthCurrent = healthMax;
		resetSize = healthRect.sizeDelta;
		anim = healthbar.GetComponent<Animator>();

        SetUnitType();

        if (healthbarImage != null) {
            if (ownership == 0) {
                healthbarImage.color = neutralOwned;
            }
            if (ownership == 1) {
                healthbarImage.color = playerOwned;
            }
            if (ownership == 2) {
                healthbarImage.color = enemyOwned;
            }
        }

        /*InteractionEvents.OnBroadcastSelected += IsSelected;
		InteractionEvents.OnBroadcastSwitch += UnitIsSelected;*/

        // Select First Commander		
        //stats = this.transform.GetComponent<ArmyStats> ();

        /*if (unitID == 1) {
			UnitIsSelected(unitID);
			IsSelected(unitID, unitButtonSprite, this.unitPos, stats);
		}*/
    }

    public void SetUnitType() {
        // Setting Unit Variables
        if (unitID == 0) {
            unitName = "Swordsman";
            unitNameText.text = unitName;
            unitType = "Infantry";
            unitCost = 1;
            movementCost = 1;
            movementMax = 3;
            speed = 1;
            unitBuildTime = 1;
            unitPop = 1;
            healthMax = 4;
            healthCurrent = 4;
            damage = 1;
            attackRange = 1;
            armourPoints = 0;
            lineOfSight = 2;
        }
        if (unitID == 1) {
            unitName = "Archer";
            unitNameText.text = unitName;
            unitType = "Ranged";
            unitCost = 2;
            movementCost = 1;
            movementMax = 3;
            speed = 1;
            unitBuildTime = 1;
            unitPop = 1;
            healthMax = 2;
            healthCurrent = 2;
            damage = 1;
            attackRange = 2;
            armourPoints = 0;
            lineOfSight = 4;
        }
        if (unitID == 2) {
            unitName = "Knight";
            unitNameText.text = unitName;
            unitType = "Mounted";
            unitCost = 4;
            movementCost = 1;
            movementMax = 6;
            speed = 2;
            unitBuildTime = 1;
            unitPop = 1;
            healthMax = 5;
            healthCurrent = 5;
            damage = 2;
            attackRange = 1;
            armourPoints = 0;
            lineOfSight = 2;
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
		/*if (onUp && !onDrag) {
			Camera_pan_RTS[] cp_arr = FindObjectsOfType<Camera_pan_RTS> ();
			foreach(Camera_pan_RTS cp in cp_arr) {
				cp.AnimateToWorldPos(unitPos.position);
			}
			
			Camera_zoom_RTS[] cz_arr = FindObjectsOfType<Camera_zoom_RTS> ();
			foreach(Camera_zoom_RTS cz in cz_arr) {
				cz.IconZoomCamera();
			}
		}*/
		/*if (onUp || onDrag) {
			onDrag = false;
			onUp = false;
		}*/

        if (healthCurrent <= 0) {
            Destroy(this.gameObject);
        }
	}

	/*void OnDestroy(){			
		InteractionEvents.OnBroadcastSelected -= IsSelected;
		InteractionEvents.OnBroadcastSwitch -= UnitIsSelected;
	}*/
}