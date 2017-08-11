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
}
