using UnityEngine;
using System.Collections;

public class Blocker : MonoBehaviour {

	public void CloseUI() {

		Debug.LogError("blocker clicked");
		GUI_Popup[] arr = FindObjectsOfType<GUI_Popup> ();
		foreach(GUI_Popup p in arr) {
			p.InteractOff();
		}
	}
}
