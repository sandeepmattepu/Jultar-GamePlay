﻿//
//  SMPlayerCustomization.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 11/08/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SandeepMattepu.Multiplayer;
using UnityEngine.UI;
using CodeStage.AntiCheat.ObscuredTypes;
using SandeepMattepu.Android;

namespace SandeepMattepu.UI
{
	/// <summary>
	/// This class will help player to customize the look and feel of the player
	/// </summary>
	public class SMPlayerCustomization : MonoBehaviour 
	{
		/// <summary>
		/// The monio race game object.
		/// </summary>
		[SerializeField]
		private GameObject monioGameObject;
		/// <summary>
		/// The jagur race game object.
		/// </summary>
		[SerializeField]
		private GameObject jagurGameObject;
		/// <summary>
		/// The current race that is shown in lobby scene.
		/// </summary>
		private MpPlayerRaceType currentRace;

		/// <summary>
		/// The monio operative helmet.
		/// </summary>
		[SerializeField]
		private GameObject monioOperativeHelmet;
		/// <summary>
		/// The jagur operative helemt.
		/// </summary>
		[SerializeField]
		private GameObject jagurOperativeHelemt;

		/// <summary>
		/// The monio pilotar helmet.
		/// </summary>
		[SerializeField]
		private GameObject monioPilotarHelmet;
		/// <summary>
		/// The jagur pilotar helmet.
		/// </summary>
		[SerializeField]
		private GameObject jagurPilotarHelmet;

		/// <summary>
		/// The monio tactical helmet.
		/// </summary>
		[SerializeField]
		private GameObject monioTacticalHelmet;
		/// <summary>
		/// The jagur tactical helmet.
		/// </summary>
		[SerializeField]
		private GameObject jagurTacticalHelmet;

		/// <summary>
		/// The monio breathore helemt.
		/// </summary>
		[SerializeField]
		private GameObject monioBreathoreHelemt;
		/// <summary>
		/// The jagur breathore helemt.
		/// </summary>
		[SerializeField]
		private GameObject jagurBreathoreHelemt;

		#region Gun UI

		/// <summary>
		/// The raini user interface set.
		/// </summary>
		[SerializeField]
		private SMWeaponEquipUI rainiUISet;
		/// <summary>
		/// The smilere user interface set.
		/// </summary>
		[SerializeField]
		private SMWeaponEquipUI smilereUISet;
		/// <summary>
		/// The sniper user interface set.
		/// </summary>
		[SerializeField]
		private SMWeaponEquipUI sniperUISet;
		/// <summary>
		/// The green eye user interface set.
		/// </summary>
		[SerializeField]
		private SMWeaponEquipUI greenEyeUISet;
		[SerializeField]
		private SMWeaponEquipUI blackDogUISet;
		[SerializeField]
		private SMWeaponEquipUI july11UISet;
		[SerializeField]
		private SMWeaponEquipUI blondeUISet;

		#endregion

		#region Perks UI

		[SerializeField]
		private SMPerksEquipUI rusherPerkUISet;
		[SerializeField]
		private SMPerksEquipUI strategyPerkUISet;
		[SerializeField]
		private SMPerksEquipUI egoPerkUISet;
		[SerializeField]
		private SMPerksEquipUI thinkerPerkUISet;
		[SerializeField]
		private SMPerksEquipUI tryHardPerkUISet;

		#endregion

		#region Helmet UI

		/// <summary>
		/// The ts tactical UI.
		/// </summary>
		[SerializeField]
		private SMHelmetEquipUI tsTacticalUISet;
		/// <summary>
		/// The brethore UI.
		/// </summary>
		[SerializeField]
		private SMHelmetEquipUI brethoreUISet;
		/// <summary>
		/// The operative UI.
		/// </summary>
		[SerializeField]
		private SMHelmetEquipUI operativeUISet;
		/// <summary>
		/// The pilotar UI.
		/// </summary>
		[SerializeField]
		private SMHelmetEquipUI pilotarUISet;

