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
using CodeStage.AntiCheat.ObscuredTypes;
using System;

namespace SandeepMattepu.Multiplayer
{
	/// <summary>
	/// This class will define the team death match game rules like number of players. Time taken for game session etc
	/// </summary>
	public class SMTeamDeathMatch : SMMultiplayerGame
	{
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
		/// This holds reference to data packets about all players in team
		/// </summary>
		private static List<PlayerInTeam> playersInTeam= new List<PlayerInTeam>();
		/// <summary>
		/// The team index of the local player.
		/// </summary>
		private static ObscuredInt localPlayerTeamIndex = -1;
		/// <summary>
		/// The team index of the local player.
		/// </summary>
		public static ObscuredInt LocalPlayerTeamIndex
		{
			get{ return localPlayerTeamIndex; }
		}
		/// <summary>
		/// This event is raised when players are splitted into individual teams
		/// </summary>
		public static event onGameRulesCreated OnPlayersSplittedToTeams;
		/// <summary>
		/// This says whether players has been splitted to teams
		/// </summary>
		private bool hasPlayerSplittedToTeams = false;
		/// <summary>
		/// This says whether players has been splitted to teams
		/// </summary>
		public bool HasPlayerSplittedToTeams {
			get {
				return hasPlayerSplittedToTeams;
			}
		}

		/// <summary>
		/// Is local player's team leading.
		/// </summary>
		private static ObscuredBool isLocalPlayerTeamLeading = false;

		public static ObscuredBool IsLocalPlayerTeamLeading {
			get {
				return isLocalPlayerTeamLeading;
			}
		}
		/// <summary>
		/// The score of team 1
		/// </summary>
		private static ObscuredInt team1Score;
		public static ObscuredInt Team1Score {
			get {
				return team1Score;
			}
		}
		/// <summary>
		/// The score of team 2
		/// </summary>
		private static ObscuredInt team2Score;
		public static ObscuredInt Team2Score {
			get {
				return team2Score;
			}
		}

		protected override void Awake()
		{
			base.Awake ();
			gameType = MPGameTypes.TEAM_DEATH_MATCH;
			instance = this;
			playersInTeam.Clear ();
			playerIdAndTeamIndex.Clear ();
			isLocalPlayerTeamLeading = false;
			team1Score = 0;
			team2Score = 0;
		}

		// Use this for initialization
		protected override void Start()
		{
			base.Start ();
			startDownloadingXpRewardData();
			splitPlayersIntoTeams ();
		}

		protected override void OnDestroy ()
		{
			base.OnDestroy ();
		}

		/// <summary>
		/// This function downloads Xp reward data from server if there is any offer for players based on that day
		/// </summary>
		private void startDownloadingXpRewardData()
		{
			//TODO write www class code
		}

		protected override void observeRoomForPlayerLeavingAndEntering ()
		{
			SMLeftOrJoinedPlayersData modifiedData = dataAboutPlayersLeftOrJoined ();

			if(modifiedData != null && PhotonNetwork.isMasterClient)
			{
				addExcessPlayersToTeam (modifiedData.PlayersIdsWhoLeftOrJoined, modifiedData.HasPlayerJoined);
				base.observeRoomForPlayerLeavingAndEntering ();
			}
		}

		/// <summary>
		/// Assigns entire score
		/// </summary>
		/// <param name="PlayersIDsAndScores">Players Ids and scores.</param>
		public static void assignEntirePlayerTeamToIDs(Dictionary<int,int> PlayersIDsAndTeamIndexes)
		{
			playerIdAndTeamIndex.Clear ();
			playersInTeam.Clear ();
			team1Score = 0;
			team2Score = 0;

			playerIdAndTeamIndex = PlayersIDsAndTeamIndexes;
			ObscuredInt tempScore = 0;

			foreach(KeyValuePair<int,int> piti in playerIdAndTeamIndex)
			{
				if(playersIdAndScore.TryGetValue(piti.Key, out tempScore))
				{
					playersInTeam.Add(new PlayerInTeam(piti.Key, tempScore, piti.Value));
					if(piti.Key == 1)
					{
						team1Score += tempScore;
					}
					else if(team2Score == 2)
					{
						team2Score += tempScore;
					}
				}

				if(piti.Key == PhotonNetwork.player.ID)
				{
					localPlayerTeamIndex = piti.Value;
				}
			}
			((SMTeamDeathMatch)INSTANCE).hasPlayerSplittedToTeams = true;
		}

