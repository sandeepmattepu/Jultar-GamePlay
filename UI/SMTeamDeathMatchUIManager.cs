//
//  SMTeamDeathMatchUIManager.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 05/08/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SandeepMattepu.Multiplayer;
using UnityEngine.UI;

namespace SandeepMattepu.UI
{
	/// <summary>
	/// This class arranges the score board based in team death match context
	/// </summary>
	public class SMTeamDeathMatchUIManager : SMScoreBoardUIManager 
	{
		/// <summary>
		/// The team1 players in the game.
		/// </summary>
		private List<PlayerScoreUI> team1Players = new List<PlayerScoreUI>();
		/// <summary>
		/// The team2 players in the game.
		/// </summary>
		private List<PlayerScoreUI> team2Players = new List<PlayerScoreUI>();
		/// <summary>
		/// This text ui shows 3 Vs 3(example)
		/// </summary>
		[SerializeField]
		private Text pVsPText;
		/// <summary>
		/// Text UI for monio race total score.
		/// </summary>
		[SerializeField]
		private Text monioRaceTotalScore;
		/// <summary>
		/// Text UI for jagur race total score.
		/// </summary>
		[SerializeField]
		private Text jagurRaceTotalScore;
		/// <summary>
		/// The local player race.
		/// </summary>
		private MpPlayerRaceType localPlayerRace;
		/// <summary>
		/// The color of the friendly team.
		/// </summary>
		[SerializeField]
		private Color friendlyTeamColor;

		protected override void Start()
		{
			base.Start ();
			int halfTeam = PhotonNetwork.playerList.Length / 2;
			pVsPText.text = halfTeam + " VS " + (PhotonNetwork.playerList.Length - halfTeam);
		}

		void hideUIInNonContext()
		{
			if(multiplayerType != MPGameTypes.TEAM_DEATH_MATCH)
			{
				foreach(Text textUI in playersTextUI)
				{
					textUI.gameObject.SetActive (false);
				}
				pVsPText.gameObject.SetActive (false);
				monioRaceTotalScore.gameObject.SetActive (false);
				jagurRaceTotalScore.gameObject.SetActive (false);
			}
		}

		/// <summary>
		/// Shows the active player's hidden score UI.
		/// </summary>
		public override void showActivePlayersHiddenScoreUI ()
		{
			foreach(PlayerScoreUI psui in team1Players)
			{
				playersTextUI [psui.uiIndex].gameObject.SetActive (true);
			}

			foreach(PlayerScoreUI psui in team2Players)
			{
				playersTextUI [psui.uiIndex].gameObject.SetActive (true);
			}
		}

		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			SMTeamDeathMatch.OnPlayersSplittedToTeams -= onTeamSplitted;
		}

		protected override void OnRulesCreated ()
		{
			if(SMMultiplayerGame.gameType == MPGameTypes.TEAM_DEATH_MATCH)
			{
				base.OnRulesCreated ();
				SMTeamDeathMatch.OnPlayersSplittedToTeams += onTeamSplitted;
			}
			else
			{
				// To avoid error
				SMMultiplayerGame.INSTANCE.OnScoreChange += OnScoreChange;
				SMTeamDeathMatch.OnPlayersSplittedToTeams += onTeamSplitted;
			}
			hideUIInNonContext ();
			hideAllUI ();
		}

		protected override void OnScoreChange (object sender, int whoKilled, int whoDied)
		{
			if (SMMultiplayerGame.gameType == MPGameTypes.TEAM_DEATH_MATCH) 
			{
				base.OnScoreChange (sender, whoKilled, whoDied);
				PlayerScoreUI theEffectedPlayer = new PlayerScoreUI();
				foreach(PlayerScoreUI psui in playerScoreAndUI)
				{
					if(psui.playerID == whoKilled)
					{
						theEffectedPlayer = psui;
						playersTextUI [psui.uiIndex].text = psui.name + " " + psui.score + "/" + psui.deaths;
					}
					else if(psui.playerID == whoDied)
					{
						playersTextUI [psui.uiIndex].text = psui.name + " " + psui.score + "/" + psui.deaths;
					}
				}
				// Below code is to arrange the position of the scoreboard
				if(theEffectedPlayer.teamIndex == 1)
				{
					team1Players.Sort ();
					for(int i = 0; i < team1Players.Count; i++)
					{
						team1Players [i].uiIndex = i;
						playersTextUI [i].text = team1Players [i].name + " " + team1Players [i].score + "/" + team1Players [i].deaths;
						setUIColor (playersTextUI [i], MpPlayerRaceType.Monio, team1Players [i].playerID);
					}
				}
				else if(theEffectedPlayer.teamIndex == 2)
				{
					team2Players.Sort ();
					for(int i = 3; i < (team1Players.Count + 3); i++)
					{
						team2Players [i-3].uiIndex = i;
						playersTextUI [i].text = team2Players [i-3].name + " " + team2Players [i-3].score + "/" + team2Players [i-3].deaths;
						setUIColor (playersTextUI [i], MpPlayerRaceType.Jagur, team2Players [i-3].playerID);
					}
				}
				int team1ScoreData = SMTeamDeathMatch.Team1Score;
				int team2ScoreData = SMTeamDeathMatch.Team2Score;
				monioRaceTotalScore.text = "Monio"+ System.Environment.NewLine + team1ScoreData.ToString();
				jagurRaceTotalScore.text = "Jagur"+ System.Environment.NewLine + team2ScoreData.ToString();
			}
		}

