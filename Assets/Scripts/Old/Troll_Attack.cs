using UnityEngine;
using System.Collections;

public class Troll_Attack : MonoBehaviour {

	public Transform _FX_CreationTarget_00;
	public Transform FX_spawnPoint_00;
	public GameObject FX_00;

	private IEnumerator SetLayerRecursively(GameObject go, int layerNumber) {
		while(!go.activeInHierarchy) {
			yield return null;
		}
		yield return null;
		//if (go.layer != layerNumber) {
		go.layer = layerNumber;
		foreach(Transform trans in go.GetComponentsInChildren<Transform>(true)) {
			trans.gameObject.layer = layerNumber;
		}
		//}
	}

	void TrollAttackFX () {
		int layer = LayerMask.NameToLayer("Default");
		GameObject _FX_00 = (GameObject)Instantiate(FX_00, FX_spawnPoint_00.position, FX_spawnPoint_00.rotation);
		_FX_00.transform.parent = _FX_CreationTarget_00;
		_FX_00.layer = layer;
		StartCoroutine(SetLayerRecursively(_FX_00, this.gameObject.layer));
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
