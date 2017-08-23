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
using CodeStage.AntiCheat.ObscuredTypes;

namespace SandeepMattepu.Multiplayer
{
	/// <summary>
	/// This class acts as a base layer for all the game types for a multiplayer game
	/// </summary>
	public class SMMultiplayerGame : MonoBehaviour
	{
		/// <summary>
		/// This occurs when game has met end condititon like when reached kill limits, time is over
		/// </summary>
		public static event onGameRulesCreated OnGameOver;
		/// <summary>
		/// If he is golden player then he has special abilities
		/// </summary>
		public static bool isGoldenPlayer = true;
		/// <summary>
		/// The xp made by the player after kill.
		/// </summary>
		public ObscuredInt xpMadeAfterKill = 10;
		/// <summary>
		/// The total xp made by local player.
		/// </summary>
		private ObscuredInt totalXpMadeByPlayer = 0;
		public ObscuredInt TotalXpMadeByPlayer {
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
		/// This dictionary holds number of deaths a player gone through
		/// </summary>
		protected static Dictionary<int, ObscuredInt> playerIdAndDeaths = new Dictionary<int, ObscuredInt>();
		/// <summary>
		/// This dictionary holds player names as values with player id as key
		/// </summary>
		public static Dictionary<int, ObscuredInt> PlayerIdAndDeaths
		{ get { return playerIdAndDeaths;}	}

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
		public ObscuredBool friendlyFire;
		/// <summary>
		/// The no of players in multiplayer game
		/// </summary>
		public int noOfPlayers;
		/// <summary>
		/// The kill streak of local player.
		/// </summary>
		private ObscuredInt killStreak = 0;
		public ObscuredInt KillStreak {
			get {
				return killStreak;
			}
		}
		/// <summary>
		/// This becomes true after game countdown is finished at the beginning
		/// </summary>
		private ObscuredBool isGameStarter = false;
		public ObscuredBool IsGameStarter {
			get {
				return isGameStarter;
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
		/// Occurs when game rules are loaded in the game.
		/// </summary>
		public static event onGameRulesCreated OnGameRulesLoaded;
		/// <summary>
		/// This represents whether the game is over or not
		/// </summary>
		private static bool isGameOver = false;

		public static bool IsGameOver {
			get {
				return isGameOver;
			}
		}

		protected virtual void Start()
		{
			isGameOver = false;
			if(OnGameRulesLoaded != null)
			{
				OnGameRulesLoaded ();
			}
		}

		protected virtual void Update()
		{
			checkGameTime ();
		}

		/// <summary>
		/// This function checs wether the time of the game has reached max time of the game
		/// </summary>
		protected virtual void checkGameTime()
		{
			if(IsGameStarter)
			{
				if (gameTimer >= gameSessionTime)
				{
					gameOver ();
				}
				else
				{
					gameTimer += Time.deltaTime;
				}
			}
		}

		/// <summary>
		/// Reports the score for particular game type.
		/// </summary>
		/// <param name="whoKilledID">ID of the player who made damage.</param>
		/// <param name="whoDiedID">ID of the player who died.</param>
		public virtual void reportScore(int whoKilledID, int whoDiedID)
		{
			if(!isGameOver)
			{
				addDeathInformation (whoDiedID);
				if(PhotonNetwork.player.ID == whoKilledID)
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
					OnScoreChange (this, whoKilledID, whoDiedID);
				}
			}
		}

		/// <summary>
		/// Adds the death information of particular ID of the player.
		/// </summary>
		/// <param name="ID">ID of the player who died</param>
		private void addDeathInformation(int ID)
		{
			ObscuredInt numberOfDeaths;
			if(playerIdAndDeaths.TryGetValue (ID, out numberOfDeaths))
			{
				numberOfDeaths += 1;
				playerIdAndDeaths.Remove (ID);
				playerIdAndDeaths.Add (ID, numberOfDeaths);
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
		public void reduceKillStreakBy(int byValue)
		{
			killStreak = (byValue >= killStreak) ? 0 : (killStreak - byValue);
			if(OnKillStreakChange != null)
			{
				OnKillStreakChange (killStreak);
			}
		}

		/// <summary>
		/// Call this function to raise event that belongs to SMMultiplayerGame class
		/// </summary>
		protected void gameOver()
		{
			if(OnGameOver != null && !isGameOver)
			{
				OnGameOver ();
			}
			isGameOver = true;
		}

		/// <summary>
		/// This function will make game rules to start operating
		/// </summary>
		public void startTheGame()
		{
			isGameStarter = true;
		}

		/// <summary>
		/// Sets the game timer.
		/// </summary>
		/// <param name="timerValue">Timer value.</param>
		public void setGameTimer(float timerValue)
		{
			gameTimer = timerValue;
		}
	}

	/// <summary>
	/// This enum describes the game type that is used in mp
	/// </summary>
	public enum MPGameTypes
	{
		NOT_INITIALIZED = 0,
		FREE_FOR_ALL,
		TEAM_DEATH_MATCH
	}

	public delegate void notifyScoreChange(object sender, int whoKilled, int whoDied);
	public delegate void onKillStreakChange(int killStreak);
	public delegate void onGameRulesCreated();
}
