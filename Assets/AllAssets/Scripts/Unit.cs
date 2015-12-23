﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Unit : MonoBehaviour {
	private HexGrid grid;
	private HexPosition position;
	public enum State {MOVE, ATTACK, WAIT};
	private State state = State.MOVE;
	public int PLAYER;
	public float MAX_HP;
	public float hp;
	public float STRENGTH;
	public float VARIATION;
	public int SPEED;
	public int RANGE;
	//private int HP_BAR_WIDTH = 64;
	//private int HP_BAR_HEIGHT = 16;
	private bool moving = false;
	private float t;
	private Vector3[] path;
	private int n;	//position on the path
	private const float MOTION_SPEED = 0.05f;

    //public bool unitIsActive = false;
    //public Sprite unitButtonSprite;

    //public RectTransform button;
    //public SelectionRingAnim selectionRing;

    //public Transform unitPos;

    public Image healthbarImage;
    public Color neutralOwned;
    public Color playerOwned;
    public Color enemyOwned;

    public GameObject unitMesh;
    private Animator unitAnimr;

    // Unit Variables
    public int unitID;
    public int ownership = 0;
    public string unitName;
    //public string unitType;
    //public int unitCost;
    //public int movementCost;
    //public int movementMax;
    //public int speed;
    //public int unitBuildTime;
    //public int unitPop;
    //public float healthMax;
    //public float healthCurrent;
    //public float damage;
    //public float attackRange;
    //public float armourPoints;
    //public float lineOfSight;
    public float movementSpeed = 1;

    public int Ownership {

        get { return ownership; }
        set {
            ownership = value;
            if (ownership == -1) {
                healthbarImage.color = neutralOwned;
            }
            if (ownership == 0) {
                healthbarImage.color = playerOwned;
            }
            if (ownership == 1) {
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
        yield return new WaitForSeconds(healthbarFadeTime);
        anim.SetBool("On", false);
    }

    IEnumerator AnimateDamage() {
        currentSize = damageRect.sizeDelta;
        yield return new WaitForSeconds(damageDurationWait);
        animateDamage = true;
    }

    public void SetGrid (HexGrid grid) {
		this.grid = grid;
		grid.SendMessage ("AddUnit", this);
	}
	
	public float HP {
		get {
			return hp;
		}
	}

    public void Damage(float value) {
        anim = healthbar.GetComponent<Animator>();
        anim.SetBool("On", true);
        hp -= value;
        //endSize = new Vector2 ((healthCurrent/healthMax) * healthbarWidth, health.sizeDelta.y);
        healthRect.sizeDelta = new Vector2((hp / MAX_HP) * healthbarWidth, healthRect.sizeDelta.y);
        StopAllCoroutines();
        StartCoroutine(AnimateDamage());
        StartCoroutine(HealthbarFade());
    }

    public void SetUnitType() {
        if (this.tag == "Unit") {
            // Setting Unit Variables
            if (unitID == 0) {
                unitName = "Swordsman";
                unitNameText.text = unitName;
                //unitType = "Infantry";
                //unitCost = 1;
                //movementCost = 1;
                //movementMax = 3;
                //speed = 1;
                //unitBuildTime = 1;
                //unitPop = 1;
                //healthMax = 4;
                //healthCurrent = 4;
                //damage = 1;
                //attackRange = 1;
                //armourPoints = 0;
                //lineOfSight = 2;

                MAX_HP = 4;
                hp = MAX_HP;
            }
            if (unitID == 1) {
                unitName = "Archer";
                unitNameText.text = unitName;
                //unitType = "Ranged";
                //unitCost = 2;
                //movementCost = 1;
                //movementMax = 3;
                //speed = 1;
                //unitBuildTime = 1;
                //unitPop = 1;
                //healthMax = 2;
                //healthCurrent = 2;
                //damage = 1;
                //attackRange = 2;
                //armourPoints = 0;
                //lineOfSight = 4;

                MAX_HP = 2;
                hp = MAX_HP;
            }
            if (unitID == 2) {
                unitName = "Knight";
                unitNameText.text = unitName;
                //unitType = "Mounted";
                //unitCost = 4;
                //movementCost = 1;
                //movementMax = 6;
                //speed = 2;
                //unitBuildTime = 1;
                //unitPop = 1;
                //healthMax = 5;
                //healthCurrent = 5;
                //damage = 2;
                //attackRange = 1;
                //armourPoints = 0;
                //lineOfSight = 2;

                MAX_HP = 5;
                hp = MAX_HP;
            }
        } else {
            MAX_HP = 10;
            hp = MAX_HP;
        }


    }

    /*void SetPosition (HexPosition position) {
		this.position = position;
		transform.position = position.getPosition ();
		position.add ("Unit", this);
		grid.SendMessage ("ActionComplete");
	}
	
	public void Move (HexPosition desitination) {
		grid.SendMessage ("MessageRecieved");
		if (desitination.containsKey ("Unit")) {
			grid.SendMessage ("ActionComplete");
			return;
		}
		position.delete ("Unit");
		desitination.add ("Unit", this);
		transform.position = desitination.getPosition();
		position = desitination;
		grid.SendMessage ("ActionComplete");
	}*/

    public HexPosition Coordinates {
		get {
			return position;
		}
		set {
			position = value;
			transform.position = value.getPosition ();
			value.add ("Unit", this);
		}
	}
	
	public State Status {
		get { return state; }
		set { state = value; }
	}
	
	public void move (HexPosition[] path) {

        if (path.Length < 2) {
			skipMove();
			return;
		}
		grid.wait();
		HexPosition destination = path[path.Length-1];
		this.path = new Vector3[path.Length];
		for (int i = 0; i < path.Length; ++i) {
			this.path[i] = path[i].getPosition();
		}
		state = State.ATTACK;
		if (destination.containsKey ("Unit")) {
			print ("ERROR: Space occupied.");
			grid.actionComplete();
			return;
		}
		position.remove ("Unit");
		destination.add ("Unit", this);
		//transform.position = desitination.getPosition();
		t = 0;
		n = 0;
		moving = true;
		position = destination;
    }
	
	public void skipMove () {
		state = State.ATTACK;
	}
	
	public void attack (Unit enemy) {
		state = State.WAIT;
		enemy.defend(STRENGTH, VARIATION);
	}
	
	public void newTurn () {
		state = State.MOVE;
	}
	
	public void defend (float strength, float variation) {
		int damage = NegativeBinomialDistribution.fromMeanAndStandardDeviation(strength-1, variation)+1;
		//hp -= damage;
        Damage(strength);
        if (hp <= 0) {
			position.remove ("Unit");
			grid.remove (this);
			Object.Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {

        ownership = PLAYER;

        unitAnimr = unitMesh.GetComponent<Animator>();

        // Setting Health
        hp = MAX_HP;
        resetSize = healthRect.sizeDelta;
        anim = healthbar.GetComponent<Animator>();

        SetUnitType();

        if (healthbarImage != null) {
            if (ownership == -1) {
                healthbarImage.color = neutralOwned;
            }
            if (ownership == 0) {
                healthbarImage.color = playerOwned;
            }
            if (ownership == 1) {
                healthbarImage.color = enemyOwned;
            }
        }

        hp = MAX_HP;
	}
	
	private Vector3 bezier (Vector3 p0, Vector3 c0, Vector3 c1, Vector3 p1, float t) {
		return (1-t)*(1-t)*(1-t)*p0 + 3*(1-t)*(1-t)*t*c0 + 3*(1-t)*t*t*c1 + t*t*t*p1;
	}
	
	private Vector3 dbezier (Vector3 p0, Vector3 c0, Vector3 c1, Vector3 p1, float t) {
		Vector3 dir = 3*(1-t)*(1-t)*(c0-p0) + 6*(1-t)*t*(c1-c0) + 3*t*t*(p1-c1);
		if (dir.magnitude < 0.001) {
			return  6*(1-t)*(c1-2*c0+p0) + 6*t*(p1-2*c1+c0);
		}
		return dir;
	}
	
	/*private Quaternion bezierRotation (Vector3 p0, Vector3 c0, Vector3 c1, Vector3 p1, float t) {
		Vector3 dir = dbezier (p0, c0, c1, p1, t);
		dir.y = 0;
		Quaternion rotation = new Quaternion ();
		rotation.SetLookRotation (dir);
		return rotation;
	}*/
	
	private Quaternion horizontalLookRotation (Vector3 dir) {
		dir.y = 0;
		Quaternion rotation = new Quaternion ();
		rotation.SetLookRotation (dir);
		return rotation;
	}
	
	private void fullBezier (Vector3 p0, Vector3 c0, Vector3 c1, Vector3 p1, float t, out Vector3 position, out Quaternion rotation) {
		position = bezier (p0, c0, c1, p1, t);
		rotation = horizontalLookRotation (dbezier (p0, c0, c1, p1, t));
	}
	
	// Update is called once per frame
	void Update () {

        /*if (healthCurrent <= 0) {
            Destroy(this.gameObject);
        }*/

        if (animateDamage) {
            if (currentTime <= damageDuration) {
                currentTime += Time.deltaTime;
                damageRect.sizeDelta = Vector2.Lerp(currentSize, healthRect.sizeDelta, currentTime / damageDuration);
            }
            else {
                //damage.sizeDelta.y = currentSize;
                currentTime = 0f;
            }
            if (damageRect.sizeDelta == healthRect.sizeDelta) {
                animateDamage = false;
            }
        }

        //There has to be a better way to do this. Especially if I want to stick rotations in there.
        if (moving) {

            if (unitAnimr != null) {
                unitAnimr.SetFloat("UnitSpeed", movementSpeed);
            }

            if (path.Length < 2) {	//Shouldn't happen.
				moving = false;
				grid.actionComplete ();
				return;
			} else if (path.Length == 2) {
				if (t >= 1) {
					transform.position = path[1];
					moving = false;
					grid.actionComplete ();
					return;
				} else {
					transform.position = (1-t)*path[0] + t*path[1];
					transform.rotation = horizontalLookRotation (path[1]-path[0]);
					t += MOTION_SPEED;
				}
				
			} else if (path.Length == 3) {

                if (t >= 2) {
					transform.position = path[2];
					moving = false;
					grid.actionComplete ();
					return;
				} else {
					//transform.position = bezier (path[0], (2*path[1]+path[0])/3, (2*path[1]+path[2])/3, path[2], t/2);
					Vector3 position;
					Quaternion rotation;
					fullBezier (path[0], (2*path[1]+path[0])/3, (2*path[1]+path[2])/3, path[2], t/2, out position, out rotation);
					transform.position = position;
					transform.rotation = rotation;
					t += MOTION_SPEED;
				}
			} else {

                if (n == 0) {
					if (t >= 0.5f) {
						t -= 0.5f;
						++n;	//Falls through.
					} else {
						transform.position = (1-t)*path[0] + t*path[1];
						transform.rotation = horizontalLookRotation (path[1]-path[0]);
						t += MOTION_SPEED;
					}
				}
				if (n > 0 && n < path.Length-1) {
					if (t >= 1) {
						t -= 1;
						++n;	//Falls through.
					} else {
						//transform.position = bezier ((path[n-1]+path[n])/2, (5*path[n]+path[n-1])/6, (5*path[n]+path[n+1])/6, (path[n+1]+path[n])/2, t);
						Vector3 position;
						Quaternion rotation;
						fullBezier ((path[n-1]+path[n])/2, (5*path[n]+path[n-1])/6, (5*path[n]+path[n+1])/6, (path[n+1]+path[n])/2, t, out position, out rotation);
						transform.position = position;
						transform.rotation = rotation;
						t += MOTION_SPEED;
					}
				}
				if (n == path.Length-1) {
					if (t >= 0.5f) {
						transform.position = path[n];
						moving = false;
						grid.actionComplete ();
                        Debug.LogError(string.Format("End of movement at u:{0}v:{1} isCapturePoint?:{2}", position.U, position.V, position.containsKey("CapturePoint")));
						return;
					} else {
						transform.rotation = horizontalLookRotation (path[n]-path[n-1]);
						transform.position = (0.5f-t)*path[n-1] + (t+0.5f)*path[n];
						t += MOTION_SPEED;
					}
				}
			}
		} else {
            if (unitAnimr != null) {
                unitAnimr.SetFloat("UnitSpeed", 0);
            }
        }
	}
	
	void OnGUI () {	//TODO: Get rid of magic numbers.
		Vector3 coordinates = Camera.main.WorldToScreenPoint (transform.position + new Vector3(0,1.5f,0) + 0.5f*Camera.main.transform.up);	//TODO: Make this some kind of constant.
		coordinates.y = Screen.height - coordinates.y;
		//print (coordinates);
		Texture2D red = new Texture2D(1,1);
		red.SetPixel(0,0,Color.red);
		red.wrapMode = TextureWrapMode.Repeat;
		red.Apply ();
		Texture2D green = new Texture2D(1,1);
		green.SetPixel(0,0,Color.green);
		green.wrapMode = TextureWrapMode.Repeat;
		green.Apply ();
		//GUI.Box (new Rect(coordinates.x - 10, coordinates.y - 5, 20, 10), "test");
		//GUI.DrawTexture (new Rect(coordinates.x - HP_BAR_WIDTH/2, coordinates.y + HP_BAR_HEIGHT/2, HP_BAR_WIDTH, HP_BAR_HEIGHT), red);
		//GUI.DrawTexture (new Rect(coordinates.x - HP_BAR_WIDTH/2, coordinates.y + HP_BAR_HEIGHT/2, HP_BAR_WIDTH * hp / MAX_HP, HP_BAR_HEIGHT), green);
		GUIStyle centered = new GUIStyle ();
		centered.alignment = TextAnchor.MiddleCenter;
		//GUI.Label (new Rect(coordinates.x - HP_BAR_WIDTH/2, coordinates.y + HP_BAR_HEIGHT/2, HP_BAR_WIDTH, HP_BAR_HEIGHT), hp.ToString (), centered);
	}
}
