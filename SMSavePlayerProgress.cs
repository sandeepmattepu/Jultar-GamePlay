//
//  SMSavePlayerProgress.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 10/08/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SandeepMattepu.Multiplayer;
using CodeStage.AntiCheat.ObscuredTypes;

namespace SandeepMattepu
{
	/// <summary>
	/// This class loads and saves the player progress made in the game
	/// </summary>
	public class SMSavePlayerProgress : MonoBehaviour 
	{
		/// <summary>
		/// The killes made by player in his entire game life.
		/// </summary>
		private ObscuredInt killesMade = UserManager.Stats[(int)Stat.Kills];
		/// <summary>
		/// The deaths encountered by player in his entire game life..
		/// </summary>
		private ObscuredInt deathsEncountered = UserManager.Stats[(int)Stat.Deaths];
		/// <summary>
		/// The time played by the player in his entire game time.
		/// </summary>
		private ObscuredInt timePlayed = UserManager.Stats[(int)Stat.TimePlayed];
		/// <summary>
		/// The total wins made by player.
		/// </summary>
		private ObscuredInt totalWinsMadeByPlayer = UserManager.Stats[(int)Stat.Won];
		/// <summary>
		/// The total loses made by player.
		/// </summary>
		private ObscuredInt totalLosesMadeByPlayer = UserManager.Stats[(int)Stat.Lost];

		// Use this for initialization
		void Start () 
		{
			SMMultiplayerGame.OnGameOver += savePlayerData;
		}

		void OnDestroy()
		{
			SMMultiplayerGame.OnGameOver -= savePlayerData;
		}

		/// <summary>
		/// This function is called at the end of the game to save player progress
		/// </summary>
		private void savePlayerData()
		{
			int localPlayerId = PhotonNetwork.player.ID;
			ObscuredInt deathsNumber = 0;
			ObscuredInt scoreMade;
			bool isDataReadyToSave = false;

			timePlayed += (int)SMMultiplayerGame.INSTANCE.gameSessionTime;
			if(SMMultiplayerGame.PlayerIdAndDeaths.TryGetValue (localPlayerId, out deathsNumber))
			{
				deathsEncountered += deathsNumber;
			}

			if(SMMultiplayerGame.gameType == MPGameTypes.FREE_FOR_ALL)
			{
				if(SMFreeForAll.IsLocalPlayerLeading)
				{
					totalWinsMadeByPlayer += 1;
				}
				else
				{
					totalLosesMadeByPlayer += 1;
				}

				if(SMFreeForAll.PlayersIdAndScore.TryGetValue(localPlayerId, out scoreMade))
				{
					killesMade += scoreMade;
					isDataReadyToSave = true;
				}
			}
			else if(SMMultiplayerGame.gameType == MPGameTypes.TEAM_DEATH_MATCH)
			{
				if(SMTeamDeathMatch.IsLocalPlayerTeamLeading)
				{
					totalWinsMadeByPlayer += 1;
				}
				else
				{
					totalLosesMadeByPlayer += 1;
				}

				if(SMTeamDeathMatch.PlayersIdAndScore.TryGetValue(localPlayerId, out scoreMade))
				{
					killesMade += scoreMade;
					isDataReadyToSave = true;
				}
			}

			if(isDataReadyToSave)
			{
				UserManager.Stats [(int)Stat.Kills] = killesMade;
				UserManager.Stats [(int)Stat.Deaths] = deathsEncountered;
				UserManager.Stats [(int)Stat.TimePlayed] = timePlayed;
				UserManager.Stats [(int)Stat.Won] = totalWinsMadeByPlayer;
				UserManager.Stats [(int)Stat.Lost] = totalLosesMadeByPlayer;
				UserManager.SaveCloud ();
			}

		}
	}
}
