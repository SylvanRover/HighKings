using System;

using UnityEngine;
using Vectrosity;
using System.Collections.Generic;
using Map.Events;

public class ProgressLine : MonoBehaviour {

	//Transforms
	[SerializeField] private Transform _marker;
	[SerializeField] private Transform _home;
	[SerializeField] private Transform _hero;

	// Prefab for waypoints
	[SerializeField] private string _wayPointPrefabName = "WayPointRing";

	//Line Rendering
	[SerializeField] private int _segments = 10;
	[SerializeField] private float _thickness = 2.5f;
	[SerializeField] private float _maxWeldDistance = 0.1f;
	[SerializeField] private float _textureScale = 0.1f;
    [SerializeField] private Material _lineMaterial;
    [SerializeField] private Color _lineColor;
    [SerializeField] private Color _lineColorUnfilled;

	//Cameras
    [SerializeField] private Camera _renderCamera;
	[SerializeField] private Camera _mainCamera;
	[SerializeField] private GameObject _vectorCam;

	//Movement controls
	[SerializeField] private float _nearEnough = 0.01f;
    [SerializeField] private float _lerpSmooth = 0.5f;
	[SerializeField] private bool _rotateMarker = true;
	[SerializeField] private bool _goAutomaticallyAfterDelay = false;
	[SerializeField] private float _delayOnGo = 0f;

	// Movement speed
	[SerializeField] private float _maxTravelSpeed = 1f;
	[SerializeField] private float _minTravelSpeed = 0.1f;
	[SerializeField] private float _changeTravelSpeedAmount = 0.1f;
	public float travelSpeed = 0.1f;
	public bool _go = false;
	public bool IsAttacking = false;

	public float _lineLength = 1f;

	public float _delayUpdateTime = 5f;
	public float _delayOnAddingPoint = 4f;
	public float _mindelayUpdateTime = 5f;
	public float _scaleTexture = 1f;
	public Vectrosity.VectorLine _progressLine = null;
	public Color[] _colors;

	public bool _needToBeSelected = false;
	public bool _selected = true;
	
    public float _progress01 = 0f;
	public float _startingProgress = 0f;
    public float _targetprogress = 1f;
    public float _totalTime = 0f;
	public bool _editingLineAllowed = true;
	public LayerMask _drawLayer;

	public List<Vector3> _linePoints1 = new List<Vector3>();
	public List<GameObject> _wayPointGameObjects = new List<GameObject>();

	public ArmyStats stats;
	public MakePrefabAppear spawnHero;
	public ArmyCollision armyCol;

	public float timeToDist;
	

	public void IncreaseTravelSpeed() {
		travelSpeed = Mathf.Min(travelSpeed+_changeTravelSpeedAmount, _maxTravelSpeed);
	}
	
	public void DecreaseTravelSpeed() {
		travelSpeed = Mathf.Max(travelSpeed-_changeTravelSpeedAmount, _minTravelSpeed);
	}


	public void UnSelect() {
		if (_needToBeSelected) {
			_selected = false;
			StopLineEditing ();
		}
	}

	public void Selected() {		
		/*ProgressLine[] p_arr = FindObjectsOfType<ProgressLine> ();
		foreach(ProgressLine p in p_arr) {
			p.UnSelect();
		}*/
		_selected = true;
		AllowLineEditing ();
		stats.CommanderIsSelected(stats.commanderID);
	}

	void Start(){
		_drawLayer = LayerMask.NameToLayer ("UI");
		AddPoint (_marker.transform.position, true);
		InteractionEvents.OnBroadcastPosition += AttackPoint; //on
		InteractionEvents.OnBroadcastSendAttack += SendAttack; 

		GameObject homeObj = GameObject.FindWithTag ("Home");
		_home = homeObj.GetComponent<Transform> ();
		_mainCamera = Camera.main;
		_vectorCam = GameObject.FindWithTag ("VectorCam");
		_renderCamera = _vectorCam.GetComponent<Camera> ();

		_hero = armyCol.child;
	}

