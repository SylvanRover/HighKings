using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteriorCameraZoom: MonoBehaviour {

    [SerializeField] private Camera viewCamera;
    [SerializeField] private float min = 0f; //TODO: set suitable value
    [SerializeField] private float max = 10f; //TODO: set suitable value
    [SerializeField] private float scaleZoomDelta = 5f;
    public bool userZoomAllowed = true;

    private TKPinchRecognizer zoomRecognizer;
    private readonly Plane rayCastPlane = new Plane(Vector3.up, Vector3.zero); // Struct but only needs setting once
    //private readonly float minMovement = 0.01f;
    private float zoomTotal = 0f;
    private Vector3 screenPointInWorldBeforeZoom;

    //TODO: want bounce and momuntum on zoom

    void Start() {

        this.zoomRecognizer = new TKPinchRecognizer();
        zoomRecognizer.gestureRecognizedEvent += (obj) => {

            if(this.userZoomAllowed) {
                this.screenPointInWorldBeforeZoom = ScreenToWorldPos(this.zoomRecognizer.touchLocation());
                OnZoom(-zoomRecognizer.deltaScale * scaleZoomDelta, this.zoomRecognizer.touchLocation()); //test zoom out
            }

        };
        TouchKit.addGestureRecognizer(this.zoomRecognizer);  // Is removed on destroy
    }

    void OnDestroy() {
        TouchKit.removeGestureRecognizer(this.zoomRecognizer);
    }

    private Vector3 ScreenToWorldPos(Vector2 screenPos) {
        Vector3 result = Vector3.zero;
        Ray rayCastRay = this.viewCamera.ScreenPointToRay(screenPos);
        float rayCastEnter = 0f;
        if(this.rayCastPlane.Raycast(rayCastRay, out rayCastEnter)) {
            result = rayCastRay.GetPoint(rayCastEnter);
        }
        return result;
    }

    void OnZoom(float delta, Vector2 screenPosition) {
        if(delta > 0f || delta < 0f) {
            ZoomCameraAtWorldPoint(delta, screenPosition);
        }
    }

    private void ZoomCameraAtWorldPoint(float delta, Vector2 screenPosition) {

        ZoomCamera(delta);

        Vector3 screenPointInWorldAfterZoom = ScreenToWorldPos(screenPosition);
        Vector3 worldDiff = (screenPointInWorldBeforeZoom - screenPointInWorldAfterZoom);
        MoveCameraWorld(worldDiff);
    }

    void MoveCameraWorld(Vector3 vector3) {
        this.transform.parent.position += vector3;
    }

    private void ZoomCamera(float delta) {

        if(delta < 0f) { //Zooming in
            delta = Mathf.Clamp(delta, -(this.max - this.zoomTotal), 0f);
        } else { //Zooming out
            delta = Mathf.Clamp(delta, 0f, this.zoomTotal - this.min);
        }

        viewCamera.transform.localPosition += (delta * -this.transform.forward);
        this.zoomTotal -= delta;
    }
}

