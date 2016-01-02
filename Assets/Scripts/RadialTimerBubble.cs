using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RadialTimerBubble : MonoBehaviour {

	//private ArmyStats army;
	//private UICommanderBubble bubbleUI;
	public Image actionTimer;
	public Color colorMoving;
	public Color colorBattleAttacking;
	public Color colorBattleDefending;
	public Color colorHarvesting;
	
	// Use this for initialization
	void Start () {	
		//bubbleUI = GetComponent<UICommanderBubble> ();
		//army = bubbleUI.targetButton;
	}
	
	// Update is called once per frame
	void Update () {		
		
		/*if (bubbleUI.targetButton != null) {
			if (army.actionMoving) {
				actionTimer.color = colorMoving;
				actionTimer.fillAmount = army.progressLine._progress01;
			} else if (army.actionBattleAttacking) {
				actionTimer.color = colorBattleAttacking;
				if (army.armyCol.battleTiming) {
					actionTimer.fillAmount = army.armyCol.battleCountdown / army.armyCol.battleTime;
				}
			} else if (army.actionBattleDefending) {
				actionTimer.color = colorBattleDefending;
				actionTimer.fillAmount = 0f;
			} else if (army.actionDefending) {
				actionTimer.fillAmount = 0f;
			} else if (army.actionHarvesting) {
				actionTimer.color = colorHarvesting;
				actionTimer.fillAmount = army.armyCol.harvestCountdown/army.armyCol.harvestTime;
			} else {
				actionTimer.fillAmount = 0f;
			}
		} else {
			actionTimer.fillAmount = 0f;
		}*/
	}
}
