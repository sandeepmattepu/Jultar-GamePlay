//
//  SMGameTimer.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 30/07/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SandeepMattepu.Multiplayer;
using UnityEngine.UI;

namespace SandeepMattepu.UI
{
	/// <summary>
	/// This class takes the timer data from multiplayer rules and tick the time in the game
	/// </summary>
	public class SMGameTimer : MonoBehaviour 
	{
		[SerializeField]
		private Text gameTimeText;
		/// <summary>
		/// The total amount of game time.
		/// </summary>
		private int totalGameTime = 0;
		/// <summary>
		/// The time left in the game.
		/// </summary>
		private int timeLeftInGame = 0;
		/// <summary>
		/// This becomes true when the Multiplayer rules instance get created.
		/// </summary>
		private bool hasRevievedData = false;
		/// <summary>
		/// The multiplayer game rules that created in the scene.
		/// </summary>
		private SMMultiplayerGame multiplayerGame;
		private int minutes;
		private int seconds;
		// Use this for initialization
		void Start () 
		{
			SMMultiplayerGame.OnGameRulesLoaded += onMultiplayerRulesCreated;
		}

		void OnDestroy()
		{
			SMMultiplayerGame.OnGameRulesLoaded -= onMultiplayerRulesCreated;
		}
		// Update is called once per frame
		void Update () 
		{
			updateTimerUI ();
		}

		/// <summary>
		/// This function updates timer UI for the game
		/// </summary>
		private void updateTimerUI()
		{
			if(hasRevievedData)
			{
				timeLeftInGame = totalGameTime - (int)multiplayerGame.GameTimer;
				minutes = timeLeftInGame / 60;
				seconds = timeLeftInGame % 60;
				gameTimeText.text = "Time : " + minutes + ":" + seconds;
			}
		}

		/// <summary>
		/// This function gets called when the multiplayer instance is created
		/// </summary>
		private void onMultiplayerRulesCreated()
		{
			hasRevievedData = true;
			totalGameTime = (int)SMMultiplayerGame.INSTANCE.gameSessionTime;
			timeLeftInGame = (int)SMMultiplayerGame.INSTANCE.GameTimer;
			multiplayerGame = SMMultiplayerGame.INSTANCE;
		}
	}	
}
