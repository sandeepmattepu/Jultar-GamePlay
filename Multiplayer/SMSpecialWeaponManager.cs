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
using SandeepMattepu.Weapon;

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
		private AudioClip jagurHealthUnlockedClip;
		/// <summary>
		/// The jagur rocket announcement.
		/// </summary>
		[SerializeField]
		private AudioClip jagurRocketUnlockedClip;
		/// <summary>
		/// The monio health announcement.
		/// </summary>
		[SerializeField]
		private AudioClip monioHealthUnlockedClip;
		/// <summary>
		/// The monio rocket announcement.
		/// </summary>
		[SerializeField]
		private AudioClip monioRocketUnlockedClip;
		[SerializeField]
		/// <summary>
		/// The friendly health activated sound.
		/// </summary>
		private AudioClip friendlyHealthUsedClip;
		/// <summary>
		/// The friendly rocket activated sound.
		/// </summary>
		[SerializeField]
		private AudioClip friendlyRocketUsedClip;
		/// <summary>
		/// The enemy health activated audio clip.
		/// </summary>
		[SerializeField]
		private AudioClip enemyHealthUsedClip;
		/// <summary>
		/// The enemy rocket firing incoming clip.
		/// </summary>
		[SerializeField]
		private AudioClip enemyRocketUsedClip;
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
		[SerializeField]
		/// <summary>
		/// This audio component playes rocket fly over sound
		/// </summary>
		private AudioSource jetFlyOverAudio;
		/// <summary>
		/// The rocket prefab that gets launched.
		/// </summary>
		[SerializeField]
		private GameObject rocketPrefab;
		/// <summary>
		/// The flare game object for rocket target
		/// </summary>
		[SerializeField]
		private GameObject flareForRocketTarget;
		/// <summary>
		/// The current abilities that are unlocked for the player.
		/// </summary>
		private List<AbilityType> currentSpecialAbilities = new List<AbilityType>();
		/// <summary>
		/// The current abilities that are unlocked for the player.
		/// </summary>
		public List<AbilityType> CurrentSpecialAbilities {
			get {
				return currentSpecialAbilities;
			}
		}
		/// <summary>
		/// Becomes true when rocket is in aiming position
		/// </summary>
		private bool rocketIsInAimingPosition = false;

		/// <summary>
		/// Ability types.
		/// </summary>
		public enum AbilityType
		{
			None, HealthBoost, Rocket
		}
		private AbilityType abilityType = AbilityType.None;

		void Start()
		{
			currentSpecialAbilities.Clear ();
			rocketButton.interactable = false;
			healthBoostButton.interactable = false;
		}

		void Update()
		{
			if(Input.GetMouseButtonDown(0))
			{
				requestForRocketFire (this, new Touch ());
			}
		}

		void OnDestroy()
		{
			multiplayerGame.OnKillStreakChange -= onKillStreakHandler;
			PhotonNetwork.OnEventCall -= recieveAnnouncementsFromOtherClients;
			touchManager.OnSingleGameTap -= requestForRocketFire;
		}

		/// <summary>
		/// Adds the special abilities to the player.
		/// </summary>
		/// <param name="ability">Ability.</param>
		private void addSpecialAbilities(AbilityType ability)
		{
			if(!currentSpecialAbilities.Contains (ability))
			{
				currentSpecialAbilities.Add (ability);
			}
		}

		/// <summary>
		/// Removes the special abilities from the player.
		/// </summary>
		/// <param name="ability">Ability.</param>
		private void removeSpecialAbilities(AbilityType ability)
		{
			if(currentSpecialAbilities.Contains (ability))
			{
				currentSpecialAbilities.Remove (ability);
			}
		}

		/// <summary>
		/// This function gets called when rocket button is pressed
		/// </summary>
		public void onRocketButtonPressed()
		{
			if(currentSpecialAbilities.Contains(AbilityType.Rocket))
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
			if(currentSpecialAbilities.Contains(AbilityType.HealthBoost))
			{
				multiplayerGame.localPlayer.gameObject.GetComponent<SMPlayerHealth> ().giveArmorToPlayer ();
				healthBoostButton.interactable = false;
				removeSpecialAbilities (AbilityType.HealthBoost);
				healthBoostButton.GetComponent<Image> ().color = Color.black;
				if(multiplayerGame.localPlayer != null)
				{
					AudioSource.PlayClipAtPoint (friendlyHealthUsedClip, multiplayerGame.localPlayer.transform.position);
					PhotonNetwork.RaiseEvent ((byte)MultiplayerEvents.Announcements, (object)AbilityType.HealthBoost, true, null);
				}
			}
		}

		/// <summary>
		/// This function gets called whenever there is kill streak change
		/// </summary>
		/// <param name="killStreak">Current kill streak.</param>
		private void onKillStreakHandler(int killStreak)
		{
			if (killStreak == 0) 
			{
				rocketIsInAimingPosition = false;
				rocketButton.gameObject.SetActive (true);
				healthBoostButton.gameObject.SetActive (true);
				cancelButton.gameObject.SetActive (false);
			}
			else if (killStreak == killStreakForRocket && !currentSpecialAbilities.Contains(AbilityType.Rocket)) 
			{
				addSpecialAbilities (AbilityType.Rocket);
				rocketButton.interactable = true;
				clipToBeAnnounced = audioForUnlockedAbility (AbilityType.Rocket);
				abilityType = AbilityType.Rocket;
				StartCoroutine ("announcePlayerAbility");
				rocketButton.GetComponent<Image> ().color = Color.white;
			}
			else if (killStreak == killStreakForHealthBoost && !currentSpecialAbilities.Contains(AbilityType.HealthBoost)) 
			{
				addSpecialAbilities (AbilityType.HealthBoost);
				healthBoostButton.interactable = true;
				abilityType = AbilityType.HealthBoost;
				clipToBeAnnounced = audioForUnlockedAbility (AbilityType.HealthBoost);
				StartCoroutine ("announcePlayerAbility");
				healthBoostButton.GetComponent<Image> ().color = Color.white;
			} 
		}

		/// <summary>
		/// Announces the player ability and waits to announce if player is missing or died.
		/// </summary>
		IEnumerator announcePlayerAbility()
		{
			yield return new WaitUntil (() => (multiplayerGame.localPlayer != null));
			Transform locationToMakeAnnouncement = multiplayerGame.localPlayer.transform;
			AudioSource.PlayClipAtPoint (clipToBeAnnounced, locationToMakeAnnouncement.position);
			if(abilityType == AbilityType.HealthBoost)
			{
				healthBoostTextAnnouncement.GetComponent<Animator> ().SetTrigger ("ShowAnnouncement");
			}
			else if(abilityType == AbilityType.Rocket)
			{
				rocketTextAnnouncement.GetComponent<Animator> ().SetTrigger ("ShowAnnouncement");
			}
		}

		/// <summary>
		/// This function returns appropritae audio clip based on race and ability
		/// </summary>
		/// <returns>The announcement clip.</returns>
		/// <param name="abilityType">Ability type.</param>
		private AudioClip audioForUnlockedAbility(AbilityType abilityType)
		{
			if(abilityType == AbilityType.HealthBoost)
			{
				if(localPlayerRace == MpPlayerRaceType.Jagur)
				{
					return jagurHealthUnlockedClip;
				}
				else if(localPlayerRace == MpPlayerRaceType.Monio)
				{
					return monioHealthUnlockedClip;
				}
			}
			else if(abilityType == AbilityType.Rocket)
			{
				if(localPlayerRace == MpPlayerRaceType.Jagur)
				{
					return jagurRocketUnlockedClip;
				}
				else if(localPlayerRace == MpPlayerRaceType.Monio)
				{
					return monioRocketUnlockedClip;
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
			if(rocketIsInAimingPosition)
			{
				Ray touchRay;
				if(Input.touchCount > 0)
				{
					touchRay = Camera.main.ScreenPointToRay (touch.position);
				}
				else if(Input.GetMouseButtonDown(0))
				{
					touchRay = Camera.main.ScreenPointToRay (Input.mousePosition);
				}
				RaycastHit hitInfo;
				int layerMask = 1 << 18;				// 18 is to ignore barriers collider
				layerMask = ~layerMask;
				if(Physics.Raycast(touchRay, out hitInfo, 500.0f, layerMask))
				{
					Vector3 target = hitInfo.point;
					onCancelButtonPressed ();
					removeSpecialAbilities(AbilityType.Rocket);
					rocketIsInAimingPosition = false;
					rocketButton.interactable = false;
					rocketButton.GetComponent<Image> ().color = Color.black;
					PhotonNetwork.RaiseEvent ((byte)MultiplayerEvents.Announcements, (object)AbilityType.Rocket, true, null);
					if(multiplayerGame.localPlayer != null)
					{
						AudioSource.PlayClipAtPoint (friendlyRocketUsedClip, multiplayerGame.localPlayer.transform.position);
					}
					if(jetFlyOverAudio != null)
					{
						if(jetFlyOverAudio.isPlaying)
						{
							jetFlyOverAudio.Stop ();
						}
						jetFlyOverAudio.Play ();
					}
					Vector3 rocketInstantiateLocation = target;
					rocketInstantiateLocation.y += 50.0f;
					object[] dataToTransfer = new object[1];
					dataToTransfer [0] = (object)target;
					GameObject rocketAfterInstantiating = PhotonNetwork.Instantiate (rocketPrefab.name, rocketInstantiateLocation, 
						rocketPrefab.transform.rotation, 0, dataToTransfer) as GameObject;
					rocketAfterInstantiating.GetComponent<SMRocketBehaviour> ().targetPosition = target;
					PhotonNetwork.Instantiate (flareForRocketTarget.name, target, flareForRocketTarget.transform.rotation, 0);
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


			cancelButton.gameObject.SetActive (false);
			rocketButton.image.color = Color.black;
			healthBoostButton.image.color = Color.black;

			touchManager.OnSingleGameTap += requestForRocketFire;
			PhotonNetwork.OnEventCall += recieveAnnouncementsFromOtherClients;
		}

		/// <summary>
		/// Recieves the signals from other clients to do announcements.
		/// </summary>
		/// <param name="eventCode">Event code.</param>
		/// <param name="content">Content.</param>
		/// <param name="senderId">Sender identifier.</param>
		private void recieveAnnouncementsFromOtherClients(byte eventCode, object content, int senderId)
		{
			if(eventCode == (int)MultiplayerEvents.Announcements)
			{
				SMPlayerIdentifier clientPlayer = multiplayerGame.localPlayer;
				AbilityType ability = (AbilityType)content;
				if(clientPlayer != null)
				{
					if(SMMultiplayerGame.gameType == MPGameTypes.FREE_FOR_ALL)
					{
						if(ability == AbilityType.HealthBoost)
						{
							AudioSource.PlayClipAtPoint (enemyHealthUsedClip, clientPlayer.transform.position);
						}
						else if(ability == AbilityType.Rocket)
						{
							AudioSource.PlayClipAtPoint (enemyRocketUsedClip, clientPlayer.transform.position);
							if(jetFlyOverAudio != null)
							{
								if(jetFlyOverAudio.isPlaying)
								{
									jetFlyOverAudio.Stop ();
								}
								jetFlyOverAudio.Play ();
							}
						}
					}
					else if(SMMultiplayerGame.gameType == MPGameTypes.TEAM_DEATH_MATCH)
					{
						int senderTeamIndex = 0;
						if(SMTeamDeathMatch.PlayerIdAndTeamIndex.TryGetValue (senderId, out senderTeamIndex))
						{
							if(senderTeamIndex == SMTeamDeathMatch.LocalPlayerTeamIndex)
							{
								if(ability == AbilityType.HealthBoost)
								{
									AudioSource.PlayClipAtPoint (friendlyHealthUsedClip, clientPlayer.transform.position);
								}
								else if(ability == AbilityType.Rocket)
								{
									AudioSource.PlayClipAtPoint (friendlyRocketUsedClip, clientPlayer.transform.position);
									if(jetFlyOverAudio != null)
									{
										if(jetFlyOverAudio.isPlaying)
										{
											jetFlyOverAudio.Stop ();
										}
										jetFlyOverAudio.Play ();
									}
								}
							}
							else
							{
								if(ability == AbilityType.HealthBoost)
								{
									AudioSource.PlayClipAtPoint (enemyHealthUsedClip, clientPlayer.transform.position);
								}
								else if(ability == AbilityType.Rocket)
								{
									AudioSource.PlayClipAtPoint (enemyRocketUsedClip, clientPlayer.transform.position);
									if(jetFlyOverAudio != null)
									{
										if(jetFlyOverAudio.isPlaying)
										{
											jetFlyOverAudio.Stop ();
										}
										jetFlyOverAudio.Play ();
									}
								}
							}
						}
					}
				}
			}
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
