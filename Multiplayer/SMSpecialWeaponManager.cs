//
//  SMSpecialWeaponManager.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 22/07/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SandeepMattepu.Multiplayer
{
	/// <summary>
	/// This class helps to maintain special weapons and abilities of the player
	/// </summary>
	public class SMSpecialWeaponManager : MonoBehaviour 
	{
		/// <summary>
		/// The kill strak for health boost.
		/// </summary>
		[SerializeField]
		private int killStreakForHealthBoost = 6;
		/// <summary>
		/// The kill streak for rocket.
		/// </summary>
		[SerializeField]
		private int killStreakForRocket = 4;
		/// <summary>
		/// The rules for multiplayer game
		/// </summary>
		private SMMultiplayerGame multiplayerGame;
		/// <summary>
		/// The rocket button.
		/// </summary>
		public Button rocketButton;
		/// <summary>
		/// The health boost button.
		/// </summary>
		public Button healthBoostButton;
		/// <summary>
		/// The cancel button.
		/// </summary>
		public Button cancelButton;
		/// <summary>
		/// The can lauch rocket.
		/// </summary>
		private bool canLauchRocket = false;
		/// <summary>
		/// The can have health boost.
		/// </summary>
		private bool canHaveHealthBoost = false;
		/// <summary>
		/// The rocket is in aiming position.
		/// </summary>
		private bool rocketIsInAimingPosition = false;
		// Use this for initialization
		void Start () 
		{
			cancelButton.gameObject.SetActive (false);
			rocketButton.image.color = Color.black;
			healthBoostButton.image.color = Color.black;
		}

		/// <summary>
		/// This function gets called when rocket button is pressed
		/// </summary>
		public void onRocketButtonPressed()
		{
			if(canLauchRocket)
			{
				rocketButton.gameObject.SetActive (false);
				healthBoostButton.gameObject.SetActive (false);
				cancelButton.gameObject.SetActive (true);
				rocketIsInAimingPosition = true;
			}
		}

		/// <summary>
		/// This function gets called when cancel button is pressed
		/// </summary>
		public void onCancelButtonPressed()
		{
			if(rocketIsInAimingPosition)
			{
				rocketIsInAimingPosition = false;
				rocketButton.gameObject.SetActive (true);
				healthBoostButton.gameObject.SetActive (true);
				cancelButton.gameObject.SetActive (false);
			}
		}

		/// <summary>
		/// This function gets called when health button is pressed
		/// </summary>
		public void onHealthBoostButtonPressed()
		{
			if(canHaveHealthBoost)
			{
				multiplayerGame.localPlayer.gameObject.GetComponent<SMPlayerHealth> ().giveArmorToPlayer ();
				multiplayerGame.reduceHealthStreakBy (killStreakForHealthBoost);
			}
		}

		/// <summary>
		/// This function gets called whenever there is kill streak change
		/// </summary>
		/// <param name="killStreak">Current kill streak.</param>
		private void onKillStreakHandler(int killStreak)
		{
			if(killStreak == 0)
			{
				canLauchRocket = false;
				canHaveHealthBoost = false;
				rocketIsInAimingPosition = false;

				rocketButton.image.color = Color.black;
				healthBoostButton.image.color = Color.black;
				cancelButton.gameObject.SetActive (false);
			}
			else if(killStreak >= killStreakForHealthBoost)
			{
				if(SMMultiplayerGame.isGoldenPlayer)
				{
					canHaveHealthBoost = true;
					healthBoostButton.image.color = Color.white;
				}
				else
				{
					healthBoostButton.image.color = Color.yellow;
					// show panel that buy gold
				}
			}
			else if(killStreak >= killStreakForRocket)
			{
				if(SMMultiplayerGame.isGoldenPlayer)
				{
					canLauchRocket = true;
					rocketButton.image.color = Color.white;
				}
				else
				{
					rocketButton.image.color = Color.yellow;
					// show panel that buy gold
				}
			}
		}

		/// <summary>
		/// Registers the multiplayer game.
		/// </summary>
		/// <param name="gameType">Multiplayer game type.</param>
		public void registerMultiplayerGame(SMMultiplayerGame gameType)
		{
			multiplayerGame = gameType;
			multiplayerGame.OnKillStreakChange += onKillStreakHandler;
		}
	}	
}
