//
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

		// Use this for initialization
		void Start () 
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
	}

	/// <summary>
	/// This class will make the make collective UI elements to interact and change themselves based on availabity 
	/// of the product that these UI represent
	/// </summary>
	[System.Serializable]
	public class SMProductEquipUI
	{
		[SerializeField]
		protected Text levelToUnlockUI;
		[SerializeField]
		protected Button productEquipperButton;
		[SerializeField]
		protected Text productAvialabilityUI;

		/// <summary>
		/// Is product available
		/// </summary>
		protected ObscuredBool isProductAvailable = false;

		/// <summary>
		/// Sets the product availability.
		/// </summary>
		/// <param name="isAvailable">If set to <c>true</c> then product is available.</param>
		/// <param name="messageToShowProductAvailable">Message to show when product available.</param>
		/// <param name="messageToShowProductNotAvailable">Message to show when product not available.</param>
		/// <param name="levelWhenUnlocks">Level when unlocks.</param>
		public virtual void setProductAvailability(bool isAvailable, string messageToShowProductAvailable, string 
			messageToShowProductNotAvailable, string levelWhenUnlocks)
		{
			isProductAvailable = isAvailable;

			if(isProductAvailable)
			{
				if(productEquipperButton != null)
				{
					productAvialabilityUI.color = Color.white;
				}

				if(levelToUnlockUI != null)
				{
					levelToUnlockUI.gameObject.SetActive (false);
				}

				if(productAvialabilityUI != null)
				{
					productAvialabilityUI.text = messageToShowProductAvailable;
					productAvialabilityUI.color = Color.green;
				}
			}
			else
			{
				if(productEquipperButton != null)
				{
					productAvialabilityUI.color = Color.grey;
				}

				if(levelToUnlockUI != null)
				{
					levelToUnlockUI.gameObject.SetActive (true);
					levelToUnlockUI.color = Color.red;
				}

				if(productAvialabilityUI != null)
				{
					productAvialabilityUI.text = messageToShowProductNotAvailable;
					productAvialabilityUI.color = Color.green;
				}
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
	/// of the product that these UI represent. This is mainly used to represent Helmet UI
	/// </summary>
	public class SMHelmetEquipUI : SMProductEquipUI
	{
		[SerializeField]
		protected Button viewHelmetButton;

		/// <summary>
		/// Sets the product availability.
		/// </summary>
		/// <param name="isAvailable">If set to <c>true</c> then product is available.</param>
		/// <param name="messageToShowProductAvailable">Message to show when product available.</param>
		/// <param name="messageToShowProductNotAvailable">Message to show when product not available.</param>
		/// <param name="levelWhenUnlocks">Level when unlocks.</param>
		public override void setProductAvailability (bool isAvailable, string messageToShowProductAvailable,
			string messageToShowProductNotAvailable, string levelWhenUnlocks)
		{
			base.setProductAvailability (isAvailable, messageToShowProductAvailable, messageToShowProductNotAvailable, levelWhenUnlocks);

			if(levelToUnlockUI != null)
			{
				levelToUnlockUI.gameObject.SetActive (false);
			}
		}

		/// <summary>
		/// Sets the product availability.
		/// </summary>
		/// <param name="isAvailable">If set to <c>true</c> then product is available.</param>
		/// <param name="messageToShowProductAvailable">Message to show when product available.</param>
		/// <param name="messageToShowProductNotAvailable">Message to show when product not available.</param>
		public virtual void setProductAvailability (bool isAvailable, string messageToShowProductAvailable,
			string messageToShowProductNotAvailable)
		{
			setProductAvailability (isAvailable, messageToShowProductAvailable, messageToShowProductNotAvailable, "0");
		}

		/// <summary>
		/// This function sets the method which will be called when view button is pressed
		/// </summary>
		/// <param name="methodToBeCalled">Method to be called.</param>
		public virtual void setOnClickViewButtonMethod(UnityEngine.Events.UnityAction methodToBeCalled)
		{
			if(viewHelmetButton != null)
			{
				viewHelmetButton.onClick.AddListener (methodToBeCalled);
			}
		}
	}
}
