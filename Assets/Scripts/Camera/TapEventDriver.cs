using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TapEventDriver : MonoBehaviour {
    public LayerMask maskToUse;

    private TKTapRecognizer tapRecognizer;
    private Camera cameraToUse;

    void Awake() {
        this.cameraToUse = this.gameObject.GetComponent<Camera>();

        tapRecognizer = new TKTapRecognizer(0.5f, 20f);
        tapRecognizer.customTouchValidator = TKUITouchValidator.rayCastBlockersNotValid;
        tapRecognizer.gestureRecognizedEvent += TapDetected;
    }

    void OnEnable() {
        TouchKit.addGestureRecognizer(tapRecognizer);
    }

    void OnDisable() {
        TouchKit.removeGestureRecognizer(tapRecognizer);
    }

    void OnDestroy() {
        TouchKit.removeGestureRecognizer(tapRecognizer);
    }

    private void TapDetected(TKTapRecognizer gesture) {
        if(this.cameraToUse != null) {
            Ray ray = cameraToUse.ScreenPointToRay(new Vector3(gesture.startTouchLocation().x, gesture.startTouchLocation().y, 0f));
            RaycastHit[] hits = Physics.RaycastAll(ray, cameraToUse.farClipPlane, maskToUse);
            //The distance from the ray's origin to the impact point.
            ColliderEventBroadcaster nearest = null;
            float currentDistance =  0;
            for(int i = 0; i < hits.Length; ++i) {
                ColliderEventBroadcaster broadcaster = hits [i].collider.gameObject.GetComponent<ColliderEventBroadcaster> ();
                if(broadcaster != null) {
                    if(nearest == null) {
                        nearest = broadcaster;
                        currentDistance = hits[i].distance;
                    } else if(hits[i].distance < currentDistance) {
                        currentDistance = hits[i].distance;
                        nearest = broadcaster;
                    }
                }
            }
            for(int i = 0; i < hits.Length; ++i) {
                if(nearest != null) {
                    Debug.Log("TapEventDriver > TapDetected on: " + nearest.name);
                    nearest.onClick.Invoke();
                    break;
                }
            }
        }
    }
}

