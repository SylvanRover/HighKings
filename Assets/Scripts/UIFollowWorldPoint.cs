using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIFollowWorldPoint : MonoBehaviour {
	
	public Transform target;
	public Camera camera;	
	private Vector3 screenPos;

	void LateUpdate () {
		screenPos = camera.WorldToScreenPoint(target.position);	
		transform.position = screenPos;
	}
}
