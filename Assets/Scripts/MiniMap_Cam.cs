using UnityEngine;
using System.Collections;

public class MiniMap_Cam : MonoBehaviour {
	public float treshold = 19f;
	
	public bool MiniMapEnabled = false;
	// Update is called once per frame
	void Update () {
		MiniMapEnabled = this.transform.position.y > treshold;
		this.GetComponent<Camera>().enabled = MiniMapEnabled;
	}
}
