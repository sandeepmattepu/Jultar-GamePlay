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
using SandeepMattepu.MobileTouch;

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
		/// The touch manager.
		/// </summary>
		[SerializeField]
		private SMTouchManager touchManager;
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
		/// This defines what race type local player is
		/// </summary>
		public MpPlayerRaceType localPlayerRace = MpPlayerRaceType.Jagur;
		/// <summary>
		/// The jagur health announcement.
		/// </summary>
		[SerializeField]
		private AudioClip jagurHealthAnnouncement;
		/// <summary>
		/// The jagur rocket announcement.
		/// </summary>
		[SerializeField]
		private AudioClip jagurRocketAnnouncement;
		/// <summary>
		/// The monio health announcement.
		/// </summary>
		[SerializeField]
		private AudioClip monioHealthAnnouncement;
		/// <summary>
		/// The monio rocket announcement.
		/// </summary>
		[SerializeField]
		private AudioClip monioRocketAnnouncement;
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
		/// <summary>
		/// The clip to be announced.
		/// </summary>
		private AudioClip clipToBeAnnounced;
		/// <summary>
		/// The rocket text announcement.
		/// </summary>
		[SerializeField]
		private RectTransform rocketTextAnnouncement;
		/// <summary>
		/// The health boost text announcement.
		/// </summary>
		[SerializeField]
		private RectTransform healthBoostTextAnnouncement;
		/// <summary>
		/// Game has already announced rocket.
		/// </summary>
		private bool hasAlreadyAnnouncedRocket = false;
		/// <summary>
		/// Game has already announced health.
		/// </summary>
		private bool hasAlreadyAnnouncedHealth = false;

		/// <summary>
		/// Ability types.
		/// </summary>
		private enum AbilityType
		{
			HealthBoost, Rocket, None
		}
		/// <summary>
		/// The type of the ability.
		/// </summary>
		private AbilityType abilityType = AbilityType.None;
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
				multiplayerGame.reduceKillStreakBy (killStreakForHealthBoost);
				hasAlreadyAnnouncedHealth = false;
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
				hasAlreadyAnnouncedHealth = false;
				hasAlreadyAnnouncedRocket = false;

				rocketButton.image.color = Color.black;
				healthBoostButton.image.color = Color.black;
				cancelButton.gameObject.SetActive (false);
				abilityType = AbilityType.None;
			}
			else if(killStreak >= killStreakForHealthBoost)
			{
				if(SMMultiplayerGame.isGoldenPlayer)
				{
					canHaveHealthBoost = true;
					healthBoostButton.image.color = Color.white;
					if(!hasAlreadyAnnouncedHealth)
					{
						clipToBeAnnounced = announcementType (AbilityType.HealthBoost);
						abilityType = AbilityType.HealthBoost;
						StartCoroutine ("announcePlayerAbility");
					}
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
					if(!hasAlreadyAnnouncedRocket)
					{
						clipToBeAnnounced = announcementType (AbilityType.Rocket);
						abilityType = AbilityType.Rocket;
						StartCoroutine ("announcePlayerAbility");
					}
				}
				else
				{
					rocketButton.image.color = Color.yellow;
					// show panel that buy gold
				}
			}
			else
			{
				abilityType = AbilityType.None;
			}
		}

		/// <summary>
		/// Announces the player ability and waits to announce if player is missing or died.
		/// </summary>
		IEnumerator announcePlayerAbility()
		{
			yield return new WaitUntil (() => (multiplayerGame.localPlayer != null));
			AudioSource.PlayClipAtPoint (clipToBeAnnounced, multiplayerGame.localPlayer.transform.position);
			if(abilityType == AbilityType.HealthBoost)
			{
				hasAlreadyAnnouncedHealth = true;
				healthBoostTextAnnouncement.GetComponent<Animator> ().SetTrigger ("ShowAnnouncement");
			}
			else if(abilityType == AbilityType.Rocket)
			{
				hasAlreadyAnnouncedRocket = true;
				rocketTextAnnouncement.GetComponent<Animator> ().SetTrigger ("ShowAnnouncement");
			}
		}

		/// <summary>
		/// This function returns appropritae audio clip based on race and ability
		/// </summary>
		/// <returns>The announcement clip.</returns>
		/// <param name="abilityType">Ability type.</param>
		private AudioClip announcementType(AbilityType abilityType)
		{
			if(abilityType == AbilityType.HealthBoost)
			{
				if(localPlayerRace == MpPlayerRaceType.Jagur)
				{
					return jagurHealthAnnouncement;
				}
				else if(localPlayerRace == MpPlayerRaceType.Monio)
				{
					return monioHealthAnnouncement;
				}
			}
			else if(abilityType == AbilityType.Rocket)
			{
				if(localPlayerRace == MpPlayerRaceType.Jagur)
				{
					return jagurRocketAnnouncement;
				}
				else if(localPlayerRace == MpPlayerRaceType.Monio)
				{
					return monioRocketAnnouncement;
				}
			}

			return null;
		}

		/// <summary>
		/// This function gets called whenever there is a request for grenade launch
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="touch">Touch.</param>
		private void requestForRocketFire(object sender, Touch touch)
		{
			if(canLauchRocket)
			{
				canLauchRocket = false;
				onCancelButtonPressed ();
				multiplayerGame.reduceKillStreakBy (killStreakForRocket);
				hasAlreadyAnnouncedRocket = false;
				Debug.Log ("Rocket dropped KABOOOM!!!");
				// Instantiate rocket in mp
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


			cancelButton.gameObject.SetActive (false);
			rocketButton.image.color = Color.black;
			healthBoostButton.image.color = Color.black;

			touchManager.OnSingleGameTap += requestForRocketFire;
		}
	}

	/// <summary>
	/// Multiplayer race type.
	/// </summary>
	public enum MpPlayerRaceType
	{
		Jagur, Monio
	}
}
