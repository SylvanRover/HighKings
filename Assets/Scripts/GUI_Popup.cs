using UnityEngine;
using System.Collections;
using Map.Events;

public class GUI_Popup : MonoBehaviour {
	
	public bool onUp = false;
	public bool onDrag = false;

	private Animator anim;
	private GameObject homeOBJ;
	private CommanderMGMT cMGMT;
	private bool guiOn = true;
	public float waitForAnim = 1f;

	IEnumerator WaitForAnim(){
		yield return new WaitForSeconds (waitForAnim);
		Destroy (this.gameObject);
	}

	public void InfoOn() {
		anim = GetComponent<Animator>();
		anim.SetBool("On", true);
#if !UNITY_EDITOR
		Camera_pan_RTS.AllowDrag = false;
#endif
	}

	public void InfoOff() {
		anim = GetComponent<Animator>();
		anim.SetBool("On", false);
	}

	public void InteractOn() {
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
		cMGMT.lockOnTarget = false;
		anim = GetComponent<Animator>();
		anim.SetBool("On", true);
	}
	
	public void InteractOff() {
		//if (GuiOn != guiOn) {
			anim = GetComponent<Animator> ();
			anim.SetBool ("On", false);
		//	StartCoroutine (WaitForAnim ());
		//}
	}

	void Start(){
		homeOBJ = GameObject.FindWithTag ("Home");
		if (homeOBJ != null) {
			cMGMT = homeOBJ.GetComponent<CommanderMGMT> ();
		}

		/*InteractionEvents.OnBroadcastSelected += InteractOff;		
		InteractionEvents.BroadcastGuiOn (guiOn);
		guiOn = false;*/
	}		
	
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
			InteractOn();
		}
		if (onUp || onDrag) {
			onDrag = false;
			onUp = false;
		}
	}
		
	public void BroadCastMyGuiOn () {
		InteractionEvents.BroadcastGuiOn (guiOn);
	}

}

namespace Map.Events {
	partial class InteractionEvents {
		
		public delegate void SimpleGUIEvent(bool GuiOn);
		private static SimpleGUIEvent OnBroadcastGuiOnDelegate;
		public static event SimpleGUIEvent OnBroadcastGuiOn {
			add {
				OnBroadcastGuiOnDelegate -= value;
				OnBroadcastGuiOnDelegate += value;
			} remove {
				OnBroadcastGuiOnDelegate -= value;
			}
		}
		public static void BroadcastGuiOn(bool GuiOn) {
			if(OnBroadcastGuiOnDelegate != null) {
				OnBroadcastGuiOnDelegate(GuiOn);
			}
		}
	}
}
