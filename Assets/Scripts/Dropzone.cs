using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Dropzone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
   
    private HexGrid grid;
    private HexPosition position;
    public enum State { NEUTRAL, PLAYER1, PLAYER2};
    public State state = State.NEUTRAL;

    private PlayerController playerController;
    private UnitCardStats unitCardStats;
    private MakePrefabAppear spawn;
    public HexGrid hexGrid;

    //private SimpleStatus.DropZoneState _state;
    public CapturePoint capturePoint;
    public Transform captureParent;

    private GameObject myObject;
    private Unit myUnit;


    void Start() {
        /*_state = new SimpleStatus.DropZoneState ();
		_state.isCapturePoint = true;
		_state.playerOccupier = -1;
		_state.owned = false;*/
        spawn = gameObject.GetComponent<MakePrefabAppear>();
        hexGrid = GameObject.FindGameObjectWithTag("HexGrid").GetComponent<HexGrid>();

        playerController = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
        captureParent = transform.parent;
        capturePoint = captureParent.transform.parent.GetComponent<CapturePoint>();

        if (capturePoint.ownership == 0) {
            state = State.PLAYER1;
        } else if (capturePoint.ownership == 1) {
            state = State.PLAYER2;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        //Debug.Log("OnDrop to " + gameObject.name);
    }

    public void OnPointerExit(PointerEventData eventData) {
        //Debug.Log("OnDrop to " + gameObject.name);
    }

    public void OnDrop(PointerEventData eventData) {
        Debug.Log(eventData.pointerDrag.name + " was dropped on " + gameObject.name);

        Draggable d = eventData.pointerDrag.GetComponent<Draggable>();
        /*if (d != null) {
            d.ParentToReturnTo = this.transform;
        }*/

        unitCardStats = d.GetComponent<UnitCardStats>();

        if (capturePoint.ownership == unitCardStats.ownership) {
            //_state.owned = true;
        }

        if (position.containsKey("Unit")) {
            Debug.LogError("Tile occupied by Unit");
        }

		//FOR now no checking
        if (state == State.PLAYER1 && unitCardStats.ownership == 0 && !position.containsKey("Unit")) {
            if (playerController.GoldCurrent >= unitCardStats.unitCost) {
                myObject = spawn.SpawnUnit(unitCardStats.unitID, 0);
                myObject.name = "Unit";
                myUnit = myObject.GetComponent<Unit>();
                myUnit.SetGrid(hexGrid);
                hexGrid.unselect();

                playerController.GoldCurrent = playerController.GoldCurrent - unitCardStats.unitCost;
            } else {
            Debug.LogError("Not enough Gold");
            }
        } else if (state != State.PLAYER1) {
            Debug.LogError("Dropzone not owned by you");
        }

        if (state == State.PLAYER2 && unitCardStats.ownership == 1 && !position.containsKey("Unit")) {
            if (playerController.GoldCurrent2 >= unitCardStats.unitCost) {
                myObject = spawn.SpawnUnit(unitCardStats.unitID, 1);
                myObject.name = "Unit";
                myUnit = myObject.GetComponent<Unit>();
                myUnit.SetGrid(hexGrid);
                hexGrid.unselect();

                playerController.GoldCurrent2 = playerController.GoldCurrent2 - unitCardStats.unitCost;
            } else {
                Debug.LogError("Not enough Gold");
            }
        } else if (state != State.PLAYER1) {
            Debug.LogError("Dropzone not owned by you");
        }
        //}
    }

    public void SetGrid(HexGrid grid) {
        this.grid = grid;
        grid.SendMessage("AddDropzone", this);
    }

    public HexPosition Coordinates {
        get {
            return position;
        }
        set {
            position = value;
            transform.parent.position = value.getPosition();
            value.add("Dropzone", this);
        }
    }

    public State Status {
        get { return state; }
        set { state = value; }
    }

    public void capture(Unit unit) {
        if (unit.ownership != capturePoint.ownership && state == State.NEUTRAL) {
            if (unit.ownership == 0) {
                state = State.PLAYER1;
            } else {
                state = State.PLAYER2;
            }
            capturePoint.Ownership = unit.ownership;
        } else if (unit.ownership != capturePoint.ownership && state != State.NEUTRAL) {
            state = State.NEUTRAL;
            capturePoint.Ownership = -1;
        }
    }

    /*public void Capture() {
        _state.owned = true;	
        _state.playerOccupier = SimpleNet.PlayerID;
    }*/

	/*public void UpdateState(SimpleStatus.DropZoneState state){
		if (this._state.playerOccupier != state.playerOccupier || this._state.owned != state.owned) {
			Debug.LogError ("STATE DOES NOT MATCH");
			if (state.playerOccupier != -1) {
				// THEN must have an occupier
				spawn.Appear (); //FOR NOW
				//spawn.SpawnUnit(unitCardStats.unitID, unitCardStats.ownership);
			} else {
				// THEN no longer has an occupier
				//TODO: delete the occupier graphic
			}
		}
		this._state = state;
	}

	public SimpleStatus.DropZoneState GetState(){
		return this._state;
	}*/

}
