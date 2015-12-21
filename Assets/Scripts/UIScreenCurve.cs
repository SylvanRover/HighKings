using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Vectrosity;

public class UIScreenCurve : MonoBehaviour {
	public enum CurvePtsMode {
		INLINE,
		ANCHOR
	}

	public CurvePtsMode mode = CurvePtsMode.INLINE;
	public List<RectTransform> curvePts;
	public List<RectTransform> anchorCurvePts;

	public VectorLine line;
	public bool recalc = true;
	public Material lineMaterial;

	public GameObject testPt;

	private VectorLine intersectionLine;
	private int _prevCurvePtsCount = -1;
	private Vector3 _prevIntersectionPtPos = Vector3.zero;

	public bool showLine = false;
	public bool showIntersection = false;

	public List<RectTransform> CurvePts {
		get {
			if(this.mode == CurvePtsMode.ANCHOR) {
				return this.anchorCurvePts;
			}
			return this.curvePts;
		}
	}

	public bool LineVisible {
		get {
			return this.line.active;
		} set {
			this.line.active = value;
		}
	}

	public bool LineIntersectionVisible {
		get {
			return this.intersectionLine.active;
		} set {
			this.intersectionLine.active = value;
		}
	}

//	void OnGUI() {
//		if (GUI.Button (new Rect(100f, 100f, 100f, 50f), "TOGGLE CURVE")) {
//			this.LineVisible = !this.LineVisible;
//		}
//		if (GUI.Button (new Rect(100f, 300f, 100f, 50f), "TOGGLE LINE")) {
//			this.LineIntersectionVisible = !this.LineIntersectionVisible;
//		}
//	}

	private void Start() {

		this.line = new VectorLine ("TESTLINE", new Vector2[2], Color.red, this.lineMaterial, 5f, LineType.Continuous, Joins.Weld);
		this.line.vectorObject.transform.SetParent(this.transform);
		this.line.vectorObject.transform.localPosition = Vector3.zero;
		this.line.vectorObject.transform.localRotation = Quaternion.identity;
		this.line.vectorObject.transform.localScale = Vector3.one;
		RecalcPoints ();
		SetRainbowColors ();

		this.intersectionLine = new VectorLine ("INTERSECTIONLINE", new Vector2[2], Color.blue, this.lineMaterial, 5f, LineType.Continuous, Joins.Weld);
		this.intersectionLine.vectorObject.transform.SetParent(this.transform);
		this.intersectionLine.vectorObject.transform.localPosition = Vector3.zero;
		this.intersectionLine.vectorObject.transform.localRotation = Quaternion.identity;
		this.intersectionLine.vectorObject.transform.localScale = Vector3.one;
		this.intersectionLine.points2 [0] = Vector2.zero;
		this.intersectionLine.points2 [1] = Vector2.zero;
		this.intersectionLine.Draw();
	}

	private void OnDestroy() {
		VectorLine.Destroy (ref this.line);
		VectorLine.Destroy (ref this.intersectionLine);
	}

	private void Update() {
		this.LineVisible = this.showLine;
		this.LineIntersectionVisible = this.showIntersection;

		if (this.recalc) {
			RecalcPoints();
		}

		Vector2 testPtPos = new Vector2 (testPt.transform.localPosition.x, testPt.transform.localPosition.y);
		Vector2 dir2D = (testPtPos - Vector2.zero).normalized;
		Ray2D r = new Ray2D (Vector2.zero, dir2D);

		for (int i = 0; i < this.line.points2.Length; i += 2) {
			int segmentStartIndex = i;
			int segmentEndIndex = i+1 < this.line.points2.Length ? i+1 : 0;

			Vector2 a = this.line.points2[segmentStartIndex];
			Vector2 b = this.line.points2[segmentEndIndex];

			float t = 0f;
			Vector2 p = Vector2.zero;
			if(Test2DSegmentSegment(a, b, Vector2.zero, r.GetPoint(1000f), ref t, ref p)) {// Why 1000f? Just a line i know is longer than the curve
				//Debug.LogError(string.Format("Intersection of Curve happened at segment {0}:{1} at point {2}", segmentStartIndex, segmentEndIndex, p));
				//SetIntersectionSegmentColor(segmentStartIndex, segmentEndIndex, Color.yellow);
			}
		}

		// INTERSECTION LINE STUFF
//		if (this._prevIntersectionPtPos != testPt.transform.localPosition) {
//			this._prevIntersectionPtPos = testPt.transform.localPosition;
//			this.intersectionLine.points2 [0] = Vector2.zero;
//			this.intersectionLine.points2 [1] = r.GetPoint (1000f);// Why 1000f? Just a line i know is longer than the curve
//			this.intersectionLine.Draw();
//		}
//		//angle += Time.deltaTime;
//		angle = Mathf.Repeat (angle, 360f);
//		testPt.transform.RotateAround(Vector3.zero, testPt.transform.forward, angle);
	}
	public float angle = 0f;

	private void SetIntersectionSegmentColor(int segmentStartIndex, int segmentEndIndex, Color color) {
		Color[] colors = new Color[this.line.points2.Length - 1];
		for (int i = 0; i < colors.Length; ++i) {
			if(i >= segmentStartIndex && i <= segmentEndIndex) {
				colors[i] = color;
			} else {
				colors[i] = Color.red;
			}
		}
		this.line.SetColors (colors);
	}

