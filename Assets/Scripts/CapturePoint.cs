using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.EventSystems;

public class CapturePoint : NetworkBehaviour {

    private HexGrid grid;
    private HexPosition position;

    public enum State { NEUTRAL, PLAYER, ENEMY };
    [SyncVar]
    public State state = State.NEUTRAL;
    [SyncVar]
    public int PLAYER = -1;

    private GameObject[] playerControllers;
    private UnitCardStats unitCardStats;
    public HexGrid hexGrid;

    private GameObject myObject;
    private Unit myUnit;

    public int captureState = 0;
    public int goldAmount;
    public bool isTower = false;
    public bool isBarracks = false;


    public Image dropZoneImage;
    private Renderer hexRenderer;

    public Color neutralOwned;
    public Color playerOwned;
    public Color enemyOwned;

    public int Ownership {

        get { return PLAYER; }
        set {
            PLAYER = value;
            if (PLAYER == -1) {
                for (int i = 0; i < gameObject.transform.childCount; i++) {
                    Transform Go = gameObject.transform.GetChild(i);
                    Transform GoChild = Go.transform.GetChild(0);
                    Dropzone GoDrop = GoChild.transform.GetComponent<Dropzone>();
                    GoDrop.Status = Dropzone.State.NEUTRAL;
                    hexRenderer = Go.GetComponent<Renderer>();
                    hexRenderer.material.SetColor("_Color", neutralOwned);
                }
                //hexRenderer.material.SetColor("_Color", neutralOwned);
            }
            if (PLAYER == 0) {
                for(int i = 0; i < gameObject.transform.childCount; i++) {
                    Transform Go = gameObject.transform.GetChild(i);
                    Transform GoChild = Go.transform.GetChild(0);
                    Dropzone GoDrop = GoChild.transform.GetComponent<Dropzone>();
                    GoDrop.Status = Dropzone.State.PLAYER;
                    hexRenderer = Go.GetComponent<Renderer>();
                    hexRenderer.material.SetColor("_Color", playerOwned);
                }
                //hexRenderer.material.SetColor("_Color", playerOwned);
            }
            if (PLAYER == 1) {
                for(int i = 0; i < gameObject.transform.childCount; i++) {
                    Transform Go = gameObject.transform.GetChild(i);
                    Transform GoChild = Go.transform.GetChild(0);
                    Dropzone GoDrop = GoChild.transform.GetComponent<Dropzone>();
                    GoDrop.Status = Dropzone.State.ENEMY;
                    hexRenderer = Go.GetComponent<Renderer>();
                    hexRenderer.material.SetColor("_Color", enemyOwned);
                }
                //hexRenderer.material.SetColor("_Color", enemyOwned);
            }
        }
    }

    void Start() {
        Ownership = PLAYER;

        //spawn = gameObject.GetComponent<MakePrefabAppear>();
        hexGrid = GameObject.FindGameObjectWithTag("HexGrid").GetComponent<HexGrid>();

        playerControllers = GameObject.FindGameObjectsWithTag("PlayerController");
        
        if (PLAYER == 0) {
            state = State.PLAYER;
        } else if (PLAYER == 1) {
            state = State.ENEMY;
        }
    }

    public void SetGrid(HexGrid grid) {
        this.grid = grid;
        grid.SendMessage("AddCapturePoint", this);
    }

    public HexPosition Coordinates {
        get {
            return position;
        }
        set {
            position = value;
            transform.position = value.getPosition();
            value.add("CapturePoint", this);
        }
    }

    public State Status {
        get { return state; }
        set { state = value; }
    }

    [Client]
    public void capture(Unit unit) {
        if (unit.PLAYER != PLAYER && state == State.NEUTRAL) {
            if (unit.PLAYER == PLAYER) {
                state = State.PLAYER;
                foreach (GameObject playerController in playerControllers) {
                    playerController.GetComponent<PlayerController>().AddGoldPerTurn(goldAmount);
                }
            } else {
                state = State.ENEMY;
            }
            Ownership = unit.PLAYER;
        } else if (unit.PLAYER != PLAYER && state != State.NEUTRAL) {
            state = State.NEUTRAL;
            Ownership = -1;

            if (unit.PLAYER != PLAYER) {
                foreach (GameObject playerController in playerControllers) {
                    playerController.GetComponent<PlayerController>().SubtractGoldPerTurn(goldAmount);
                }
            }
        }        
    }
}
