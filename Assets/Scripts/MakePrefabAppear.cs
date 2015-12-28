using UnityEngine;
using System.Collections;

public class MakePrefabAppear : MonoBehaviour {


	[SerializeField] private string _prefabName = "Type Prefab Name Here";
	[SerializeField] public Transform _where;
	private GameObject _theGameObject;
    private Unit unitStats;
	// Use this for initialization

	public void Appear () {
		if (_where == null) {
			_where = this.transform;
		}
		//Clear();
		_theGameObject = (GameObject)GameObject.Instantiate(Resources.Load(_prefabName));
		_theGameObject.transform.parent = _where;
		_theGameObject.transform.localPosition = Vector3.zero;
	}
	public void Appear (Vector3 lookAtPoint) {
		Appear ();
		_theGameObject.transform.LookAt(lookAtPoint);
	}

    public GameObject SpawnUnit(int i, int o) {
        if (_where == null) {
            _where = this.transform;
        }
        //Clear();
        _theGameObject = (GameObject)GameObject.Instantiate(Resources.Load(_prefabName));
        _theGameObject.transform.parent = _where;
        _theGameObject.transform.localPosition = Vector3.zero;
        _theGameObject.transform.rotation = Quaternion.identity;
        unitStats = _theGameObject.GetComponent<Unit>();
        unitStats.unitID = i;
        unitStats.ownership = o;
        _theGameObject.transform.parent = GameObject.FindGameObjectWithTag("UnitHolder").transform;
        return _theGameObject;
    }
    public GameObject SpawnUnitObject(int i, string n) {
        if (_where == null) {
            _where = this.transform;
        }
        //Clear();
        _theGameObject = (GameObject)GameObject.Instantiate(Resources.Load(n));
        _theGameObject.transform.parent = _where;
        _theGameObject.transform.localPosition = Vector3.zero;
        _theGameObject.transform.rotation = _where.rotation;
        return _theGameObject;
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