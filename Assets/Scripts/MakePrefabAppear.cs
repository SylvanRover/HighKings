using UnityEngine;
using System.Collections;

public class MakePrefabAppear : MonoBehaviour {


	[SerializeField] private string _prefabName = "Type Prefab Name Here";
	[SerializeField] public Transform _where;
	private GameObject _theGameObject;
	// Use this for initialization

	public void Appear () {
		if (_where == null) {
			_where = this.transform;
		}
		Clear();
		_theGameObject = (GameObject)GameObject.Instantiate(Resources.Load(_prefabName));
		_theGameObject.transform.parent = _where;
		_theGameObject.transform.localPosition = Vector3.zero;
	}
	public void Appear (Vector3 lookAtPoint) {
		Appear ();
		_theGameObject.transform.LookAt(lookAtPoint);
	}
	
	// Update is called once per frame
	public void Clear() {
		if (_theGameObject != null) {
			Destroy (_theGameObject);
		}
	}

	public GameObject SpawnAPrefab(string name, Vector3 pos) {
		GameObject go = (GameObject)GameObject.Instantiate(Resources.Load(name));
		go.transform.position = pos;

		return go;
	}
}