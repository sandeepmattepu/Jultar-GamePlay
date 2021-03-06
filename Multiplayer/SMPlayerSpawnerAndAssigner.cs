﻿//
//  SMPlayerSpawner.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 25/02/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using UnityEngine;
using UnityEngine.UI;
using SandeepMattepu.Weapon;
using SandeepMattepu;
using SandeepMattepu.Multiplayer;
using System.Collections;
using SandeepMattepu.UI;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine.Events;


/// <summary>
/// This class will spawn the players in the game at certain positions based on their index values in network and assignes proper values to
/// player components. This class also spawns game rules in the scene.
/// </summary>
public class SMPlayerSpawnerAndAssigner : Photon.PunBehaviour 
{
    /// <summary>
    /// This will decide which rules that need to be maintained in the game. Set this value before loading the game
    /// </summary>
    public static MPGameTypes gameType = MPGameTypes.TEAM_DEATH_MATCH;
	/// <summary>
	/// The player game object which will be spawned by photon
	/// </summary>
	public GameObject playerToBeSpawned;
	/// <summary>
	/// The monio spawn music.
	/// </summary>
	[SerializeField]
	private AudioClip monioSpawnMusic;
	/// <summary>
	/// The jagur spawn music.
	/// </summary>
	[SerializeField]
	private AudioClip jagurSpawnMusic;
	/// <summary>
	/// One of the game object in the array will get instantiated based on the game type player wants to play
	/// </summary>
	public GameObject[] gameRulesToBeSpawned;
	/// <summary>
	/// This is the time interval after which time gets synched.
	/// </summary>
	public float timeIntervalBetweenDataSynces;
	/// <summary>
	/// The game rules prefab that spawned.
	/// </summary>
	private SMMultiplayerGame gameRulesThatSpawned;
	/// <summary>
	/// This score manager acts as central scoring system which syncs all the players about the scores in the game.
	/// </summary>
	public SMMultiplayerScoreManager scoreManager;
	/// <summary>
	/// The spawn points in region 1.
	/// </summary>
	public Transform[] region1SpawnPoints;
	/// <summary>
	/// The spawn points in region 2.
	/// </summary>
	public Transform[] region2SpawnPoints;
	/// <summary>
	/// The camera which follows the player
	/// </summary>
	public Camera cameraPlayerShouldFollow;
	/// <summary>
	/// The movement stick which controls the player movement.
	/// </summary>
	public SMJoyStick movementStick;
	/// <summary>
	/// The orientation stick which controls the player movement.
	/// </summary>
	public SMJoyStick orientationStick;
	/// <summary>
	/// Grenade UI Button
	/// </summary>
	public SMGrenadeInput grenadeButton;
	/// <summary>
	/// The pick up button UI.
	/// </summary>
	public Button pickUpButton;
	/// <summary>
	/// The current weapon text UI which shows the current weapon player is holding.
	/// </summary>
	public Text currentWeaponText;
	/// <summary>
	/// The ammo details UI which shows number of bullets and clips that are present in the gun player is holding.
	/// </summary>
	public Text ammoDetailsUI;
	/// <summary>
	/// The lean button UI which when pressed will make monio lean or stand.
	/// </summary>
	public Button leanButtonUI;
	/// <summary>
	/// The hide button when pressed will make monio to hide or unhide in hidable region
	/// </summary>
	public Button hideButtonUI;
	/// <summary>
	/// This UI is the main UI where the player interacts with the game
	/// </summary>
	public GameObject[] playInteractableUI;
	/// <summary>
	/// Hides user interface even afer respawn.
	/// </summary>
	public GameObject[] hideUIEvenAferRespawn;
	/// <summary>
	/// Just respawn heading
	/// </summary>
	public Text respawnText;
	/// <summary>
	/// You died text.
	/// </summary>
	public Text youDiedText;
	/// <summary>
	/// The special weapon manager.
	/// </summary>
	public SMSpecialWeaponManager specialWeaponManager;
	/// <summary>
	/// Occurs when player respawnes in multiplayer context.
	/// </summary>
	public static event onGameRulesCreated OnPlayerRespawned;
	/// <summary>
	/// The area observer 1.
	/// </summary>
	public SMAreaObserver areaObserver1;
	/// <summary>
	/// The area observer 2.
	/// </summary>
	public SMAreaObserver areaObserver2;
	/// <summary>
	/// The UI represents when the match starts in
	/// </summary>
	public Text matchStartsInUI;
	/// <summary>
	/// This is initial count down before match start
	/// </summary>
	public ObscuredFloat timeDelayToStartMatch = 10;
	/// <summary>
	/// The respawn time for player who has operative helmet.
	/// </summary>
	public ObscuredFloat respawnTimeForOperative = 1.0f;
	/// <summary>
	/// The respawn time for player who has no helmets.
	/// </summary>
	public ObscuredFloat respawnTimeForNormalPlayers = 3.0f;
	/// <summary>
	/// The team death match user interface manager.
	/// </summary>
	public SMTeamDeathMatchUIManager teamDeathMatchUIManager;
	/// <summary>
	/// The free for all user interface manager.
	/// </summary>
	public SMFreeForAllUIManager freeForAllUIManager;
	/// <summary>
	/// This becomes true when we recieve time data from master for first time
	/// </summary>
	private ObscuredBool hasRecievedTimeDataFromMaster = false;
	/// <summary>
	/// The grenade user interface input.
	/// </summary>
	public SMGrenadeInput grenadeUIInput;
	/// <summary>
	/// This invokes when this client is not master and recieves data from master. Occures only when game is FFA format
	/// </summary>
	public UnityEvent freeForAllWhenRecievedDataAtBeginning;
	/// <summary>
	/// This invokes when this client is not master and recieves data from master. Occures only when game is TDM format
	/// </summary>
	public UnityEvent teamDeathMatchWhenRecieveDataAtBeginning;
	// Use this for initialization
	void Start () 
	{
		PhotonNetwork.OnEventCall += OnEvent;
		spawnGameRules();
		hideUIAfterDeath ();		// To make player in ideal state until countdown is finished
		StartCoroutine ("spawnPlayersAfterDelay");		// To fix double instantiation of the player
		SMTeamDeathMatch.OnGameOver += hideUIAfterDeath;
	}