		#endregion

		#region Laser UI

		/// <summary>
		/// The red laser user interface set.
		/// </summary>
		[SerializeField]
		private SMLaserEquipUI redLaserUISet;
		/// <summary>
		/// The blue laser user interface set.
		/// </summary>
		[SerializeField]
		private SMLaserEquipUI blueLaserUISet;
		/// <summary>
		/// The green laser user interface set.
		/// </summary>
		[SerializeField]
		private SMLaserEquipUI greenLaserUISet;

		#endregion

		// Use this for initialization
		void Start () 
		{
			helmetAndRaceViewing ();
			StartCoroutine ("loadDataToDisplay");
		}

		void Update()
		{
			if(Input.GetKeyDown(KeyCode.Space))
			{
				ObscuredPrefs.DeleteAll ();
			}
		}

		/// <summary>
		/// This function loads the data to be displayed on UI
		/// </summary>
		private IEnumerator loadDataToDisplay()
		{
			yield return new WaitUntil (() => SMPlayerDataManager.PlayerData != null);
			showUIBasedOnData ();
		}

		/// <summary>
		/// This function will help user to see how helmet looks like for different race. This function wont equip helmet in game
		/// </summary>
		/// <returns>The and race viewing.</returns>
		private void helmetAndRaceViewing()
		{
			currentRace = MpPlayerRaceType.Monio;
			jagurGameObject.SetActive (false);
			hideAllHelmets ();
		}

		/// <summary>
		/// This function gets called when switch race button is pressed
		/// </summary>
		public void switchRaceButtonPressed()
		{
			if(currentRace == MpPlayerRaceType.Jagur)
			{
				currentRace = MpPlayerRaceType.Monio;
				monioGameObject.SetActive (true);
				jagurGameObject.SetActive (false);
			}
			else if(currentRace == MpPlayerRaceType.Monio)
			{
				currentRace = MpPlayerRaceType.Jagur;
				monioGameObject.SetActive (false);
				jagurGameObject.SetActive (true);
			}
		}

		/// <summary>
		/// This function hides all helmets of monio and jagur race
		/// </summary>
		public void hideAllHelmets()
		{
			monioOperativeHelmet.SetActive (false);
			monioPilotarHelmet.SetActive (false);
			monioTacticalHelmet.SetActive (false);
			monioBreathoreHelemt.SetActive (false);

			jagurOperativeHelemt.SetActive (false);
			jagurPilotarHelmet.SetActive (false);
			jagurTacticalHelmet.SetActive (false);
			jagurBreathoreHelemt.SetActive (false);
		}

		/// <summary>
		/// This function shows all customization UI based on player data
		/// </summary>
		private void showUIBasedOnData()
		{
			showWeaponUIBasedOnData ();
			showPerksUIBasedOnData ();
			showHelmetUIBasedOnData ();
			showLaserUIBasedOnData ();
		}

