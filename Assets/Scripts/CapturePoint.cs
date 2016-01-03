using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.EventSystems;

public class CapturePoint : NetworkBehaviour {

    private HexGrid grid;
    private HexPosition position;

    public enum State { NEUTRAL, PLAYER, ENEMY };
    public State state = State.NEUTRAL;
    [SyncVar (hook = "ChangeOwnership")] public int PLAYER = -1;

    private GameObject[] players;
    public PlayerController player;
    private UnitCardStats unitCardStats;
    //public HexGrid hexGrid;

    private GameObject myObject;
    private Unit myUnit;
    private GameObject captureParent;

    public int captureState = 0;
    public int goldAmount;
    public bool isTower = false;
    public bool isBarracks = false;


    public Image dropZoneImage;
    private Renderer hexRenderer;

    public Color neutralOwned;
    public Color playerOwned;
    public Color enemyOwned;

    void Start() {
        //if (transform.parent.tag != "PlayerController") {
        captureParent = GameObject.Find("CapturePoints");
        transform.parent = captureParent.transform;
        unitCardStats = GameObject.FindGameObjectWithTag("UnitCard").GetComponent<UnitCardStats>();
        grid = GameObject.FindGameObjectWithTag("HexGrid").GetComponent<HexGrid>();
        //Ownership = PLAYER;
        //spawn = gameObject.GetComponent<MakePrefabAppear>();
        //}        
    }

    /*void SceneReady() {
        captureParent = GameObject.Find("CapturePoints");
        transform.parent = captureParent.transform;
        unitCardStats = GameObject.FindGameObjectWithTag("UnitCard").GetComponent<UnitCardStats>();
        grid = GameObject.FindGameObjectWithTag("HexGrid").GetComponent<HexGrid>();
        //Ownership = PLAYER;
        //spawn = gameObject.GetComponent<MakePrefabAppear>();
    }*/

    public int Ownership {

        get { return PLAYER; }
        set {
            PLAYER = value;
            players = GameObject.FindGameObjectsWithTag("PlayerController");
            foreach (GameObject p in players) {
                if (p.GetComponent<NetworkIdentity>().isLocalPlayer) {
                    player = p.GetComponent<PlayerController>();
                }
            }
            if (PLAYER == -1) {
                for (int i = 0; i < gameObject.transform.childCount; i++) {
                    Transform Go = gameObject.transform.GetChild(i);
                    Transform GoChild = Go.transform.GetChild(0);
                    Dropzone GoDrop = GoChild.transform.GetComponent<Dropzone>();
                    GoDrop.Status = Dropzone.State.NEUTRAL;
                    state = State.NEUTRAL;
                    hexRenderer = Go.GetComponent<Renderer>();
                    hexRenderer.material.SetColor("_Color", neutralOwned);
                }
                //hexRenderer.material.SetColor("_Color", neutralOwned);
            } else if (PLAYER == player.playerID) {
                for(int i = 0; i < gameObject.transform.childCount; i++) {
                    Transform Go = gameObject.transform.GetChild(i);
                    Transform GoChild = Go.transform.GetChild(0);
                    Dropzone GoDrop = GoChild.transform.GetComponent<Dropzone>();
                    GoDrop.Status = Dropzone.State.PLAYER;
                    state = State.PLAYER;
                    hexRenderer = Go.GetComponent<Renderer>();
                    hexRenderer.material.SetColor("_Color", playerOwned);
                }
                //hexRenderer.material.SetColor("_Color", playerOwned);
            } else {
                for(int i = 0; i < gameObject.transform.childCount; i++) {
                    Transform Go = gameObject.transform.GetChild(i);
                    Transform GoChild = Go.transform.GetChild(0);
                    Dropzone GoDrop = GoChild.transform.GetComponent<Dropzone>();
                    GoDrop.Status = Dropzone.State.ENEMY;
                    state = State.ENEMY;
                    hexRenderer = Go.GetComponent<Renderer>();
                    hexRenderer.material.SetColor("_Color", enemyOwned);
                }
                //hexRenderer.material.SetColor("_Color", enemyOwned);
            }
        }
    }

    void ChangeOwnership(int o) {
        Ownership = o;
    }

    public State Status {
        get { return state; }
        set { state = value; }
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

    public void capture(Unit unit) {
        if (unit.PLAYER != PLAYER && state == State.NEUTRAL) {
            if (unit.PLAYER == PLAYER) {
                state = State.PLAYER;
                player.AddGoldPerTurn(goldAmount);
            } else {
                state = State.ENEMY;
            }
            Ownership = unit.PLAYER;
        } else if (unit.PLAYER != PLAYER && state != State.NEUTRAL) {
            state = State.NEUTRAL;
            Ownership = -1;

            if (unit.PLAYER != PLAYER) {
                player.SubtractGoldPerTurn(goldAmount);
            }
        }        
    }
}
