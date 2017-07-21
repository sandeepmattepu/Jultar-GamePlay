//
//  SMGrenadeInput.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 21/07/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SandeepMattepu.UI
{
	/// <summary>
	/// This class will handle grenade input
	/// </summary>
	public class SMGrenadeInput : MonoBehaviour, IPointerUpHandler, IPointerDownHandler 
	{
		public delegate void pressIntensity(float intensity);
		public delegate void onTouchDown();
		/// <summary>
		/// Occurs when on player presses button for more time.
		/// </summary>
		public event pressIntensity OnPressIntensity;
		/// <summary>
		/// Occurs when on touch down happens.
		/// </summary>
		public event onTouchDown OnTouchDownHandler;
		/// <summary>
		/// The player is in process of throwing.
		/// </summary>
		private bool isThrowing = false;
		/// <summary>
		/// The time player is holding button.
		/// </summary>
		private float timePlayerIsHoldingButton = 0.0f;
		/// <summary>
		/// The max button holding time.
		/// </summary>
		[SerializeField]
		private float maxHoldingTime = 1.6f;

		// Update is called once per frame
		void Update () 
		{
			if(timePlayerIsHoldingButton >= maxHoldingTime)
			{
				if(OnPressIntensity != null)
				{
					OnPressIntensity(1.0f);
				}
				isThrowing = false;
				timePlayerIsHoldingButton = 0.0f;
			}
			else if(isThrowing)
			{
				timePlayerIsHoldingButton += Time.deltaTime;
			}
		}

		public virtual void OnPointerDown(PointerEventData data)
		{
			isThrowing = true;

			if(OnTouchDownHandler != null)
			{
				OnTouchDownHandler ();
			}
		}

		public virtual void OnPointerUp(PointerEventData data)
		{
			if(isThrowing)
			{
				if(timePlayerIsHoldingButton < maxHoldingTime)
				{
					float percentage = findPercentage (0.0f, maxHoldingTime, timePlayerIsHoldingButton);
					if(OnPressIntensity != null)
					{
						OnPressIntensity(percentage);
					}
				}
				else
				{
					if(OnPressIntensity != null)
					{
						OnPressIntensity(1.0f);
					}
				}
			}

			isThrowing = false;
			timePlayerIsHoldingButton = 0.0f;
		}

		/// <summary>
		/// Interpolates the values.
		/// </summary>
		/// <returns>percentage of the value.</returns>
		/// <param name="minValue">Minimum value.</param>
		/// <param name="maxValue">Max value.</param>
		/// <param name="actualValue">Actual value.</param>
		private float findPercentage(float minValue, float maxValue, float actualValue)
		{
			return (actualValue - minValue) / (maxValue - minValue);
		}
	}	
}
