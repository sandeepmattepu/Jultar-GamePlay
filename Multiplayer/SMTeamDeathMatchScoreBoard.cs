//
//  SMTeamDeathMatchScoreBoard.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 13/05/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeStage.AntiCheat.ObscuredTypes;

namespace SandeepMattepu.Multiplayer
{
	/// <summary>
	/// This class is responsible to display scores on the screen made by each player from different teams. Currently it
	/// supports only two teams
	/// </summary>
	public class SMTeamDeathMatchScoreBoard : MonoBehaviour 
	{
		/// <summary>
		/// All the UI Text which shows score made by individual player in team one
		/// </summary>
		public Text[] teamOneScoreTextsInOrder;
		/// <summary>
		/// The team one total score text.
		/// </summary>
		public Text teamOneTotalScoreText;
		/// <summary>
		/// All the UI Text which shows score made by individual player in team two
		/// </summary>
		public Text[] teamTwoScoreTextsInOrder;
		/// <summary>
		/// The team two total score text.
		/// </summary>
		public Text teamTwoTotalScoreText;
		/// <summary>
		/// The team one's scores data.
		/// </summary>
		private List<SMPlayerScoreData> teamOneData = new List<SMPlayerScoreData>();
		/// <summary>
		/// The team two's score data.
		/// </summary>
		private List<SMPlayerScoreData> teamTwoData = new List<SMPlayerScoreData>();
		/// <summary>
		/// The list of all players scores data.
		/// </summary>
		private List<SMPlayerScoreData> playerScoresData = new List<SMPlayerScoreData>();
		/// <summary>
		/// Total score made by team 1
		/// </summary>
		private int teamOneTotalScore = 0;
		/// <summary>
		/// Total score made by team two
		/// </summary>
		private int teamTwoTotalScore = 0;

		// Use this for initialization
		void Start () 
		{
			splitPlayerScoresToTeams ();
			showTeamOneScore ();
			showTeamTwoScore ();
		}

		/// <summary>
		/// This function collects data about players and splits them into teams
		/// </summary>
		private void splitPlayerScoresToTeams()
		{
			foreach (KeyValuePair<int, string> playerNamePair in SMTeamDeathMatch.PlayersIdAndName)
			{
				foreach (KeyValuePair<int, ObscuredInt> playerScoresPair in SMTeamDeathMatch.PlayersIdAndScore)
				{
					if (playerNamePair.Key == playerScoresPair.Key)
					{
						playerScoresData.Add(
							new SMPlayerScoreData(playerNamePair.Key, playerNamePair.Value, playerScoresPair.Value));
						break;
					}
				}
			}

			foreach(SMPlayerScoreData player in playerScoresData)
			{
				foreach(KeyValuePair<int,int> valuePair in SMTeamDeathMatch.PlayerIdAndTeamIndex)
				{
					if(player.ID == valuePair.Key)
					{
						player.team = valuePair.Value;
						break;
					}
				}
			}

			foreach(SMPlayerScoreData player in playerScoresData)
			{
				if(player.ID == 1)
				{
					teamOneData.Add (player);
				}
				else if(player.ID == 2)
				{
					teamTwoData.Add (player);
				}
			}

			foreach(SMPlayerScoreData player in teamOneData)
			{
				teamOneTotalScore += player.score;
			}
			teamOneTotalScoreText.text = "Total Score : " + teamOneTotalScore.ToString ();

			foreach(SMPlayerScoreData player in teamTwoData)
			{
				teamTwoTotalScore += player.score;
			}
			teamTwoTotalScoreText.text = "Total Score : " + teamTwoTotalScore.ToString ();
		}

		/// <summary>
		/// This function displays team one score
		/// </summary>
		private void showTeamOneScore ()
		{
			for (int i = 0; i < teamOneData.Count; i++) 
			{
				for (int j = 0; j < teamOneData.Count; j++) 
				{
					if (teamOneData [i].score > teamOneData [j].score) 
					{
						SMPlayerScoreData temp = teamOneData [i];
						teamOneData.RemoveAt (i);
						teamOneData.Insert (j, temp);
					}
				}
			}

			for (int i = 0; i < teamOneData.Count; i++) 
			{
				teamOneScoreTextsInOrder [i].text = (i + 1).ToString () + ")\t" + teamOneData [i].name + "-" + teamOneData [i].score;
			}
		}

		/// <summary>
		/// This function displays team two score
		/// </summary>
		private void showTeamTwoScore()
		{
			for (int i = 0; i < teamTwoData.Count; i++) 
			{
				for (int j = 0; j < teamTwoData.Count; j++) 
				{
					if (teamTwoData [i].score > teamTwoData [j].score) 
					{
						SMPlayerScoreData temp = teamTwoData [i];
						teamTwoData.RemoveAt (i);
						teamTwoData.Insert (j, temp);
					}
				}
			}

			for (int i = 0; i < teamTwoData.Count; i++) 
			{
				teamTwoScoreTextsInOrder [i].text = (i + 1).ToString () + ")\t" + teamTwoData [i].name + "-" + teamTwoData [i].score;
			}
		}

	}
		
}