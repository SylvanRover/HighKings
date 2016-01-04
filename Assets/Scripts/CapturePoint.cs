using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class CapturePoint : MonoBehaviour {

    private HexGrid grid;
    private HexPosition position;
    public enum State { NEUTRAL, PLAYER1, PLAYER2 };
    public State state = State.NEUTRAL;

    private PlayerController playerController;
    private UnitCardStats unitCardStats;
    //private MakePrefabAppear spawn;
    public HexGrid hexGrid;

    private GameObject myObject;
    private Unit myUnit;

    public int captureState = 0;
    public int goldAmount;
    public bool isTower = false;
    public bool isBarracks = false;
    //public Sprite captureIcon;

    public int ownership = -1;

    public Image dropZoneImage;
    private Renderer hexRenderer;

    public Color neutralOwned;
    public Color playerOwned;
    public Color enemyOwned;

    public int Ownership {

        get { return ownership; }
        set {
            ownership = value;
            if (ownership == -1) {
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
            if (ownership == 0) {
                for(int i = 0; i < gameObject.transform.childCount; i++) {
                    Transform Go = gameObject.transform.GetChild(i);
                    Transform GoChild = Go.transform.GetChild(0);
                    Dropzone GoDrop = GoChild.transform.GetComponent<Dropzone>();
                    GoDrop.Status = Dropzone.State.PLAYER1;
                    hexRenderer = Go.GetComponent<Renderer>();
                    hexRenderer.material.SetColor("_Color", playerOwned);
                }
                //hexRenderer.material.SetColor("_Color", playerOwned);
            }
            if (ownership == 1) {
                for(int i = 0; i < gameObject.transform.childCount; i++) {
                    Transform Go = gameObject.transform.GetChild(i);
                    Transform GoChild = Go.transform.GetChild(0);
                    Dropzone GoDrop = GoChild.transform.GetComponent<Dropzone>();
                    GoDrop.Status = Dropzone.State.PLAYER2;
                    hexRenderer = Go.GetComponent<Renderer>();
                    hexRenderer.material.SetColor("_Color", enemyOwned);
                }
                //hexRenderer.material.SetColor("_Color", enemyOwned);
            }
        }
    }

    void Start() {
        Ownership = ownership;

        //spawn = gameObject.GetComponent<MakePrefabAppear>();
        hexGrid = GameObject.FindGameObjectWithTag("HexGrid").GetComponent<HexGrid>();

        playerController = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
        
        if (ownership == 0) {
            state = State.PLAYER1;
        } else if (ownership == 1) {
            state = State.PLAYER2;
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

    public void capture(Unit unit) {
        if (unit.ownership != ownership && state == State.NEUTRAL) {
            if (unit.ownership == 0) {
                state = State.PLAYER1;
                playerController.AddGoldPerTurn(goldAmount);
            } else {
                state = State.PLAYER2;
                playerController.AddGoldPerTurn(goldAmount);
            }
            Ownership = unit.ownership;
        } else if (unit.ownership != ownership && state != State.NEUTRAL) {
            state = State.NEUTRAL;
            Ownership = -1;

            if (unit.ownership != ownership) {
                playerController.SubtractGoldPerTurn(goldAmount);
            }
        }
    }
}
