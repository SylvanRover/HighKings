using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// For unreferenced var
#pragma warning disable 0414
public class TKDoubleTapRecognizer : TKAbstractGestureRecognizer
{
	public event Action<TKDoubleTapRecognizer> gestureRecognizedEvent;

	private int numberOfTapsRequired = 2;
	private int numberOfTouchesRequired = 1;

	// taps that last longer than this duration will be ignored
	private float _maxDurationForTapConsideration = 0.5f;
	private float _maxDeltaMovementForTapConsideration = 5f;

	private float _previousTouchBeganTime;
	private float _currentTouchBeganTime;
	private int intimeTaps;

	public TKDoubleTapRecognizer() : this( 0.5f, 5f )
	{}


	public TKDoubleTapRecognizer( float maxDurationForTapConsideration, float maxDeltaMovementForTapConsideration )
	{
		_maxDurationForTapConsideration = maxDurationForTapConsideration;
		_maxDeltaMovementForTapConsideration = maxDeltaMovementForTapConsideration * TouchKit.instance.runtimeDistanceModifier;

		// Make starting touch times out of bounds
		_previousTouchBeganTime = Time.time - _maxDurationForTapConsideration;
		_currentTouchBeganTime = _previousTouchBeganTime;
	}


	internal override void fireRecognizedEvent()
	{
		if( gestureRecognizedEvent != null )
			gestureRecognizedEvent( this );
	}


	internal override bool touchesBegan( List<TKTouch> touches )
	{
		if( state == TKGestureRecognizerState.Possible )
		{
			for( int i = 0; i < touches.Count; i++ )
			{
				// only add touches in the Began phase
				if( touches[i].phase == TouchPhase.Began )
				{
					_trackingTouches.Add( touches[i] );

					if( _trackingTouches.Count == numberOfTouchesRequired )
						break;
				}
			} // end for

			if( _trackingTouches.Count == numberOfTouchesRequired )
			{
				_previousTouchBeganTime = _currentTouchBeganTime;
				_currentTouchBeganTime = Time.time;
				state = TKGestureRecognizerState.Began;

				return true;
			}
		}

		return false;
	}


	internal override void touchesMoved( List<TKTouch> touches )
	{
		/*
		if( state == TKGestureRecognizerState.Began )
		{
			// did we move?
			for( var i = 0; i < touches.Count; i++ )
			{
				if( (touches[i].position - touches[i].startPosition).sqrMagnitude > _maxDeltaMovementForTapConsideration )
				{
					//intimeTaps = 0;
					state = TKGestureRecognizerState.FailedOrEnded;
					break;
				}
			}
		}
		*/
	}


	internal override void touchesEnded( List<TKTouch> touches )
	{
		//state == TKGestureRecognizerState.Possible && 
		if (state == TKGestureRecognizerState.Began && (Time.time <= _currentTouchBeganTime + _maxDurationForTapConsideration)){

			// Check if curentTouch fullfills requirement on own
			if (touches[0].tapCount >= numberOfTapsRequired && Time.time < _currentTouchBeganTime + _maxDurationForTapConsideration){
				state = TKGestureRecognizerState.Recognized;
			}else if (Time.time < _previousTouchBeganTime + _maxDurationForTapConsideration){
				// Checking that previous touch and this new one fullfill requirement
				state = TKGestureRecognizerState.Recognized;
			}else{
				_previousTouchBeganTime = _currentTouchBeganTime;
			}
		} else{
			state = TKGestureRecognizerState.FailedOrEnded;
		}
	}

}
#pragma warning restore 0414