		/// <summary>
		/// This function shows weapon customization UI based on player data
		/// </summary>
		private void showWeaponUIBasedOnData()
		{
			rainiUISet.setProductAvailability (SMPlayerDataManager.PlayerData.gunsAndPerksData.rainiGunBought, false);
			smilereUISet.setProductAvailability (SMPlayerDataManager.PlayerData.gunsAndPerksData.smilerieGunBought, false);
			sniperUISet.setProductAvailability (SMPlayerDataManager.PlayerData.gunsAndPerksData.sniperGunBought, false);
			greenEyeUISet.setProductAvailability (SMPlayerDataManager.PlayerData.gunsAndPerksData.greenEyeGunBought, false);
			blackDogUISet.setProductAvailability (SMPlayerDataManager.PlayerData.gunsAndPerksData.leoBlackDogGunBought, false);
			july11UISet.setProductAvailability (SMPlayerDataManager.PlayerData.gunsAndPerksData.jully11Bought, false);
			blondeUISet.setProductAvailability (SMPlayerDataManager.PlayerData.gunsAndPerksData.blondeGunBought, false);

			GUN_TYPE gunTypeEquipped = (GUN_TYPE)((int)SMProductEquipper.INSTANCE.CurrentEquippedGunType);
			switch(gunTypeEquipped)
			{
			case GUN_TYPE.Blonde:
				blondeUISet.setProductAvailability (SMPlayerDataManager.PlayerData.gunsAndPerksData.blondeGunBought, true);
				break;
			case GUN_TYPE.GreenEye:
				greenEyeUISet.setProductAvailability (SMPlayerDataManager.PlayerData.gunsAndPerksData.greenEyeGunBought, true);
				break;
			case GUN_TYPE.July11:
				july11UISet.setProductAvailability (SMPlayerDataManager.PlayerData.gunsAndPerksData.jully11Bought, true);
				break;
			case GUN_TYPE.LeoBlackDog:
				blackDogUISet.setProductAvailability (SMPlayerDataManager.PlayerData.gunsAndPerksData.leoBlackDogGunBought, true);
				break;
			case GUN_TYPE.Raini:
				rainiUISet.setProductAvailability (SMPlayerDataManager.PlayerData.gunsAndPerksData.rainiGunBought, true);
				break;
			case GUN_TYPE.Smilere:
				smilereUISet.setProductAvailability (SMPlayerDataManager.PlayerData.gunsAndPerksData.smilerieGunBought, true);
				break;
			case GUN_TYPE.Sniper:
				sniperUISet.setProductAvailability (SMPlayerDataManager.PlayerData.gunsAndPerksData.sniperGunBought, true);
				break;
			}
		}

		/// <summary>
		/// This function shows Perks customization UI based on player data
		/// </summary>
		private void showPerksUIBasedOnData()
		{
			rusherPerkUISet.setProductAvailability (SMPlayerDataManager.PlayerData.gunsAndPerksData.rusherPerkBought, false);
			strategyPerkUISet.setProductAvailability (SMPlayerDataManager.PlayerData.gunsAndPerksData.strategyPerkBought, false);
			egoPerkUISet.setProductAvailability (SMPlayerDataManager.PlayerData.gunsAndPerksData.egoPerkBought, false);
			thinkerPerkUISet.setProductAvailability (SMPlayerDataManager.PlayerData.gunsAndPerksData.thinkerPerkBought, false);
			tryHardPerkUISet.setProductAvailability (SMPlayerDataManager.PlayerData.gunsAndPerksData.tryHardPerkBought, false);

			Perks_Type equippedPerk = (Perks_Type)((int)SMProductEquipper.INSTANCE.CurrentPerk);

			switch(equippedPerk)
			{
			case Perks_Type.EGO:
				egoPerkUISet.setProductAvailability (SMPlayerDataManager.PlayerData.gunsAndPerksData.egoPerkBought, true);
				break;
			case Perks_Type.RUSHER:
				rusherPerkUISet.setProductAvailability (SMPlayerDataManager.PlayerData.gunsAndPerksData.rusherPerkBought, true);
				break;
			case Perks_Type.STRATEGY:
				strategyPerkUISet.setProductAvailability (SMPlayerDataManager.PlayerData.gunsAndPerksData.strategyPerkBought, true);
				break;
			case Perks_Type.THINKER:
				thinkerPerkUISet.setProductAvailability (SMPlayerDataManager.PlayerData.gunsAndPerksData.thinkerPerkBought, true);
				break;
			case Perks_Type.TRY_HARD:
				tryHardPerkUISet.setProductAvailability (SMPlayerDataManager.PlayerData.gunsAndPerksData.tryHardPerkBought, true);
				break;
			}
		}