	private void Update()
	{
		syncDataAfterFewSeconds ();
	}

	void OnDestroy()
	{
		SMTeamDeathMatch.OnGameOver -= hideUIAfterDeath;
		PhotonNetwork.OnEventCall -= OnEvent;
	}

	IEnumerator spawnPlayersAfterDelay()
	{
		yield return new WaitForSeconds (0.85f);
		spawnPlayers ();
	}

	// Used in below function
	private float tempGameTimeStorager = 0.0f;
	/// <summary>
	/// This function syncs data after every few seconds to make all player's data in sync
	/// </summary>
	private void syncDataAfterFewSeconds()
	{
		if(timeIntervalBetweenDataSynces > 0.0f)		// Other values means don't sync time
		{
			if(tempGameTimeStorager < timeIntervalBetweenDataSynces)
			{
				tempGameTimeStorager += Time.deltaTime;
			}
			else
			{
				tempGameTimeStorager = 0.0f;
				sendSignalToAllToRefreshData ();
			}
		}
	}

	/// <summary>
	/// This function will spawn the player and assigns appropriate values to player's components
	/// </summary>
	private void spawnPlayers()
	{
		object[] dataToTransfer = { 
			((int)SMProductEquipper.INSTANCE.CurrentHelmet)
		};
		GameObject player = PhotonNetwork.Instantiate (playerToBeSpawned.name, Vector3.zero, Quaternion.identity, 0
			, dataToTransfer) as GameObject;
		Transform playerPosition = placePlayerPositionAtStart ();
		player.transform.position = playerPosition.position;
		player.transform.rotation = playerPosition.rotation;

		if(player.GetComponent<PhotonView>().isMine)
		{
			SMPlayerController playerController = player.GetComponent<SMPlayerController> ();
			playerController.characterFocusedCamera = cameraPlayerShouldFollow;
			cameraPlayerShouldFollow.GetComponent<SMThirdPersonCamera> ().targetCameraShouldFocus = playerController.playerPartCamerToBeFocussed.transform;
			cameraPlayerShouldFollow.GetComponent<SMThirdPersonCamera> ().rotationKnob = playerController.playerPartCamerToBeFocussed.transform;

			player.transform.GetComponent<SMPlayerController> ().movementJoyStick = movementStick;
			player.transform.GetComponent<SMPlayerController> ().orientationJoyStick = orientationStick;

			player.transform.GetComponent<SMWeaponManager> ().pickUpButton = pickUpButton;
			pickUpButton.onClick.AddListener (player.GetComponent<SMWeaponManager> ().pickUpButtonPressed);
			pickUpButton.gameObject.SetActive (false);
			player.transform.GetComponent<SMWeaponManager> ().currentWeaponText = currentWeaponText;

			SMPlayerFiring playerFiring = player.GetComponent<SMPlayerFiring> ();
			playerFiring.ammoDetailsUI = ammoDetailsUI;
			playerFiring.grenadeInput = grenadeButton;
			grenadeUIInput.PlayerFiring = playerFiring;
			grenadeUIInput.PlayerGrenade = player.GetComponent<IKControl> ();

			player.transform.GetComponent<SMPlayerController> ().leanButton = leanButtonUI;
			player.transform.GetComponent<SMPlayerController> ().hideOrUnhideButton = hideButtonUI;

			player.transform.GetComponent<SMPlayerHealth>().spawnManager = this;
			player.GetComponent<SMPlayerHealth>().playInteractableUI = playInteractableUI;
			player.GetComponent<SMPlayerHealth> ().OnLocalPlayerDead += teamDeathMatchUIManager.showActivePlayersHiddenScoreUI;
			player.GetComponent<SMPlayerHealth> ().OnLocalPlayerDead += freeForAllUIManager.showActivePlayersHiddenScoreUI;

			gameRulesThatSpawned.localPlayer = player.GetComponent<SMPlayerIdentifier>();
			player.GetComponent<SMPlayerIdentifier>().gameType = gameRulesThatSpawned;
			gameRulesThatSpawned.localPlayer.setPlayerUIColor(player.GetComponent<PhotonView>());

			scoreManager.setGameType (gameRulesThatSpawned);
		}
		StartCoroutine ("tickCountDownAtBeginingOfGame");
	}

