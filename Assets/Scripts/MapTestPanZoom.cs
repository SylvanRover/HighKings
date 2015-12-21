//using UnityEngine;
//using System.Collections.Generic;
//using Digit.Shared.Utility;
//using Digit.Kotr.Services;
//using Digit.Kotr.Services.Tutorial;
//using Digit.Lumberjack;
//
///**
//* MapTestPanZoom gets touch events from attached collider and pans and zooms the panZoomGroup accordingly
//*/
//
//// Namespaces don't work for MonoBehaviours
//using Digit.Kotr.Events;
//
//
//public class MapTestPanZoom : MonoBehaviour {
//
//    //======
//    //Think need
//	public float inputScale = 1f;
//    public const float MaxZoom = 7f;
//    public const float MinZoom = 1f;
//    private const float MinSensitivity = 2f;
//    private const float MaxSensitivity = 6f;
//    public static float ZoomSensitivity = 4f;
//
//    public static Camera SceneCamera = null;
//
//    private static bool _userInteractionFrozen = false;
//    private static bool _codeInteractionFrozen = false;
//
//    // INSPECTOR VARIABLES
//    [SerializeField] private AnimationCurve _lerpCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
//    [SerializeField] private AnimationCurve _momentumCurve;
//    [SerializeField] private Camera _sceneCamera;
//    [SerializeField] private Rect _panAreaRestriction;
//    [SerializeField] private float _touchPanThreshold = 5f;
//    [SerializeField] private float _maxTapTime = 0.5f;
//
//    // PRRIVATE VARIABLES FOR PANNING TO TARGET
//
//    private float _lerp = 0f;
//    private float _minDistanceForPanLerp = 0.005f;
//
//    // Tapping variables
//    private float lastClickTime = -1.0f;
//    private bool _possibleTap = false; // Was there the start of a tap
//    private float _tapDownTime; // When did that start
//
//
//    // Momentum and drag deltas
//    private float _momentumStartTime;
//    private float _timeSinceLastBigDrag;
//    private float _timeCancelMomentum = 0.05f;
//    private float _lastDragCap = 10.0f;
//    private Vector2 _lastDragDelta;
//    private Vector2 _dragMomentum = new Vector2(0.0f, 0.0f); // Current drag momentum applied after a drag is completed by the player.
//    private List<Vector2> _dragDeltaList = new List<Vector2> (50);
//
//    // PRIVATE VARIABLES FOR DRAG AND PINCH/ZOOM
//    private bool _dragging = false; // Is there a drag currently in progress?
//    private float _gestureScale = 0.02f; // No need to scale this for ipad vs ipad retina
//    //======
//
//    //Might need
//    //public const float SceneCameraDepth = 2f; // Make sure only map camera has this depth
//
//    //private float _lerpMult = 4f;
//    // public delegate void OnClickBackgroundHandler();
//
//    //======
//    // Think don't need
//    // Delegates for other code interested in zoom changes
//    public event ZoomValueDelegate OnZoomValueChanged;
//    public event ZoomNormalizedDelegate OnZoomNormalizedChanged;
//    public event OnZoomCompleteDelegate OnZoomComplete;
//    public delegate void ZoomValueDelegate(float realZoomLevel);
//    public delegate void ZoomNormalizedDelegate(float normalizedZoomLevel);
//    public delegate void OnZoomCompleteDelegate();
//
//    // Delegates for other code interested in pan changes
//    public event TargetReachedDelegate OnTargetReached;
//    public delegate void TargetReachedDelegate();
//
//    public event PanDelegate OnPanChanged;
//    public delegate void PanDelegate(Vector2 position);
//    public event PanOrZoomChanged OnLateChange;
//    public delegate void PanOrZoomChanged(Vector2 panPosition, float zoom);
//
//
//    private float _lerpMult = 4f;
//    private static bool _HaveATargetPosition = false;
//
//    private static Vector3 _StartingPositionPan = Vector3.zero;
//    private static Vector3 _PanTarget = Vector3.zero;
//    private static float _ZoomNormalizedTarget = 0f;
//    private static float _StartZoomNormalized = 0f;
//
//    public static void SetPanZoomTarget(Vector3 targetPosition, float targetNormalizedZoomLevel) {
//        _StartingPositionPan = SceneCamera.transform.localPosition;
//		_StartZoomNormalized = Mathf.InverseLerp(MapTestPanZoom.MaxZoom, MapTestPanZoom.MinZoom, MapTestPanZoom.SceneCamera.orthographicSize);
//        _PanTarget = targetPosition;
//        _ZoomNormalizedTarget = targetNormalizedZoomLevel;
//        _HaveATargetPosition = true;
//    }
//
//    void Awake() {
//		MapTestPanZoom.SceneCamera = this._sceneCamera;
//    }
//
//    public static void LerpZoomSensitvity(float lerp) {
//        ZoomSensitivity = Mathf.Lerp(MinSensitivity, MaxSensitivity, lerp);
//    }
//
//    private void RefreshPostion() {
//        // Will call Digit.Kotr.Events.SettlementControl.TiggerCoordsInRealmChanged();
//        CameraWasMoved(true);
//    }
//
//    void OnDestroy() {
//        //TODO: review
//        /*
//        Digit.Kotr.Events.WorldMap.OnRegionChanged -= this.RefreshPostion;
//        Digit.Kotr.Events.SettlementControl.OnFreezeCodeMapInteraction -= FreezeCodeMapInteraction;
//        Digit.Kotr.Events.SettlementControl.OnFreezeUserMapInteraction -= FreezeUserMapInteraction;
//
//        // Nulling local variables and delegates to ensure clean up
//        _bgClickMethod = null;
//        OnZoomValueChanged = null;
//        OnZoomNormalizedChanged = null;
//        OnTargetReached = null;
//        OnPanChanged = null;
//        OnLateChange = null;
//        _uiModel = null;
//        _tutorialManager = null;
//        */
//
//    }
//    //TODO: rename
//    public static bool IsUserMapInteractionFrozen() {
//        return _userInteractionFrozen;
//    }
//
//    public static bool IsCodeMapInteractionFrozen() {
//        return _codeInteractionFrozen;
//    }
//
//    private void FreezeCodeMapInteraction(bool freeze) {
//        _codeInteractionFrozen = freeze;
//    }
//    private void FreezeUserMapInteraction(bool freeze) {
//        _userInteractionFrozen = freeze;
//    }
//
//    private float TimeSinceMomentumStart {
//        get {
//            return Time.time - this._momentumStartTime;
//        }
//    }
//
//    private void ResetMomentum() {
//        this._dragMomentum.x = 0.0f;
//        this._dragMomentum.y = 0.0f;
//        this._momentumStartTime = 0.0f;
//        this._dragDeltaList.Clear();
//    }
//
//#if UNITY_WEBPLAYER || UNITY_EDITOR
//    private bool _isOver = false;
//    void OnHover(bool isOver) {
//        this._isOver = isOver;
//    }
//#endif
//
//    public static void SetZoomNormalized(float zoom) {
//        if(!IsUserMapInteractionFrozen() && !IsCodeMapInteractionFrozen()) {
//			MapTestPanZoom.SceneCamera.orthographicSize = Mathf.Lerp(MaxZoom, MinZoom, Mathf.Clamp01(zoom));
//			SettlementControl.TriggerOnZoomChange(MapTestPanZoom.SceneCamera.orthographicSize);
//        }
//    }
//
//    void OnDrag(Vector2 delta) {
//
//		if(!MapTestPanZoom.IsUserMapInteractionFrozen()) {
//            this._dragging = true;
//
//
//            // On ipad don't want panning while pinching
//            if(Input.touchCount > 1) {
//                return;
//            }
//            //TODO: review is this inputScale needed here ?
//            delta *= inputScale; // Scale ipad retina value is want to work in lower res space (1024x768)
//
//            this._lastDragDelta = delta;
//            if(Mathf.Abs(this._lastDragDelta.x) > this._lastDragCap * inputScale || Mathf.Abs(this._lastDragDelta.y) > this._lastDragCap * inputScale) {
//                this._timeSinceLastBigDrag = Time.time;
//            }
//
//            if(this._dragDeltaList.Count < this._dragDeltaList.Capacity) {
//                this._dragDeltaList.Add(delta);
//            } else {
//                this._dragDeltaList.RemoveAt(0);
//                this._dragDeltaList.Add(delta);
//            }
//
//            if(Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android) {
//                if(Mathf.Abs(delta.x) > this._touchPanThreshold || Mathf.Abs(delta.y) > this._touchPanThreshold) {
//                    this._possibleTap = false;
//					MoveCamera(delta * MapTestPanZoom.SceneCamera.orthographicSize);
//                }
//            } else {
//                this._possibleTap = false;
//				MoveCamera(delta * MapTestPanZoom.SceneCamera.orthographicSize);
//            }
//        }
//
//    }
//
//    private void ZoomCameraAtPoint(float delta, Vector2 screenPoint) {
//        float cappedDelta = delta;
//		/*
//        if(delta > 0) { //Zooming in
//            cappedDelta = Mathf.Clamp(delta, 0f, _sceneCamera.orthographicSize - MinZoom);
//        } else { //Zooming out
//            cappedDelta = Mathf.Clamp(delta, -(MaxZoom - _sceneCamera.orthographicSize), 0f);
//        }
//
//        Vector3 worldPoint = this._sceneCamera.ScreenToWorldPoint(screenPoint);
//        // Calculate how much we will have to move towards the zoomTowards position
//        float multiplier = (1.0f / this._sceneCamera.orthographicSize * cappedDelta);
//        // Move camera
//		*/
////        MoveCameraWorld(_sceneCamera.transform.position - ((worldPoint - _sceneCamera.transform.position) * multiplier), false);
//        ZoomCamera(cappedDelta);
//    }
//
//    private void ZoomCamera(float delta) {
//
//		/*
//		Vector3 movement = _sceneCamera.transform.forward * delta;
//		//_sceneCamera.transform.localPosition = new Vector3(_sceneCamera.transform.localPosition.x, zoomY, _sceneCamera.transform.localPosition.z);
//		if ((_sceneCamera.transform.position + movement).y>1f || delta<0){
//			_sceneCamera.transform.position += movement;
//		}
//
//		if (_sceneCamera.transform.position.y<1f){
//			_sceneCamera.transform.position = new Vector3(_sceneCamera.transform.position.x, 1f, _sceneCamera.transform.position.z);
//		}
//*/
//    }
//
//    protected Vector2 GetScaledMousePosition() {
//#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8
//        float mouseScale = Utilities.GetInputScale();
//#else
//        float mouseScale = 1f;
//#endif
//        return GetScreenPosition(Input.mousePosition * mouseScale);
//    }
//
//    protected Vector2 GetScreenPosition(Vector2 point) {
//        // x is flipped in our unity scene
//        // Let's count y zero as bottom of screen
//        return new Vector2(Utilities.GetNonRetinaScreenSize().x - point.x, Utilities.GetNonRetinaScreenSize().y - point.y);
//    }
//
//    protected Vector2 GetScaledLastTouchPosition() {
//        // x is flipped in our unity scene
//        // Let's count y zero as bottom of screen.
//        return new Vector2(Utilities.GetNonRetinaScreenSize().x - (UICamera.lastTouchPosition.x * Utilities.GetInputScale()), Utilities.GetNonRetinaScreenSize().y - (UICamera.lastTouchPosition.y * Utilities.GetInputScale()));
//    }
//
//    private Vector2 GetScaledTouchPosition(int index) {
//        return Input.GetTouch(index).position * Utilities.GetInputScale();
//    }
//
//    private Vector2 GetScaledTouchDeltaPosition(int index) {
//        // fix or android zoom issues
//        float dt = Time.deltaTime / Input.GetTouch(index).deltaTime;
//        if(float.IsNaN(dt) || float.IsInfinity(dt)) {
//            dt = 1.0f;
//        }
//        return Input.GetTouch(index).deltaPosition * Utilities.GetInputScale() * dt;
//    }
//
//    private void CameraWasMoved(bool force = false) {
//        //May need an event here?
//    }
//
//    private void MoveCameraWorld(Vector3 position, bool applyZpostion) {
//        if(applyZpostion) {
//            _sceneCamera.transform.position = position;
//        } else {
//			_sceneCamera.transform.position = new Vector3(position.x, _sceneCamera.transform.position.y, position.z);
//        }
//        //float x = Mathf.Clamp(_sceneCamera.transform.localPosition.x, _panAreaRestriction.xMin, _panAreaRestriction.xMax);
//        //float y = Mathf.Clamp(_sceneCamera.transform.localPosition.y, _panAreaRestriction.yMin, _panAreaRestriction.yMax);
//        //_sceneCamera.transform.localPosition = new Vector3(x, y, _sceneCamera.transform.localPosition.z);
//        CameraWasMoved();
//    }
//
//    private void MoveCamera(Vector2 delta) {
//        float x = _sceneCamera.transform.localPosition.x - delta.x;//xMathf.Clamp(_sceneCamera.transform.localPosition.x - delta.x, _panAreaRestriction.xMin, _panAreaRestriction.xMax);
//        float z = _sceneCamera.transform.localPosition.z - delta.y;//Mathf.Clamp(_sceneCamera.transform.localPosition.y - delta.y, _panAreaRestriction.yMin, _panAreaRestriction.yMax);
//        _sceneCamera.transform.localPosition = new Vector3(x, _sceneCamera.transform.localPosition.y , z);
//        CameraWasMoved();
//    }
//
//    void Update() {
//        if(!IsUserMapInteractionFrozen()) {
//
//            // Apply momentum after Drag has completed.
//            if(!this._dragging && this._dragMomentum != Vector2.zero) {
//                this._dragMomentum *= this._momentumCurve.Evaluate(this.TimeSinceMomentumStart) * Utilities.GetMomentumScale();
//				MoveCamera(this._dragMomentum * MapTestPanZoom.SceneCamera.orthographicSize);
//            }
//
//            //Check for scroll wheel movement or pinchzoom gesture
//            float zoomDelta = 0f;
//#if UNITY_WEBPLAYER || UNITY_EDITOR
//			if(this._isOver) { // Only get mouse wheel if we are hovering over the MapTestPanZoom box collider - to fix zooming while using mouse wheel while UI is open
//                zoomDelta = Input.GetAxis("Mouse ScrollWheel");
//            }
//            zoomDelta *= ZoomSensitivity; //As webplayer mouse wheel seems sensitive
//#elif UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8
//            zoomDelta = Input.GetAxis("Mouse ScrollWheel");
//#endif
//            if(Mathf.Approximately(zoomDelta, 0f)) {
//                // Check if pinch gesture
//                if(Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved) {
//                    Vector2 curDist = GetScaledTouchPosition(0) - GetScaledTouchPosition(1);   //current distance between finger touches
//                    Vector2 prevDist = ((GetScaledTouchPosition(0) - GetScaledTouchDeltaPosition(0)) - (GetScaledTouchPosition(1) - GetScaledTouchDeltaPosition(1)));     //difference in previous locations using delta positions
//                    zoomDelta = (curDist.magnitude - prevDist.magnitude) * this._gestureScale;
//                }
//            }
//
//            if(!Mathf.Approximately(zoomDelta, 0f)) {
//                // x is flipped in our unity scene
//                // Let's count y zero as bottom of screen
//                Vector2 zoomPoint = new Vector2(UnityEngine.Screen.width - Input.mousePosition.x, UnityEngine.Screen.height - Input.mousePosition.y);
//                ZoomCameraAtPoint(zoomDelta, zoomPoint); //GetScaledMousePosition()
//
//            }
//        }
//
//		if(MapTestPanZoom._HaveATargetPosition) {
//            this._lerp = Mathf.Clamp01(this._lerp + Time.deltaTime * this._lerpMult);
//            float lerp = this._lerpCurve.Evaluate(this._lerp);
//            // Unity vector3 == tests for approx equal
//            bool targetPositionReached = false;
//            bool zoomTargetReached = false;
//            if(_sceneCamera.transform.localPosition != _PanTarget) {
//                Vector3 cameraPosition = Vector3.Lerp(_StartingPositionPan, _PanTarget, lerp);
//                float x = cameraPosition.x;// Mathf.Clamp(cameraPosition.x, _panAreaRestriction.xMin, _panAreaRestriction.xMax);
//                float y = cameraPosition.y;//Mathf.Clamp(cameraPosition.y, _panAreaRestriction.yMin, _panAreaRestriction.yMax);
//                _sceneCamera.transform.localPosition = new Vector3(x, y, _sceneCamera.transform.localPosition.z);
//
//                if(this._minDistanceForPanLerp >= Vector2.SqrMagnitude(_sceneCamera.transform.localPosition - _PanTarget)) {
//                    _sceneCamera.transform.localPosition = _PanTarget;
//                    targetPositionReached = true;
//                }
//
//                CameraWasMoved();
//
//            } else {
//                targetPositionReached = true;
//            }
//
//			float normalizedCurrent = Mathf.InverseLerp(MapTestPanZoom.MaxZoom, MapTestPanZoom.MinZoom, MapTestPanZoom.SceneCamera.orthographicSize);
//            if(!Mathf.Approximately(normalizedCurrent, _ZoomNormalizedTarget)) {
//                SetZoomNormalized(Mathf.Lerp(_StartZoomNormalized, _ZoomNormalizedTarget, lerp));
//            } else {
//
//                SetZoomNormalized(_ZoomNormalizedTarget);
//                zoomTargetReached = true;
//            }
//
//            if(targetPositionReached && zoomTargetReached) {
//                SettlementControl.TriggerOnTargetReached();
//				MapTestPanZoom._HaveATargetPosition = false;
//                this._lerp = 0f;
//            }
//        }
//    }
//
//    void OnPress(bool pressed) {
//        if(pressed) {
//            this._possibleTap = true;
//            this._tapDownTime = Time.time;
//            if(this._dragMomentum != Vector2.zero) {
//                // Stop momentum if the user taps the screen.
//                ResetMomentum();
//            }
//        } else if(this._dragging) {
//            this._dragging = false;
//            this._momentumStartTime = Time.time;
//            if(Time.time - this._timeSinceLastBigDrag > this._timeCancelMomentum) {
//                // Stop momentum if the last drag amount was a relatively long time ago (in millis).
//                ResetMomentum();
//            } else {
//                foreach(Vector2 delta in this._dragDeltaList) {
//                    this._dragMomentum += delta;
//                }
//                this._dragMomentum /= this._dragDeltaList.Count;
//                this._dragDeltaList.Clear();
//            }
//        }
//
//        if(!pressed && Input.touchCount < 2 && this._possibleTap && Time.time - this._tapDownTime < this._maxTapTime && lastClickTime + 0.3f < Time.time) {
//            lastClickTime = Time.time;
//            SettlementControl.TriggerSettlementBGClicked();
//        }
//    }
//
//    /*
//    protected virtual void OnPress(bool pressed) {
//        if(pressed) {
//            this._possibleTap = true;
//            this._tapDownTime = Time.time;
//            if(this._dragMomentum != Vector2.zero) {
//                // Stop momentum if the user taps the screen.
//                ResetMomentum();
//            }
//        } else if(this._dragging) {
//            this._dragging = false;
//            this._momentumStartTime = Time.time;
//            if(Time.time - this._timeSinceLastBigDrag > this._timeCancelMomentum) {
//                // Stop momentum if the last drag amount was a relatively long time ago (in millis).
//                ResetMomentum();
//            } else {
//                foreach(Vector2 delta in this._dragDeltaList) {
//                    this._dragMomentum += delta;
//                }
//                this._dragMomentum /= this._dragDeltaList.Count;
//                this._dragDeltaList.Clear();
//            }
//        }
//
//        // Check if the background was clicked.
//        if(pressed && Input.touchCount < 2 && this._possibleTap && Time.time - this._tapDownTime < this._maxTapTime) {
//            Ray ray = UICamera.currentCamera.ScreenPointToRay(Input.mousePosition);
//            // Adjust the ray to look behind the the collider this script is attached to
//            ray.origin = new Vector3(ray.origin.x, ray.origin.y, UICamera.currentCamera.gameObject.transform.localPosition.z);
//            RaycastHit hit;
//            if(Physics.Raycast(ray, out hit) && hit.collider.gameObject == this.gameObject) {
//                NotifyBackgroundClick();
//            }
//        }
//    }
//
//    public void RegisterOnClickBackgroundHandler(OnClickBackgroundHandler callback) {
//        if(callback != null) {
//            this._bgClickMethod -= callback;
//            this._bgClickMethod += callback;
//        }
//    }
//
//    public void UnregisterOnClickBackgroundHandler(OnClickBackgroundHandler callback) {
//        if(callback != null) {
//            this._bgClickMethod -= callback;
//        }
//    }
//
//    protected void NotifyBackgroundClick() {
//        if(this._bgClickMethod != null) {
//            this._bgClickMethod();
//        }
//    }
//    */
//}
//
//
//namespace Digit.Kotr.Events {
//    partial class SettlementControl {
//
//        public delegate void SimpleBoolEvent(bool value);
//        private static SimpleBoolEvent OnFreezeUserMapInteractionDelegate;
//        public static event SimpleBoolEvent OnFreezeUserMapInteraction {
//            add {
//                OnFreezeUserMapInteractionDelegate -= value;
//                OnFreezeUserMapInteractionDelegate += value;
//            } remove {
//                OnFreezeUserMapInteractionDelegate -= value;
//            }
//        }
//        public static void TriggerFreezeUserMapInteraction(bool freeze) {
//            if(OnFreezeUserMapInteractionDelegate != null) {
//                OnFreezeUserMapInteractionDelegate(freeze);
//            }
//        }
//
//        private static SimpleBoolEvent OnFreezeCodeMapInteractionDelegate;
//        public static event SimpleBoolEvent OnFreezeCodeMapInteraction {
//            add {
//                OnFreezeCodeMapInteractionDelegate -= value;
//                OnFreezeCodeMapInteractionDelegate += value;
//            } remove {
//                OnFreezeCodeMapInteractionDelegate -= value;
//            }
//        }
//        public static void TriggerFreezeCodeMapInteraction(bool freeze) {
//            if(OnFreezeCodeMapInteractionDelegate != null) {
//                OnFreezeCodeMapInteractionDelegate(freeze);
//            }
//        }
//
//        public delegate void SimpleFloatEvent(float zoom);
//        private static SimpleFloatEvent OnZoomChangeDelegate;
//        public static event SimpleFloatEvent OnZoomChange {
//            add {
//                OnZoomChangeDelegate -= value;
//                OnZoomChangeDelegate += value;
//            } remove {
//                OnZoomChangeDelegate -= value;
//            }
//        }
//        public static void TriggerOnZoomChange(float zoom) {
//            if(OnZoomChangeDelegate != null) {
//                OnZoomChangeDelegate(zoom);
//            }
//        }
//
//        public delegate void SimpleEvent();
//        private static SimpleEvent OnTargetReachedDelegate;
//        public static event SimpleEvent OnTargetReached {
//            add {
//                OnTargetReachedDelegate -= value;
//                OnTargetReachedDelegate += value;
//            } remove {
//                OnTargetReachedDelegate -= value;
//            }
//        }
//        public static void TriggerOnTargetReached() {
//            if(OnTargetReachedDelegate != null) {
//                OnTargetReachedDelegate();
//            }
//        }
//
//        private static SimpleEvent OnSettlementBGClickDelegate;
//        public static event SimpleEvent OnSettlementBGClicked {
//            add {
//                OnSettlementBGClickDelegate -= value;
//                OnSettlementBGClickDelegate += value;
//            } remove {
//                OnSettlementBGClickDelegate -= value;
//            }
//        }
//
//        public static void TriggerSettlementBGClicked() {
//            if(OnSettlementBGClickDelegate != null) {
//                OnSettlementBGClickDelegate();
//            }
//        }
//
//
//        public static void ClearAll() {
//            SettlementControl.OnFreezeCodeMapInteractionDelegate = null;
//            SettlementControl.OnFreezeUserMapInteractionDelegate = null;
//            SettlementControl.OnZoomChangeDelegate = null;
//            SettlementControl.OnTargetReachedDelegate = null;
//            SettlementControl.OnSettlementBGClickDelegate = null;
//        }
//    }
//}
