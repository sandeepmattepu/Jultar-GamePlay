//
//  SMFreeForAllScoreBoard.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 03/04/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

namespace SandeepMattepu.Multiplayer
{
	/// <summary>
	/// This class will show the score board of the free for all after game is completed
	/// </summary>
	public class SMFreeForAllScoreBoard : MonoBehaviour
	{
		/// <summary>
		/// This array contains score holder from first place to last place text UI
		/// </summary>
		public Text[] scoreHolder;

		/// <summary>
		/// The list of all players scores data.
		/// </summary>
		private List<SMPlayerScoreData> playerScoresData = new List<SMPlayerScoreData>();

		// Use this for initialization
		void Start()
		{
			showScores();
		}

		/// <summary>
		/// This function displayes the score in the respective UI
		/// </summary>
		private void showScores()
		{
			foreach (KeyValuePair<int, string> playerNamePair in SMMultiplayerGame.PlayersIdAndName)
			{
				foreach (KeyValuePair<int, ObscuredInt> playerScoresPair in SMFreeForAll.PlayersIdAndScore)
				{
					if (playerNamePair.Key == playerScoresPair.Key)
					{
						playerScoresData.Add(
							new SMPlayerScoreData(playerNamePair.Key, playerNamePair.Value, playerScoresPair.Value));
						break;
					}
				}
			}

			for (int i = 0; i < playerScoresData.Count; i++)
			{
				for (int j = 0; j < playerScoresData.Count; j++)
				{
					if (playerScoresData[i].score > playerScoresData[j].score)
					{
						SMPlayerScoreData temp = playerScoresData[i];
						playerScoresData.RemoveAt(i);
						playerScoresData.Insert(j, temp);
					}
				}
			}

			for (int i = 0; i < playerScoresData.Count; i++)
			{
				scoreHolder[i].text = (i+1).ToString() + ")\t" + playerScoresData[i].name + "-" + playerScoresData[i].score;
			}
		}
	}

	public class SMPlayerScoreData
	{
		public int ID;
		public string name;
		public int score;
		public int team;

		public SMPlayerScoreData(int id, string Name, int Score)
		{
			ID = id;
			name = Name;
			score = Score;
			team = 0;
		}

		public SMPlayerScoreData(int id, string Name, int Score, int TeamIndex)
		{
			ID = id;
			name = Name;
			score = Score;
			team = TeamIndex;
		}
	}
}