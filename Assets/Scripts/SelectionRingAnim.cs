using UnityEngine;
using System.Collections;
using Map.Events;

public class SelectionRingAnim : MonoBehaviour {

	public bool onUp = false;
	public bool onDrag = false;
	public bool selected = false;
	private Animator anim;

	public ArmyStats stats;

	public void Normal() {
		anim = GetComponent<Animator>();
		if (!selected) {
			anim.SetTrigger ("Normal");
		}
		anim.SetBool("Highlighted", false);
		//selected = false;
	}
	public void Selected() {
		if (stats != null && stats.commanderIsActive) {
			anim = GetComponent<Animator> ();
			anim.SetTrigger ("Selected");
		}
		if (stats == null) {	
			anim = GetComponent<Animator> ();
			anim.SetTrigger ("Selected");
		}
	}

	public void Deselect(){
		selected = false;
	}

	public void Highlighted() {
		if (stats != null && stats.commanderIsActive) {
			anim = GetComponent<Animator>();
			anim.SetBool("Highlighted", true);
		}
		if (stats == null) {
			anim = GetComponent<Animator>();
			anim.SetBool("Highlighted", true);
		}		
	}
	public void Pressed() {
		if (stats != null && stats.commanderIsActive) {	
			anim = GetComponent<Animator>();
			anim.SetTrigger ("Pressed");
			selected = true;	
		}
		if (stats == null) {	
			anim = GetComponent<Animator>();
			anim.SetTrigger ("Pressed");
			selected = true;	
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
	
	void Update() {
		if (onUp && !onDrag) {
			Pressed();
			Selected();
		}
		if (onUp || onDrag) {
			onDrag = false;
			onUp = false;
		}
	}
}