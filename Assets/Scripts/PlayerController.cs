using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

    public int playerID;
    private int maxPlayers = 1;
    public int popCap;
   [SerializeField] private int goldCurrent;
    public int goldMax;
    public int goldPerTurn;
    public int techAge = 0;
    public int playerRound = 0;

    public Text goldCurrentText;
    public Text goldPerTurnText;

    public int GoldCurrent {

        get { return goldCurrent; }
        set {
            goldCurrent = value;
            goldCurrentText.text = goldCurrent.ToString();
        }
    }
    [SerializeField]
    private int goldCurrent2;
    public int goldMax2;
    public int goldPerTurn2;

    public int GoldCurrent2 {

        get { return goldCurrent2; }
        set {
            goldCurrent2 = value;
            goldCurrentText.text = goldCurrent.ToString();
        }
    }

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

    public void SwitchToPlayer(int p) {
        playerID = p;
        if (playerID == 0) {
            goldCurrentText.text = goldCurrent.ToString();
            goldPerTurnText.text = ("+" + goldPerTurn.ToString());
        } else {
            goldCurrentText.text = goldCurrent2.ToString();
            goldPerTurnText.text = ("+" + goldPerTurn2.ToString());
        }
    }


    public void AddGoldPerTurn(int goldAmount) {
        if (playerID == 0) {
            goldPerTurn += goldAmount;
            goldPerTurnText.text = ("+" + goldPerTurn.ToString());
        } else {
            goldPerTurn2 += goldAmount;
            goldPerTurnText.text = ("+" + goldPerTurn2.ToString());
        }
    }
    public void SubtractGoldPerTurn(int goldAmount) {
        if (playerID == 0) {
            goldPerTurn -= goldAmount;
            goldPerTurnText.text = ("+" + goldPerTurn.ToString());
        } else {
            goldPerTurn2 -= goldAmount;
            goldPerTurnText.text = ("+" + goldPerTurn2.ToString());
        }
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
