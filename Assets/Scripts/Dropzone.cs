using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Dropzone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

    public bool isCapturePoint = false;
    public bool isOwned = false;
    public bool isAvailable = true;
    private PlayerController playerController;
    private UnitCardStats unitCardStats;
    private MakePrefabAppear spawn;
    
    void Start() {
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

        if (isCapturePoint && isOwned && isAvailable) {
            spawn.Appear();
            if (playerController.GoldCurrent >= unitCardStats.unitCost) {
                playerController.GoldCurrent = playerController.GoldCurrent - unitCardStats.unitCost;
            }
        }
    }

}
