//
//  SMStatsUI.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 25/08/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SandeepMattepu.UI
{
	/// <summary>
	/// This class will display stats of the player using UI elements
	/// </summary>
	public class SMStatsUI : MonoBehaviour 
	{
		#region UI variables
		/// <summary>
		/// This UI displays current level
		/// </summary>
		[SerializeField]
		private Text currentLevelText;
		/// <summary>
		/// This Ui displays number of kills made
		/// </summary>
		[SerializeField]
		private Text killsMadeText;
		/// <summary>
		/// This UI displays deaths encountered
		/// </summary>
		[SerializeField]
		private Text deathsEncounteredText;
		/// <summary>
		/// This UI displays win to lose ratio
		/// </summary>
		[SerializeField]
		private Text winToLoseRatioText;
		/// <summary>
		/// This slider indicated how much xp is required for next level
		/// </summary>
		[SerializeField]
		private Slider xpRequiredForNextLevelSlider;
		#endregion

		#region Data Variables
		/// <summary>
		/// The current level of player.
		/// </summary>
		private int currentLevel;
		/// <summary>
		/// The additional xp of user.
		/// </summary>
		private int additionalXpOfUser;
		/// <summary>
		/// The required xp for next level.
		/// </summary>
		private int requiredXpForNextLevel;
		/// <summary>
		/// The kills made by the player.
		/// </summary>
		private int killsMade;
		/// <summary>
		/// The deaths encountered by player.
		/// </summary>
		private int deathsEncountered;
		/// <summary>
		/// The total wins of the player.
		/// </summary>
		private int totalWins;
		/// <summary>
		/// The total loses of the player.
		/// </summary>
		private int totalLoses;
		/// <summary>
		/// The win to lose ratio.
		/// </summary>
		private float winToLoseRatio;
		#endregion

		private void Start () 
		{
			loadDataToDisplay ();
			assignDataToUI ();
		}

		/// <summary>
		/// This function assigns data to UI
		/// </summary>
		private void assignDataToUI()
		{
			currentLevelText.text = "Level: " + currentLevel.ToString ();
			killsMadeText.text = "Kills : " + killsMade.ToString ();
			deathsEncounteredText.text = "Deaths : " + deathsEncountered.ToString ();
			winToLoseRatioText.text = "W/L Ratio : " + winToLoseRatio.ToString ();
			xpRequiredForNextLevelSlider.minValue = 0;
			xpRequiredForNextLevelSlider.maxValue = requiredXpForNextLevel;
			xpRequiredForNextLevelSlider.value = additionalXpOfUser;
		}

		/// <summary>
		/// This function loads the data to be displayed on UI
		/// </summary>
		private void loadDataToDisplay()
		{
			currentLevel = UserManager.Stats [(int)Stat.Level];
			additionalXpOfUser = UserManager.Stats [(int)Stat.AdditionalXp];
			killsMade = UserManager.Stats [(int)Stat.Kills];
			deathsEncountered = UserManager.Stats [(int)Stat.Deaths];
			totalWins = UserManager.Stats [(int)Stat.Won];
			totalLoses = UserManager.Stats [(int)Stat.Lost];
			if(totalLoses == 0)
			{
				winToLoseRatio = totalWins > 0 ? totalWins : 0;
			}
			else
			{
				winToLoseRatio = (totalWins / totalLoses);
			}
			requiredXpForNextLevel = SMSavePlayerProgress.calculateTotalXpRequiredForNextLevel (currentLevel);
		}
	}
}
