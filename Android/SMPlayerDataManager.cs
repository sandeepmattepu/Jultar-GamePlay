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
	/// <summary>
	/// This class handles loading and saving data from local and cloud in android platform
	/// </summary>
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

		private static SMPlayerPurchasedProducts playerPurchasedProducts = new SMPlayerPurchasedProducts(false);
		/// <summary>
		/// This has data of which product is purchased and which product is not. WARNING : - DONT CHANGE CONTENTS OF IT UNLESS YOU ARE SURE
		/// </summary>
		/// <value>The player purchased products.</value>
		public static SMPlayerPurchasedProducts PlayerPurchasedProducts {
			get {
				return playerPurchasedProducts;
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

				if(playerNameInCloudData == playerNameInLocalData)
				{
					if(cloudBackUpToLocal)
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

		/// <summary>
		/// This function force saves the data so that even when there is cloud data present it will overrite it with this local data.
		///  WARNING : - USE IT ONLY WHEN PLAYER UNLOCKED GUN OR PERK WITH GEMS
		/// </summary>
		public static void forceSaveData()
		{
			playerData.isBackedUpByGoogle = true;
			playerData.reformatStringWithChanges ();
			localPlayerData = new SMPlayerDataFormatter (playerData);
			cloudPlayerData = new SMPlayerDataFormatter (playerData);

			saveData ();
		}

		#endregion

		/// <summary>
		/// This function formatted string which contains default data
		/// </summary>
		/// <returns>The default data.</returns>
		private static string getDefaultData()
		{
			SMPlayerDataFormatter defaultData = new SMPlayerDataFormatter (1, "Not-Logged-Player", 1, 0, 0, 0, 0, 0, 0, 0
				, new SMGunAndPerkData(1), false);
			return defaultData.FormattedString;
		}

		/// <summary>
		/// This function resets all data and assigns to null
		/// </summary>
		public static void resetAllData()
		{
			cloudPlayerData = null;
			playerData = null;
			localPlayerData = null;
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
		public SMGunAndPerkData gunsAndPerksData;
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
			int deathsToPlayer, int TotalWins, int TotalLoses, int timePlayed, int crownsNumber, 
			SMGunAndPerkData GunAndPerkData, bool backedUpGoogle)
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
			gunsAndPerksData = new SMGunAndPerkData(GunAndPerkData);
			gunsAndPerksData.unlockGunOrPerk (playerLevel);
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
			gunsAndPerksData = new SMGunAndPerkData(dataFormatter.gunsAndPerksData);
			gunsAndPerksData.unlockGunOrPerk (playerLevel);
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

			// Guns and Perks data
			gunsAndPerksData.formatDataToStringForSaving ();
			stringBuilder.Append ((string)gunsAndPerksData.formattedString);
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

			// Guns and perks data
			string gunAndPerkDataString = splittedData [2];
			gunAndPerkDataString += "|";
			gunAndPerkDataString += splittedData [3];
			gunsAndPerksData = new SMGunAndPerkData (gunAndPerkDataString);
			gunsAndPerksData.unlockGunOrPerk (playerLevel);

			// Backed up by google
			isBackedUpByGoogle = bool.Parse(splittedData[4]);

			// Time stamp
			timeStamp = double.Parse(splittedData[5]);
		}
	}

	/// <summary>
	/// This acts as a data packet to see which products are purchased and which are not
	/// </summary>
	public class SMPlayerPurchasedProducts
	{
		// XP boost
		public ObscuredBool xp2Boost;
		public ObscuredBool xp5Boost;
		public ObscuredBool xp8Boost;

		// Lasers
		public ObscuredBool blueLaserBought;
		public ObscuredBool greenLaserBought;
		public ObscuredBool redLaserBought;

		// Helmet
		public ObscuredBool breathoreHelmetBought;
		public ObscuredBool maskeraHelmetBought;
		public ObscuredBool operativeHelmetBought;
		public ObscuredBool pilotarHelmetBought;
		public ObscuredBool tsTacticalHelmetBought;

		public SMPlayerPurchasedProducts(bool defaultValue)
		{
			xp2Boost = defaultValue;
			xp5Boost = defaultValue;
			xp8Boost = defaultValue;

			// Lasers
			blueLaserBought = defaultValue;
			greenLaserBought = defaultValue;
			redLaserBought = defaultValue;

			// Helmet
			breathoreHelmetBought = defaultValue;
			maskeraHelmetBought = defaultValue;
			operativeHelmetBought = defaultValue;
			pilotarHelmetBought = defaultValue;
			tsTacticalHelmetBought = defaultValue;
		}

		/// <summary>
		/// This function will make highest xp available to player when he bought
		/// </summary>
		/// <param name="boostType">Boost type player bought.</param>
		public void purchasedXp(Boost boostType)
		{
			switch(boostType)
			{
			case Boost.EightTimesExp:
				xp2Boost = false;
				xp5Boost = false;
				xp8Boost = true;
				break;
			case Boost.FiveTimesExp:
				if (!xp8Boost) 
				{
					xp2Boost = false;
					xp5Boost = true;
				} 
				else 
				{
					xp2Boost = false;
					xp5Boost = false;
				}
				break;
			case Boost.TwoTimesExp:
				if (!xp8Boost && !xp5Boost) 
				{
					xp2Boost = true;
				}
				else 
				{
					xp2Boost = false;
				}
				break;
			}
   		}

		/// <summary>
		/// This function will get access to laser in the game
		/// </summary>
		/// <param name="laserType">Laser type that is purchased.</param>
		public void purchaseLaser(Laser_Type laserType)
		{
			switch(laserType)
			{
			case Laser_Type.BLUE_LASER:
				blueLaserBought = true;
				break;
			case Laser_Type.GREEN_LASER:
				greenLaserBought = true;
				break;
			case Laser_Type.RED_LASER:
				redLaserBought = true;
				break;
			}
		}

		/// <summary>
		/// This function will get access to helmet in game
		/// </summary>
		/// <param name="helmetType">Helmet type that is purchased.</param>
		public void purchaseHelmet(Helmet_Type helmetType)
		{
			switch(helmetType)
			{
			case Helmet_Type.BREATHOR:
				breathoreHelmetBought = true;
				break;
			case Helmet_Type.MASKERA:
				maskeraHelmetBought = true;
				break;
			case Helmet_Type.OPERATIVE:
				operativeHelmetBought = true;
				break;
			case Helmet_Type.PILOTAR:
				pilotarHelmetBought = true;
				break;
			case Helmet_Type.TS_TACTICAL:
				tsTacticalHelmetBought = true;
				break;
			}
		}

  	}

	/// <summary>
	/// This acts as a data packet to see which guns and perks are unlocked
	/// </summary>
	public struct SMGunAndPerkData
	{
		// Guns Data
		public ObscuredBool greenEyeGunBought;
		public ObscuredBool leoBlackDogGunBought;
		public ObscuredBool jully11Bought;
		public ObscuredBool blondeGunBought;
		public ObscuredBool mrozykGunBought;
		public ObscuredBool rainiGunBought;
		public ObscuredBool smilerieGunBought;
		public ObscuredBool sniperGunBought;

		// Perks
		public ObscuredBool egoPerkBought;
		public ObscuredBool rusherPerkBought;
		public ObscuredBool strategyPerkBought;
		public ObscuredBool thinkerPerkBought;
		public ObscuredBool tryHardPerkBought;

		public ObscuredString formattedString;

		public SMGunAndPerkData(ObscuredInt levelOfPlayer)
		{
			greenEyeGunBought = ((int)levelOfPlayer) >= 23 ? true : false;
			leoBlackDogGunBought = ((int)levelOfPlayer) >= 27 ? true : false;
			jully11Bought = ((int)levelOfPlayer) >= 44 ? true : false;
			blondeGunBought = ((int)levelOfPlayer) >= 50 ? true : false;
			mrozykGunBought = true;
			rainiGunBought = ((int)levelOfPlayer) >= 7 ? true : false;
			smilerieGunBought = ((int)levelOfPlayer) >= 11 ? true : false;
			sniperGunBought = ((int)levelOfPlayer) >= 15 ? true : false;

			egoPerkBought = ((int)levelOfPlayer) >= 22 ? true : false;
			rusherPerkBought = ((int)levelOfPlayer) >= 13 ? true : false;
			strategyPerkBought = ((int)levelOfPlayer) >= 18 ? true : false;
			thinkerPerkBought = ((int)levelOfPlayer) >= 28 ? true : false;
			tryHardPerkBought = ((int)levelOfPlayer) >= 35 ? true : false;

			formattedString = null;

			formatDataToStringForSaving ();
   		}

		/// <summary>
		/// This creates instance out of string
		/// </summary>
		/// <param name="dataInString">Data in string.</param>
		public SMGunAndPerkData(string dataInString)
		{
			greenEyeGunBought = false;
			leoBlackDogGunBought = false;
			jully11Bought = false;
			blondeGunBought = false;
			mrozykGunBought = true;
			rainiGunBought = false;
			smilerieGunBought = false;
			sniperGunBought = false;

			egoPerkBought = false;
			rusherPerkBought = false;
			strategyPerkBought = false;
			thinkerPerkBought = false;
			tryHardPerkBought = false;

			formattedString = dataInString;

			extractDataFromString ();
		}

		public SMGunAndPerkData(SMGunAndPerkData GunAndPerkData)
		{
			greenEyeGunBought = GunAndPerkData.greenEyeGunBought;
			leoBlackDogGunBought = GunAndPerkData.leoBlackDogGunBought;
			jully11Bought = GunAndPerkData.jully11Bought;
			blondeGunBought = GunAndPerkData.blondeGunBought;
			mrozykGunBought = GunAndPerkData.mrozykGunBought;
			rainiGunBought = GunAndPerkData.rainiGunBought;
			smilerieGunBought = GunAndPerkData.smilerieGunBought;
			sniperGunBought = GunAndPerkData.sniperGunBought;

			egoPerkBought = GunAndPerkData.egoPerkBought;
			rusherPerkBought = GunAndPerkData.rusherPerkBought;
			strategyPerkBought = GunAndPerkData.strategyPerkBought;
			thinkerPerkBought = GunAndPerkData.thinkerPerkBought;
			tryHardPerkBought = GunAndPerkData.tryHardPerkBought;

			formattedString = GunAndPerkData.formattedString;
		}

		/// <summary>
		/// Formats the data to string for saving.
		/// </summary>
		public void formatDataToStringForSaving()
		{
			StringBuilder stringBuilder = new StringBuilder ();

			// Guns Data
			stringBuilder.Append((bool)mrozykGunBought);
			stringBuilder.Append ("%");
			stringBuilder.Append ((bool)blondeGunBought);
			stringBuilder.Append ("%");
			stringBuilder.Append ((bool)greenEyeGunBought);
			stringBuilder.Append ("%");
			stringBuilder.Append ((bool)jully11Bought);
			stringBuilder.Append ("%");
			stringBuilder.Append ((bool)leoBlackDogGunBought);
			stringBuilder.Append ("%");
			stringBuilder.Append ((bool)rainiGunBought);
			stringBuilder.Append ("%");
			stringBuilder.Append ((bool)smilerieGunBought);
			stringBuilder.Append ("%");
			stringBuilder.Append ((bool)sniperGunBought);
			stringBuilder.Append ("|");

			// Perks data
			stringBuilder.Append((bool)egoPerkBought);
			stringBuilder.Append ("%");
			stringBuilder.Append ((bool)rusherPerkBought);
			stringBuilder.Append ("%");
			stringBuilder.Append ((bool)strategyPerkBought);
			stringBuilder.Append ("%");
			stringBuilder.Append ((bool)thinkerPerkBought);
			stringBuilder.Append ("%");
			stringBuilder.Append ((bool)tryHardPerkBought);

			formattedString = stringBuilder.ToString ();
   		}

		/// <summary>
		/// This function extracts data from fromatted string
		/// </summary>
		private void extractDataFromString()
		{
			string[] dataOfGunAndPerk = ((string)formattedString).Split ('|');
			string[] gunData = (dataOfGunAndPerk[0]).Split ('%');
			
			mrozykGunBought = true;
			blondeGunBought = bool.Parse (gunData [1]);
			greenEyeGunBought = bool.Parse (gunData [2]);
			jully11Bought = bool.Parse (gunData [3]);
			leoBlackDogGunBought = bool.Parse (gunData [4]);
			rainiGunBought = bool.Parse (gunData [5]);
			smilerieGunBought = bool.Parse (gunData [6]);
			sniperGunBought = bool.Parse (gunData [7]);

			// Perks
			string[] perksData = dataOfGunAndPerk[1].Split('%');
			egoPerkBought = bool.Parse (perksData [0]);
			rusherPerkBought = bool.Parse (perksData [1]);
			strategyPerkBought = bool.Parse (perksData [2]);
			thinkerPerkBought = bool.Parse (perksData [3]);
			tryHardPerkBought = bool.Parse(perksData[4]);
		}

		/// <summary>
		/// Unlocks the gun or perk based on player level.
		/// </summary>
		/// <param name="levelOfPlayer">Gun type.</param>
		public void unlockGunOrPerk(ObscuredInt levelOfPlayer)
		{
			greenEyeGunBought = ((int)levelOfPlayer) >= 23 ? true : (bool)greenEyeGunBought;
			leoBlackDogGunBought = ((int)levelOfPlayer) >= 27 ? true : (bool)leoBlackDogGunBought;
			jully11Bought = ((int)levelOfPlayer) >= 44 ? true : (bool)jully11Bought;
			blondeGunBought = ((int)levelOfPlayer) >= 50 ? true : (bool)blondeGunBought;
			mrozykGunBought = true;
			rainiGunBought = ((int)levelOfPlayer) >= 7 ? true : (bool)rainiGunBought;
			smilerieGunBought = ((int)levelOfPlayer) >= 11 ? true : (bool)smilerieGunBought;
			sniperGunBought = ((int)levelOfPlayer) >= 15 ? true : (bool)sniperGunBought;

			egoPerkBought = ((int)levelOfPlayer) >= 22 ? true : (bool)egoPerkBought;
			rusherPerkBought = ((int)levelOfPlayer) >= 13 ? true : (bool)rusherPerkBought;
			strategyPerkBought = ((int)levelOfPlayer) >= 18 ? true : (bool)strategyPerkBought;
			thinkerPerkBought = ((int)levelOfPlayer) >= 28 ? true : (bool)thinkerPerkBought;
			tryHardPerkBought = ((int)levelOfPlayer) >= 35 ? true : (bool)tryHardPerkBought;

			formattedString = null;

			formatDataToStringForSaving ();
		}

		/// <summary>
		/// Unlocks the gun.
		/// </summary>
		/// <param name="guntype">Gun type.</param>
		public void unlockGun(GUN_TYPE guntype)
		{
			switch(guntype)
			{
			case GUN_TYPE.Blonde:
				blondeGunBought = true;
				break;
			case GUN_TYPE.GreenEye:
				greenEyeGunBought = true;
				break;
			case GUN_TYPE.July11:
				jully11Bought = true;
				break;
			case GUN_TYPE.LeoBlackDog:
				leoBlackDogGunBought = true;
				break;
			case GUN_TYPE.Mrozyk:
				mrozykGunBought = true;
				break;
			case GUN_TYPE.Raini:
				rainiGunBought = true;
				break;
			case GUN_TYPE.Smilere:
				smilerieGunBought = true;
				break;
			case GUN_TYPE.Sniper:
				sniperGunBought = true;
				break;
			}
		}

		/// <summary>
		/// Unlocks the perk.
		/// </summary>
		/// <param name="perkType">Perk type.</param>
		public void unlockPerk(Perks_Type perkType)
		{
			switch(perkType)
			{
			case Perks_Type.EGO:
				egoPerkBought = true;
				break;
			case Perks_Type.RUSHER:
				rusherPerkBought = true;
				break;
			case Perks_Type.STRATEGY:
				strategyPerkBought = true;
				break;
			case Perks_Type.THINKER:
				thinkerPerkBought = true;
				break;
			case Perks_Type.TRY_HARD:
				tryHardPerkBought = true;
				break;
			}
		}
	}
}

