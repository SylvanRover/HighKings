using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class TKRectTransformValidator
{
	public RectTransform rectArea {
		get;
		set;
	}
	private Camera cam;
	private bool outsideIsValid = true;

	public TKRectTransformValidator(RectTransform  rectArea, Camera cam, bool outsideIsValid=true){
		this.rectArea = rectArea;
		this.outsideIsValid = outsideIsValid;
	}

	public bool Validate(Vector2 touchPosition){
		bool isInside = RectTransformUtility.RectangleContainsScreenPoint(this.rectArea, touchPosition, this.cam);
		return outsideIsValid ? !isInside : isInside;
	}

	public bool Validate(TKTouch touch){
		return Validate(touch.position);
	}
	
}
