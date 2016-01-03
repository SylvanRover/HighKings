using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.EventSystems;

public class Dropzone : NetworkBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
   
    private HexGrid grid;
    private HexPosition position;
    public enum State { NEUTRAL, PLAYER, ENEMY};
    public State state = State.NEUTRAL;

    private GameObject[] players;
    private PlayerController player;
    private UnitCardStats unitCardStats;
    private MakePrefabAppear spawn;
    //public HexGrid hexGrid;

    //private SimpleStatus.DropZoneState _state;
    private CapturePoint capturePoint;
    private Transform captureParent;

    public GameObject myObject;
    private Unit myUnit;


    void Start() {
            captureParent = transform.parent;
            capturePoint = captureParent.transform.parent.GetComponent<CapturePoint>();
        if (captureParent.transform.parent.tag != "PlayerController") {
            spawn = gameObject.GetComponent<MakePrefabAppear>();
            grid = GameObject.Find("HexGrid").GetComponent<HexGrid>();
            //players = GameObject.FindGameObjectsWithTag("PlayerController");

            if (capturePoint != null) {
                if (capturePoint.PLAYER == 0) {
                    state = State.PLAYER;
                } else if (capturePoint.PLAYER == 1) {
                    state = State.ENEMY;
                }
            }
        }
    }
    void SceneReady() {
        captureParent = transform.parent;
        capturePoint = captureParent.transform.parent.GetComponent<CapturePoint>();
        spawn = gameObject.GetComponent<MakePrefabAppear>();
        grid = GameObject.Find("HexGrid").GetComponent<HexGrid>();
        //players = GameObject.FindGameObjectsWithTag("PlayerController");

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
        players = GameObject.FindGameObjectsWithTag("PlayerController");
        foreach (GameObject p in players) {
            if (p.GetComponent<NetworkIdentity>().isLocalPlayer) {
                player = p.GetComponent<PlayerController>();
            }
        }
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
        if (state == State.PLAYER && !position.containsKey("Unit")) {

            if (player.GoldCurrent >= unitCardStats.unitCost) {
                spawn = gameObject.GetComponent<MakePrefabAppear>();
                myObject = spawn.SpawnUnit(unitCardStats.unitID, unitCardStats.cardID);
                myObject.name = "Unit";
                myUnit = myObject.GetComponent<Unit>();
                myUnit.SetGrid(grid);
                grid.unselect();
                player.GoldCurrent = player.GoldCurrent - unitCardStats.unitCost;

                // spawn on the clients
                NetworkServer.Spawn(myObject);

                // Send command to server about unit spawn
                //CmdTellServerOfUnit(myObject, myUnit.PLAYER);
            } else {
            Debug.LogError("Not enough Gold");
            }
            
        } else if (state != State.PLAYER) {
            Debug.LogError("Dropzone owned by " + capturePoint.PLAYER);
        }
        //}
    }

    /*[Command]
    void CmdTellServerOfUnit(GameObject unit, int unitPlayer) {
        Unit myUniqueUnit = unit.GetComponent<Unit>();
        myUniqueUnit.PLAYER = unitPlayer;
    }*/


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
