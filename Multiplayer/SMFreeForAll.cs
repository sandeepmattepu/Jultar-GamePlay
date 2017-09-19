//
//  SMFreeForAll.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 02/04/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

namespace SandeepMattepu.Multiplayer
{
	/// <summary>
	/// This class will define the free for all game rules like number of players. Time taken for game session etc
	/// </summary>
	public class SMFreeForAll : SMMultiplayerGame
	{
		/// <summary>
		/// Is the local player leading
		/// </summary>
		private static ObscuredBool isLocalPlayerLeading = false;

		public static ObscuredBool IsLocalPlayerLeading {
			get {
				return isLocalPlayerLeading;
			}
		}

		// Use this for initialization

		void Awake()
		{
			gameType = MPGameTypes.FREE_FOR_ALL;
			instance = this;

			// When player is playing mp game back to back then to avoid bugs
			playersIdAndName.Clear();
			playersIdAndScore.Clear();
			playerIdAndDeaths.Clear ();
			isLocalPlayerLeading = false;
		}

		protected override void Start()
		{
			base.Start ();
			if(PhotonNetwork.isMasterClient)
			{
				registerPlayersForScoreBoard ();
			}
			startDownloadingXpRewardData();
		}

		/// <summary>
		/// Update the score to the score board
		/// </summary>
		/// <param name="whoKilledID">Identifier who won the score.</param>
		/// <param name="whoDiedID">Identifier who died.</param>
		public override void reportScore(int whoKilledID, int whoDiedID)
		{
			if (playersIdAndScore.ContainsKey(whoKilledID) && !IsGameOver)
			{
				ObscuredInt score = 0;
				playersIdAndScore.TryGetValue(whoKilledID, out score);
				score ++;
				playersIdAndScore.Remove(whoKilledID);
				playersIdAndScore.Add(whoKilledID, score);
				checkEndGameScore(score);
				if(whoKilledID == PhotonNetwork.player.ID)
				{
					checkIfLocalPlayerIsLeading ();
				}
			}
			base.reportScore(whoKilledID, whoDiedID);
		}

		/// <summary>
		/// Registers the player for score board.
		/// </summary>
		/// <param name="photonView">Photon view of the player.</param>
		public void registerPlayersForScoreBoard()
		{
			foreach(PhotonPlayer player in PhotonNetwork.playerList)
			{
				playersIdAndName.Add (player.ID, player.NickName);
				playersIdAndScore.Add (player.ID, 0);
				playerIdAndDeaths.Add (player.ID, 0);
			}
			base.raiseOnPlayerJoinedOrLeftEvent ();
		}

		protected override void checkGameTime ()
		{
			base.checkGameTime ();
			if (gameTimer >= gameSessionTime) 
			{
				checkIfLocalPlayerIsLeading ();
			}
		}

		/// <summary>
		/// This function will check number of kills made by single person to the maximum number of kills
		/// </summary>
		private void checkEndGameScore(int score)
		{
			if (hasKillsLimit)
			{
				if (score >= maxKillsToEndGame)
				{
					checkIfLocalPlayerIsLeading ();
					gameOver ();
				}
			}
		}

		/// <summary>
		/// This function downloads Xp reward data from server if there is any offer for players based on that day
		/// </summary>
		private void startDownloadingXpRewardData()
		{
			//TODO write www class code
		}

		/// <summary>
		/// This function checks if local player is leading.
		/// </summary>
		private void checkIfLocalPlayerIsLeading()
		{
			ObscuredInt scoreMadeByLocalPlayer;
			if(PlayersIdAndScore.TryGetValue (PhotonNetwork.player.ID, out scoreMadeByLocalPlayer))
			{
				foreach(KeyValuePair<int,ObscuredInt> kvp in playersIdAndScore)
				{
					if(kvp.Value > scoreMadeByLocalPlayer)
					{
						isLocalPlayerLeading = false;
						return;
					}
				}
				isLocalPlayerLeading = true;
			}
		}
	}
}
