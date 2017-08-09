//
//  SMTouchManager.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 04/02/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SandeepMattepu.UI;

namespace SandeepMattepu.MobileTouch
{
	/// <summary>
	/// This class script will continuosly perform raycasting from the touches player made and it will differentiate whether the touch is made on UI or on the game scene.
	/// Later it will register those touches which can be later be used for analyzing those touches and their raycast results
	/// </summary>
	public class SMTouchManager : MonoBehaviour 
	{
		/// <summary>
		/// This is GraphicRaycaster component attached to the canvas
		/// </summary>
		private GraphicRaycaster rayCaster;
		/// <summary>
		/// This records all the touches in current frame
		/// </summary>
		private static List<SMTouchIdentifier> touchesInFrame = new List<SMTouchIdentifier>();
		/// <summary>
		/// Occurs when single tap was made on screen.
		/// </summary>
		public event singleGameTapMade OnSingleGameTap;
		#region Long Press variables

		/// <summary>
		/// This becomes true when there is a game touch in particular frame
		/// </summary>
		private bool isGameTouchMade = false;
		/// <summary>
		/// This holds reference to first touch that is made in the game
		/// </summary>
		private Touch gameTouch;
		/// <summary>
		/// The initial position of touch.
		/// </summary>
		private Vector2 initialPositionOfTouch;
		/// <summary>
		/// This timer ticks after touch is made on the screen.
		/// </summary>
		private float timerAfterTouch = 0.0f;
		/// <summary>
		/// This event will raise when there is a long press on game screen(Not swipe)
		/// </summary>
		public event longPressMade OnGameLongPress;

		#endregion
		// Use this for initialization
		void Start () 
		{
			rayCaster = GetComponent<GraphicRaycaster> ();
		}
		
		// Update is called once per frame
		void Update () 
		{
			manageTouches ();
			calculateTouchTypes ();
		}

		/// <summary>
		/// Calculate all the touches type that are made on the game
		/// </summary>
		private void calculateTouchTypes()
		{
			if(isGameTouchMade)
			{
				// Below if code is written because touches are value types and changes every frame.
				// We can't reference them to track changes
				if(Input.touchCount > 0)
				{
					foreach(Touch touch in Input.touches)
					{
						if(touch.fingerId == gameTouch.fingerId)
						{
							gameTouch = touch;
							break;
						}
					}
				}

				if((gameTouch.position - initialPositionOfTouch).magnitude < 0.20f)
				{
					if(timerAfterTouch < 1.5f)
					{
						TouchPhase gameTouchPhase = gameTouch.phase;
						if(gameTouchPhase == TouchPhase.Canceled || gameTouchPhase == TouchPhase.Ended)
						{
							if(timerAfterTouch < 0.30f)
							{
								if(OnSingleGameTap != null)
								{
									OnSingleGameTap (this, gameTouch);
								}
							}
							isGameTouchMade = false;
							timerAfterTouch = 0.0f;
						}
						else
						{
							timerAfterTouch += Time.deltaTime;
						}
					}
					else
					{
						if(OnGameLongPress != null)
						{
							OnGameLongPress (this, gameTouch);
						}
						isGameTouchMade = false;
						timerAfterTouch = 0.0f;
					}
				}
				else
				{
					isGameTouchMade = false;
					timerAfterTouch = 0.0f;
				}
			}
			else if(Input.touchCount > 0)
			{
				foreach(Touch touch in Input.touches)
				{
					if(touch.phase == TouchPhase.Began)
					{
						if(processWhichTouchBelongsTo(touch) == TouchBelongs.GAME)
						{
							isGameTouchMade = true;
							gameTouch = touch;
							initialPositionOfTouch = gameTouch.position;
							break;
						}
					}
				}
			}
		}

		/// <summary>
		/// This function will check whether the touch is made on UI
		/// </summary>
		/// <param name="touchMade">Pass the touch value</param>
		/// <returns>Layer value which indicates where the touch is made on UI</returns>
		public int layerTouchMadeOn(Touch touchMade)
		{
			PointerEventData input = new PointerEventData (null);		// Null is passed to make PointerEventData to consider touches
			input.position =touchMade.position;							// Manually assigned position of PointerEventData as Touch position
			List<RaycastResult> results = new List<RaycastResult> ();

			rayCaster.Raycast (input, results);							// Perform raycasting
			if(results.Count > 0)
			{
				RaycastResult result = results [0];
				if(result.gameObject.activeInHierarchy)
				{
					return result.gameObject.layer;
				}
			}
			return 0;
		}

		/// <summary>
		/// This function will check whether the touch/mouse click is made on UI
		/// </summary>
		/// <returns>Layer value which indicates where the mouse click is made on UI.</returns>
		/// <param name="position">Position of the touch/mouse in screen pixels</param>
		public int layerTouchMadeOn(Vector3 position)
		{
			PointerEventData input = new PointerEventData (null);		// Null is passed to make PointerEventData to consider touches
			input.position = position;							// Manually assigned position of PointerEventData as Touch position
			List<RaycastResult> results = new List<RaycastResult> ();

			rayCaster.Raycast (input, results);							// Perform raycasting
			if(results.Count > 0)
			{
				return results [0].gameObject.layer;
			}
			return 0;
		}

		/// <summary>
		/// This function will manage all the touches made on the screen. It will register new touches and delete the older ones
		/// </summary>
		private void manageTouches()
		{
			if(Input.touchCount > 0)
			{
				foreach(Touch touch in Input.touches)
				{
					if(touch.phase == TouchPhase.Began)
					{
						registerNewTouch (touch);			// Register new touches
					}
					else if(touch.phase == TouchPhase.Ended)
					{
						for (int x = 0; x < touchesInFrame.Count; x++) 
						{
							if(touch.fingerId == touchesInFrame[x].fingerID)
							{
								touchesInFrame.RemoveAt (x);		// Remove old touches
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// This function will register new touch made on the screen and makes a record of it
		/// </summary>
		/// <param name="touch">Pass the touch value to register it</param>
		private void registerNewTouch(Touch touch)
		{
			int layerBelongTo = layerTouchMadeOn (touch);
			SMTouchIdentifier newTouchIdentifier = new SMTouchIdentifier (touch, layerBelongTo); 
			touchesInFrame.Add (newTouchIdentifier);
		}

		/// <summary>
		/// This function will return where the touch belongs to based on the touch value
		/// </summary>
		/// <param name="touch">Pass the touch value to analyze it</param>
		/// <returns>Where the touch belongs to</returns>
		public static TouchBelongs processWhichTouchBelongsTo(Touch touch)
		{
			foreach(SMTouchIdentifier touchIdentifier in touchesInFrame)
			{
				if(touch.fingerId == touchIdentifier.fingerID)
				{
					return touchIdentifier.touchBelongsTo;
				}
			}
			return TouchBelongs.GAME;
		}
	}

}
