using UnityEngine;
using System.Collections;

public class MapRender : MonoBehaviour {

	public int mapRenderQueue = 1;

	// Use this for initialization
	void Start () {
		GetComponent<Renderer>().sortingOrder = mapRenderQueue;
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<Renderer>().sortingLayerName = "MiniMap";
	}
}
