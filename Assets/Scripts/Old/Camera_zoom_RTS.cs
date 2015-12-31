using UnityEngine;
using System.Collections;

public class Camera_zoom_RTS : MonoBehaviour {
	public Transform lookatpoint;
	public float duration = 0.5f;
	public int speed = 20;
	public float zoomToHeight = 6;
	public float min;
	public float max;
	public float scrollWheelMultiplier;
	public Vector3 dir = Vector3.back;

	public Vector3 offset;

	// maps values in a clamped and linear fashion
	public static float LinearMap(float inVal, float inFrom, float inTo, float outFrom, float outTo) {
		float inScale = (inFrom != inTo) ? ((inVal - inFrom) / (inTo - inFrom)) : 0.0f;
		float outVal = outFrom + (inScale * (outTo - outFrom));
		outVal = (outFrom < outTo) ? Mathf.Clamp(outVal, outFrom, outTo) : Mathf.Clamp(outVal, outTo, outFrom);
		return outVal;
	}
	
	public static int LinearMap(int inVal, int inFrom, int inTo, int outFrom, int outTo) {
		double inScale = (inFrom != inTo) ? ((inVal - inFrom) / (double)(inTo - inFrom)) : 0.0;
		int outVal = outFrom + (int)System.Math.Round(inScale * (outTo - outFrom), System.MidpointRounding.AwayFromZero);
		outVal = (outFrom < outTo) ? Mathf.Clamp(outVal, outFrom, outTo) : Mathf.Clamp(outVal, outTo, outFrom);
		return outVal;
	}


	private Vector2 GetScaledTouchPosition(int index) {
		return Input.GetTouch(index).position ;//* Utilities.GetInputScale();
	}
	
	private Vector2 GetScaledTouchDeltaPosition(int index) {
		// fix or android zoom issues
		float dt = Time.deltaTime / Input.GetTouch(index).deltaTime;
		if(float.IsNaN(dt) || float.IsInfinity(dt)) {
			dt = 1.0f;
		}

		return Input.GetTouch(index).deltaPosition * /*Utilities.GetInputScale() **/ dt;
	}

	void MoveCamera(float delta) {
		Vector3 pos = transform.position;
		pos.y = Mathf.Clamp(pos.y - delta, min, max);
		transform.position = pos;
		transform.LookAt (lookatpoint, Vector3.up);
	}
	bool IsBusy = false;
	IEnumerator AnimateCamera(float delta) {
		IsBusy = true;
		float del = delta / duration;
		float oldDelta = delta;

		while (Mathf.Abs(delta) <= Mathf.Abs(oldDelta)) {
			oldDelta = delta;
			MoveCamera (del * Time.deltaTime);
			delta -= del * Time.deltaTime;

			if (Mathf.Abs(oldDelta) < Mathf.Abs(delta)) {
				IsBusy = false;
				yield break;
			}

			yield return null;
		}
		IsBusy = false;
		MoveCamera (delta); // one last move to get delta to 0
	}

	float lastTapTime = 0f;
	public void IconZoomCamera() {
		if (!IsBusy) {
			StartCoroutine(AnimateCamera(transform.position.y - zoomToHeight));
		}

	}

	public void ZoomCamera() {
		if (Time.time - lastTapTime < 0.5f && !IsBusy) {
			StartCoroutine(AnimateCamera(transform.position.y - zoomToHeight));
		}
		lastTapTime = Time.time;
	}

	void Start() {
		var recognizer = new TKRotationRecognizer();
		recognizer.gestureRecognizedEvent += ( r ) =>
		{
			//dir = Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(recognizer.deltaRotation * -20f * Time.deltaTime, Vector3.up), Vector3.one) * dir;
		};
		TouchKit.addGestureRecognizer( recognizer );


		var recognizer2 = new TKTapRecognizer ();
		recognizer2.numberOfTapsRequired = 2;
		recognizer2.gestureRecognizedEvent += ( r ) =>
		{
			// tap detected
			if (BlockerClicked.MayAnimateCamera()) {
				ZoomCamera();
			}
		};
		TouchKit.addGestureRecognizer (recognizer2);
	}

	void Update(){
		 /*if (Input.GetAxis ("Mouse ScrollWheel") > 0) {
			transform.Translate(Vector3.forward * speed * Time.deltaTime);
			transform.Rotate(transform.right * -speed * Time.deltaTime, Space.World);
		}
		if (Input.GetAxis ("Mouse ScrollWheel") < 0) {
			transform.Translate(Vector3.back * speed * Time.deltaTime);
			transform.Rotate(transform.right * speed * Time.deltaTime, Space.World);
		}*/

		float zoomDelta = -Input.GetAxis ("Mouse ScrollWheel") * scrollWheelMultiplier;
		if(Mathf.Approximately(zoomDelta, 0f)) {
			// Check if pinch gesture
			if(Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved) {
				Vector2 curDist = GetScaledTouchPosition(0) - GetScaledTouchPosition(1);   //current distance between finger touches
				Vector2 prevDist = ((GetScaledTouchPosition(0) - GetScaledTouchDeltaPosition(0)) - (GetScaledTouchPosition(1) - GetScaledTouchDeltaPosition(1)));     //difference in previous locations using delta positions
				zoomDelta = (curDist.magnitude - prevDist.magnitude) * -0.02f;
			}
		}


		Vector3 startOffset = transform.localPosition;
		offset = transform.localPosition;

		float y = offset.y;
		offset.y = 0;

		offset = dir.normalized * offset.magnitude;
		offset.y = y;
		transform.localPosition = offset;

		transform.position += (Vector3.up * speed * zoomDelta * transform.position.y * Time.deltaTime);

		Vector3 pos = transform.position;
		pos.y = Mathf.Clamp (pos.y, min, max);
		transform.position = pos;

		transform.LookAt (lookatpoint, Vector3.up);
	}
}