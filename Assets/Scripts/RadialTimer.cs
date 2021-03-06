﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RadialTimer : MonoBehaviour {

	public Unit unitStats;
	//public Text textLevel;
	public Image damageTimer;
	public Image healthTimer;
	//public Image actionTimer;
	//public Image actionBubble;
	//public Sprite actionIdle;
	//public Sprite actionMoving;
	//public Sprite actionBattleAttacking;
	//public Sprite actionBattleDefending;
	//public Sprite actionDefending;
	//public Sprite actionHarvesting;
	//public Color colorMoving;
	//public Color colorBattleAttacking;
	//public Color colorBattleDefending;
	//public Color colorHarvesting;

	// Update is called once per frame
	void Update () {

		//textLevel.text = commanderMGMT.commanderButtonStats.commanderLevel.ToString();

		if (unitStats != null) {
			//army = commanderMGMT.commanderButtonStats;
			damageTimer.fillAmount = (unitStats.damageRect.sizeDelta.x / unitStats.resetSize.x);
			healthTimer.fillAmount = (unitStats.hp / unitStats.MAX_HP);
			
			/*if (army.actionMoving) {
				actionBubble.sprite = actionMoving;
				actionTimer.color = colorMoving;
				actionTimer.fillAmount = army.progressLine._progress01 / 2;
			} else if (army.actionBattleAttacking){				
				actionBubble.sprite = actionBattleAttacking;
				actionTimer.color = colorBattleAttacking;
				if (army.armyCol.battleTiming){
					actionTimer.fillAmount = (army.armyCol.battleCountdown/army.armyCol.battleTime)/2;
				}
			} else if (army.actionBattleDefending){				
				actionBubble.sprite = actionBattleDefending;
				actionTimer.color = colorBattleDefending;
				actionTimer.fillAmount = 0f;
			} else if (army.actionDefending){				
				actionBubble.sprite = actionDefending;
				actionTimer.fillAmount = 0f;
			} else if (army.actionHarvesting){				
				actionBubble.sprite = actionHarvesting;
				actionTimer.color = colorHarvesting;
				actionTimer.fillAmount = (army.armyCol.harvestCountdown/army.armyCol.harvestTime)/2;
			} else {
				actionBubble.sprite = actionIdle;
				actionTimer.fillAmount = 0f;
			}*/
		//} else {
		//	actionTimer.fillAmount = 0f;
		}
	}
}
