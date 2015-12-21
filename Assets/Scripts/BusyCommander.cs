using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BusyCommander : MonoBehaviour {

	private GameObject home;
	private CommanderMGMT cMGMT;
	public Button attackButton;
	public Text interactTextField;
	public string interactText;
	public string busyCommanderText;
	public string busyEnemyText;

	public bool busyEnemy = false; 

	public void BusyEnemy(){
		busyEnemy = true;
	}

	void Start () {
		home = GameObject.FindGameObjectWithTag ("Home");
		cMGMT = home.GetComponent<CommanderMGMT> ();
	}

	void LateUpdate () {
		if (cMGMT.commanderButtonStats.commanderIsActive) {
			//attackButton.enabled = false;
			interactTextField.text = busyCommanderText;
			attackButton.interactable = false;
		} else if (busyEnemy) {			
			interactTextField.text = busyEnemyText;
			attackButton.interactable = false;
		} else {
			//attackButton.enabled = true;
			interactTextField.text = interactText;
			attackButton.interactable = true;
		}
	}
}