	IEnumerator tickCountDownAtBeginingOfGame()
	{
		if(PhotonNetwork.isMasterClient)
		{
			hasRecievedTimeDataFromMaster = true;
		}
		else
		{
			requestDataToMasterClient ();
		}
		matchStartsInUI.text = "Match starts in ...";

		yield return new WaitUntil (() => (hasRecievedTimeDataFromMaster || PhotonNetwork.isMasterClient));

		while(timeDelayToStartMatch > 0)
		{
			matchStartsInUI.text = "Match starts in " + ((int)timeDelayToStartMatch);
			timeDelayToStartMatch -= 1;
			yield return new WaitForSeconds (1.0f);
		}
		showUIAfterRespwan ();	// This makes hidden UI to show to player
		matchStartsInUI.gameObject.SetActive(false);
		gameRulesThatSpawned.startTheGame ();
	}

	/// <summary>
	/// This will decide which position the player need to be placed at start of the game
	/// </summary>
	/// <returns>The player position at start.</returns>
	private Transform placePlayerPositionAtStart()
	{
		if(gameType == MPGameTypes.FREE_FOR_ALL)
		{
			List<int> playerIDs = new List<int> ();
			int requiredIndex = 0;
			foreach(PhotonPlayer photonPlayer in PhotonNetwork.playerList)
			{
				playerIDs.Add(photonPlayer.ID);
			}
			playerIDs.Sort();
			int numberOfPlayersInEachRegion = playerIDs.Count / 2;
			for(int i = 0; i < playerIDs.Count; i++)
			{
				if(playerIDs[i] == PhotonNetwork.player.ID)
				{
					requiredIndex = i;
					break;
				}
			}

			if(requiredIndex < numberOfPlayersInEachRegion)		// Belongs to region one
			{
				return region1SpawnPoints [requiredIndex];
			}
			else                  // Belongs to region two
			{
				return region2SpawnPoints [requiredIndex - numberOfPlayersInEachRegion];
			}
		}
		else if(gameType == MPGameTypes.TEAM_DEATH_MATCH)
		{
			int localPlayerTeam = 1;
			int requiredIndex = 0;
			if(SMTeamDeathMatch.PlayerIdAndTeamIndex.TryGetValue (PhotonNetwork.player.ID, out localPlayerTeam))
			{
				List<int> teamPlayers = new List<int> ();
				foreach(KeyValuePair<int,int> vp in SMTeamDeathMatch.PlayerIdAndTeamIndex)
				{
					if(vp.Value == localPlayerTeam)
					{
						teamPlayers.Add (vp.Key);
					}
				}
				teamPlayers.Sort ();
				for(int i = 0; i < teamPlayers.Count; i++)
				{
					if(teamPlayers[i] == PhotonNetwork.player.ID)
					{
						requiredIndex = i;
						break;
					}
				}
				if(localPlayerTeam == 1)
				{
					return region1SpawnPoints [requiredIndex];
				}
				else if(localPlayerTeam == 2)
				{
					return region2SpawnPoints [requiredIndex];
				}
			}
			else
			{
				Debug.LogError ("Recursive function calling");
				return placePlayerPositionAtStart ();	// This wont happen but on safe side
			}
		}

		return this.transform; // this is to avoid error code doesn't reach here
	}