	public float GetCurrentVelocity(){
		return _go ? travelSpeed : 0f;
	}

	public void ClearPath(){
		_go = false;
		_linePoints1.Clear ();
		ClearWayPointPrefabs ();
		VectorLine.Destroy(ref this._progressLine);
		ResetProgressValues ();
		AddPoint (_marker.transform.position, true);
	}
	
	public void PathHome(){
		ClearPath ();
		AddPoint (_home.transform.position);
		_go = true; //Assume want by default
	}

	//This should not be here but just testing
	public void AddPointAtMouse(){
		/*_go = true;
		if (_editingLineAllowed) {
			Ray ray = _mainCamera.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit; // hit.point = The impact point in world space where the ray hit the collider.
			if (Physics.Raycast(ray, out hit, Mathf.Infinity)) { //may want this._eventMask
				if (hit.collider != null) {
					AddPoint (hit.point);
					AddWayPointPrefab(hit.point);
				}
			}
		}*/
	}
			
	public void AddPoint(Vector3 point, bool force=false){
		if (_editingLineAllowed || force) {
			if (_delayUpdateTime < (_mindelayUpdateTime + _delayOnAddingPoint)) {
				_delayUpdateTime += _delayOnAddingPoint;
				if (_delayUpdateTime < _mindelayUpdateTime) {
					_delayUpdateTime += _mindelayUpdateTime;
				}
			}
			_linePoints1.Add (point - this.transform.position);
			CreateOrUpdateLine ();
			RecalcProgress ();
		}
	}
	
	public void StopMoving(){
		_go = false;
	}
	
	public void ToggleGo(){
		_go = !_go;
	}

	public void StopLineEditing(){
		_editingLineAllowed = false;
	}
	
	public void AllowLineEditing(){
		_editingLineAllowed = true;
	}

	private void ClearWayPointPrefabs(){
		foreach (GameObject wayPointMarker in _wayPointGameObjects){ 
			Destroy(wayPointMarker);
		}
		_wayPointGameObjects.Clear ();
	}
	
	private void AddWayPointPrefab(Vector3 place){
		if (_selected || !_needToBeSelected) {
			GameObject go = (GameObject)GameObject.Instantiate (Resources.Load (_wayPointPrefabName));
			go.transform.parent = this.transform;
			go.transform.localPosition = Vector3.zero;
			go.transform.position = place;
			_wayPointGameObjects.Add (go);
		}
	}
	
	private void AttackPoint(Vector3 point, string targetType){
		if (!stats.commanderIsActive) {
			ClearPath ();
			_linePoints1.Add (point - stats.transform.position);
			CreateOrUpdateLine ();
			RecalcProgress ();
			ClearPath ();
		}
		/*if (!stats.commanderIsActive && _selected || !_needToBeSelected) {
			ClearPath ();
			AddPoint (point);
		}*/
	}

	private void SendAttack(Vector3 point, bool attack){
		if (_selected || !_needToBeSelected) {
			ClearPath ();
			AddPoint (point);
			_go = attack;
			stats.commanderIsActive = attack;
			if (_selected || !_needToBeSelected) {
				IsAttacking = true;
			}
			if (armyCol.childObj == null) {
				spawnHero.Appear ();
				stats.commanderIsActive = true;
				stats.selectionRing.Selected ();
				stats.selectionRing.Pressed ();
			}
			armyCol.spawnedChild ();
		}
	}
	
	private void ResetProgressValues(){
		_totalTime = 0f;
		_startingProgress = 0f;
		_progress01 = 0f;
		_targetprogress = 1f;
	}

	private void RecalcProgress(){
		_totalTime = 0f;
		_startingProgress = _progress01;
	}