	private void SetRainbowColors() {
		Color[] colors = new Color[this.line.points2.Length - 1];
		for (int i = 0; i < colors.Length; ++i) {
			if(i % 4 == 1) {
				colors[i] = Color.yellow;
			} else if(i % 3 == 1) {
				colors[i] = Color.red;
			} else if(i % 2 == 1) {
				colors[i] = Color.blue;
			} else {
				colors[i] = Color.green;
			}
		}
		this.line.SetColors (colors);
	}

	private void RecalcPoints() {
		if(this._prevCurvePtsCount == -1 || this._prevCurvePtsCount != CurvePts.Count) {
			this.line.Resize (CurvePts.Count * 8 + 1);
			this.line.SetColor(Color.red); // reset color - will be updated 
		}

		this._prevCurvePtsCount = CurvePts.Count;

		Vector2[] pts = new Vector2[CurvePts.Count];
		for(int i = 0; i < CurvePts.Count; ++i) {
			Vector2 localPosition = new Vector2(CurvePts[i].localPosition.x, CurvePts[i].localPosition.y);
			if(this.mode == CurvePtsMode.ANCHOR) {
				Vector3 localPosition3 = this.transform.InverseTransformPoint(CurvePts[i].position);
				localPosition = new Vector2(localPosition3.x, localPosition3.y);
			}
			pts[i] = localPosition;
		}

		this.line.MakeSpline(pts, false);
		this.line.Draw ();
	}

	private bool AltTest2DSegmentSegment(Vector2 a, Vector2 b, Vector2 c, Vector2 d, ref Vector2 p) {
		Vector2 s1 = b - a;
		Vector2 s2 = d - c;
		
		Vector2 u = a - c;
		
		float ip = 1f / (-s2.x * s1.y + s1.x * s2.y);
		
		float s = (-s1.y * u.x + s1.x * u.y) * ip;
		float t = ( s2.x * u.y - s2.y * u.x) * ip;
		
		if (s >= 0 && s <= 1 && t >= 0 && t <= 1) {
			p = a + (s1 * t);
			return true;
		}
		
		return false;
	}

	private float Signed2DTriArea(Vector2 a, Vector2 b, Vector2 c) {
		return (a.x - c.x) * (b.y - c.y) - (a.y - c.y) * (b.x - c.x);
	}
	
	private bool Test2DSegmentSegment(Vector2 a, Vector2 b, Vector2 c, Vector2 d, ref float t, ref Vector2 p) {
		// signs of areas correspond to which side of ab points c and d are
		float a1 = Signed2DTriArea(a,b,d); // Compute winding of abd (+ or -)
		float a2 = Signed2DTriArea(a,b,c); // To intersect, must have sign opposite of a1
		
		// If c and d are on different sides of ab, areas have different signs
		if(a1 * a2 < 0f) // require unsigned x & y values.
		{
			float a3 = Signed2DTriArea(c,d,a); // Compute winding of cda (+ or -)
			float a4 = a3 + a2 - a1; // Since area is constant a1 - a2 = a3 - a4, or a4 = a3 + a2 - a1
			
			// Points a and b on different sides of cd if areas have different signs
			if(a3 * a4 < 0f) {
				// Segments intersect. Find intersection point along L(t) = a + t * (b - a).
				t = a3 / (a3 - a4);
				p = a + t * (b - a); // the point of intersection
				return true;
			}
		}
		
		// Segments not intersecting or collinear
		return false;
	}

	public Vector3 FitInsideCurve(Vector3 screenPos) {
		Vector3 screenCenterPos = new Vector3 (Screen.width * 0.5f, Screen.height * 0.5f, 0f);
		Vector3 tmpScreenPos = screenPos - screenCenterPos;
		Vector2 screenPos2D = new Vector2 (tmpScreenPos.x, tmpScreenPos.y);

		for (int i = 0; i < this.line.points2.Length; i++) {
			int segmentStartIndex = i;
			int segmentEndIndex = i+1 < this.line.points2.Length ? i+1 : 0;

			Vector2 a = this.line.points2[segmentStartIndex];
			Vector2 b = this.line.points2[segmentEndIndex];
			Vector2 c = Vector2.zero;
			Vector2 d = screenPos2D;

			float t = 0f;
			Vector2 intersectPt = Vector2.zero;
			if(Test2DSegmentSegment(a, b, c, d, ref t, ref intersectPt)) {
				this.intersectionLine.points2 [0] = c;
				this.intersectionLine.points2 [1] = d;
				this.intersectionLine.Draw();
				Vector3 intersectPt3 = new Vector3(intersectPt.x, intersectPt.y, 0f);
				intersectPt3 += screenCenterPos;
				//Debug.LogError(string.Format("INTERSECTION between [{0}:{1}] and [{2}:{3}] => intersectPt3 = {4}", a, b, c, d, intersectPt3));
				return intersectPt3;
			}
		}

		//Debug.LogError ("No intersect point screenPos=" + screenPos + " tmpScreenPos="+tmpScreenPos);
		return screenPos;
	}
}
