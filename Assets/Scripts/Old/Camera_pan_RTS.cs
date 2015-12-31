using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Camera_pan_RTS : MonoBehaviour	
{

	public float maxSize;

	public Camera SceneCamera;

	// PRRIVATE VARIABLES FOR PANNING TO TARGET
	
	private float _lerp = 0f;
	public float duration = 0.25f;
	private float _minDistanceForPanLerp = 0.005f;

	// Tapping variables
	private float lastClickTime = -1.0f;
	private bool _possibleTap = false; // Was there the start of a tap
	private float _tapDownTime; // When did that start

	// Momentum and drag deltas
	private float _momentumStartTime;
	private float _timeSinceLastBigDrag;
	private float _timeCancelMomentum = 0.05f;
	private float _lastDragCap = 10.0f;
	private Vector2 _lastDragDelta;
	private Vector2 _dragMomentum = new Vector2(0.0f, 0.0f); // Current drag momentum applied after a drag is completed by the player.
	private List<Vector2> _dragDeltaList = new List<Vector2> (50);
	
	// PRIVATE VARIABLES FOR DRAG AND PINCH/ZOOM
	private bool _dragging = false; // Is there a drag currently in progress?
	private float _gestureScale = 0.02f; // No need to scale this for ipad vs ipad retina

	[SerializeField] private float _touchPanThreshold = 5f;

	public static bool AllowDrag = true;

	private void MoveCameraWorld(Vector3 position, bool applyZpostion) {
		if(applyZpostion) {
			transform.position = position;
		} else {
			transform.position = new Vector3(position.x, transform.position.y, position.z);
		}
		//CameraWasMoved();
	}
	
	IEnumerator AnimateCamera(Vector2 delta) {
		IsBusy = true;
		Vector2 del = delta / duration;
		Vector2 oldDelta = delta;

		while (delta.sqrMagnitude <= oldDelta.sqrMagnitude) {
			oldDelta = delta;
			MoveCamera (del * Time.deltaTime);
			delta -= del * Time.deltaTime;

			if (oldDelta.sqrMagnitude < delta.sqrMagnitude) {
				Vector2 goBack = delta - oldDelta;
				MoveCamera (goBack);
				IsBusy = false;
				yield break;
			}

			yield return null;
		}

		MoveCamera (delta); // one last move to get delta to 0
		IsBusy = false;
	}

	public void AnimateToWorldPos(Vector3 pos) {
		if (!IsBusy) {
			Vector2 delta = new Vector2(pos.x - this.transform.position.x, pos.z - this.transform.position.z);
			StartCoroutine(AnimateCamera(-delta));
		}
	}

	bool IsBusy = false;

	float lastTapTime = 0f;
	public void IconZoomCamera(Vector2 pos) {
		if (Time.time - lastTapTime < 0.5f && !IsBusy) {

			Vector2 delta = new Vector2(GetMouseWorldPos(oldPos).x - this.transform.position.x, GetMouseWorldPos(oldPos).z - this.transform.position.z);
			StartCoroutine(AnimateCamera(-delta));

		}
		
		//Debug.LogError ("" + Time.time);
		lastTapTime = Time.time;

	}

	public void ZoomCamera() {
		if (!IsBusy) {
			Vector2 delta = new Vector2(GetMouseWorldPos(oldPos).x - this.transform.position.x, GetMouseWorldPos(oldPos).z - this.transform.position.z);
			StartCoroutine(AnimateCamera(-delta));
		}
	}

	Vector2 dragLocation;
	Vector2 dragDelta;

	void Start() {

		TouchKit.removeAllGestureRecognizers ();

		var recognizer2 = new TKTapRecognizer ();
		recognizer2.numberOfTapsRequired = 2;
		recognizer2.gestureRecognizedEvent += ( r ) =>
		{
			// tap detected
			if (BlockerClicked.MayAnimateCamera()) {
				IconZoomCamera(r.startTouchLocation());
			}
		};
		TouchKit.addGestureRecognizer (recognizer2);

		var recognizer = new TKPanRecognizer ();
		recognizer.gestureRecognizedEvent += (obj) => {

			if (AllowDrag) {
				dragLocation =  recognizer.touchLocation();
				dragDelta = recognizer.deltaTranslation;


				Vector3 avgWorldOldPos = GetMouseWorldPos(dragLocation);
				Vector3 avgWorldPos = GetMouseWorldPos(dragLocation + dragDelta);
				
				Vector2 bla = new Vector2(avgWorldPos.x - avgWorldOldPos.x, avgWorldPos.z - avgWorldOldPos.z);

				MoveCamera(bla);
			}
		};

		recognizer.gestureCompleteEvent += (obj) => {
			AllowDrag = true;
		};
		TouchKit.addGestureRecognizer (recognizer);
	}

	void OnGUI() {
		//GUI.Label (new Rect (20, 220, 200, 30), "loc: " + dragLocation);
		//GUI.Label (new Rect (20, 260, 200, 30), "delta: " + dragDelta);
	}

	private void MoveCamera(Vector2 delta) {

		float x = transform.localPosition.x - delta.x;//xMathf.Clamp(_sceneCamera.transform.localPosition.x - delta.x, _panAreaRestriction.xMin, _panAreaRestriction.xMax);
		float z = transform.localPosition.z - delta.y;//Mathf.Clamp(_sceneCamera.transform.localPosition.y - delta.y, _panAreaRestriction.yMin, _panAreaRestriction.yMax);
		transform.localPosition = new Vector3(x, transform.localPosition.y , z);

		transform.localPosition = Vector3.ClampMagnitude(transform.localPosition, maxSize);

		//CameraWasMoved();
	}


	Vector3 GetMouseWorldPos(Vector3 pos) {
		Vector3 result = Vector3.zero;
		
		if (Camera.main == null) {
			return result;
		}

		pos = new Vector3(pos.x, pos.y , 0f);

		
		Plane pla = new Plane(Vector3.up, Vector3.zero);
		Ray ray = Camera.main.ScreenPointToRay(pos);

		float r = 0f;
		if (pla.Raycast(ray, out r)) {
			result = ray.GetPoint(r);
		}
		return result;
	}

	Vector3 GetMouseWorldPos() {
		Vector3 result = Vector3.zero;

		if (Camera.main == null) {
			return result;
		}

		Plane pla = new Plane(Vector3.up, Vector3.zero);
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		float r = 0f;
		if (pla.Raycast(ray, out r)) {
			result = ray.GetPoint(r);
		}
		return result;
	}

	private Vector2 GetScaledTouchPosition(int index) {
//#if UNITY_EDITOR
		return Input.mousePosition + new Vector3(50, 50, 0) * index;
//#endif
		//return Input.GetTouch(index).position ;//* Utilities.GetInputScale();
	}
	
	private Vector2 GetScaledTouchDeltaPosition(int index) {
		//#if UNITY_EDITOR
		return Input.mousePosition - oldPos;
		//#endif

		// fix or android zoom issues
		float dt = Time.deltaTime / Input.GetTouch(index).deltaTime;
		if(float.IsNaN(dt) || float.IsInfinity(dt)) {
			dt = 1.0f;
		}
		return Input.GetTouch(index).deltaPosition * /*Utilities.GetInputScale() **/ dt;
	}

	bool dragging = false;
	Vector3 oldPos;
	void Update() {
		if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began) {
			//start
			dragging = true;
		}

		if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended) {
			//stop
			dragging = false;
		}

		if (Input.touchCount > 1) {
			//stop
			dragging = false;
		}

		#if UNITY_EDITOR
		if (Input.GetMouseButtonDown(0)) {
			oldPos = Input.mousePosition;
		}
		dragging = Input.GetMouseButton(0);
		if (Input.GetMouseButton(0)) {

		#else
			if (Input.GetMouseButtonDown(0)) {
				oldPos = Input.mousePosition;
			}
			if (dragging) {
		#endif
		//if (/*Input.GetMouseButton(0) ||*/ Input.touchCount > 0) {
			
				/*Vector3 delta3 = GetMouseWorldPos() - oldPos;
				OnDrag(new Vector2(delta3.x, delta3.z));
				oldPos = GetMouseWorldPos();

			  }*/
			//if (Input.touchCount == 2) {

				//Vector2 curDist = GetScaledTouchPosition(0) - GetScaledTouchPosition(1);   //current distance between finger touches
				//Vector2 prevDist = ((GetScaledTouchPosition(0) - GetScaledTouchDeltaPosition(0)) - (GetScaledTouchPosition(1) - GetScaledTouchDeltaPosition(1)));     //difference in previous locations using delta positions

				Vector3 avgWorldOldPos = GetMouseWorldPos(((GetScaledTouchPosition(0) - GetScaledTouchDeltaPosition(0)) + (GetScaledTouchPosition(1) - GetScaledTouchDeltaPosition(1))) * 0.5f);
				Vector3 avgWorldPos = GetMouseWorldPos((GetScaledTouchPosition(0) + GetScaledTouchPosition(1)) * 0.5f);

				Vector2 bla = new Vector2(avgWorldPos.x - avgWorldOldPos.x, avgWorldPos.z - avgWorldOldPos.z);

				//OnDrag(new Vector2(bla.x, bla.y));
			//}

			oldPos = Input.mousePosition;
		}
	}

	void OnDrag(Vector2 delta) {



		this._dragging = true;
		float inputScale = 1.0f;
		
		// On ipad don't want panning while pinching
		//if(Input.touchCount > 1) {
		//	return;
	//	}
		//TODO: review is this inputScale needed here ?
		delta *= inputScale; // Scale ipad retina value is want to work in lower res space (1024x768)
		
		this._lastDragDelta = delta;
		if(Mathf.Abs(this._lastDragDelta.x) > this._lastDragCap * inputScale || Mathf.Abs(this._lastDragDelta.y) > this._lastDragCap * inputScale) {
			this._timeSinceLastBigDrag = Time.time;
		}
		
		if(this._dragDeltaList.Count < this._dragDeltaList.Capacity) {
			this._dragDeltaList.Add(delta);
		} else {
			this._dragDeltaList.RemoveAt(0);
			this._dragDeltaList.Add(delta);
		}


		MoveCamera(delta);

		
	}
		
}