	/// <summary>
	/// This function will show respawning message and spwans single player in the Multiplayer game
	/// </summary>
	public void spawnAfterDeath()
	{
		if (gameRulesThatSpawned.isRespwanAllowed)
		{
			gameRulesThatSpawned.resetLocalPlayerKillStreak ();
			StartCoroutine("respawnCountDown");
		}
	}

	IEnumerator respawnCountDown()
	{
		respawnText.gameObject.SetActive(true);
		youDiedText.gameObject.SetActive (true);
		float spawnTime = respawnTimeForNormalPlayers;
		if((int)SMProductEquipper.INSTANCE.CurrentHelmet == (int)Helmet_Type.OPERATIVE)		// Operative has faster respawn
		{
			spawnTime = respawnTimeForOperative;
		}
		while(spawnTime > 0.0f)
		{
			respawnText.text = "Respawn in " + spawnTime.ToString();
			spawnTime -= 1.0f;
			yield return new WaitForSeconds (1.0f);
		}
		respawnText.gameObject.SetActive(false);
		youDiedText.gameObject.SetActive (false);
		spawnSinglePlayer();
	}

	/// <summary>
	/// This function spawns single player in the room. Usually this function is called when player died and needs to respawn
	/// </summary>
	public void spawnSinglePlayer()
	{
		AudioClip clipToBePlayed = null;
		clipToBePlayed = monioSpawnMusic;
		if(SMMultiplayerGame.gameType == MPGameTypes.TEAM_DEATH_MATCH)
		{
			clipToBePlayed = SMTeamDeathMatch.LocalPlayerTeamIndex == 1 ? monioSpawnMusic : jagurSpawnMusic;
		}
		object[] dataToTransfer = { 
			((int)SMProductEquipper.INSTANCE.CurrentHelmet)
		};
		if(OnPlayerRespawned != null)
		{
			OnPlayerRespawned ();
		}
		GameObject player = PhotonNetwork.Instantiate(playerToBeSpawned.name, Vector3.zero, Quaternion.identity, 0,
			dataToTransfer) as GameObject;
		if(areaObserver1.NumberOfEnimeiesInRegion > areaObserver2.NumberOfEnimeiesInRegion)
		{
			int random = Random.Range (0, region2SpawnPoints.Length);
			player.transform.position = region2SpawnPoints [random].position;
			player.transform.rotation = region2SpawnPoints [random].rotation;
		}
		else
		{
			int random = Random.Range (0, region1SpawnPoints.Length);
			player.transform.position = region1SpawnPoints [random].position;
			player.transform.rotation = region1SpawnPoints [random].rotation;
		}

		if (player.GetComponent<PhotonView>().isMine)
		{
			SMPlayerController playerController = player.GetComponent<SMPlayerController> ();
			playerController.characterFocusedCamera = cameraPlayerShouldFollow;
			cameraPlayerShouldFollow.GetComponent<SMThirdPersonCamera> ().targetCameraShouldFocus = playerController.playerPartCamerToBeFocussed.transform;
			cameraPlayerShouldFollow.GetComponent<SMThirdPersonCamera> ().rotationKnob = playerController.playerPartCamerToBeFocussed.transform;

			player.transform.GetComponent<SMPlayerController>().movementJoyStick = movementStick;
			player.transform.GetComponent<SMPlayerController>().orientationJoyStick = orientationStick;

			player.transform.GetComponent<SMWeaponManager>().pickUpButton = pickUpButton;
			pickUpButton.onClick.AddListener(player.GetComponent<SMWeaponManager>().pickUpButtonPressed);
			pickUpButton.gameObject.SetActive(false);
			player.transform.GetComponent<SMWeaponManager>().currentWeaponText = currentWeaponText;

			SMPlayerFiring playerFiring = player.GetComponent<SMPlayerFiring> ();
			playerFiring.ammoDetailsUI = ammoDetailsUI;
			playerFiring.grenadeInput = grenadeButton;
			grenadeUIInput.PlayerFiring = playerFiring;
			grenadeUIInput.PlayerGrenade = player.GetComponent<IKControl> ();

			player.transform.GetComponent<SMPlayerController>().leanButton = leanButtonUI;
			player.transform.GetComponent<SMPlayerController>().hideOrUnhideButton = hideButtonUI;

			player.transform.GetComponent<SMPlayerHealth>().spawnManager = this;
			player.GetComponent<SMPlayerHealth>().playInteractableUI = playInteractableUI;
			player.GetComponent<SMPlayerHealth> ().OnLocalPlayerDead += teamDeathMatchUIManager.showActivePlayersHiddenScoreUI;
			player.GetComponent<SMPlayerHealth> ().OnLocalPlayerDead += freeForAllUIManager.showActivePlayersHiddenScoreUI;

			gameRulesThatSpawned.localPlayer = player.GetComponent<SMPlayerIdentifier>();
			player.GetComponent<SMPlayerIdentifier>().gameType = gameRulesThatSpawned;
			gameRulesThatSpawned.localPlayer.setPlayerUIColor(player.GetComponent<PhotonView>());
		}

		AudioSource.PlayClipAtPoint (clipToBePlayed, player.GetComponent<SMPlayerController> ().characterFocusedCamera.transform.position);
		showUIAfterRespwan ();
	}

