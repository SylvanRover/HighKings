using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

    public int playerID;
    public int popCap;
    public int goldCurrent;
    public int goldMax;
    public int goldPerTurn;
    public int techAge = 0;
    public int playerRound = 0;

    public Text goldCurrentText;
    public Text goldPerTurnText;

    /*[System.Serializable]
    public class Unit {
        public string Name;
        public int ID;
        public int Level;
    }

    public List<Unit> unitData;*/

    void Start() {
        RoundStart();
    }

    public void RoundStart() {
        goldCurrent = goldCurrent + goldPerTurn;
        if (goldCurrent > goldMax) {
            goldCurrent = goldMax;
        }
        goldCurrentText.text = goldCurrent.ToString();
        goldPerTurnText.text = ( "+" + goldPerTurn.ToString() );
    }

}
