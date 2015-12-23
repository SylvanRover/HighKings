using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Dropzone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {
	/*
    public bool isCapturePoint = false;
    public bool isOwned = false;
    public bool isAvailable = true;
    */
    private PlayerController playerController;
    private UnitCardStats unitCardStats;
    private MakePrefabAppear spawn;
    private HexGrid hexGrid;

    private SimpleStatus.DropZoneState _state;
    private CapturePoint capturePoint;


    void Start() {
		_state = new SimpleStatus.DropZoneState ();
		_state.isCapturePoint = true;
		_state.playerOccupier = -1;
		_state.owned = false;
        spawn = gameObject.GetComponent<MakePrefabAppear>();
        hexGrid = GameObject.FindGameObjectWithTag("HexGrid").GetComponent<HexGrid>();

        playerController = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();
        capturePoint = transform.parent.GetComponent<CapturePoint>();

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

        if (capturePoint.ownership == playerController.playerID) {
            _state.owned = true;
        }

		//FOR now no checking
        if (_state.owned) {
            //_state.owned = true;	
			_state.playerOccupier = SimpleNet.PlayerID;

            if (playerController.GoldCurrent >= unitCardStats.unitCost) {
                GameObject myObject = spawn.SpawnUnit(unitCardStats.unitID, unitCardStats.ownership);
                Unit myUnit = myObject.GetComponent<Unit>();
                myUnit.SetGrid(hexGrid);
                hexGrid.AddUnit(myUnit);
                playerController.GoldCurrent = playerController.GoldCurrent - unitCardStats.unitCost;
            }                
        }
        //}
    }

    public void Capture() {
        _state.owned = true;	
        _state.playerOccupier = SimpleNet.PlayerID;
    }

	public void UpdateState(SimpleStatus.DropZoneState state){
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
	}

}