	/// <summary>
	/// Hides the user interface after death.
	/// </summary>
	private void hideUIAfterDeath()
	{
		foreach(GameObject playerUI in playInteractableUI)
		{
			playerUI.SetActive(false);
		}
  	}

	/// <summary>
	/// Shows the user interface after respawn.
	/// </summary>
	private void showUIAfterRespwan()
	{
		if(!SMMultiplayerGame.IsGameOver)
		{
			foreach(GameObject playerUI in playInteractableUI)
			{
				playerUI.SetActive(true);
			}

			foreach(GameObject playerUI in hideUIEvenAferRespawn)
			{
				playerUI.SetActive (false);
			}
		}
	}

	/// <summary>
	/// This function will spawn the game rules in the scene first based on the game type user selected for
	/// </summary>
	private void spawnGameRules()
	{
		foreach (GameObject gameRulePrefab in gameRulesToBeSpawned)
		{
			if (gameType == MPGameTypes.FREE_FOR_ALL)
			{
				SMFreeForAll freeForAll = gameRulePrefab.GetComponent<SMFreeForAll>();

				if (freeForAll != null)
				{
					GameObject spawnedObject = Instantiate(gameRulePrefab, Vector3.zero, Quaternion.identity) as GameObject;
					gameRulesThatSpawned = spawnedObject.GetComponent<SMFreeForAll>();
					break;
				}
			}
			else if (gameType == MPGameTypes.TEAM_DEATH_MATCH)
			{
				SMTeamDeathMatch teamDeathMatch = gameRulePrefab.GetComponent<SMTeamDeathMatch>();

				if (teamDeathMatch != null)
				{
					GameObject spawnedObject = Instantiate(gameRulePrefab, Vector3.zero, Quaternion.identity) as GameObject;
					gameRulesThatSpawned = spawnedObject.GetComponent<SMTeamDeathMatch>();
					break;
				}
			}
		}
		specialWeaponManager.registerMultiplayerGame(gameRulesThatSpawned);
		gameRulesThatSpawned.xpMadeAfterKill *= SMProductEquipper.INSTANCE.CurrentExpBoostMultiplier;
		gameRulesThatSpawned.xpMadeAfterKill *= UnityAds.PlayerHasAdRewardXp;
		UnityAds.reduceNumberOfGames ();
		gameRulesThatSpawned.setDataTransmitter (this);
	}

	/// <summary>
	/// Event raised by Photon network
	/// </summary>
	/// <param name="eventCode">Event code.</param>
	/// <param name="content">Content.</param>
	/// <param name="senderID">Sender I.</param>
	private void OnEvent(byte eventCode, object content, int senderID)
	{
		if(eventCode == (int)MultiplayerEvents.ReceiveData && !PhotonNetwork.isMasterClient)
		{
			receiveDataFromMaster ((object[])content);
		}
		else if(eventCode == (int)MultiplayerEvents.RequestForData && PhotonNetwork.isMasterClient)
		{
			receiveRequestForDataFromOtherClientsAndRespond ();
		}
	}

	/// <summary>
	/// This function request the master client for time remaining to start the game
	/// </summary>
	private void requestDataToMasterClient()
	{
		if(!PhotonNetwork.isMasterClient)
		{
			PhotonNetwork.RaiseEvent ((int)MultiplayerEvents.RequestForData, null, true, null);
		}
	}

