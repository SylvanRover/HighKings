using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using Map.Events;

public class EventTriggerDrag : MonoBehaviour {

	public bool onUp = false;
	public bool onDrag = false;
	private GameObject homeObj;
	private CommanderMGMT cMGMT;

	public void CloseUI() {
		if (onUp && !onDrag) {
			//Debug.LogError("blocker clicked");
			GUI_Popup[] arr = FindObjectsOfType<GUI_Popup> ();
			foreach(GUI_Popup p in arr) {
				p.InteractOff();
			}
			SelectionRingAnim[] s_arr = FindObjectsOfType<SelectionRingAnim> ();
			foreach(SelectionRingAnim s in s_arr) {
				if (s.stats == null){
					s.selected = false;
					s.Normal();
				}
			}
			/*ProgressLine[] p_arr = FindObjectsOfType<ProgressLine> ();
			foreach(ProgressLine p in p_arr) {
				p.UnSelect();
			}*/
		}
	}
	public void OnDragEnd() {
		onDrag = true;
	}

	public void OnPointerUp() {
		if (!onDrag) {
			onUp = true;
		}
	}

	public void OnClick(){		
		cMGMT.lockOnTarget = false;
	}

	void Start(){
		homeObj = GameObject.FindWithTag ("Home");
		cMGMT = homeObj.GetComponent<CommanderMGMT> ();
	}

	void Update() {
		if (onUp && !onDrag) {
			//Debug.LogError("blocker clicked");
			GUI_Popup[] arr = FindObjectsOfType<GUI_Popup> ();
			foreach(GUI_Popup p in arr) {
				p.InteractOff();
			}
			SelectionRingAnim[] s_arr = FindObjectsOfType<SelectionRingAnim> ();
			foreach(SelectionRingAnim s in s_arr) {
				if (s.stats == null){
					s.selected = false;
					s.Normal();
				}
			}
			/*ProgressLine[] p_arr = FindObjectsOfType<ProgressLine> ();
			foreach(ProgressLine p in p_arr) {
				p.UnSelect();
			}*/
		}
		if (onUp || onDrag) {
			onDrag = false;
			onUp = false;
		}
	}
}