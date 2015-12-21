using UnityEngine;
using System.Collections;

public class OptionsButton : MonoBehaviour {

	public GameObject optionsPanel;

	public void OpenOptions(){
		optionsPanel.SetActive (true);
	}
	public void CloseOptions(){
		optionsPanel.SetActive (false);
	}
}