	/// <summary>
	/// Receives the request for time from other clients and respond them with time data.
	/// </summary>
	private void receiveRequestForDataFromOtherClientsAndRespond()
	{
		Dictionary<int,int> playerIdAndScore = new Dictionary<int, int> ();
		Dictionary<int,int> playerIdAndTeamIndex = new Dictionary<int, int> ();
		Dictionary<int,int> playerIdAndDeathInfo = new Dictionary<int, int> ();

		float[] times = { 
			(float)timeDelayToStartMatch,
			SMMultiplayerGame.INSTANCE.GameTimer,
		};

		object[] data = new object[0];

		if(SMMultiplayerGame.gameType == MPGameTypes.FREE_FOR_ALL)
		{
			foreach(KeyValuePair<int,ObscuredInt> pidAS in SMFreeForAll.PlayersIdAndScore)
			{
				playerIdAndScore.Add (pidAS.Key, (int)pidAS.Value);
			}
			foreach(KeyValuePair<int,ObscuredInt> pidAS in SMFreeForAll.PlayerIdAndDeaths)
			{
				playerIdAndDeathInfo.Add (pidAS.Key, (int)pidAS.Value);
			}
			data = new object[] {
				times,
				playerIdAndScore,
				playerIdAndDeathInfo
			};
		}
		else if(SMMultiplayerGame.gameType == MPGameTypes.TEAM_DEATH_MATCH)
		{
			foreach(KeyValuePair<int,ObscuredInt> pidAS in SMFreeForAll.PlayersIdAndScore)
			{
				playerIdAndScore.Add (pidAS.Key, (int)pidAS.Value);
			}
			foreach(KeyValuePair<int,ObscuredInt> pidAS in SMFreeForAll.PlayerIdAndDeaths)
			{
				playerIdAndDeathInfo.Add (pidAS.Key, (int)pidAS.Value);
			}
			playerIdAndTeamIndex = SMTeamDeathMatch.PlayerIdAndTeamIndex;
			data = new object[] {
				times,
				playerIdAndScore,
				playerIdAndDeathInfo,
				playerIdAndTeamIndex
			};
		}

		if(PhotonNetwork.isMasterClient)
		{
			PhotonNetwork.RaiseEvent ((int)MultiplayerEvents.ReceiveData, (object)data, true, null);
		}
	}

	/// <summary>
	/// Receives the data from master cleint.
	/// </summary>
	/// <param name="data">Data.</param>
	private void receiveDataFromMaster(object[] data)
	{
		if(!hasRecievedTimeDataFromMaster)
		{
			float[] times = (float[])(data [0]);
			timeDelayToStartMatch = times [0];
			if(gameRulesThatSpawned != null)
			{
				gameRulesThatSpawned.setGameTimer (times [1]);
			}
			hasRecievedTimeDataFromMaster = true;
		}

		SMMultiplayerGame.assignEntireScoreToAllPlayers ((Dictionary<int,int>)data [1]);
		SMMultiplayerGame.assignEntireDeathInfoToAllPlayers ((Dictionary<int,int>)data [2]);

		if (SMMultiplayerGame.gameType == MPGameTypes.FREE_FOR_ALL) 
		{
			if(freeForAllWhenRecievedDataAtBeginning == null)
			{
				freeForAllWhenRecievedDataAtBeginning = new UnityEvent ();
			}
			freeForAllWhenRecievedDataAtBeginning.Invoke ();
		}
		else if (SMMultiplayerGame.gameType == MPGameTypes.TEAM_DEATH_MATCH) 
		{
			SMTeamDeathMatch.assignEntirePlayerTeamToIDs ((Dictionary<int,int>)data [3]);
			if(teamDeathMatchWhenRecieveDataAtBeginning == null)
			{
				teamDeathMatchWhenRecieveDataAtBeginning = new UnityEvent ();
			}
			teamDeathMatchWhenRecieveDataAtBeginning.Invoke ();
		}
	}

	public override void OnMasterClientSwitched (PhotonPlayer newMasterClient)
	{
		if(newMasterClient.ID == PhotonNetwork.player.ID)
		{
			if(!hasRecievedTimeDataFromMaster)
			{
				hasRecievedTimeDataFromMaster = true;
				receiveRequestForDataFromOtherClientsAndRespond ();
			}
		}
	}

	public void sendSignalToAllToRefreshData()
	{
		receiveRequestForDataFromOtherClientsAndRespond ();		// This does required job for us job
	}
}
