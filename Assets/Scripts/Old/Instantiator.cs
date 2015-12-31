using UnityEngine;
using System.Collections;

public class Instantiator : MonoBehaviour {

	public GameObject stuffToSpawn;
	public float chanceToSpawn = 0.5f;

	// Use this for initialization
	void Start () {
		if (Random.Range(0f, 1f) < chanceToSpawn) {
			GameObject go = GameObject.Instantiate (stuffToSpawn);
			go.transform.parent = this.transform;
			go.transform.localPosition = Vector3.zero;
			//go.transform.localScale = Vector3.one;
			go.transform.localRotation = Quaternion.identity;
		}
	}

}
