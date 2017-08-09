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
	public class SMUIManager : MonoBehaviour 
	{
		public RectTransform areYouSureExitPanel;
		public Text victoryText;
		public Text defeatText;
		public Text nextMapIn;
		public string loadSceneAfterGame;
		public int gameStartsInSeconds;
		public AudioClip victorySound;
		public AudioClip defeatSound;
		public AudioSource audioSource;

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
			SceneManager.LoadScene (sceneName);
		}

		public void noSureToExitPressed()
		{
			areYouSureExitPanel.gameObject.SetActive (false);
		}

		private void OnGameOver()
		{
			StartCoroutine ("tickCountDownForRoomScene");
			if(audioSource.isPlaying)
			{
				audioSource.Stop ();
			}
			AudioClip clipTobePlayed = null;
			if(SMPlayerSpawnerAndAssigner.gameType == MPGameTypes.FREE_FOR_ALL)
			{
				if(SMFreeForAll.IsLocalPlayerLeading)
				{
					victoryText.gameObject.SetActive (true);
					clipTobePlayed = victorySound;
				}
				else
				{
					defeatText.gameObject.SetActive (true);
					clipTobePlayed = defeatSound;
				}
			}
			else if(SMPlayerSpawnerAndAssigner.gameType == MPGameTypes.TEAM_DEATH_MATCH)
			{
				if(SMFreeForAll.IsLocalPlayerLeading)
				{
					victoryText.gameObject.SetActive (true);
					clipTobePlayed = victorySound;
				}
				else
				{
					defeatText.gameObject.SetActive (true);
					clipTobePlayed = defeatSound;
				}
			}
			audioSource.clip = clipTobePlayed;
			audioSource.Play ();
		}

		IEnumerator tickCountDownForRoomScene()
		{
			nextMapIn.gameObject.SetActive (true);
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
	}	
}
