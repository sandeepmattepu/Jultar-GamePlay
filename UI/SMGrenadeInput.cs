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
using UnityEngine.UI;
using SandeepMattepu.Weapon;

namespace SandeepMattepu.UI
{
	/// <summary>
	/// This class will handle grenade input
	/// </summary>
	public class SMGrenadeInput : MonoBehaviour, IPointerUpHandler, IPointerDownHandler 
	{
		/// <summary>
		/// This image changes its color based on availability, under firing
		/// </summary>
		[SerializeField]
		private Image grenadeImage;
		/// <summary>
		/// Color of grenade button when available and not pressed.
		/// </summary>
		public Color whenAvailableAndNotUnderThrowing;
		/// <summary>
		/// Color of grenade when available and under pressed.
		/// </summary>
		public Color whenAvailableAndUnderThrowing;
		/// <summary>
		/// Color of grenade when not available
		/// </summary>
		public Color whenNotAvailable;
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
		/// <summary>
		/// The player component which has grenade's date.
		/// </summary>
		private IKControl playerGrenade;
		/// <summary>
		/// The player component which has grenade's date.
		/// </summary>
		public IKControl PlayerGrenade {
			get {
				return playerGrenade;
			}
			set {
				playerGrenade = value;
				assignEventsToPlayer (playerGrenade);
			}
		}
		/// <summary>
		/// The player firing component.
		/// </summary>
		private SMPlayerFiring playerFiring;
		/// <summary>
		/// The player firing component.
		/// </summary>
		public SMPlayerFiring PlayerFiring {
			get {
				return playerFiring;
			}
			set {
				playerFiring = value;
				setUIColor ();
			}
		}

		void Start()
		{
			// This is to fix when player is in process of throwing and got killed, grenade ui color wont be normal but will has
			// whenAvailableAndUnderTouching color
			SMPlayerHealth.LocalPlayerDied += OnGrenadeThrowFinishHandler;
		}

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

		private void setUIColor()
		{
			OnGrenadeThrowFinishHandler ();
		}

		/// <summary>
		/// This function assigns required functions to events of the player
		/// </summary>
		private void assignEventsToPlayer(IKControl player)
		{
			player.OnGrenadeRelease += (() => grenadeImage.color = whenAvailableAndUnderThrowing);
			player.OnGrenadeThrowFinished += OnGrenadeThrowFinishHandler;
		}

		private void OnGrenadeThrowFinishHandler()
		{
			if(playerFiring.NumberOfGrenadeBombs > 0)
			{
				grenadeImage.color = whenAvailableAndNotUnderThrowing;
			}
			else
			{
				grenadeImage.color = whenNotAvailable;
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
