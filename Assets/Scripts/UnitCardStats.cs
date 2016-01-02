using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class UnitCardStats : NetworkBehaviour {

    
    private NetworkInstanceId cardNetID;
    public int cardID;

    public int unitID;
    public string unitName;
    public int unitCost = 1;
    public Text unitNameText;
    public Text unitCostText;

    private GameObject[] players;

    void Start() {
        players = GameObject.FindGameObjectsWithTag("PlayerController");
        GetNetIdentity();
        //if (cardID == player.playerID) {

        //}
        unitNameText.text = unitName;
        unitCostText.text = unitCost.ToString();
    }

    [Client]
    void GetNetIdentity() {
        foreach (GameObject player in players) {
            if (player.GetComponent<NetworkIdentity>().isLocalPlayer) {
                cardNetID = player.GetComponent<NetworkIdentity>().netId;
                int.TryParse(cardNetID.ToString(), out cardID);
                CmdTellServerMyIdentity(cardID);
                Debug.LogError("Card ID is " + cardID);
            }
        }
    }

    [Command]
    void CmdTellServerMyIdentity(int id) {
        cardID = id;
    }

}
