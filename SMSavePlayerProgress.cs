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
		private ObscuredInt killesMade;
		/// <summary>
		/// The deaths encountered by player in his entire game life..
		/// </summary>
		private ObscuredInt deathsEncountered;
		/// <summary>
		/// The time played by the player in his entire game time.
		/// </summary>
		private ObscuredInt timePlayed;
		/// <summary>
		/// The total wins made by player.
		/// </summary>
		private ObscuredInt totalWinsMadeByPlayer;
		/// <summary>
		/// The total loses made by player.
		/// </summary>
		private ObscuredInt totalLosesMadeByPlayer;
		/// <summary>
		/// The current level of player.
		/// </summary>
		private ObscuredInt currentLevelOfPlayer;
		/// <summary>
		/// The additional xp of player.
		/// </summary>
		private ObscuredInt additionalXpOfPlayer;
		/// <summary>
		/// The total crowns available to player.
		/// </summary>
		private ObscuredInt totalCrowns;
		/// <summary>
		/// The xp to penalize player when left.
		/// </summary>
		[SerializeField]
		private ObscuredInt xpToPenalize;
		/// <summary>
		/// The gems to award when won.
		/// </summary>
		[SerializeField]
		private ObscuredInt gemsToAwardWhenWon;

		// Use this for initialization
		void Start () 
		{
			loadPlayerData ();
			SMMultiplayerGame.OnGameOver += savePlayerData;
		}

		void OnDestroy()
		{
			SMMultiplayerGame.OnGameOver -= savePlayerData;
		}

		/// <summary>
		/// Loads the player data.
		/// </summary>
		private void loadPlayerData()
		{
			totalCrowns = SMPlayerDataManager.PlayerData.numberOfCrowns;
			killesMade = SMPlayerDataManager.PlayerData.killsMadeByPlayer;
			deathsEncountered = SMPlayerDataManager.PlayerData.deathsOccuredToPlayer;
			timePlayed = SMPlayerDataManager.PlayerData.timePlayerPlayed;
			totalWinsMadeByPlayer = SMPlayerDataManager.PlayerData.totalWins;
			totalLosesMadeByPlayer = SMPlayerDataManager.PlayerData.totalLoses;
			currentLevelOfPlayer = SMPlayerDataManager.PlayerData.playerLevel;
			additionalXpOfPlayer = SMPlayerDataManager.PlayerData.additionalXp;
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
					totalCrowns += gemsToAwardWhenWon;
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
					totalCrowns += gemsToAwardWhenWon;
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
				SMPlayerDataManager.PlayerData.numberOfCrowns = totalCrowns;
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

		/// <summary>
		/// Penalizes when player leaves and save player progress.
		/// </summary>
		public void penalizeAndSavePlayerProgress()
		{
			while(xpToPenalize > 0)
			{
				if(additionalXpOfPlayer >= xpToPenalize)
				{
					additionalXpOfPlayer -= xpToPenalize;
					xpToPenalize = 0;
				}
				else
				{
					xpToPenalize -= additionalXpOfPlayer;
					additionalXpOfPlayer = 0;
					if(currentLevelOfPlayer == 1)
					{
						xpToPenalize = 0;
					}
					else
					{
						currentLevelOfPlayer -= 1;
						additionalXpOfPlayer = calculateTotalXpRequiredForNextLevel (currentLevelOfPlayer);
					}
				}
			}

			savePlayerData ();
		}
	}
}