	private void CreateOrUpdateLine(){
		// Bit crappy, should update line rather than make new
		if (_linePoints1.Count > 1) {
			if (_progressLine != null) {
				VectorLine.Destroy (ref this._progressLine);
			}
			CreateProgressLine (0, _linePoints1.ToArray ()); //Hacky
			_colors = new Color[_progressLine.points3.Length - 1]; //Very Hacky, use list
		}
	}
	
	void Update() {

		if (!_go && Time.time > _delayUpdateTime && _goAutomaticallyAfterDelay) {
			_go = true;
		}

		if(_go && _linePoints1.Count>1) {
            if(!Mathf.Approximately(_progress01, _targetprogress)) {
				_totalTime += (this.travelSpeed * Time.deltaTime) / _lineLength;
                _progress01 = Mathf.Lerp(_startingProgress, _targetprogress, _totalTime);

                ShowProgress(_progress01);
            }
			if(_targetprogress-_progress01<_nearEnough) {
				if (_go){
					_go=false;
				}
			}
        }

		// Time to target
		if (_progressLine != null) {
			_lineLength = _progressLine.GetLength ();
		}
		timeToDist = ((_lineLength * (_targetprogress - _progress01) / travelSpeed));
    }

	private void ShowProgress(float percent) {
		// Cull part of path outside the grid region
		int visible = (int)((_progressLine.points3.Length) * percent);
		for(int i = 0; i < _progressLine.points3.Length - 1; i++) {
			_colors[i] = (i < visible ? _lineColor : _lineColorUnfilled);
		}
		this._progressLine.SetColorsSmooth(_colors);
		Vector3 markerPos = this._progressLine.GetPoint3D01 (percent);

		_marker.transform.localPosition = new Vector3(markerPos.x, _marker.transform.localPosition.y, markerPos.z);
		if (_rotateMarker) {
			int segment = this._progressLine.GetSegmentIndexNearest01(percent)+_segments; //Look Ahead
			if (segment<(_progressLine.points3.Length)){
				Vector3 nextPoint = _progressLine.points3[segment];
				Vector3 targetLook = new Vector3(nextPoint.x, _marker.transform.localPosition.y, nextPoint.z) + this.transform.position;
				_hero.transform.LookAt(targetLook); //Can smooth later
			}
		}
	}

    private void CreateProgressLine(int progressLinesIndex, Vector3[] linePoints) {

        VectorLine progressLine;
		progressLine = new Vectrosity.VectorLine("ProgressLine", new Vector3[linePoints.Length*_segments + 1], this._lineMaterial, _thickness, LineType.Continuous);

        progressLine.vectorObject.transform.parent = this.transform;
        progressLine.vectorObject.transform.localPosition = Vector3.zero;
        progressLine.vectorObject.transform.localRotation = Quaternion.identity;
        progressLine.vectorObject.transform.localScale = new Vector3(1f, 1f, 1f);
        progressLine.joins = Joins.Weld;
		progressLine.maxWeldDistance = _maxWeldDistance;
        progressLine.MakeSpline(linePoints);
		progressLine.layer = _drawLayer;
        progressLine.SetColor(this._lineColorUnfilled);
        VectorLine.SetCamera(_renderCamera);
		progressLine.SetTextureScale(_textureScale);
        progressLine.AddNormals();
        progressLine.Draw3D();
		_progressLine = progressLine;
		_lineLength = _progressLine.GetLength();
    }

    void OnDestroy() {
		VectorLine.Destroy(ref this._progressLine);
		InteractionEvents.OnBroadcastPosition -= AttackPoint; //off
		InteractionEvents.OnBroadcastSendAttack -= SendAttack; 

	}

	/*void OnGUI() {
		if (_selected) {
			GUI.Label (new Rect (20, 180, 220, 50), "Total Time is: " + timeToDist);			
			GUI.Label (new Rect (20, 200, 220, 50), "Line Length is: " + _lineLength);
		}
	}*/
}
