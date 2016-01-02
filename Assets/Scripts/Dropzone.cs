using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Dropzone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
   
    private HexGrid grid;
    private HexPosition position;
    public enum State { NEUTRAL, PLAYER, ENEMY};
    public State state = State.NEUTRAL;

    private GameObject[] playerControllers;
    private UnitCardStats unitCardStats;
    private MakePrefabAppear spawn;
    public HexGrid hexGrid;

    //private SimpleStatus.DropZoneState _state;
    public CapturePoint capturePoint;
    public Transform captureParent;

    private GameObject myObject;
    private Unit myUnit;


    void Start() {
        spawn = gameObject.GetComponent<MakePrefabAppear>();
        hexGrid = GameObject.FindGameObjectWithTag("HexGrid").GetComponent<HexGrid>();

        playerControllers = GameObject.FindGameObjectsWithTag("PlayerController");

        captureParent = transform.parent;
        capturePoint = captureParent.transform.parent.GetComponent<CapturePoint>();

        if (capturePoint != null) {
            if (capturePoint.PLAYER == 0) {
                state = State.PLAYER;
            } else if (capturePoint.PLAYER == 1) {
                state = State.ENEMY;
            }
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

        //if (capturePoint.ownership == playerController.playerID) {
            //_state.owned = true;
        //}

        if (position.containsKey("Unit")) {
            Debug.LogError("Tile occupied by Unit");
        }

		//FOR now no checking
        if (state == State.PLAYER && !position.containsKey("Unit")) { // Need to fix it to make it usable by both players, not just hard coded to be player 1
            //_state.owned = true;	
			//_state.playerOccupier = SimpleNet.PlayerID;

            foreach (GameObject playerController in playerControllers) {
                if (playerController.GetComponent<PlayerController>().GoldCurrent >= unitCardStats.unitCost) {
                    myObject = spawn.SpawnUnit(unitCardStats.unitID, unitCardStats.ownership);
                    myObject.name = "Unit";
                    myUnit = myObject.GetComponent<Unit>();
                    myUnit.SetGrid(hexGrid);
                    hexGrid.unselect();

                    playerController.GetComponent<PlayerController>().GoldCurrent = playerController.GetComponent<PlayerController>().GoldCurrent - unitCardStats.unitCost;
                } else {
                Debug.LogError("Not enough Gold");
                }
            }
            
        } else if (state != State.PLAYER) {
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
        if (unit.PLAYER != capturePoint.PLAYER && state == State.NEUTRAL) {
            if (unit.PLAYER == 0) {
                state = State.PLAYER;
            } else {
                state = State.ENEMY;
            }
            capturePoint.Ownership = unit.PLAYER;
        } else if (unit.PLAYER != capturePoint.PLAYER && state != State.NEUTRAL) {
            state = State.NEUTRAL;
            capturePoint.Ownership = -1;
        }
    }
}
