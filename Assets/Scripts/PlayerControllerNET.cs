using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class PlayerControllerNET : NetworkBehaviour {

    public int playerID;
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

    void Start() {
        RoundStart();
        goldCurrentText = GameObject.Find("Current Gold").GetComponent<Text>();
        goldPerTurnText = GameObject.Find("Gold Per Turn").GetComponent<Text>(); ;
    }

    public void AddGoldPerTurn(int goldAmount) {
        if (isLocalPlayer) {
            goldPerTurn += goldAmount;
            goldPerTurnText.text = ("+" + goldPerTurn.ToString());
        }
        
    }

    public void SubtractGoldPerTurn(int goldAmount) {
        if (isLocalPlayer) {
             goldPerTurn -= goldAmount;
             goldPerTurnText.text = ("+" + goldPerTurn.ToString());
        }       
    }

    public void RoundStart() {
        if (isLocalPlayer) {
            goldCurrent = goldCurrent + goldPerTurn;
            if (goldCurrent > goldMax) {
                goldCurrent = goldMax;
            }
            goldCurrentText.text = goldCurrent.ToString();
            goldPerTurnText.text = ( "+" + goldPerTurn.ToString() );
        }
    }
}
