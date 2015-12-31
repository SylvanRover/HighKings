using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player_NetworkSetup : NetworkBehaviour {

    void Start () {
        if (isLocalPlayer) {
            gameObject.SetActive(false);
        }

	}

}