		/// <summary>
		/// Adds the excess players to team.
		/// </summary>
		/// <param name="excessivePlayers">Excessive players IDs.</param>
		/// <param name="hasJoinedOrLeft">If set to <c>true</c> then players has joined or left.</param>
		private void addExcessPlayersToTeam(int[] excessivePlayers, bool hasJoinedOrLeft)
		{
			List<int> excesPlayers = new List<int> (excessivePlayers);
			excesPlayers.Sort ();

			while(excesPlayers.Count != 0)
			{
				if(hasJoinedOrLeft)
				{
					int playerInTeam1 = 0;
					int playersInTeam2 = 0;

					foreach(PlayerInTeam pit in playersInTeam)
					{
						if(pit.TeamID == 1)
						{
							playerInTeam1 += 1;
						}
						else if(pit.TeamID == 2)
						{
							playersInTeam2 += 1;
						}
					}

					// If player left this room and joined again this room which is low probability but still we will handle the case
					if(playerIdAndTeamIndex.ContainsKey(excesPlayers[0]))
					{
						playerIdAndTeamIndex.Remove (excesPlayers [0]);
						playersInTeam.Remove(new PlayerInTeam(excesPlayers[0], 0, 0));
					}

					if(playerInTeam1 > playersInTeam2)
					{
						playerIdAndTeamIndex.Add (excesPlayers [0], 2);
						playersInTeam.Add(new PlayerInTeam(excesPlayers[0], 0, 2));
						excesPlayers.Remove (excesPlayers [0]);
					}
					else if(playerInTeam1 <= playersInTeam2)
					{
						playerIdAndTeamIndex.Add (excesPlayers [0], 1);
						playersInTeam.Add(new PlayerInTeam(excesPlayers[0], 0, 1));
						excesPlayers.Remove (excesPlayers [0]);
					}
				}
				else
				{
					playerIdAndTeamIndex.Remove (excesPlayers [0]);
					for(int i = 0; i < playersInTeam.Count; i++)
					{
						if(playersInTeam[i].ID == excesPlayers [0])
						{
							playersInTeam.RemoveAt (i);
							break;
						}
					}
					excesPlayers.Remove (excesPlayers [0]);
				}
			}
		}

