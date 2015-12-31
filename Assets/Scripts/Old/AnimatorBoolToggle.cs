using UnityEngine;
using System.Collections;

public class AnimatorBoolToggle : MonoBehaviour {

	Animator ani;
	// Use this for initialization
	void Start () {
		ani = GetComponent<Animator> ();
	}
	
	public void Toggle(string boolName) {
		ani.SetBool(boolName, !ani.GetBool (boolName));
	}
}
