//
//  SMScoreBoardUIManager.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 30/07/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SandeepMattepu.Multiplayer;

namespace SandeepMattepu.UI
{
	/// <summary>
	/// This class will update the score board UI in real time
	/// </summary>
	public class SMScoreBoardUIManager : MonoBehaviour 
	{
		/// <summary>
		/// This acts as a data packet
		/// </summary>
		protected class PlayerScoreUI
		{
			public string name;
			public int playerID;
			public int index;
			public int teamIndex = 0;
			public int score;
			public int deaths;

			public PlayerScoreUI()
			{}

			public PlayerScoreUI(PlayerScoreUI playerScoreUI)
			{
				name = playerScoreUI.name;
				playerID = playerScoreUI.playerID;
				index = playerScoreUI.index;
				teamIndex = playerScoreUI.teamIndex;
				score = playerScoreUI.score;
				deaths = playerScoreUI.deaths;
			}
		}

		/// <summary>
		/// The players UI text to show their scores in free for all.
		/// </summary>
		[SerializeField]
		protected Text[] playersTextUI;
		/// <summary>
		/// This holds players data packet as list
		/// </summary>
		protected List<PlayerScoreUI> playerScoreAndUI = new List<PlayerScoreUI>();
		/// <summary>
		/// The type of the multiplayer rules in the game.
		/// </summary>
		protected MPGameTypes multiplayerType;

		protected void Awake () 
		{
			playerScoreAndUI.Clear ();
			SMMultiplayerGame.OnGameRulesLoaded += OnRulesCreated;
		}

		protected virtual void OnDestroy()
		{
			SMMultiplayerGame.OnGameRulesLoaded -= OnRulesCreated;
			SMMultiplayerGame.INSTANCE.OnScoreChange -= OnScoreChange;
		}

		/// <summary>
		/// This gets called when multiplayer rules have been created in scene
		/// </summary>
		protected virtual void OnRulesCreated()
		{
			multiplayerType = SMMultiplayerGame.gameType;
			PhotonPlayer[] players = PhotonNetwork.playerList;
			int index = 0;
			foreach(PhotonPlayer player in players)
			{
				PlayerScoreUI playerSU = new PlayerScoreUI ();
				playerSU.name = player.NickName;
				playerSU.playerID = player.ID;
				playerSU.index = index;
				index++;
				playerSU.score = 0;
				playerSU.deaths = 0;
				playerScoreAndUI.Add (playerSU);
			}
			SMMultiplayerGame.INSTANCE.OnScoreChange += OnScoreChange;
		}

		protected virtual void OnScoreChange(object sender, int whoKilled, int whoDied)
		{
			foreach(PlayerScoreUI psu in playerScoreAndUI)
			{
				if(psu.playerID == whoDied)
				{
					int deaths = psu.deaths;
					deaths += 1;
					psu.deaths = deaths;
				}
				else if(psu.playerID == whoKilled)
				{
					int score = psu.score;
					score += 1;
					psu.score = score;
				}
			}
		}
	}
}
