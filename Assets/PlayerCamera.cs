using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {

	void Start () {
        if (!GetComponent<NetworkView>().isMine) { Camera.main.GetComponent<Camera>().enabled = false; }
    }

}
