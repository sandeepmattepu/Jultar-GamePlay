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
using CodeStage.AntiCheat.ObscuredTypes;

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
		protected class PlayerScoreUI : System.IComparable
		{
			public string name;
			public int playerID;
			public int uiIndex;
			public int teamIndex = 0;
			public int score;
			public int deaths;

			public PlayerScoreUI()
			{}

			public PlayerScoreUI(PlayerScoreUI playerScoreUI)
			{
				name = playerScoreUI.name;
				playerID = playerScoreUI.playerID;
				uiIndex = playerScoreUI.uiIndex;
				teamIndex = playerScoreUI.teamIndex;
				score = playerScoreUI.score;
				deaths = playerScoreUI.deaths;
			}

			#region IComparable implementation

			public int CompareTo (object obj)
			{
				if(obj == null)
				{
					return 1;
				}

				PlayerScoreUI otherPsui = obj as PlayerScoreUI;
				return -(this.score - otherPsui.score);
			}

			#endregion
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
		/// <summary>
		/// The color of the enemy team.
		/// </summary>
		[SerializeField]
		protected Color enemyTeamColor;
		/// <summary>
		/// The color of the local player.
		/// </summary>
		[SerializeField]
		protected Color localPlayerColor;
		/// <summary>
		/// This becomes true when game is already showing scoreboard.
		/// </summary>
		protected bool showButtonAlreadyCalled = false;
		/// <summary>
		/// The amount of time score board should be shown on screen.
		/// </summary>
		protected float timeScoreBoardShouldBeShownOnScreen = 3.0f;

		protected virtual void Start()
		{
			SMPlayerSpawnerAndAssigner.OnPlayerRespawned += hideAllUI;
			SMMultiplayerGame.OnGameOver += showActivePlayersHiddenScoreUI;
		}

		/// <summary>
		/// Shows the active player's hidden score UI.
		/// </summary>
		public virtual void showActivePlayersHiddenScoreUI()
		{
			foreach(PlayerScoreUI psui in playerScoreAndUI)
			{
				playersTextUI [psui.uiIndex].gameObject.SetActive (true);
			}
		}

		/// <summary>
		/// This function is called when show scoreboard button is pressed
		/// </summary>
		public virtual void showScoreBoardButtonPressed()
		{
			if(!showButtonAlreadyCalled)
			{
				showButtonAlreadyCalled = true;
				showActivePlayersHiddenScoreUI ();
				StartCoroutine ("autoHideUIAfterSomeTime");
			}
		}

		private IEnumerator autoHideUIAfterSomeTime()
		{
			yield return new WaitForSeconds (timeScoreBoardShouldBeShownOnScreen);
			hideAllUI ();
			showButtonAlreadyCalled = false;
		}

		/// <summary>
		/// This function hides all scoreboard
		/// </summary>
		protected virtual void hideAllUI()
		{
			foreach(Text ui in playersTextUI)
			{
				ui.gameObject.SetActive (false);
			}
		}

		protected void Awake () 
		{
			playerScoreAndUI.Clear ();
			SMMultiplayerGame.OnGameRulesLoaded += OnRulesCreated;
		}

		protected virtual void OnDestroy()
		{
			SMMultiplayerGame.OnGameRulesLoaded -= OnRulesCreated;
			SMMultiplayerGame.INSTANCE.OnScoreChange -= OnScoreChange;
			SMPlayerSpawnerAndAssigner.OnPlayerRespawned -= hideAllUI;
			SMMultiplayerGame.OnGameOver -= showActivePlayersHiddenScoreUI;
			SMMultiplayerGame.OnPlayerJoinedOrLeft -= refreshScoreBoard;
		}

		/// <summary>
		/// This gets called when multiplayer rules have been created in scene
		/// </summary>
		protected virtual void OnRulesCreated()
		{
			multiplayerType = SMMultiplayerGame.gameType;
			PhotonPlayer[] players = PhotonNetwork.playerList;
			int index = 0;
			foreach(KeyValuePair<int,ObscuredInt> pias in SMMultiplayerGame.PlayersIdAndScore)
			{
				PlayerScoreUI playerSU = new PlayerScoreUI ();
				string name = null;
				if(SMMultiplayerGame.PlayersIdAndName.TryGetValue(pias.Key, out name))
				{
					playerSU.name = name;
				}
				playerSU.playerID = pias.Key;
				playerSU.uiIndex = index;
				index++;
				playerSU.score = pias.Value;
				ObscuredInt deaths;
				if(SMMultiplayerGame.PlayerIdAndDeaths.TryGetValue(pias.Key, out deaths))
				{
					playerSU.deaths = deaths;
				}
				playerScoreAndUI.Add (playerSU);
			}
			SMMultiplayerGame.INSTANCE.OnScoreChange += OnScoreChange;
			SMMultiplayerGame.OnPlayerJoinedOrLeft += refreshScoreBoard;
		}

		/// <summary>
		/// Refreshs the score board whenever player joins or leaves the game.
		/// </summary>
		public virtual void refreshScoreBoard()
		{
			playerScoreAndUI.Clear ();
			PhotonPlayer[] players = PhotonNetwork.playerList;
			int index = 0;
			foreach (KeyValuePair<int,ObscuredInt> pias in SMMultiplayerGame.PlayersIdAndScore) 
			{
				PlayerScoreUI playerSU = new PlayerScoreUI ();
				string name = null;
				if (SMMultiplayerGame.PlayersIdAndName.TryGetValue (pias.Key, out name)) 
				{
					playerSU.name = name;
				}
				playerSU.playerID = pias.Key;
				playerSU.uiIndex = index;
				index++;
				playerSU.score = pias.Value;
				ObscuredInt deaths;
				if (SMMultiplayerGame.PlayerIdAndDeaths.TryGetValue (pias.Key, out deaths)) 
				{
					playerSU.deaths = deaths;
				}
				playerScoreAndUI.Add (playerSU);
			}
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
