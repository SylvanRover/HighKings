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
    
	private SimpleStatus.DropZoneState _state;


    void Start() {
		_state = new SimpleStatus.DropZoneState ();
		_state.isCapturePoint = true;
		_state.playerOccupier = -1;
		_state.owned = false;
        spawn = gameObject.GetComponent<MakePrefabAppear>();
        playerController = GameObject.FindGameObjectWithTag("PlayerController").GetComponent<PlayerController>();

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
		//FOR now no checking
        //if (isCapturePoint && isOwned && isAvailable) {
            //if (playerController.GoldCurrent >= unitCardStats.unitCost) {
                _state.owned = false;	
				_state.playerOccupier = SimpleNet.PlayerID;
				spawn.SpawnUnit(unitCardStats.unitID, unitCardStats.ownership);
                playerController.GoldCurrent = playerController.GoldCurrent - unitCardStats.unitCost;
           // }
        //}
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
