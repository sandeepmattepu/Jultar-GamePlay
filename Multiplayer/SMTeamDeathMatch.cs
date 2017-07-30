﻿//
//  SMTeamDeathMatch.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 03/04/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SandeepMattepu.Multiplayer
{
	/// <summary>
	/// This class will define the team death match game rules like number of players. Time taken for game session etc
	/// </summary>
	public class SMTeamDeathMatch : SMMultiplayerGame
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
			get { return playersIdAndScore; }
		}
		/// <summary>
		/// This dictionary holds player id as key and player's team index as value
		/// </summary>
		private static Dictionary<int,int> playerIdAndTeamIndex = new Dictionary<int, int>();
		/// <summary>
		/// This dictionary holds player's photon ID as key and player's team index as value
		/// </summary>
		public static Dictionary<int,int> PlayerIdAndTeamIndex
		{
			get{ return playerIdAndTeamIndex; }
		}
		/// <summary>
		/// The team index of the local player.
		/// </summary>
		private static int localPlayerTeamIndex = -1;
		/// <summary>
		/// The team index of the local player.
		/// </summary>
		public static int LocalPlayerTeamIndex
		{
			get{ return localPlayerTeamIndex; }
		}

		void Awake()
		{
			gameType = MPGameTypes.TEAM_DEATH_MATCH;
			instance = this;

			// When player is playing mp game back to back then to avoid bugs
			playersIdAndName.Clear();
            playersIdAndScore.Clear();
			playerIdAndTeamIndex.Clear ();
			playerIdAndDeaths.Clear ();
		}

		// Use this for initialization
		protected override void Start()
		{
			base.Start ();
			startDownloadingXpRewardData();
			splitPlayersIntoTeams ();
		}

		// Update is called once per frame
		void Update()
		{
			checkGameTime ();
		}

		/// <summary>
		/// This function downloads Xp reward data from server if there is any offer for players based on that day
		/// </summary>
		private void startDownloadingXpRewardData()
		{
			//TODO write www class code
		}

		/// <summary>
		/// This function splits the players into teams and assigns each player a team number
		/// </summary>
		private void splitPlayersIntoTeams()
		{
			List<PhotonPlayer> players = new List<PhotonPlayer> (PhotonNetwork.playerList);
			if(isTeamMatch)
			{
				players = sortPlayersInOrder (players);
				int teamIndex = 1;
				int tempTeamNumber = noOfTeams;

				while (players.Count > 0) 
				{
					int countInTeam = (players.Count / tempTeamNumber);
					while (countInTeam > 0) 
					{
						playerIdAndTeamIndex.Add (players [0].ID, teamIndex);
						players.RemoveAt (0);
						countInTeam--;
					}
					teamIndex++;
					tempTeamNumber--;
				}

				assignLocalPlayerHisTeamIndex ();
			}
			else
			{
				localPlayerTeamIndex = 1;
				foreach(PhotonPlayer player in players)
				{
					playerIdAndTeamIndex.Add(player.ID, 0);
				}
			}

			players = new List<PhotonPlayer> (PhotonNetwork.playerList);
			foreach(PhotonPlayer player in players)
			{
				playersIdAndName.Add (player.ID, player.NickName);
				playersIdAndScore.Add (player.ID, 0);
				playerIdAndDeaths.Add (player.ID, 0);
			}
		}

		/// <summary>
		/// This function sorts the Photon player list and assigns them in ascending order of their ids
		/// </summary>
		/// <returns>Players in order.</returns>
		/// <param name="players">UnSorted List.</param>
		private List<PhotonPlayer> sortPlayersInOrder(List<PhotonPlayer> players)
		{
			List<PhotonPlayer> orderedPlayers = new List<PhotonPlayer>();
			foreach(PhotonPlayer player in players)
			{
				if(orderedPlayers.Count != 0)
				{
					for(int i = 0; i < orderedPlayers.Count; i++)
					{
						if(player.ID < orderedPlayers[i].ID)
						{
							orderedPlayers.Insert (i, player);
							break;
						}
						else if(i == (orderedPlayers.Count - 1))
						{
							orderedPlayers.Add (player);
							break;
						}
					}
				}
				else
				{
					orderedPlayers.Add (player);
				}
			}
			return orderedPlayers;
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
		/// Assigns the team index of the local player
		/// </summary>
		private void assignLocalPlayerHisTeamIndex()
		{
			int localPlayerID = PhotonNetwork.player.ID;

			foreach(KeyValuePair<int,int> valuePair in playerIdAndTeamIndex)
			{
				if(localPlayerID == valuePair.Key)
				{
					localPlayerTeamIndex = valuePair.Value;
				}
			}
		}

		/// <summary>
		/// Update the score to the score board
		/// </summary>
		/// <param name="whoKilledID">Identifier who won the score.</param>
		public override void reportScore(int whoKilledID)
		{
			base.reportScore(whoKilledID);
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
	}
}