		/// <summary>
		/// This function splits the players into teams and assigns each player a team number. First players are sorted 
		/// according to their ID number in ascending order and all players are seperated from the srted list
		/// </summary>
		//[System.Obsolete("Don't use this function. Player's team info should be recieved from Room scene")]
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
						playersInTeam.Add(new PlayerInTeam(players[0].ID, 0, teamIndex));
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
					playersInTeam.Add(new PlayerInTeam(players[0].ID, 0, 1));
				}
			}
			hasPlayerSplittedToTeams = true;
			if(OnPlayersSplittedToTeams != null)
			{
				OnPlayersSplittedToTeams ();
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

		protected override void checkGameTime ()
		{
			base.checkGameTime ();

			if (gameTimer >= gameSessionTime) 
			{
				checkIfPlayerTeamIsLeading ();
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
				foreach(PlayerInTeam pit in playersInTeam)
				{
					if(pit.ID == whoKilledID)
					{
						pit.Score = score;
						if(pit.TeamID == 1)
						{
							team1Score += 1;
						}
						else if(pit.TeamID == 2)
						{
							team2Score += 1;
						}
						break;
					}
				}
				checkIfPlayerTeamIsLeading ();
				base.reportScore(whoKilledID, whoDiedID);
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
				if (team1Score >= maxKillsToEndGame || team2Score >= maxKillsToEndGame)
				{
					checkIfPlayerTeamIsLeading ();
					gameOver ();
				}
			}
		}

		/// <summary>
		/// This function checks if player team is leading.
		/// </summary>
		private void checkIfPlayerTeamIsLeading()
		{
			int scoreMadeByPlayerTeam = 0;
			int scoreMadeByEnemyTeam = 0;

			foreach(PlayerInTeam pit in playersInTeam)
			{
				if(LocalPlayerTeamIndex == pit.TeamID)
				{
					scoreMadeByPlayerTeam += pit.Score;
				}
				else
				{
					scoreMadeByEnemyTeam += pit.Score;
				}
			}

			if(scoreMadeByPlayerTeam >= scoreMadeByEnemyTeam)
			{
				isLocalPlayerTeamLeading = true;
				return;
			}
			isLocalPlayerTeamLeading = false;
		}

		private class PlayerInTeam : IEquatable<PlayerInTeam>
		{
			public int ID;
			public int Score;
			public int TeamID;

			public PlayerInTeam()
			{}

			public PlayerInTeam(int id, int score, int teamID)
			{
				ID = id;
				Score = score;
				TeamID = teamID;
			}

			public PlayerInTeam(PlayerInTeam playerInTeam)
			{
				ID = playerInTeam.ID;
				Score = playerInTeam.Score;
				TeamID = playerInTeam.TeamID;
			}

			#region IEquatable implementation

			public bool Equals (PlayerInTeam other)
			{
				if (other == null)
					return false;
				return (this.ID == other.ID);
			}

			#endregion

			public override bool Equals (object obj)
			{
				if(obj == null)
				{
					return false;
				}
				else
				{
					PlayerInTeam player = (PlayerInTeam)obj;
					return (player.ID == this.ID);
				}
			}

			public override int GetHashCode ()
			{
				return ID;
			}

			public static bool operator ==(PlayerInTeam a, PlayerInTeam b)
			{
				if(System.Object.ReferenceEquals(a,b))
				{
					return true;
				}
				else if(a == null || b == null)
				{
					return false;
				}

				return (a.ID == b.ID);
			}

			public static bool operator !=(PlayerInTeam a, PlayerInTeam b)
			{
				return !(a == b);
			}
		}

		/// <summary>
		/// Set players into teams. 
		/// WARNING :- Call it only in Team death match context.
		/// </summary>
		/// <param name="team1">Team1 players.</param>
		/// <param name="team2">Team2 players.</param>
		public static void setPlayersTeamInfo(PhotonPlayer[] team1, PhotonPlayer[] team2)
		{
			List<PhotonPlayer> Team1 = new List<PhotonPlayer> (team1);
			List<PhotonPlayer> Team2 = new List<PhotonPlayer> (team2);
			setPlayersTeamInfo (Team1, Team2);
		}

		/// <summary>
		/// Set players into teams.
		/// WARNING :- Call it only in Team death match context.
		/// </summary>
		/// <param name="team1">Team1 players.</param>
		/// <param name="team2">Team2 players.</param>
		public static void setPlayersTeamInfo(List<PhotonPlayer> team1, List<PhotonPlayer> team2)
		{
			if(PhotonNetwork.isMasterClient)
			{
				
			}
		}

		/// <summary>
		/// Set players into teams.
		/// WARNING :- Call it only in Team death match context.
		/// </summary>
		/// <param name="team1">ID of Team1 players.</param>
		/// <param name="team2">ID of Team2 players.</param>
		public static void setPlayersTeamInfo(int[] team1, int[] team2)
		{
			Dictionary<int,int> tempPlayerIdAndTeamIndex = new Dictionary<int, int> ();

			foreach(int id in team1)
			{
				tempPlayerIdAndTeamIndex.Add (id, 1);
			}
			foreach(int id in team2)
			{
				tempPlayerIdAndTeamIndex.Add (id, 2);
			}

			assignEntirePlayerTeamToIDs (tempPlayerIdAndTeamIndex);
		}
	}
}