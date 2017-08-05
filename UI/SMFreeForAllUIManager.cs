//
//  SMFreeForAllUIManager.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 04/08/17.
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
	/// This class arranges text UI to show scores to player
	/// </summary>
	public class SMFreeForAllUIManager : SMScoreBoardUIManager
	{
		void Start()
		{
			if(multiplayerType == MPGameTypes.TEAM_DEATH_MATCH)
			{
				foreach(Text textUI in playersTextUI)
				{
					textUI.gameObject.SetActive (false);
				}
			}
		}

		protected override void OnRulesCreated ()
		{
			if(SMMultiplayerGame.gameType == MPGameTypes.FREE_FOR_ALL)
			{
				base.OnRulesCreated ();
				int unUsedUILength = Mathf.Abs(playersTextUI.Length - PhotonNetwork.playerList.Length);
				for(; unUsedUILength > 0; unUsedUILength--)
				{
					playersTextUI [playersTextUI.Length - unUsedUILength].gameObject.SetActive (false);
				}
				assignUIAtBeginning ();
			}
			else
			{
				// To avoid error
				SMMultiplayerGame.INSTANCE.OnScoreChange += OnScoreChange;
			}
		}

		protected override void OnScoreChange (object sender, int whoKilled, int whoDied)
		{
			if (SMMultiplayerGame.gameType == MPGameTypes.FREE_FOR_ALL) 
			{
				base.OnScoreChange (sender, whoKilled, whoDied);
				int requiredIndex = 0;
				foreach(PlayerScoreUI psui in playerScoreAndUI)
				{
					if(psui.playerID == whoKilled)
					{
						requiredIndex = psui.index;
						playersTextUI [psui.index].text = psui.name + " " + psui.score + "/" + psui.deaths;
					}
					else if(psui.playerID == whoDied)
					{
						playersTextUI [psui.index].text = psui.name + " " + psui.score + "/" + psui.deaths;
					}
				}
				// Below code is to arrange the position of the scoreboard
				PlayerScoreUI theEffectedPlayer = playerScoreAndUI [requiredIndex];
				foreach(PlayerScoreUI psui in playerScoreAndUI)
				{
					if(theEffectedPlayer.score > psui.score)
					{
						int tempindex = theEffectedPlayer.index;
						theEffectedPlayer.index = psui.index;
						psui.index = tempindex;

						playersTextUI [theEffectedPlayer.index].text = 
							theEffectedPlayer.name + " " + theEffectedPlayer.score + "/" + theEffectedPlayer.deaths;
						playersTextUI [psui.index].text = 
							psui.name + " " + psui.score + "/" + psui.deaths;
						break;
					}
				}
			}
		}

		private void assignUIAtBeginning()
		{
			for(int i = 0; i < playersTextUI.Length; i++)
			{
				foreach(PlayerScoreUI psu in playerScoreAndUI)
				{
					if(psu.index == i)
					{
						playersTextUI [i].text = psu.name + " " + psu.score + "/" + psu.deaths;
						break;
					}
				}
			}
		}
	}	
}
