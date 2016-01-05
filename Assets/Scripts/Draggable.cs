using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    public Transform ParentToReturnTo = null;
    GameObject placeholder = null;

	public void OnBeginDrag( PointerEventData eventData ) {
        Debug.Log("OnBeginDrag");

        placeholder = new GameObject();
        placeholder.transform.SetParent( this.transform.parent );
        LayoutElement le = placeholder.AddComponent<LayoutElement>();
        le.preferredWidth = this.GetComponent<LayoutElement>().preferredWidth;
        le.preferredHeight = this.GetComponent<LayoutElement>().preferredHeight;
        le.flexibleWidth = 0;
        le.flexibleHeight = 0;

        placeholder.transform.SetSiblingIndex( this.transform.GetSiblingIndex() );

        ParentToReturnTo = this.transform.parent;
        this.transform.SetParent( this.transform.parent.parent );

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) {
        //Debug.Log("OnDrag");

        this.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData) {
        Debug.Log("OnEndDrag");
        this.transform.SetParent( ParentToReturnTo );
        this.transform.SetSiblingIndex( placeholder.transform.GetSiblingIndex() );

        //this.transform.localPosition = Vector3.zero;
        //this.transform.localScale = Vector3.one;
        //this.transform.rotation = Quaternion.identity;

        Destroy(placeholder);
    }

    public void ResetRaycast(bool b) {
        GetComponent<CanvasGroup>().blocksRaycasts = b;
    }
}
