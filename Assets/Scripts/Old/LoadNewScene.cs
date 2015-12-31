using UnityEngine;
using System.Collections;

public class LoadNewScene : MonoBehaviour {

	public void LoadScene(string name) {
		Application.LoadLevel (name);
	}
}
