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
using SandeepMattepu.Android;

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
		private ObscuredInt killesMade = SMPlayerDataManager.PlayerData.killsMadeByPlayer;
		/// <summary>
		/// The deaths encountered by player in his entire game life..
		/// </summary>
		private ObscuredInt deathsEncountered = SMPlayerDataManager.PlayerData.deathsOccuredToPlayer;
		/// <summary>
		/// The time played by the player in his entire game time.
		/// </summary>
		private ObscuredInt timePlayed = SMPlayerDataManager.PlayerData.timePlayerPlayed;
		/// <summary>
		/// The total wins made by player.
		/// </summary>
		private ObscuredInt totalWinsMadeByPlayer = SMPlayerDataManager.PlayerData.totalWins;
		/// <summary>
		/// The total loses made by player.
		/// </summary>
		private ObscuredInt totalLosesMadeByPlayer = SMPlayerDataManager.PlayerData.totalLoses;
		/// <summary>
		/// The current level of player.
		/// </summary>
		private ObscuredInt currentLevelOfPlayer = SMPlayerDataManager.PlayerData.playerLevel;
		/// <summary>
		/// The additional xp of player.
		/// </summary>
		private ObscuredInt additionalXpOfPlayer = SMPlayerDataManager.PlayerData.additionalXp;

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
			additionalXpOfPlayer += SMMultiplayerGame.INSTANCE.TotalXpMadeByPlayer;
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
				checkPlayerEligibleToNextLevel ();
				SMPlayerDataManager.PlayerData.killsMadeByPlayer = killesMade;
				SMPlayerDataManager.PlayerData.deathsOccuredToPlayer = deathsEncountered;
				SMPlayerDataManager.PlayerData.timePlayerPlayed = timePlayed;
				SMPlayerDataManager.PlayerData.totalWins = totalWinsMadeByPlayer;
				SMPlayerDataManager.PlayerData.totalLoses = totalLosesMadeByPlayer;
				SMPlayerDataManager.PlayerData.additionalXp = additionalXpOfPlayer;
				SMPlayerDataManager.PlayerData.playerLevel = currentLevelOfPlayer;
				SMPlayerDataManager.PlayerData.reformatStringWithChanges ();
				SMPlayerDataManager.saveData ();
			}

		}

		/// <summary>
		/// This function checks the no of xp points in experiencePoints and compare it with points required for next level and promots the player
		/// to next level if necessary.
		/// </summary>
		private void checkPlayerEligibleToNextLevel()
		{
			ObscuredInt xpRequiredForEachLevel = calculateTotalXpRequiredForNextLevel (currentLevelOfPlayer);
			if (additionalXpOfPlayer >= xpRequiredForEachLevel) 
			{
				if (currentLevelOfPlayer <= 91)
				{
					currentLevelOfPlayer += 1;
					additionalXpOfPlayer = (additionalXpOfPlayer - xpRequiredForEachLevel);
				}
			}
		}

		/// <summary>
		/// Calculates the xp required for next level.
		/// </summary>
		/// <returns>The xp required for next level.</returns>
		/// <param name="currentLevel">Current level.</param>
		public static int calculateTotalXpRequiredForNextLevel(int currentLevel)
		{
			int factor = currentLevel / 10;
			int xpRequiredUptoNextTenLevels = 70000 + (factor * 5000);
			xpRequiredUptoNextTenLevels = factor >= 8 ? (200000) : xpRequiredUptoNextTenLevels;
			int xpRequiredForEachLevel = (factor == 0 ? (xpRequiredUptoNextTenLevels / 9) : (xpRequiredUptoNextTenLevels / 10));
			return xpRequiredForEachLevel;
		}
	}
}
