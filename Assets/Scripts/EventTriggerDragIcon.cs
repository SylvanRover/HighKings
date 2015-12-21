using UnityEngine;
using System.Collections;

public class EventTriggerDragIcon : MonoBehaviour {
	
	public bool onUp = false;
	public bool onDrag = false;
	
	public void OnDragEnd() {
		onDrag = true;
	}
	
	public void OnPointerUp() {
		if (!onDrag) {
			onUp = true;
		}
	}
	
	void Update() {
		if (onUp && !onDrag) {
			Camera_pan_RTS[] cp_arr = FindObjectsOfType<Camera_pan_RTS> ();
			foreach(Camera_pan_RTS cp in cp_arr) {
				cp.AnimateToWorldPos(transform.position);
			}
			
			Camera_zoom_RTS[] cz_arr = FindObjectsOfType<Camera_zoom_RTS> ();
			foreach(Camera_zoom_RTS cz in cz_arr) {
				cz.IconZoomCamera();
			}
		}
		if (onUp || onDrag) {
			onDrag = false;
			onUp = false;
		}
	}
}
