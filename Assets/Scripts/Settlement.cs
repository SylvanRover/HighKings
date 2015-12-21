using UnityEngine;
using System.Collections;
using Map.Events;

public class Settlement : MonoBehaviour {

	public GameObject minimapIcon;
	public GameObject selectionButton;
	public GameObject selectionRingObj;
	
	public SelectionRingAnim selectionRing;

	public GUI_Popup homeInfo;

	MiniMap_Cam minimapCamera;

	// Use this for initialization
	void Start () {
		minimapCamera = FindObjectOfType<MiniMap_Cam> ();
	}
	
	// Update is called once per frame
	void Update () {
		/*if (selectionRingObj != null && minimapCamera.MiniMapEnabled) {
			selectionRingObj.SetActive (false);
		}
		if (selectionRingObj != null && !minimapCamera.MiniMapEnabled){			
			selectionRingObj.SetActive (true);
		}
		if (selectionButton.activeInHierarchy == minimapCamera.MiniMapEnabled) {
			selectionButton.SetActive (!minimapCamera.MiniMapEnabled);
		}*/
	}

	public void Goto() {
		FindObjectOfType<Camera_zoom_RTS> ().ZoomCamera ();
		FindObjectOfType<Camera_pan_RTS> ().AnimateToWorldPos (this.transform.position);
	}
}
