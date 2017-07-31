//
//  SMMultiplayerScoreManager.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 02/04/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SandeepMattepu.Multiplayer
{
	/// <summary>
	/// This class acts as central scoring system which syncs all the players about the scores in the game
	/// </summary>
	public class SMMultiplayerScoreManager : MonoBehaviour 
	{
		private SMMultiplayerGame gameType = null;

		private void OnEvent(byte eventCode, object content, int senderID)
		{
			byte[] dataInContent;
			if(eventCode == (int)MultiplayerEvents.FreeForAll || eventCode == (int)MultiplayerEvents.TeamDeathMatch)
			{
				dataInContent = (byte[])content;
				if(eventCode == (int)MultiplayerEvents.FreeForAll)		// Means Free for all game
				{
					int scoreReciever = (int)dataInContent[0];
					int diedID = (int)dataInContent [1];
					gameType.reportScore(scoreReciever, diedID);
				}
				else if(eventCode == (int)MultiplayerEvents.TeamDeathMatch)	// Means Team Death match
				{
					int scoreReciever = (int)dataInContent [0];
					int diedID = (int)dataInContent [1];
					gameType.reportScore (scoreReciever, diedID);
				}
			}
		}

		public void setGameType(SMMultiplayerGame GameType)
		{
			gameType = GameType;
			PhotonNetwork.OnEventCall += this.OnEvent;
		}

		void OnDestroy()
		{
			PhotonNetwork.OnEventCall -= this.OnEvent;
		}
	}

	/// <summary>
	/// Multiplayer events that helps us to understand custom events in photon.
	/// </summary>
	public enum MultiplayerEvents
	{
		FreeForAll = 1, TeamDeathMatch, Announcements, Votes
	}
}
