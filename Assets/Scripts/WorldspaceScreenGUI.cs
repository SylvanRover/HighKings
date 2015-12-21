using UnityEngine;
using System.Collections;

public class WorldspaceScreenGUI : MonoBehaviour {

	public Transform target;

	void Update() {
		Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position);
	}
}