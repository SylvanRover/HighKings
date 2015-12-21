using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class InteriorCameraPan : MonoBehaviour {

    [Range(0.0f, 1.0f)]
    [SerializeField] private float MomenutumDampener = 0.9f;
    [SerializeField] private float scalePanDelta = 1f;
    [SerializeField] private Camera viewCamera;
    [SerializeField] private float _lerpBreakDistance = 5f;
    [SerializeField] private Vector3 floorPlanePosition = Vector3.zero;
    [SerializeField] private float timeMultiply = 12f;

    public float thresholdAccumulation = 0f;
    public float threshold = 0f;
    public Action OnThresholdReached;

    public bool userPanAllowed = true;
    public Action OnPanComplete;
    private TKPanRecognizer panRecognizer;
    private Vector2 lastDelta;
    private Vector3 panTargetWorldPosition;

    private float lerp = 0f;
    private Vector3 startWorldPosition;

    private bool havePanTarget = false;
    private readonly float minMovement = 0.01f;
    public Vector3 gizmoSize; //TEMP
    //default/startup values
    private Vector3 ourDefaultPosition;
    private Vector3 cameraDefaultLocalPosition;
    private Quaternion cameraDefaultLocalRotation;
    bool hasStarted = false;
    float lastClickTime;
    void Awake() {
    }

    void Start() {
        hasStarted = true;
        this.ourDefaultPosition = this.transform.position;
        this.cameraDefaultLocalPosition = this.viewCamera.transform.localPosition;
        this.cameraDefaultLocalRotation = this.viewCamera.transform.localRotation;
        this.panRecognizer = new TKPanRecognizer();
        this.panRecognizer.minimumNumberOfTouches = 1;
        this.panRecognizer.maximumNumberOfTouches = 1;
        panRecognizer.gestureRecognizedEvent += (obj) => {
            lastClickTime = Time.time;
            if(this.userPanAllowed) {
                Vector3 avgWorldOldPos = ScreenToWorldPos(this.panRecognizer.touchLocation() - this.panRecognizer.deltaTranslation);
                Vector3 avgWorldPos = ScreenToWorldPos(this.panRecognizer.touchLocation());
                Vector2 nonScaledDelta = new Vector2(avgWorldPos.x - avgWorldOldPos.x, avgWorldPos.z - avgWorldOldPos.z);
                OnPan(nonScaledDelta);
            }

        };
        panRecognizer.customTouchValidator = TKUITouchValidator.rayCastBlockersNotValid;
        TouchKit.addGestureRecognizer(panRecognizer);  // Is removed on destroy
    }

    void OnDestroy() {
        TouchKit.removeGestureRecognizer(panRecognizer);
    }

    void OnEnabled() {
        ClearMomentum();
    }

    public void ClearMomentum() {
        this.lastDelta = Vector2.zero;
    }

    public void ClearPanTarget() {
        this.havePanTarget = false;
    }

    public void SetPanTargetPosition(Vector3 worldPosition, bool clearVelocity = true) {
        if(clearVelocity) {
            ClearMomentum();
        }
        this.panTargetWorldPosition = worldPosition;
        this.lerp = 0f;
        this.startWorldPosition = this.transform.position;

        this.havePanTarget = true;
    }

    private void MoveCamera(Vector2 delta, bool addMomentum = true) {
        delta *= this.scalePanDelta;
        this.transform.localPosition = new Vector3(transform.localPosition.x - delta.x, transform.localPosition.y , transform.localPosition.z - delta.y);
        if(addMomentum) {
            this.lastDelta = delta;
        } else {
            ClearMomentum();
        }
    }

    private Vector3 ScreenToWorldPos(Vector2 screenPos) {
        Plane rayCastPlane = new Plane(Vector3.up, floorPlanePosition);
        Vector3 result = Vector3.zero;
        Ray rayCastRay = this.viewCamera.ScreenPointToRay(screenPos);
        float rayCastEnter = 0f;
        if(rayCastPlane.Raycast(rayCastRay, out rayCastEnter)) {
            result = rayCastRay.GetPoint(rayCastEnter);
        }
        return result;
    }

    void OnPan(Vector2 delta) {
        if(threshold > 0f && userPanAllowed) {
            thresholdAccumulation += delta.SqrMagnitude();
            if(thresholdAccumulation >= threshold) {
                if(OnThresholdReached != null) {
                    OnThresholdReached();
                }
                MoveCamera(delta);
            }
        } else {
            MoveCamera(delta);
        }
    }

    void LateUpdate() {
        if(!Input.GetMouseButton(0) && Input.touchCount == 0) {
            this.thresholdAccumulation = 0f;
        }
        if(this.havePanTarget) {
            //TODO: figure out lerp, prob should use the passing the animation curve approach.
            lerp = Mathf.Clamp01(lerp += (Time.deltaTime * timeMultiply));
            this.transform.position = Vector3.Lerp(startWorldPosition, panTargetWorldPosition, lerp);
            float distance = Mathf.Abs(Vector3.Distance(transform.position, panTargetWorldPosition));
            if(distance <= this._lerpBreakDistance) {
                this.transform.position = panTargetWorldPosition;
                ClearPanTarget();
                if(OnPanComplete != null) {
                    OnPanComplete();
                }
            }
        } else {
            if(lastDelta.sqrMagnitude > this.minMovement && Time.time > lastClickTime) {
                if(!Input.GetMouseButton(0) && Input.touchCount == 0) {
                    MoveCamera(lastDelta, true);
                    lastDelta *= MomenutumDampener;//Mathf.Clamp(60f * Time.deltaTime, 0.1f, 0.95f);
                }
            }
        }
        return; //TODO: review
    }

    public void ResetInteriorCameraToDefaults() {
        if(this.hasStarted) {
            this.transform.position = this.ourDefaultPosition;
            this.viewCamera.transform.localPosition = this.cameraDefaultLocalPosition;
            this.viewCamera.transform.localRotation = this.cameraDefaultLocalRotation;
        }
    }

    void OnDrawGizmos() {
        Gizmos.DrawCube(floorPlanePosition, gizmoSize);
    }
}

