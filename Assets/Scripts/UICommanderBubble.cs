using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UICommanderBubble : MonoBehaviour {
	public enum ScreenMode {
		SCREEN_EDGES,
		CURVE
	}

	public ScreenMode mode;
	public Transform target;	
	public Transform targetChild;
	public RectTransform bubbleArrow;
	public GameObject bubbleArrowObj;
	public GameObject RadialTimer;
	public GameObject commanderHead;
	public Sprite bubbleArrowSprite;
	public Sprite bubbleNoArrowSprite;
	public Sprite bubbleArrowSpriteSelected;
	public Sprite bubbleNoArrowSpriteSelected;
	public Image currentBubbleSprite;
	public Button commanderHeadButton;
	public float screenBuffer = 100f;
	public Camera mainCamera;
	public GameObject minimapCamera;
	private MiniMap_Cam minimapCameraScript;

	public bool visible = true;
	public bool onHover = false;

	private Vector3 screenPos;
	private float leftBorder;
	private float rightBorder;
	private float bottomBorder;
	private float topBorder;

	private Animator anim;
	private UIScreenCurve screenCurve;

	public ArmyStats targetButton;
	public Settlement home;	
	public EventTriggerDragIcon homeButton;
	public ProgressLine homeLine;

	private GameObject homeObj;
	private CommanderMGMT cMGMT;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		screenCurve = GameObject.FindWithTag("ScreenCurve").GetComponent<UIScreenCurve> ();

		mainCamera = Camera.main;
		minimapCamera = GameObject.FindWithTag ("MiniMapCam");
		minimapCameraScript = minimapCamera.GetComponent<MiniMap_Cam> ();

		leftBorder = 0f + screenBuffer;
		rightBorder = mainCamera.pixelWidth - screenBuffer;
		bottomBorder = 0f + screenBuffer;
		topBorder = mainCamera.pixelHeight - screenBuffer;

		targetButton = target.GetComponent<ArmyStats> ();
		home = target.GetComponent<Settlement> ();
		homeButton = target.GetComponent<EventTriggerDragIcon> ();
		homeLine = target.GetComponent<ProgressLine> ();
				
		targetChild = target.transform.GetChild (0);

		if (targetButton != null) {
			selectionRing = targetButton.selectionRing;
			selectionAnim = targetButton.selectionRing.GetComponent<Animator>();;
		}
		if (home != null) {
			selectionRing = home.selectionRing;
			selectionAnim = home.selectionRing.GetComponent<Animator> ();
			;
		}
		
		homeObj = GameObject.FindWithTag ("Home");
		cMGMT = homeObj.GetComponent<CommanderMGMT> ();
	}

	public void OnPointerUp(){
		if (targetButton != null) {
			targetButton.OnPointerUp ();
			targetButton.selectionRing.OnPointerUp();
			homeLine.Selected();
			cMGMT.CameraToCommander();
		}
		if (home != null) {
			home.selectionRing.OnPointerUp();			
			home.homeInfo.OnPointerUp();
			homeButton.OnPointerUp();
		}

		// Selection Ring Animations
		if (!onDrag) {
			onUp = true;
		}
	}
	
	// Update is called once per frame
	void LateUpdate () {
		// Set visibility
		visible = minimapCameraScript.MiniMapEnabled;
		if (visible) {
			anim.SetBool ("On", true);
		}

		if (targetButton != null && !targetButton.commanderIsActive) {
			bubbleArrowObj.SetActive (false);
			RadialTimer.SetActive (false);
			commanderHead.SetActive (false);
		} else {			
			bubbleArrowObj.SetActive(true);
			RadialTimer.SetActive(true);
			commanderHead.SetActive(true);
		}
				
		leftBorder = 0f + screenBuffer;
		rightBorder = mainCamera.pixelWidth - screenBuffer;
		bottomBorder = 0f + screenBuffer;
		topBorder = mainCamera.pixelHeight - screenBuffer;

		//screenPos = mainCamera.WorldToScreenPoint(target.position);

		Vector3 relativePosition = mainCamera.transform.InverseTransformPoint (targetChild.position);
		relativePosition.z = Mathf.Max (relativePosition.z, 1.0f);
		screenPos = mainCamera.WorldToScreenPoint(mainCamera.transform.TransformPoint(relativePosition));

		if (this.mode == ScreenMode.SCREEN_EDGES) {
			FitToScreenEdge();
		} else if(this.mode == ScreenMode.CURVE) {
			FitToCurve();
		}
	}

	private void FitToCurve() {
		Vector3 preClampedScreenPos = screenPos;
		transform.position = screenPos = this.screenCurve.FitInsideCurve (screenPos);

		Vector3 dir = (preClampedScreenPos - screenPos).normalized;
		float angle = Mathf.Acos (Vector3.Dot (dir, Vector3.up)) * Mathf.Rad2Deg;
		if (screenPos.x >= mainCamera.pixelWidth * 0.5f) {
			angle = 360 - angle;
		}

		if (preClampedScreenPos != screenPos) {
			//ChangeLayersRecursively (this.transform, "UI");
			anim.SetBool ("On", true);
			bubbleArrow.transform.localRotation = Quaternion.Euler (new Vector3 (0f, 0f, angle));
			if (homeLine != null && homeLine._selected) {
				currentBubbleSprite.sprite = bubbleArrowSpriteSelected;
			}
			else {
				currentBubbleSprite.sprite = bubbleArrowSprite;
			}
		} else {
			//ChangeLayersRecursively (this.transform, "MiniMap");
			if (homeLine != null && homeLine._selected) {
				currentBubbleSprite.sprite = bubbleNoArrowSpriteSelected;
			}
			else {
				currentBubbleSprite.sprite = bubbleNoArrowSprite;
			}
			//anim.SetBool ("On",false);
		}
	}

	private void FitToScreenEdge() {
		leftBorder = 0f + screenBuffer;
		rightBorder = mainCamera.pixelWidth - screenBuffer;
		bottomBorder = 0f + screenBuffer;
		topBorder = mainCamera.pixelHeight - screenBuffer;
		
		Vector3 preClampedScreenPos = screenPos;
		screenPos.x = Mathf.Clamp (screenPos.x, leftBorder, rightBorder);
		screenPos.y = Mathf.Clamp (screenPos.y, bottomBorder, topBorder);		
		transform.position = screenPos;
		
		Vector3 dir = (preClampedScreenPos - screenPos).normalized;
		float angle = Mathf.Acos (Vector3.Dot (dir, Vector3.up)) * Mathf.Rad2Deg;
		if (screenPos.x >= rightBorder) {
			angle = 360 - angle;
		}

		if ((screenPos.x <= leftBorder && targetButton != null && targetButton.commanderIsActive) || (screenPos.x <= leftBorder && home != null)) {
			visible = true;
			anim.SetBool ("On",true);
			bubbleArrow.transform.localRotation = Quaternion.Euler (new Vector3 (0f, 0f, angle));
			if ((homeLine != null && homeLine._selected) || (home != null && home.selectionRing.selected)){
				currentBubbleSprite.sprite = bubbleArrowSpriteSelected;
			} else if (!onHover) {
				currentBubbleSprite.sprite = bubbleArrowSprite;
			}
		}
		else if ((screenPos.x >= rightBorder && targetButton != null && targetButton.commanderIsActive) || (screenPos.x >= rightBorder && home != null)) {
			visible = true;
			anim.SetBool ("On",true);
			bubbleArrow.transform.localRotation = Quaternion.Euler (new Vector3 (0f, 0f, angle));
			if ((homeLine != null && homeLine._selected) || (home != null && home.selectionRing.selected)){
				currentBubbleSprite.sprite = bubbleArrowSpriteSelected;
			} else if (!onHover) {
				currentBubbleSprite.sprite = bubbleArrowSprite;
			}
		}
		else if ((screenPos.y <= bottomBorder && targetButton != null && targetButton.commanderIsActive) || (screenPos.y <= bottomBorder && home != null)) {
			visible = true;
			anim.SetBool ("On",true);
			bubbleArrow.transform.localRotation = Quaternion.Euler (new Vector3 (0f, 0f, angle));
			if ((homeLine != null && homeLine._selected) || (home != null && home.selectionRing.selected)){
				currentBubbleSprite.sprite = bubbleArrowSpriteSelected;
			} else if (!onHover) {
				currentBubbleSprite.sprite = bubbleArrowSprite;
			}
		}
		else if ((screenPos.y >= topBorder && targetButton != null && targetButton.commanderIsActive) || (screenPos.y >= topBorder && home != null)) {
			visible = true;
			anim.SetBool ("On",true);
			bubbleArrow.transform.localRotation = Quaternion.Euler (new Vector3 (0f, 0f, angle));
			if ((homeLine != null && homeLine._selected) || (home != null && home.selectionRing.selected)){
				currentBubbleSprite.sprite = bubbleArrowSpriteSelected;
			} else if (!onHover) {
				currentBubbleSprite.sprite = bubbleArrowSprite;
			}
		} else {
			if ((homeLine != null && homeLine._selected) || (home != null && home.selectionRing.selected)){
				currentBubbleSprite.sprite = bubbleNoArrowSpriteSelected;
			} else if (!onHover) {
				currentBubbleSprite.sprite = bubbleNoArrowSprite;
			}
			if (targetButton != null && !targetButton.commanderIsActive){
				visible = false;
			}
			if (!visible){
				anim.SetBool ("On",false);
			}
		}

		//bubbleArrow.transform.localRotation = Quaternion.Euler (new Vector3 (0f, 0f, angle));
	}

	// Selection Ring Animations
	public bool onUp = false;
	public bool onDrag = false;
	private SelectionRingAnim selectionRing;
	private Animator selectionAnim;
	
	public void Normal() {
		if (selectionAnim != null){
			if (!selectionRing.selected) {
				selectionAnim.SetTrigger ("Normal");
				currentBubbleSprite.sprite = bubbleArrowSpriteSelected;
			}
			selectionAnim.SetBool("Highlighted", false);
			currentBubbleSprite.sprite = bubbleArrowSprite;
			//selected = false;
			onHover = false;
		}
	}
	public void Selected() {
		if (selectionAnim != null) {
			selectionAnim.SetTrigger ("Selected");
		}
	}
	
	public void Deselect(){
		selectionRing.selected = false;
	}
	
	public void Highlighted() {
		if (selectionAnim != null) {
			selectionAnim.SetBool ("Highlighted", true);
			onHover = true;
		}
		
	}
	public void Pressed() {
		if (selectionAnim != null) {
			selectionAnim.SetTrigger ("Pressed");
			selectionRing.selected = true;	
		}
	}
	public void OnDragEnd() {
		onDrag = true;
	}
	
	void Update() {
		/*if (targetButton != null && targetButton.commanderIsActive) {
			anim.SetBool ("On", true);
		} else {
			anim.SetBool("On", false);
		}
		if (homeButton != null) {
			anim.SetBool ("On", true);
		} else {
			anim.SetBool("On", false);
		}*/

		if (onUp && !onDrag) {
			Pressed();
		}
		if (onUp || onDrag) {
			onDrag = false;
			onUp = false;
		}
	}
}
