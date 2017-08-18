//
//  SMJoyStick.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 31/01/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using SandeepMattepu.UI;
using SandeepMattepu.MobileTouch;

/// <summary>
/// This class will help the UI Images to behave like joystick and we can retrive joystick values like rotation and magnitude
/// </summary>
[RequireComponent(typeof(Image))]
public class SMJoyStick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
	/// <summary>
	/// This will determine the joystick type
	/// </summary>
	public JoyStickType joyStickType;
	/// <summary>
	/// This is the background image for the joystick
	/// </summary>
	private Image joyStickBackground;

	/// <summary>
	/// This is the image of the joystick knob
	/// </summary>
	private Image joyStickKnob;

	/// <summary>
	/// This will store whether user touches the joystick or not
	/// </summary>
	private bool isPlayerTouchingJoyStick = false;
	/// <summary>
	/// This is the vector used to store the input values
	/// </summary>
	private Vector2 inputPositionVector;

	/// <summary>
	/// Register functions to this to get called when single tap is made
	/// </summary>
	public event singleTapMade SingleTapEvent;
	/// <summary>
	/// Register function to this to get called when double tap is made
	/// </summary>
	public event doubleTapMade DoubleTapEvent;

	#region Variable to Calculate Single and Double Tap

	/// <summary>
	/// This timer will tick when a touch or click is made
	/// </summary>
	private float timerForSingleTap = 0.0f;
	/// <summary>
	/// This timer will tick when a single tap is made
	/// </summary>
	private float timerAfterSingleTap = 0.0f;
	/// <summary>
	/// This variable becomes true when previously there was a single tap made
	/// </summary>
	private bool isPreviouslySingleTapMade = false;
	#endregion
	// Use this for initialization
	void Start () 
	{
		inputPositionVector = Vector2.zero;
		joyStickBackground = GetComponent<Image> ();
		joyStickKnob = transform.GetChild (0).GetComponent<Image> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		tickTimersWhileTouchingAndNot ();
	}

	/// <summary>
	/// This function ticks the timers while touching and not touching.
	/// </summary>
	private void tickTimersWhileTouchingAndNot()
	{
		if(isPlayerTouchingJoyStick)
		{
			if(isPreviouslySingleTapMade)
			{
				if(timerForSingleTap > 0.30f)
				{
					isPreviouslySingleTapMade = false;
					if(SingleTapEvent != null)
					{
						SingleTapEvent (this, joyStickType);
					}
				}
				else
				{
					timerForSingleTap += Time.deltaTime;
				}
			}
			else
			{
				timerForSingleTap += Time.deltaTime;
			}
		}
		else if(isPreviouslySingleTapMade)
		{
			if(timerAfterSingleTap < 0.15f)
			{
				timerAfterSingleTap += Time.deltaTime;
			}
			else
			{
				if(SingleTapEvent != null)
				{
					SingleTapEvent (this, joyStickType);
				}
				timerForSingleTap = 0.0f;
				timerAfterSingleTap = 0.0f;
				isPreviouslySingleTapMade = false;
			}
		}
	}

	// When touch is pressed on the joystick it is called
	public virtual void OnPointerDown(PointerEventData data)
	{
		// We want to call this even when just touch is recieved at one point without drag
		OnDrag (data);
		isPlayerTouchingJoyStick = true;
	}

	// When touch is released from the joystick it is called
	public virtual void OnPointerUp (PointerEventData data)
	{
		positionKnob (Vector2.zero);
		if (joyStickType == JoyStickType.MOVEMENT)		// If joystick is movement type then assign zero vector everytime player removes finger from stick
		{
			inputPositionVector = Vector2.zero;
		}
		else
		{
			inputPositionVector = 0.2f * (inputPositionVector);	// Keeping the orientation same but reducing the magnitude to avoid firing from the player
		}
		isPlayerTouchingJoyStick = false;

		calculateTouchTypeProduced ();
	}

	/// <summary>
	/// This function calculates the touch type made
	/// </summary>
	private void calculateTouchTypeProduced()
	{
		if(timerForSingleTap < 0.30f)
		{
			if(isPreviouslySingleTapMade && timerAfterSingleTap < 0.15f)
			{
				if(DoubleTapEvent != null)
				{
					DoubleTapEvent (this, joyStickType);
				}
				isPreviouslySingleTapMade = false;
				timerForSingleTap = 0.0f;
				timerAfterSingleTap = 0.0f;
			}
			else
			{
				isPreviouslySingleTapMade = true;
			}
		}
		else
		{
			isPreviouslySingleTapMade = false;
			timerForSingleTap = 0.0f;
			timerAfterSingleTap = 0.0f;
		}
	}

	// When touch is dragged it is called
	public virtual void OnDrag(PointerEventData data)
	{
		Vector2 position;
		float xFraction, yFraction;		// Store fractional values, whose values show the relative movement of knob from background
		if(RectTransformUtility.ScreenPointToLocalPointInRectangle(joyStickBackground.rectTransform, data.position, data.pressEventCamera, out position))
		{
			xFraction = (position.x / joyStickBackground.rectTransform.rect.width);
			yFraction = (position.y / joyStickBackground.rectTransform.rect.height);

			inputPositionVector = new Vector2 (xFraction, yFraction);
			if(inputPositionVector.magnitude > 1.0f)		// Knob acts like square so we might not get values identical to circle radius vector
			{
				inputPositionVector = inputPositionVector.normalized;
			}

			// Position the knob based on the touch drag
			Vector2 knobPosition = new Vector2((inputPositionVector.x * (joyStickBackground.rectTransform.rect.width/2)),
				(inputPositionVector.y * (joyStickBackground.rectTransform.rect.height/2)));
			positionKnob (knobPosition);
		}
	}

	/// <summary>
	/// This function will set the position of the Joystick knob based on the given vector
	/// </summary>
	/// <param name="knobPosition">Pass the Knob position vector</param>
	private void positionKnob(Vector2 knobPosition)
	{
		joyStickKnob.rectTransform.anchoredPosition = knobPosition;
	}

	// Note :- You can use this function for retriving the vector of joystick
	/// <summary>
	/// Use this function for retriving the vector of joystick
	/// </summary>
	public Vector2 getKnobVector()
	{
		return inputPositionVector;
	}

	// Note :- You can use this function to know whether player is still touching the joystick or not
	/// <summary>
	/// Use this function to retrive bool value which says whether the user is touching the joystick or not
	/// </summary>
	/// <returns>Bool value which says whether the user is touching the joystick or not</returns>
	public bool isJoyStickUnderTouching()
	{
		return isPlayerTouchingJoyStick;
	}
}

/// <summary>
/// This enum will determine the functionality of the joystick
/// </summary>
public enum JoyStickType
{
	MOVEMENT, AIMING
}

namespace SandeepMattepu.UI
{
	public delegate void singleTapMade(object sender, JoyStickType stickType);

	public delegate void doubleTapMade(object sender, JoyStickType stickType);

	public delegate void longPressMade(object sender, Touch touchType);

	public delegate void singleGameTapMade(object sender, Vector3 touchPosOnScreen);
}
