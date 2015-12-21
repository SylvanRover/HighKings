using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TKUITouchValidator
{
	public static bool uiNotValid(Vector2 touchPosition){
		PointerEventData data = new PointerEventData(EventSystem.current);
		data.position = touchPosition;
		List<RaycastResult> raycastResults = new List<RaycastResult>();
		EventSystem.current.RaycastAll(data, raycastResults);			
		
		foreach(RaycastResult res in raycastResults) {
			EventTrigger trig = res.gameObject.GetComponent<EventTrigger>();
			UnityEngine.UI.Graphic graphic = res.gameObject.GetComponent<UnityEngine.UI.Graphic>();
			if (graphic != null) {
				return false;
			}
		}
		
		return true;
	}

	public static bool uiNotValid(TKTouch touch){
		return uiNotValid(touch.position);
	}

	public static bool rayCastBlockersNotValid(Vector2 touchPosition){
		PointerEventData data = new PointerEventData(EventSystem.current);
		data.position = touchPosition;
		List<RaycastResult> raycastResults = new List<RaycastResult>();
		EventSystem.current.RaycastAll(data, raycastResults);			
		
		foreach(RaycastResult res in raycastResults) {
			EventTrigger trig = res.gameObject.GetComponent<EventTrigger>();
			UnityEngine.UI.Graphic graphic = res.gameObject.GetComponent<UnityEngine.UI.Graphic>();
			if (graphic != null) {

				CanvasGroup _group = graphic.GetComponentInParent<CanvasGroup>();
				while (_group!=null){
					if ( _group.blocksRaycasts){
						if (Application.isEditor){
							Debug.LogWarning("Collision with RayCastBlocker named: "+_group.name);
						}
						return false;
					}
					_group = _group.transform.parent.GetComponentInParent<CanvasGroup>();
				}
			}
		}
		
		return true;
	}
	
	public static bool rayCastBlockersNotValid(TKTouch touch){
		return rayCastBlockersNotValid(touch.position);
	}
}
