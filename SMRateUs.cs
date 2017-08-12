//
//  SMRateUs.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 11/08/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SandeepMattepu
{
	/// <summary>
	/// This class shows rate us popup to the player and makes them land on market page to rate the game.
	/// </summary>
	public class SMRateUs : MonoBehaviour 
	{
		/// <summary>
		/// The number of times lobby scene loaded.
		/// </summary>
		private static int TimesLobbySceneLoaded = 0;

		#region Keys for Player Prefs
		/// <summary>
		/// Key for player times opened
		/// </summary>
		private const string TIMES_OPENED = "Times_Opened";
		/// <summary>
		/// Key for not intrested
		/// </summary>
		private const string NOT_INTERESTED_RATING = "Not_Intrested";
		/// <summary>
		/// Key for already played
		/// </summary>
		private const string ALREADY_RATED = "Already_Rated";
		#endregion

		#region Loaded data from Player prefs
		/// <summary>
		/// The times player opened game.
		/// </summary>
		private static int TimesPlayerOpenedGame = 0;
		/// <summary>
		/// Is player intrested in rating.
		/// </summary>
		private static bool isPlayerIntrestedInRating = true;
		/// <summary>
		/// Is player already rated
		/// </summary>
		private static bool alreadyRated = false;
		#endregion

		/// <summary>
		/// The rate us panel.
		/// </summary>
		[SerializeField]
		private GameObject rateUsPanel;
		// Use this for initialization
		void Start () 
		{
			TimesLobbySceneLoaded += 1;
			rateUsPanel.SetActive (false);
			loadPlayerPrefs ();
			checkWhetherToShowPopUp ();
		}

		void OnDestroy()
		{
			PlayerPrefs.SetInt (TIMES_OPENED, TimesPlayerOpenedGame + 1);
		}

		/// <summary>
		/// This function loads the player prefs to analyze how many times player has opened the game
		/// </summary>
		private void loadPlayerPrefs()
		{
			if(PlayerPrefs.HasKey(TIMES_OPENED))
			{
				TimesPlayerOpenedGame = PlayerPrefs.GetInt (TIMES_OPENED);
			}
			else
			{
				PlayerPrefs.SetInt (TIMES_OPENED, 1);
				TimesPlayerOpenedGame = 1;
			}

			if(PlayerPrefs.HasKey(NOT_INTERESTED_RATING))
			{
				int loadedIntForBool = PlayerPrefs.GetInt (NOT_INTERESTED_RATING);
				// Since playerprefs cannot save and load bool
				if(loadedIntForBool != 0)		// true case
				{
					isPlayerIntrestedInRating = false;
				}
				else
				{
					isPlayerIntrestedInRating = true;
				}
			}
			else
			{
				PlayerPrefs.SetInt (NOT_INTERESTED_RATING, 0);
				isPlayerIntrestedInRating = true;
			}

			if(PlayerPrefs.HasKey(ALREADY_RATED))
			{
				int loadedIntForBool = PlayerPrefs.GetInt (ALREADY_RATED);
				// Since playerprefs cannot save and load bool
				if(loadedIntForBool != 0)		// true case
				{
					alreadyRated = true;
				}
				else
				{
					alreadyRated = false;
				}
			}
			else
			{
				PlayerPrefs.SetInt (ALREADY_RATED, 0);
				alreadyRated = false;
			}
		}

		/// <summary>
		/// This condition checks whether to show pop up to player or not
		/// </summary>
		private void checkWhetherToShowPopUp()
		{
			if(!alreadyRated && isPlayerIntrestedInRating)
			{
				if(TimesPlayerOpenedGame >= 8 && TimesLobbySceneLoaded > 1)
				{
					// then show rate us popup
					alreadyRated = true;		// We dont save this. This is to avoid showing multiple popups in single game
					rateUsPanel.SetActive(true);
				}
			}
		}

		/// <summary>
		/// This function will open market store for player to rate
		/// </summary>
		public void okRateUsButtonPressed()
		{
			rateUsPanel.SetActive (false);
			#if UNITY_EDITOR
			Debug.Log("It will open market store in phone but not in editor");
			#endif
			#if UNITY_ANDROID
			string idOfApp = Application.bundleIdentifier;
			Application.OpenURL("market://details?id=" + idOfApp);
			alreadyRated = true;
			#elif UNITY_IPHONE
			Application.OpenURL("itms-apps://itunes.apple.com/app/idYOUR_ID");
			alreadyRated = true;
			#endif
			int valueToSave = alreadyRated ? 1 : 0;
			PlayerPrefs.SetInt (ALREADY_RATED, valueToSave);
		}

		/// <summary>
		/// This function will reset value of times opened so that he will be reminded later
		/// </summary>
		public void remindMeLaterButtonPressed()
		{
			rateUsPanel.SetActive (false);
			alreadyRated = true; // Dont save it
			TimesPlayerOpenedGame = 0;
			PlayerPrefs.SetInt (TIMES_OPENED, TimesPlayerOpenedGame);
		}

		/// <summary>
		/// This will make the game never show the popup screen to the player
		/// </summary>
		public void notIntrestedButtonPressed()
		{
			rateUsPanel.SetActive (false);
			isPlayerIntrestedInRating = false;
			int valueToSet = !(isPlayerIntrestedInRating) ? 1 : 0;
			PlayerPrefs.SetInt (NOT_INTERESTED_RATING, valueToSet);
		}
	}
}