		/// <summary>
		/// This function shows Helmet customization UI based on player data
		/// </summary>
		private void showHelmetUIBasedOnData()
		{
			tsTacticalUISet.setProductAvailability (SMPlayerDataManager.PlayerPurchasedProducts.tsTacticalHelmetBought, false);
			brethoreUISet.setProductAvailability (SMPlayerDataManager.PlayerPurchasedProducts.breathoreHelmetBought, false);
			operativeUISet.setProductAvailability (SMPlayerDataManager.PlayerPurchasedProducts.operativeHelmetBought, false);
			pilotarUISet.setProductAvailability (SMPlayerDataManager.PlayerPurchasedProducts.pilotarHelmetBought, false);

			Helmet_Type helmetType = (Helmet_Type)((int)SMProductEquipper.INSTANCE.CurrentHelmet);
			switch(helmetType)
			{
			case Helmet_Type.BREATHOR:
				brethoreUISet.setProductAvailability (SMPlayerDataManager.PlayerPurchasedProducts.breathoreHelmetBought, true);
				break;
			case Helmet_Type.OPERATIVE:
				operativeUISet.setProductAvailability (SMPlayerDataManager.PlayerPurchasedProducts.operativeHelmetBought, true);
				break;
			case Helmet_Type.PILOTAR:
				pilotarUISet.setProductAvailability (SMPlayerDataManager.PlayerPurchasedProducts.pilotarHelmetBought, true);
				break;
			case Helmet_Type.TS_TACTICAL:
				tsTacticalUISet.setProductAvailability (SMPlayerDataManager.PlayerPurchasedProducts.tsTacticalHelmetBought, true);
				break;
			}
		}

		/// <summary>
		/// This function shows Laser customization UI based on player data
		/// </summary>
		private void showLaserUIBasedOnData()
		{
			blueLaserUISet.setProductAvailability (SMPlayerDataManager.PlayerPurchasedProducts.blueLaserBought, false);
			greenLaserUISet.setProductAvailability (SMPlayerDataManager.PlayerPurchasedProducts.greenLaserBought, false);
			redLaserUISet.setProductAvailability (SMPlayerDataManager.PlayerPurchasedProducts.redLaserBought, false);

			Laser_Type laserType = (Laser_Type)((int)SMProductEquipper.INSTANCE.CurrentLaserType);
			switch(laserType)
			{
			case Laser_Type.BLUE_LASER:
				blueLaserUISet.setProductAvailability (SMPlayerDataManager.PlayerPurchasedProducts.blueLaserBought, true);
				break;
			case Laser_Type.GREEN_LASER:
				greenLaserUISet.setProductAvailability (SMPlayerDataManager.PlayerPurchasedProducts.greenLaserBought, true);
				break;
			case Laser_Type.RED_LASER:
				redLaserUISet.setProductAvailability (SMPlayerDataManager.PlayerPurchasedProducts.redLaserBought, true);
				break;
			}
		}
	}

	/// <summary>
	/// This class will make the make collective UI elements to interact and change themselves based on availabity 
	/// of the product that these UI represent
	/// </summary>
	[System.Serializable]
	public class SMProductEquipUI
	{
		[SerializeField]
		protected Button productEquipperButton;
		[SerializeField]
		protected Color buttonColorWhenProductNotAvialable;
		[SerializeField]
		protected Color buttonColorProductAvailableNotEquipped;
		[SerializeField]
		protected Color buttonColorProductAvailableAndEquipped;

		/// <summary>
		/// Is product available
		/// </summary>
		protected ObscuredBool isProductAvailable = false;

		/// <summary>
		/// Sets the product availability and prepares entire UI.
		/// </summary>
		/// <param name="isAvailable">Is product available.</param>
		/// <param name="isEquipped">Is product equipped.</param>
		public virtual void setProductAvailability(ObscuredBool isAvailable, ObscuredBool isEquipped)
		{
			isProductAvailable = isAvailable;
			if(isAvailable)
			{
				ColorBlock buttonColorBlock = new ColorBlock ();
				buttonColorBlock.normalColor = isEquipped ? buttonColorProductAvailableAndEquipped :
					buttonColorProductAvailableNotEquipped;
				productEquipperButton.interactable = true;
				productEquipperButton.colors = buttonColorBlock;
			}
			else
			{
				productEquipperButton.interactable = false;
				ColorBlock buttonColorBlock = new ColorBlock ();
				buttonColorBlock.disabledColor = buttonColorBlock.normalColor = buttonColorWhenProductNotAvialable;
				productEquipperButton.colors = buttonColorBlock;
			}
		}

