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
		void hideUIInNonContext()
		{
			if(multiplayerType != MPGameTypes.FREE_FOR_ALL)
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
			hideUIInNonContext ();
			hideAllUI ();
		}

		protected override void OnScoreChange (object sender, int whoKilled, int whoDied)
		{
			if (SMMultiplayerGame.gameType == MPGameTypes.FREE_FOR_ALL) 
			{
				base.OnScoreChange (sender, whoKilled, whoDied);
				foreach(PlayerScoreUI psui in playerScoreAndUI)
				{
					if(psui.playerID == whoKilled)
					{
						playersTextUI [psui.uiIndex].text = psui.name + " " + psui.score + "/" + psui.deaths;
					}
					else if(psui.playerID == whoDied)
					{
						playersTextUI [psui.uiIndex].text = psui.name + " " + psui.score + "/" + psui.deaths;
					}
				}
				// Below code is to arrange the position of the scoreboard
				playerScoreAndUI.Sort();
				for(int i = 0; i < playerScoreAndUI.Count; i++)
				{
					playerScoreAndUI [i].uiIndex = i;
					playersTextUI [i].text = 
						playerScoreAndUI [i].name + " " + playerScoreAndUI [i].score + "/" + playerScoreAndUI [i].deaths;
				}
			}
		}

		public override void showScoreBoardButtonPressed ()
		{
			if(SMMultiplayerGame.gameType == MPGameTypes.FREE_FOR_ALL)
			{
				base.showScoreBoardButtonPressed ();
			}
		}

		private void assignUIAtBeginning()
		{
			for(int i = 0; i < playersTextUI.Length; i++)
			{
				foreach(PlayerScoreUI psu in playerScoreAndUI)
				{
					if(psu.uiIndex == i)
					{
						playersTextUI [i].text = psu.name + " " + psu.score + "/" + psu.deaths;
						break;
					}
				}
			}
		}
	}	
}
