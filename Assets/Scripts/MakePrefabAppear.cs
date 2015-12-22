using UnityEngine;
using System.Collections;

public class MakePrefabAppear : MonoBehaviour {


	[SerializeField] private string _prefabName = "Type Prefab Name Here";
	[SerializeField] public Transform _where;
	private GameObject _theGameObject;
    private UnitStats unitStats;
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

    public void SpawnUnit(int i, int o) {
        if (_where == null) {
            _where = this.transform;
        }
        Clear();
        _theGameObject = (GameObject)GameObject.Instantiate(Resources.Load(_prefabName));
        _theGameObject.transform.parent = _where;
        _theGameObject.transform.localPosition = Vector3.zero;
        unitStats = _theGameObject.GetComponent<UnitStats>();
        unitStats.unitID = i;
        unitStats.ownership = o;
        unitStats.SetUnitType();
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