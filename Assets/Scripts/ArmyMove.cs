using UnityEngine;
using System.Collections;
public class ArmyMove : MonoBehaviour
{
	Vector3 newPosition;
	public bool selected = false;
	public Transform startMarker;
	public Transform endMarker;
	public float speed = 1.0f;
	private float startTime;
	private float journeyLength;


	public void ArmySelected() {
		selected = true;
	}

	void Start () {

	}

	void Update()
	{
		if (selected) {

		}
	}
}