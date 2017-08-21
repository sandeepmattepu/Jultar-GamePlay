//
//  SMPlayerEquipper.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 18/08/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SandeepMattepu.Multiplayer;
using CodeStage.AntiCheat.ObscuredTypes;
using SandeepMattepu.Weapon;

namespace SandeepMattepu
{
	/// <summary>
	/// This class will analyze SMProductEquipper and assigns player with his abilities like helmets, perks etc
	/// </summary>
	public class SMPlayerEquipper : MonoBehaviour 
	{
		[SerializeField]
		private GameObject pilotarHelmet;
		[SerializeField]
		private GameObject operativeHelmet;
		[SerializeField]
		private GameObject brethorHelmet;
		[SerializeField]
		private GameObject tsTacticalHelmet;
		private SMPlayerHealth playerHealth;
		private Animator animator;
		[SerializeField]
		private ObscuredFloat breathorControllerSpeed = 1.05f;
		[SerializeField]
		private ObscuredFloat operativeControllerSpeed = 1.15f;
		[SerializeField]
		private ObscuredFloat pilotarControllerSpeed = 1.0f;
		[SerializeField]
		private ObscuredFloat tsTacticalControllerSpeed = 1.05f;
		private PhotonView photonViewComponent;
		[SerializeField]
		private ObscuredBool isUsingMultiplayer = false;
		private ObscuredInt currentEquippedHelmet = 0;
		/// <summary>
		/// Gets the current equipped helmet.
		/// </summary>
		/// <value>The int value of current equipped helmet.</value>
		public ObscuredInt CurrentEquippedHelmet {
			get {
				return currentEquippedHelmet;
			}
		}
		private SMPlayerFiring playerFiring;

		void Start () 
		{
			playerHealth = GetComponent<SMPlayerHealth> ();
			playerFiring = GetComponent<SMPlayerFiring> ();
			animator = GetComponent<Animator> ();
			photonViewComponent = GetComponent<PhotonView> ();

			assignPerks ((int)SMProductEquipper.INSTANCE.CurrentPerk);
			if(photonViewComponent != null && isUsingMultiplayer)
			{
				object[] dataFromInstantiation = photonViewComponent.instantiationData;
				if(dataFromInstantiation != null)
				{
					assignHelmet ((int)dataFromInstantiation[0]);
				}
			}
			else if(!isUsingMultiplayer)
			{
				assignHelmet ((int)(SMProductEquipper.INSTANCE.CurrentHelmet));
			}
		}

		/// <summary>
		/// Assigns the perks to the player.
		/// </summary>
		private void assignPerks(int perkType)
		{
			if(playerFiring != null && playerHealth != null)
			{
				ObscuredBool canAssignPerk = false;
				if(isUsingMultiplayer && photonViewComponent.isMine)
				{
					canAssignPerk = true;
				}
				else if(!isUsingMultiplayer)
				{
					canAssignPerk = true;
				}

				if(canAssignPerk)
				{
					Perks_Type perkTypeForPlayer = (Perks_Type)perkType;

					switch (perkTypeForPlayer) 
					{
					case Perks_Type.RUSHER:
						playerFiring.addGrenadeBombsToPlayerBy (1);
						break;
					case Perks_Type.STRATEGY:
						playerFiring.addMagazineToGunBy (1);
						break;
					case Perks_Type.TRY_HARD:
						playerHealth.assignHealthRegenerationRate (4.0f);
						break;
					case Perks_Type.THINKER:
						playerFiring.assignPlayerCollectBulletsFromDeadBody (true);
						break;
					}
				}
			}
		}

		/// <summary>
		/// This function assigns helmet to the player
		/// </summary>
		/// <param name="helmetTypeValue">Helmet type value.</param>
		private void assignHelmet(int helmetTypeValue)
		{
			currentEquippedHelmet = helmetTypeValue;
			Helmet_Type helmetType = ((Helmet_Type)helmetTypeValue);
			float speedOfAnimator = 1.0f;
			switch(helmetType)
			{
			case Helmet_Type.BREATHOR:
				brethorHelmet.SetActive (true);
				playerHealth.addPointsToMaxHealthBy (25.0f);
				playerHealth.setPlayerImmunityTowardsGasBombs (true);
				speedOfAnimator = breathorControllerSpeed;
				break;
			case Helmet_Type.OPERATIVE:
				operativeHelmet.SetActive (true);
				playerHealth.addPointsToMaxHealthBy (10.0f);
				speedOfAnimator = operativeControllerSpeed;
				break;
			case Helmet_Type.PILOTAR:
				pilotarHelmet.SetActive (true);
				playerHealth.addPointsToMaxHealthBy (20.0f);
				playerFiring.setGrenadeOuterDamageValue (0.55f);
				speedOfAnimator = pilotarControllerSpeed;
				break;
			case Helmet_Type.TS_TACTICAL:
				tsTacticalHelmet.SetActive (true);
				playerHealth.addPointsToMaxHealthBy (10.0f);
				speedOfAnimator = tsTacticalControllerSpeed;
				break;
			}

			if(animator != null)
			{
				animator.speed = speedOfAnimator;
			}
		}

		/// <summary>
		/// Assigns the helmet to dead body.
		/// </summary>
		/// <param name="helmetType">Helmet type.</param>
		public void assignHelmetToDeadBody(int helmetType)
		{
			assignHelmet (helmetType);
		}
	}
}
