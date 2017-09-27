//
//  SMUIManager.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 27/07/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SandeepMattepu.Multiplayer;

namespace SandeepMattepu.UI
{
	public class SMUIManager : Photon.PunBehaviour
	{
		public RectTransform areYouSureExitPanel;
		public Text victoryText;
		public Text defeatText;
		public Text nextMapIn;
		public string loadSceneAfterGame;
		public int gameStartsInSeconds;
		public AudioClip monioVictorySound;
		public AudioClip monioDefeatSound;
		public AudioClip jagurVictorySound;
		public AudioClip jagurDefeatSound;
		public bool showNextMapStartsIn = false;

		private string sceneToLoadWhenExittedFromGame;

		void Start()
		{
			areYouSureExitPanel.gameObject.SetActive (false);
			SMMultiplayerGame.OnGameOver += OnGameOver;
		}

		void OnDestroy()
		{
			SMMultiplayerGame.OnGameOver -= OnGameOver;
		}

		public void exitButtonPressed()
		{
			areYouSureExitPanel.gameObject.SetActive (true);
		}

		public void yesSureToExitPressed(string sceneName)
		{
			sceneToLoadWhenExittedFromGame = sceneName;
			SMMultiplayerGame.INSTANCE.gameObject.GetComponent<SMSavePlayerProgress> ().penalizeAndSavePlayerProgress ();
			PhotonNetwork.LeaveRoom ();
		}

		public void noSureToExitPressed()
		{
			areYouSureExitPanel.gameObject.SetActive (false);
		}

		private void OnGameOver()
		{
			StartCoroutine ("tickCountDownForRoomScene");
			AudioClip clipTobePlayed = null;
			if(SMPlayerSpawnerAndAssigner.gameType == MPGameTypes.FREE_FOR_ALL)
			{
				if(SMFreeForAll.IsLocalPlayerLeading)
				{
					victoryText.gameObject.SetActive (true);
					clipTobePlayed = monioVictorySound;
				}
				else
				{
					defeatText.gameObject.SetActive (true);
					clipTobePlayed = monioDefeatSound;
				}
			}
			else if(SMPlayerSpawnerAndAssigner.gameType == MPGameTypes.TEAM_DEATH_MATCH)
			{
				if(SMTeamDeathMatch.IsLocalPlayerTeamLeading)
				{
					victoryText.gameObject.SetActive (true);
					clipTobePlayed = SMTeamDeathMatch.LocalPlayerTeamIndex == 1 ? monioVictorySound : jagurVictorySound;
				}
				else
				{
					defeatText.gameObject.SetActive (true);
					clipTobePlayed = SMTeamDeathMatch.LocalPlayerTeamIndex == 1 ? monioDefeatSound : jagurDefeatSound;
				}
			}
			AudioPersister.Instance.ChangeBGM(clipTobePlayed);
		}

		IEnumerator tickCountDownForRoomScene()
		{
			nextMapIn.gameObject.SetActive (showNextMapStartsIn);
			while(gameStartsInSeconds > 0)
			{
				nextMapIn.text = "Next map starts in " + gameStartsInSeconds + " seconds";
				yield return new WaitForSeconds(gameStartsInSeconds);
				gameStartsInSeconds -= 1;
			}

			loadNextScene ();
		}

		private void loadNextScene()
		{
			if(PhotonNetwork.isMasterClient)
			{
				PhotonNetwork.room.IsOpen = true;
				PhotonNetwork.room.IsVisible = true;
				PhotonNetwork.LoadLevel (loadSceneAfterGame);
			}
		}

		public override void OnLeftRoom ()
		{
			SceneManager.LoadScene (sceneToLoadWhenExittedFromGame);
		}
	}	
}
