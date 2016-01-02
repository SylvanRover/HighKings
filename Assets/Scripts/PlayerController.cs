using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : NetworkBehaviour {

    [SyncVar]
    public string playerUniqueName;
    private NetworkInstanceId playerNetID;
    private Transform myTransform;

    [SyncVar]
    public int playerID;
    public int popCap;
   [SerializeField] private int goldCurrent;
    public int goldMax;
    public int goldPerTurn;
    public int techAge = 0;
    public int playerRound = 0;
    private GameObject cam;

    private Text goldCurrentText;
    private Text goldPerTurnText;

    public int GoldCurrent {
        get { return goldCurrent; }
        set {
            goldCurrent = value;
            goldCurrentText.GetComponent<Text>().text = goldCurrent.ToString();
        }
    }

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
        GetNetIdentity();
        SetIdentity();
    }

    void Awake() {
        myTransform = transform;
    }

    [Client]
    void GetNetIdentity() {
        playerNetID = GetComponent<NetworkIdentity>().netId;
        CmdTellServerMyIdentity(MakeUniqueIdentity());
    }

    void SetIdentity() {
        if (!isLocalPlayer) {
            myTransform.name = playerUniqueName;
        } else {
            myTransform.name = MakeUniqueIdentity();
            int.TryParse(playerNetID.ToString(), out playerID);
        }
    }

    string MakeUniqueIdentity() {
        string uniqueName = "Player " + playerNetID.ToString();
        return uniqueName;
    }

    [Command]
    void CmdTellServerMyIdentity(string name) {
        playerUniqueName = name; 
    }



    // Start after scene is loaded
    void SceneReady() {
        if (isLocalPlayer) {
            Debug.LogError("Player ID is " + playerNetID.ToString());

            cam = GameObject.FindGameObjectWithTag("CameraHolder");
            cam.transform.position = transform.position;
            goldCurrentText = GameObject.Find("TEXTGoldCurrent").GetComponent<Text>();
            goldPerTurnText = GameObject.Find("TEXTGoldPerTurn").GetComponent<Text>();
            RoundStart();
        }
        myTransform = transform;
        transform.parent = GameObject.Find("Units").transform;        
    }

    void Update() {
        if (myTransform.name == "" || myTransform.name == "Player Castle(Clone)") {
            SetIdentity();
        }
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
            goldCurrentText.text = GoldCurrent.ToString();
            goldPerTurnText.text = ( "+" + goldPerTurn.ToString() );
        }
    }
}