		/// <summary>
		/// This function sets the method which will be called when button is pressed
		/// </summary>
		/// <param name="methodToBeCalled">Method to be called.</param>
		public virtual void setOnClickProductEquipButtonMethod(UnityEngine.Events.UnityAction methodToBeCalled)
		{
			if(productEquipperButton != null)
			{
				productEquipperButton.onClick.AddListener (methodToBeCalled);
			}
		}
	}

	/// <summary>
	/// This class will make the make collective UI elements to interact and change themselves based on availabity 
	/// of the product that these UI represent. This is mainly used to represent Weapon UI
	/// </summary>
	[System.Serializable]
	public class SMWeaponEquipUI : SMProductEquipUI
	{
		[SerializeField]
		protected Text levelToUnlockUI;
		[SerializeField]
		protected Color levelToUnlockUIColor;

		[SerializeField]
		protected Text coinsOrAvailabilityUI;
		[SerializeField]
		protected Color coinsOrAvailabilityUIColor;

		[SerializeField]
		protected ObscuredInt levelToUnlockValue;
		[SerializeField]
		protected ObscuredInt coinsRequiredToUnlock;

		/// <summary>
		/// Sets the product availability and prepares entire UI.
		/// </summary>
		/// <param name="isAvailable">Is product available.</param>
		/// <param name="isEquipped">Is product equipped.</param>
		public override void setProductAvailability(ObscuredBool isAvailable, ObscuredBool isEquipped)
		{
			base.setProductAvailability (isAvailable, isEquipped);

			if(isAvailable)
			{
				levelToUnlockUI.gameObject.SetActive (false);

				coinsOrAvailabilityUI.text = "Available!";
				coinsOrAvailabilityUI.color = coinsOrAvailabilityUIColor;
			}
			else
			{
				levelToUnlockUI.gameObject.SetActive (true);
				levelToUnlockUI.text = "Lvl " + ((int)levelToUnlockValue).ToString ();

				coinsOrAvailabilityUI.text = ((int)coinsRequiredToUnlock).ToString() + "C";
				coinsOrAvailabilityUI.color = coinsOrAvailabilityUIColor;
			}
		}
	}

	/// <summary>
	/// This class will make the make collective UI elements to interact and change themselves based on availabity 
	/// of the product that these UI represent. This is mainly used to represent Perks UI
	/// </summary>
	[System.Serializable]
	public class SMPerksEquipUI : SMWeaponEquipUI
	{}

	/// <summary>
	/// This class will make the make collective UI elements to interact and change themselves based on availabity 
	/// of the product that these UI represent. This is mainly used to represent Helmet UI
	/// </summary>
	[System.Serializable]
	public class SMHelmetEquipUI : SMProductEquipUI
	{
		[SerializeField]
		protected Text buyInStoreOrAvailabilityUI;
		[SerializeField]
		protected Color buyInStoreOrAvailabilityUIColor;

		public override void setProductAvailability (ObscuredBool isAvailable, ObscuredBool isEquipped)
		{
			base.setProductAvailability (isAvailable, isEquipped);

			buyInStoreOrAvailabilityUI.color = buyInStoreOrAvailabilityUIColor;
			buyInStoreOrAvailabilityUI.text = isAvailable ? "Available!" : "Buy in store";
		}
	}

	/// <summary>
	/// This class will make the make collective UI elements to interact and change themselves based on availabity 
	/// of the product that these UI represent. This is mainly used to represent Laser UI
	/// </summary>
	[System.Serializable]
	public class SMLaserEquipUI : SMHelmetEquipUI
	{
		
	}
}
