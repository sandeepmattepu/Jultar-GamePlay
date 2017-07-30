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

namespace SandeepMattepu.Multiplayer
{
	/// <summary>
	/// This class will define the free for all game rules like number of players. Time taken for game session etc
	/// </summary>
	public class SMFreeForAll : SMMultiplayerGame
	{
		/// <summary>
		/// This dictionary holds player's score as value and player's Id as key
		/// </summary>
		private static Dictionary<int, int> playersIdAndScore = new Dictionary<int, int>();
		/// <summary>
		/// This dictionary holds player's score as value and player's Id as key
		/// </summary>
		public static Dictionary<int, int> PlayersIdAndScore
		{
			get { return playersIdAndScore;	}
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
		}

		protected override void Start()
		{
			base.Start ();
			startDownloadingXpRewardData();
		}

		// Update is called once per frame
		void Update()
		{
			checkGameTime();
		}

		/// <summary>
		/// Update the score to the score board
		/// </summary>
		/// <param name="whoKilledID">Identifier who won the score.</param>
		/// <param name="whoDiedID">Identifier who died.</param>
		public override void reportScore(int whoKilledID, int whoDiedID)
		{
			base.reportScore(whoKilledID, whoDiedID);
			if (playersIdAndScore.ContainsKey(whoKilledID))
			{
				int score = 0;
				playersIdAndScore.TryGetValue(whoKilledID, out score);
				score ++;
				playersIdAndScore.Remove(whoKilledID);
				playersIdAndScore.Add(whoKilledID, score);
				checkEndGameScore(score);
			}
		}

		/// <summary>
		/// Registers the player for score board.
		/// </summary>
		/// <param name="photonView">Photon view of the player.</param>
		public void registerPlayerForScoreBoard(PhotonView photonView)
		{
			if (!playersIdAndName.ContainsKey(photonView.owner.ID))
			{
				playersIdAndName.Add(photonView.owner.ID, photonView.owner.NickName);
				playersIdAndScore.Add(photonView.owner.ID, 0);
				playerIdAndDeaths.Add (photonView.owner.ID, 0);
			}
		}

		/// <summary>
		/// This function checs wether the time of the game has reached max time of the game
		/// </summary>
		private void checkGameTime()
		{
			if (gameTimer >= gameSessionTime)
			{
				//localPlayer.sendEndGameMessage(MPGameTypes.FREE_FOR_ALL, sceneNameAfterEndOfGame);
				SceneManager.LoadScene(sceneNameAfterEndOfGame);
			}
			else
			{
				gameTimer += Time.deltaTime;
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
					//localPlayer.sendEndGameMessage(MPGameTypes.FREE_FOR_ALL, sceneNameAfterEndOfGame);
					SceneManager.LoadScene(sceneNameAfterEndOfGame);
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
	}
}
