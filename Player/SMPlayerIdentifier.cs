﻿//
//  SMPlayerIdentifier.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 02/04/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using UnityEngine;
using UnityEngine.UI;
using SandeepMattepu.Multiplayer;
using System.Collections;

/// <summary>
/// This class will identify the player in the network with name, id and other with attributes
/// </summary>
public class SMPlayerIdentifier : MonoBehaviour 
{
	/// <summary>
	/// The UI text which shows player name.
	/// </summary>
	public Text playerNameUI;
	/// <summary>
	/// The color of the local player in multiplayer context.
	/// </summary>
	[SerializeField]
	private Color localPlayerColor;
	/// <summary>
	/// The color of the players in same team.
	/// </summary>
	[SerializeField]
	private Color playersOfSameTeamColor;
	/// <summary>
	/// The enemy team players color in TDM or enemy players color in FFA.
	/// </summary>
	[SerializeField]
	[Tooltip("The enemy team player's color in TDM or enemy player's color in FFA.")]
	private Color enemyTeamOrEnemyPlayers;
	/// <summary>
	/// The photon view component attached to it
	/// </summary>
	private PhotonView photonViewComponenet;
	/// <summary>
	/// The game type the player is playing in MP
	/// </summary>
	[HideInInspector]
	public SMMultiplayerGame gameType;
	/// <summary>
	/// This bool value avoids multiple request for ending the game.
	/// </summary>
	private static bool requestToSendTheGameFinished = false;
	// Use this for initialization
	void Start()
	{
		photonViewComponenet = GetComponent<PhotonView>();
		playerNameUI.text = photonViewComponenet.owner.NickName;
		gameType = SMMultiplayerGame.INSTANCE;
		setPlayerUIColor(photonViewComponenet);

		if(!photonViewComponenet.isMine)
		{
			gameObject.GetComponent<Rigidbody> ().isKinematic = true;
			Destroy (GetComponent<AudioListener>());
			GetComponent<AudioSource> ().volume = 0.1f;
		}
	}

	/// <summary>
	/// This function sets the player's name UI Color based on the game rules.
	/// </summary>
	public void setPlayerUIColor(PhotonView photonView)
	{
		if (photonView.isMine)
		{
			playerNameUI.color = localPlayerColor;
		}
		else
		{
			StartCoroutine ("waitAndSetUIColor");
		}
	}

	private IEnumerator waitAndSetUIColor()
	{
		yield return new WaitUntil (() => (SMMultiplayerGame.INSTANCE != null && SMMultiplayerGame.INSTANCE.IsMultiplayerLoaded));

		if (SMMultiplayerGame.gameType == MPGameTypes.FREE_FOR_ALL)
		{
			playerNameUI.color = enemyTeamOrEnemyPlayers;
		}
		else if(SMMultiplayerGame.gameType == MPGameTypes.TEAM_DEATH_MATCH)
		{
			int idOfThisPlayer = photonViewComponenet.owner.ID;
			int teamIndexOfThisPlayer = 0;

			yield return new WaitUntil (() => ((SMTeamDeathMatch)SMMultiplayerGame.INSTANCE).HasPlayerSplittedToTeams);

			SMTeamDeathMatch.PlayerIdAndTeamIndex.TryGetValue (idOfThisPlayer, out teamIndexOfThisPlayer);
			int localPlayerTeam = SMTeamDeathMatch.LocalPlayerTeamIndex;

			if(localPlayerTeam != -1)
			{
				if(localPlayerTeam == teamIndexOfThisPlayer)
				{
					playerNameUI.color = playersOfSameTeamColor;
				}
				else
				{
					playerNameUI.color = enemyTeamOrEnemyPlayers;
				}
			}
		}
	}

	/// <summary>
	/// Reports the score made by particular ID of the player.
	/// </summary>
	/// <param name="ID">ID of the player to award the score.</param>
	public void reportScore(int ID)
	{
		if(PhotonNetwork.player.ID != ID)
		{
			sendScoreMadeBy(ID);
		}
	}

	/// <summary>
	/// This function will send message to Pun that multiplayer game has ended
	/// </summary>
	/// <param name="sceneName">Scene name.</param>
	public void showScoreBoard(string sceneName)
	{
		photonViewComponenet.RPC("endMPGame", PhotonTargets.All, sceneName);
	}

	/// <summary>
	/// This function recieves the message to end the game
	/// </summary>
	/// <param name="gmeType">Pass the game type.</param>
	public void sendEndGameMessage(MPGameTypes gmeType, string sceneName)
	{
		if(!requestToSendTheGameFinished)
		{
			if (gmeType == MPGameTypes.FREE_FOR_ALL)
			{
				photonViewComponenet.RPC("endMPGame", PhotonTargets.All, sceneName);
			}
		}
	}

	public void sendScoreMadeBy(int ID)
	{
		if(SMMultiplayerGame.gameType == MPGameTypes.FREE_FOR_ALL)
		{
			byte evCode = (int)MultiplayerEvents.FreeForAll;    // 1 denotes free for all
			byte[] content = new byte[] { (byte)ID, (byte)photonViewComponenet.owner.ID };    // Send who scored the points
			bool reliable = true;
			gameType.reportScore (ID, photonViewComponenet.owner.ID);		// Acknowledge score in local device
			PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
		}
		else if(SMMultiplayerGame.gameType == MPGameTypes.TEAM_DEATH_MATCH)
		{
			int playerTeamWhoKilledLocal = 0;
			SMTeamDeathMatch.PlayerIdAndTeamIndex.TryGetValue (ID, out playerTeamWhoKilledLocal);
			int localPlayerTeam = SMTeamDeathMatch.LocalPlayerTeamIndex;

			if(localPlayerTeam != -1)
			{
				if(localPlayerTeam != playerTeamWhoKilledLocal)
				{
					byte evCode = (int)MultiplayerEvents.TeamDeathMatch;    // 2 denotes team death match
					byte[] content = new byte[] { (byte)ID, (byte)photonViewComponenet.owner.ID };    // Send who scored the points
					bool reliable = true;
					gameType.reportScore (ID, photonViewComponenet.owner.ID);		// Acknowledge score in local device
					PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
				}
			}
		}
	}

	[PunRPC]
	public void endMPGame(string sceneName)
	{
		if (!requestToSendTheGameFinished)
		{
			requestToSendTheGameFinished = true;
			PhotonNetwork.LoadLevel(sceneName);
		}
	}
}
