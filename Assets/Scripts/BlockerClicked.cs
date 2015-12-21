using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class BlockerClicked : MonoBehaviour {
	public GameObject go;
	static GameObject lastPress;
	static GameObject blockerGO;

	void Start() {
		blockerGO = this.gameObject;
	}

	public static bool MayAnimateCamera ()
	{
		return lastPress != null && (lastPress == blockerGO || lastPress.GetComponent<Terrain>() != null);
	}

	void Update() {
		// get the gameobject where the cursor is
		PointerEventData cursor = new PointerEventData(EventSystem.current);                            // This section prepares a list for all objects hit with the raycast
		cursor.position = Input.mousePosition;
		List<RaycastResult> objectsHit = new List<RaycastResult> ();
		EventSystem.current.RaycastAll(cursor, objectsHit);
		int count = objectsHit.Count;
		int x = 0;

		if(objectsHit.Count > 0)                                         
		{    
			go = objectsHit[0].gameObject;
			lastPress = go;
		}
	}
}
