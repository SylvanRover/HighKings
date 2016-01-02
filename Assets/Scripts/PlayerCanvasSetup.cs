using UnityEngine;
using System.Collections;

public class PlayerCanvasSetup : MonoBehaviour {

    private GameObject[] players;
    
    void Start () {
        players = GameObject.FindGameObjectsWithTag("PlayerController");
        foreach (GameObject player in players) {
            player.SendMessage("SceneReady");
        }
    }
}
