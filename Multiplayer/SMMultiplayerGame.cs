//
//  SMMultiplayerGame.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 02/04/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using UnityEngine;
using System.Collections.Generic;
using SandeepMattepu.UI;

namespace SandeepMattepu.Multiplayer
{
	/// <summary>
	/// This class acts as a base layer for all the game types for a multiplayer game
	/// </summary>
	public class SMMultiplayerGame : MonoBehaviour
	{
		/// <summary>
		/// If he is golden player then he has special abilities
		/// </summary>
		public static bool isGoldenPlayer = false;
		/// <summary>
		/// The xp made by the player after kill.
		/// </summary>
		public int xpMadeAfterKill = 10;
		/// <summary>
		/// The total xp made by local player.
		/// </summary>
		private int totalXpMadeByPlayer = 0;
		public int TotalXpMadeByPlayer {
			get {
				return totalXpMadeByPlayer;
			}
		}

		/// <summary>
		/// The scene name after the end of multiplayer game.
		/// </summary>
		public string sceneNameAfterEndOfGame;
		/// <summary>
		/// This dictionary holds player names as values with player id as key
		/// </summary>
		protected static Dictionary<int, string> playersIdAndName = new Dictionary<int, string>();
		/// <summary>
		/// This dictionary holds player names as values with player id as key
		/// </summary>
		/// <value>The name of the players identifier and.</value>
		public static Dictionary<int, string> PlayersIdAndName
		{ get { return playersIdAndName;} }
		/// <summary>
		/// The current game type player is playing
		/// </summary>
		public static MPGameTypes gameType = MPGameTypes.NOT_INITIALIZED;
		/// <summary>
		/// The instance of game type player is currently playing.
		/// </summary>
		protected static SMMultiplayerGame instance = null;
		/// <summary>
		/// The instance of game type player is currently playing.
		/// </summary>
		public static SMMultiplayerGame INSTANCE
		{
			get { return instance;	}
		}
		/// <summary>
		/// The time of the game session.
		/// </summary>
		public float gameSessionTime;
		/// <summary>
		/// The time from which multiplayer game started.
		/// </summary>
		protected float gameTimer = 0.0f;
		/// <summary>
		/// The time from which multiplayer game started.
		/// </summary>
		public float GameTimer {
			get {
				return gameTimer;
			}
		}

		/// <summary>
		/// This value will tell whether to limit number of kills in multiplayer game
		/// </summary>
		public bool hasKillsLimit;
		/// <summary>
		/// This value determines whether the player is allowed to respawn in the game or not
		/// </summary>
		public bool isRespwanAllowed = true;
		/// <summary>
		/// The max number of kills to end game.
		/// </summary>
		public int maxKillsToEndGame;
		/// <summary>
		/// This value tells whether multiplayer is team match.
		/// </summary>
		public bool isTeamMatch;
		/// <summary>
		/// Number of teams in multiplayer game
		/// </summary>
		public int noOfTeams;
		/// <summary>
		/// This value tells whether friendly fire is applicable in the game or not
		/// </summary>
		public bool friendlyFire;
		/// <summary>
		/// The no of players in multiplayer game
		/// </summary>
		public int noOfPlayers;
		/// <summary>
		/// The kill streak of local player.
		/// </summary>
		private int killStreak = 0;
		public int KillStreak {
			get {
				return killStreak;
			}
		}

		/// <summary>
		/// This holds reference to the local player in the game
		/// </summary>
		public SMPlayerIdentifier localPlayer;
		/// <summary>
		/// This event is raised when a person makes a score
		/// </summary>
		public event notifyScoreChange OnScoreChange;
		/// <summary>
		/// Occurs when kill streak has been reset.
		/// </summary>
		public event onKillStreakChange OnKillStreakChange;

		/// <summary>
		/// Reports the score for particular game type.
		/// </summary>
		/// <param name="ID">ID of the player who made damage.</param>
		public virtual void reportScore(int ID)
		{
			if(PhotonNetwork.player.ID == ID)
			{
				SMShowXpMadeInstantly.addXPToQueue(xpMadeAfterKill);
				killStreak += 1;
				totalXpMadeByPlayer += xpMadeAfterKill;
				if(OnKillStreakChange != null)
				{
					OnKillStreakChange (killStreak);
				}
			}
			if(OnScoreChange != null)
			{
				OnScoreChange (this, ID);
			}
		}

		/// <summary>
		/// Resets the local player kill streak.
		/// </summary>
		public virtual void resetLocalPlayerKillStreak()
		{
			killStreak = 0;
			if(OnKillStreakChange != null)
			{
				OnKillStreakChange (0);
			}
		}

		/// <summary>
		/// Reduces the health streak by.
		/// </summary>
		/// <param name="byValue">By value.</param>
		public void reduceHealthStreakBy(int byValue)
		{
			killStreak = (byValue >= killStreak) ? 0 : (killStreak - byValue);
			if(OnKillStreakChange != null)
			{
				OnKillStreakChange (killStreak);
			}
		}
	}

	/// <summary>
	/// This enum describes the game type that is used in mp
	/// </summary>
	public enum MPGameTypes
	{
		NOT_INITIALIZED,
		FREE_FOR_ALL,
		TEAM_DEATH_MATCH
	}

	public delegate void notifyScoreChange(object sender, int ID);
	public delegate void onKillStreakChange(int killStreak);
}