		private void assignUIIndexToPlayers()
		{
			for(int i = 0; i < playersTextUI.Length; i++)
			{
				foreach(PlayerScoreUI psui in playerScoreAndUI)
				{
					if(psui.uiIndex == -1)		// This is to avoid multiple data packets to have same UI index
					{
						if(i < 3 && psui.teamIndex == 1)
						{
							psui.uiIndex = i;
							playersTextUI [i].text = psui.name + " " + psui.score + "/" + psui.deaths;
							setUIColor (playersTextUI [i], MpPlayerRaceType.Monio, psui.playerID);
							team1Players.Add (psui);
							break;
						}
						else if((i > 2 && i < 6) && psui.teamIndex == 2)
						{
							psui.uiIndex = i;
							playersTextUI [i].text = psui.name + " " + psui.score + "/" + psui.deaths;
							setUIColor (playersTextUI [i], MpPlayerRaceType.Jagur, psui.playerID);
							team2Players.Add (psui);
							break;
						}
					}
				}
			}
		}

		/// <summary>
		/// Sets the text user interface color based on player ID/Team ID.
		/// </summary>
		/// <param name="textUI">Text UI.</param>
		/// <param name="raceType">The race type</param>
		private void setUIColor(Text textUI, MpPlayerRaceType raceType)
		{
			if(raceType == localPlayerRace)
			{
				textUI.color = friendlyTeamColor;
			}
			else
			{
				textUI.color = enemyTeamColor;
			}
		}

		/// <summary>
		/// Sets the text user interface color based on player ID/Team ID.
		/// </summary>
		/// <param name="textUI">Text UI.</param>
		/// <param name="raceType">The race type</param>
		/// <param name="ID">The Id of the player</param>
		private void setUIColor(Text textUI, MpPlayerRaceType raceType, int ID)
		{
			if(ID == PhotonNetwork.player.ID)
			{
				textUI.color = localPlayerColor;
			}
			else if(raceType == localPlayerRace)
			{
				textUI.color = friendlyTeamColor;
			}
			else
			{
				textUI.color = enemyTeamColor;
			}
		}

		/// <summary>
		/// This function gets called when team has been successfully splitted.
		/// </summary>
		private void onTeamSplitted()
		{
			int teamMembersin1 = 0;
			int teamMembersin2 = 0;
			foreach(PlayerScoreUI psui in playerScoreAndUI)
			{
				foreach(KeyValuePair<int,int> playerIdAndTeamID in SMTeamDeathMatch.PlayerIdAndTeamIndex)
				{
					if(playerIdAndTeamID.Key == psui.playerID)
					{
						psui.teamIndex = playerIdAndTeamID.Value;
						psui.uiIndex = -1;		// To remove any attachments to UI
						if(playerIdAndTeamID.Value == 1)
						{
							teamMembersin1 += 1;
						}
						else if(playerIdAndTeamID.Value == 2)
						{
							teamMembersin2 += 1;
						}
						break;
					}
				}
			}

			// code to hide un used ui in team 1
			int unUsedUILength = Mathf.Abs(3 - teamMembersin1);
			for(; unUsedUILength > 0; unUsedUILength--)
			{
				playersTextUI [3 - unUsedUILength].gameObject.SetActive (false);
			}

			// code to hide un used ui in team 2
			unUsedUILength = Mathf.Abs(3 - teamMembersin2);
			for(; unUsedUILength > 0; unUsedUILength--)
			{
				playersTextUI [6 - unUsedUILength].gameObject.SetActive (false);
			}

			if(SMTeamDeathMatch.LocalPlayerTeamIndex == 1)
			{
				localPlayerRace = MpPlayerRaceType.Monio;
			}
			else if(SMTeamDeathMatch.LocalPlayerTeamIndex == 2)
			{
				localPlayerRace = MpPlayerRaceType.Jagur;
			}
			setUIColor (monioRaceTotalScore, MpPlayerRaceType.Monio);
			setUIColor (jagurRaceTotalScore, MpPlayerRaceType.Jagur);
			monioRaceTotalScore.text = "Monio" + System.Environment.NewLine + "0";
			jagurRaceTotalScore.text = "Jagur" + System.Environment.NewLine + "0";
			assignUIIndexToPlayers ();
		}
	}	
}
