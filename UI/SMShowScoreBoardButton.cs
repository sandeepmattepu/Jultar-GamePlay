//
//  SMShowScoreBoardButton.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 30/07/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SandeepMattepu.Multiplayer;

namespace SandeepMattepu.UI
{
	[RequireComponent(typeof(Button))]
	/// <summary>
	/// This class handles the functionality of show scoreboard ui button
	/// </summary>
	public class SMShowScoreBoardButton : MonoBehaviour 
	{
		/// <summary>
		/// The show score board button.
		/// </summary>
		private Button showScoreBoardButton;
		/// <summary>
		/// When true it shows button in FFA
		/// </summary>
		public bool showButtonInFFA = false;
		/// <summary>
		/// When true it shows button in TDM
		/// </summary>
		public bool showButtonInTDM = true;

		private void Start () 
		{
			showScoreBoardButton = GetComponent<Button> ();
			showOrHideButton ();
		}

		/// <summary>
		/// This function shows or hide button.
		/// </summary>
		private void showOrHideButton()
		{
			switch(SMMultiplayerGame.gameType)
			{
			case MPGameTypes.FREE_FOR_ALL:
				showScoreBoardButton.gameObject.SetActive (showButtonInFFA);
				break;
			case MPGameTypes.TEAM_DEATH_MATCH:
				showScoreBoardButton.gameObject.SetActive (showButtonInTDM);
				break;
			}
		}
	}
}
