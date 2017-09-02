//
//  SMPlayerDataManager.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 10/08/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CodeStage.AntiCheat.ObscuredTypes;
using System.Text;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

namespace SandeepMattepu.Android
{
	public class SMPlayerDataManager : MonoBehaviour 
	{
		/// <summary>
		/// This becomes true when player is connected to cloud
		/// </summary>
		/// <summary>
		/// The key for saving player data in player prefs
		/// </summary>
		private static string _PlayerPrefsKey = "SaveString";
		/// <summary>
		/// The local player data.
		/// </summary>
		private static SMPlayerDataFormatter localPlayerData = null;

		/// <summary>
		/// The cloud player data(Can be null if failed to load).
		/// </summary>
		private static SMPlayerDataFormatter cloudPlayerData = null;

		/// <summary>
		/// The player data which will be referenced by other scripts.
		/// </summary>
		private static SMPlayerDataFormatter playerData = null;
		/// <summary>
		/// The player data which will be referenced by other scripts.
		/// </summary>
		public static SMPlayerDataFormatter PlayerData {
			get {
				return playerData;
			}
		}

		#region Loading Data

		#region Load from cloud

		/// <summary>
		/// This function loads the cloud data.
		/// </summary>
		public static void loadData()
		{
			if(Social.localUser.authenticated)
			{
				((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution
				("JultarSave",
					DataSource.ReadCacheOrNetwork,
					ConflictResolutionStrategy.UseOriginal, OnGoogleLoadCallBack);
			}
			else
			{
				loadLocalData ();
				cloudPlayerData = new SMPlayerDataFormatter (localPlayerData);
				playerData = new SMPlayerDataFormatter (localPlayerData);
			}
		}

		/// <summary>
		/// This function acts as callback for handling cloud loading
		/// </summary>
		private static void OnGoogleLoadCallBack(SavedGameRequestStatus Status, ISavedGameMetadata Game)
		{
			if(Status == SavedGameRequestStatus.Success)
			{
				((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(Game, OnHandleLoadingResult);
			}
			else
			{
				loadLocalData ();
				cloudPlayerData = new SMPlayerDataFormatter (localPlayerData);
				playerData = new SMPlayerDataFormatter (localPlayerData);
			}
		}

		/// <summary>
		/// This function handles result of loading data
		/// </summary>
		private static void OnHandleLoadingResult(SavedGameRequestStatus Status, byte[] Data)
		{
			if (Status == SavedGameRequestStatus.Success)
			{
				string LocalSaveString;
				loadLocalData ();
				LocalSaveString = localPlayerData.FormattedString;

				string CloudSaveString = Encoding.ASCII.GetString(Data);
				if(CloudSaveString == null || CloudSaveString == "")   // When player logged in to services for first time
				{
					CloudSaveString = getDefaultData ();
					cloudPlayerData = new SMPlayerDataFormatter (CloudSaveString);
					cloudPlayerData.isBackedUpByGoogle = true;
					cloudPlayerData = cloudPlayerData.reformatStringWithChanges ();
					playerData = new SMPlayerDataFormatter (cloudPlayerData);
					saveData ();
				}
				else
				{
					cloudPlayerData = new SMPlayerDataFormatter (CloudSaveString);
				}

				double CloudTime = cloudPlayerData.timeStamp;
				double LocalTime = localPlayerData.timeStamp;
				bool cloudBackUpToLocal = localPlayerData.isBackedUpByGoogle;
				string playerNameInLocalData = localPlayerData.playerUniqueID;
				string playerNameInCloudData = cloudPlayerData.playerUniqueID;

				if(cloudBackUpToLocal && (playerNameInCloudData == playerNameInLocalData))
				{
					if(LocalTime > CloudTime)
					{
						playerData = new SMPlayerDataFormatter (localPlayerData);
						cloudPlayerData = new SMPlayerDataFormatter (playerData);
						localPlayerData = new SMPlayerDataFormatter (playerData);
						saveData ();
					}
					else
					{
						playerData = new SMPlayerDataFormatter (cloudPlayerData);
						localPlayerData = new SMPlayerDataFormatter (playerData);
						saveDataLocally ();
					}
				}
				else
				{
					playerData = new SMPlayerDataFormatter (cloudPlayerData);
					localPlayerData = new SMPlayerDataFormatter (playerData);
					localPlayerData.isBackedUpByGoogle = true;
					localPlayerData = localPlayerData.reformatStringWithChanges ();
					saveDataLocally ();
				}
			}
			else
			{
				loadLocalData ();
				cloudPlayerData = new SMPlayerDataFormatter (localPlayerData);
				playerData = new SMPlayerDataFormatter (localPlayerData);
			}
		}

		#endregion

		#region Load Locally

		/// <summary>
		/// This function loads data from local device player prefs
		/// </summary>
		private static void loadLocalData()
		{
			if(!ObscuredPrefs.HasKey(_PlayerPrefsKey))
			{
				localPlayerData = new SMPlayerDataFormatter (getDefaultData());
				saveDataLocally ();
			}
			else
			{
				string data = ObscuredPrefs.GetString (_PlayerPrefsKey);
				localPlayerData = new SMPlayerDataFormatter (data);
			}
		}

		#endregion

		#endregion

		#region Saving data

		#region Save Cloud

		/// <summary>
		/// This function saves data to cloud
		/// </summary>
		public static void saveData()
		{
			if (Social.localUser.authenticated)
			{
				playerData.playerUniqueID = Social.localUser.id;
				playerData.reformatStringWithChanges ();
				((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution
				("JultarSave",
					DataSource.ReadCacheOrNetwork,
					ConflictResolutionStrategy.UseLongestPlaytime,
					OnGoogleSaveCallBack);
			}
			else
			{

				cloudPlayerData = new SMPlayerDataFormatter (playerData);
				localPlayerData = new SMPlayerDataFormatter(playerData);
				saveDataLocally ();
			}
		}

		/// <summary>
		/// This function acts as a call back to handle google saving process
		/// </summary>
		private static void OnGoogleSaveCallBack(SavedGameRequestStatus Status, ISavedGameMetadata Game)
		{
			if(Status == SavedGameRequestStatus.Success)
			{
				cloudPlayerData = new SMPlayerDataFormatter (playerData);
				localPlayerData = new SMPlayerDataFormatter(playerData);
				playerData.isBackedUpByGoogle = true;
				playerData.reformatStringWithChanges ();
				string DataString = playerData.formattedString;
				byte[] Data = Encoding.ASCII.GetBytes(DataString);
				SavedGameMetadataUpdate.Builder _Builder = new SavedGameMetadataUpdate.Builder();

				SavedGameMetadataUpdate Update = _Builder.Build();
				((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(Game, Update, Data, OnHandleSavingResult);
			}
			else
			{
				cloudPlayerData = new SMPlayerDataFormatter (playerData);
				localPlayerData = new SMPlayerDataFormatter(playerData);
				saveDataLocally ();
			}
		}

		/// <summary>
		/// This function handles the result of saving data to cloud
		/// </summary>
		private static void OnHandleSavingResult(SavedGameRequestStatus Status, ISavedGameMetadata Game)
		{
			if(Status == SavedGameRequestStatus.Success)
			{
				cloudPlayerData = new SMPlayerDataFormatter (playerData);
				localPlayerData = new SMPlayerDataFormatter (playerData);
				saveDataLocally ();
			}
			else
			{
				saveDataLocally ();
			}
		}

		#endregion

		#region Save Local

		/// <summary>
		/// Saves the data locally to device
		/// </summary>
		private static void saveDataLocally()
		{
			if(localPlayerData == null)
			{
				localPlayerData = new SMPlayerDataFormatter (getDefaultData ());
			}

			ObscuredPrefs.SetString (_PlayerPrefsKey, localPlayerData.formattedString);
		}

		#endregion

		#endregion

		/// <summary>
		/// This function formatted string which contains default data
		/// </summary>
		/// <returns>The default data.</returns>
		private static string getDefaultData()
		{
			SMPlayerDataFormatter defaultData = new SMPlayerDataFormatter (1, "Not-Logged-Player", 1, 0, 0, 0, 0, 0, 0, 0, false);
			return defaultData.FormattedString;
		}
	}

	/// <summary>
	/// This class handles string formatting from data and extracting data from string
	/// </summary>
	public class SMPlayerDataFormatter
	{
		/// <summary>
		/// The game developed time. NOTE :- DONT CHANGE THIS FOR ANY VERSION OF APP
		/// </summary>
		private static DateTime gameDevelopedTime = new DateTime(2017, 8, 27, 0, 0, 0, 0, DateTimeKind.Utc);

		public ObscuredInt gameVersionNumber;
		public ObscuredString playerUniqueID;
		public ObscuredInt playerLevel;
		public ObscuredInt additionalXp;
		public ObscuredInt killsMadeByPlayer;
		public ObscuredInt deathsOccuredToPlayer;
		public ObscuredInt totalWins;
		public ObscuredInt totalLoses;
		public ObscuredInt timePlayerPlayed;
		public ObscuredInt numberOfCrowns;
		public ObscuredBool isBackedUpByGoogle;
		public ObscuredDouble timeStamp;
		public ObscuredString formattedString;
		/// <summary>
		/// This string is in formatted form as VERSION_NUMBER | DATA |
		///  IS_BACKEDUP_BY_GOOGLE | TIME_STAMP
		/// </summary>
		/// <value>The formatted string.</value>
		public ObscuredString FormattedString {
			get {
				return formattedString;
			}
		}

		public SMPlayerDataFormatter(int gameVersion, string UniqueID, int PlayerLevel, int AdditionalXP, int playerMadeKills,
			int deathsToPlayer, int TotalWins, int TotalLoses, int timePlayed, int crownsNumber, bool backedUpGoogle)
		{
			gameVersionNumber = gameVersion;
			playerUniqueID = UniqueID;
			playerLevel = PlayerLevel;
			additionalXp = AdditionalXP;
			killsMadeByPlayer = playerMadeKills;
			deathsOccuredToPlayer = deathsToPlayer;
			totalWins = TotalWins;
			totalLoses = TotalLoses;
			timePlayerPlayed = timePlayed;
			numberOfCrowns = crownsNumber;
			isBackedUpByGoogle = backedUpGoogle;
			formatDataToStringForSavingOrLoading ();
		}

		/// <summary>
		/// Creates an instance and initilizes inner data based on string.
		/// </summary>
		/// <param name="FormattedString">Formatted string. Make sure that string is in right format. 
		///  The format is VERSION_NUMBER | DATA |
		///  IS_BACKEDUP_BY_GOOGLE | TIME_STAMP. For example 1|25%34%14%0.75%12%14|0|6312.69</param>
		public SMPlayerDataFormatter(string FormattedString)
		{
			formattedString = FormattedString;
			extractDataFromString ();
		}

		public SMPlayerDataFormatter(SMPlayerDataFormatter dataFormatter)
		{
			gameVersionNumber = dataFormatter.gameVersionNumber;
			playerUniqueID = dataFormatter.playerUniqueID;
			playerLevel = dataFormatter.playerLevel;
			additionalXp = dataFormatter.additionalXp;
			killsMadeByPlayer = dataFormatter.killsMadeByPlayer;
			deathsOccuredToPlayer = dataFormatter.deathsOccuredToPlayer;
			totalWins = dataFormatter.totalWins;
			totalLoses = dataFormatter.totalLoses;
			timePlayerPlayed = dataFormatter.timePlayerPlayed;
			numberOfCrowns = dataFormatter.numberOfCrowns;
			isBackedUpByGoogle = dataFormatter.isBackedUpByGoogle;
			timeStamp = dataFormatter.timeStamp;
			formattedString = dataFormatter.formattedString;
			formatDataToStringForSavingOrLoading ();
		}

		/// <summary>
		/// Reformats the string with changes.
		/// </summary>
		/// <returns>The instance with proper changes.</returns>
		public SMPlayerDataFormatter reformatStringWithChanges()
		{
			formatDataToStringForSavingOrLoading ();
			return this;
		}

		/// <summary>
		/// This function formats the data to string for saving or loading. The format is VERSION_NUMBER | DATA |
		///  IS_BACKEDUP_BY_GOOGLE | TIME_STAMP
		/// </summary>
		private void formatDataToStringForSavingOrLoading()
		{
			StringBuilder stringBuilder = new StringBuilder();

			// Version number
			stringBuilder.Append((int)gameVersionNumber);
			stringBuilder.Append ("|");

			// Data
			stringBuilder.Append((string)playerUniqueID);
			stringBuilder.Append ("%");
			stringBuilder.Append((int)playerLevel);
			stringBuilder.Append ("%");
			stringBuilder.Append ((int)additionalXp);
			stringBuilder.Append ("%");
			stringBuilder.Append((int)killsMadeByPlayer);
			stringBuilder.Append ("%");
			stringBuilder.Append((int)deathsOccuredToPlayer);
			stringBuilder.Append ("%");
			stringBuilder.Append((int)totalWins);
			stringBuilder.Append ("%");
			stringBuilder.Append ((int)totalLoses);
			stringBuilder.Append ("%");
			stringBuilder.Append((int)timePlayerPlayed);
			stringBuilder.Append ("%");
			stringBuilder.Append((int)numberOfCrowns);
			stringBuilder.Append ("|");

			// Backed up by google
			stringBuilder.Append ((bool)isBackedUpByGoogle);
			stringBuilder.Append ("|");

			// Time stamp
			DateTime now = DateTime.UtcNow;
			TimeSpan elapsedTime = now - gameDevelopedTime;
			timeStamp = elapsedTime.TotalSeconds;
			stringBuilder.Append ((double)timeStamp);

			formattedString = stringBuilder.ToString ();
		}

		/// <summary>
		/// This function extracts data from fromatted string
		/// </summary>
		private void extractDataFromString()
		{
			string[] splittedData = ((string)formattedString).Split ('|');

			// Version number
			gameVersionNumber = int.Parse(splittedData[0]);

			// Data
			string[] playerData = splittedData[1].Split('%');
			playerUniqueID = playerData [0];
			playerLevel = int.Parse(playerData[1]);
			additionalXp = int.Parse (playerData [2]);
			killsMadeByPlayer = int.Parse(playerData[3]);
			deathsOccuredToPlayer = int.Parse(playerData[4]);
			totalWins = int.Parse(playerData[5]);
			totalLoses = int.Parse (playerData [6]);
			timePlayerPlayed = int.Parse(playerData[7]);
			numberOfCrowns = int.Parse(playerData[8]);

			// Backed up by google
			isBackedUpByGoogle = bool.Parse(splittedData[2]);

			// Time stamp
			timeStamp = double.Parse(splittedData[3]);
		}
	}
}

