//
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


/// <summary>
/// This class will spawn the players in the game at certain positions based on their index values in network and assignes proper values to
/// player components. This class also spawns game rules in the scene.
/// </summary>
public class SMPlayerSpawnerAndAssigner : MonoBehaviour 
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
	/// One of the game object in the array will get instantiated based on the game type player wants to play
	/// </summary>
	public GameObject[] gameRulesToBeSpawned;
	/// <summary>
	/// The game rules prefab that spawned.
	/// </summary>
	private SMMultiplayerGame gameRulesThatSpawned;
	/// <summary>
	/// This score manager acts as central scoring system which syncs all the players about the scores in the game.
	/// </summary>
	public SMMultiplayerScoreManager scoreManager;
	/// <summary>
	/// This transform contains several spawn points as their children which can be used to spawn the players
	/// </summary>
	public Transform spawnPoints;
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
	public GameObject playInteractableUI;
	/// <summary>
	/// Just respawn heading
	/// </summary>
	public Text respawnHeading;
	/// <summary>
	/// This UI text will show countdown when the player is going to respawn again
	/// </summary>
	public Text respawnCounter;
	/// <summary>
	/// The special weapon manager.
	/// </summary>
	public SMSpecialWeaponManager specialWeaponManager;
	/// <summary>
	/// Occurs when player respawnes in multiplayer context.
	/// </summary>
	public static event onGameRulesCreated OnPlayerRespawned;
	// Use this for initialization
	void Start () 
	{
		spawnGameRules();
		StartCoroutine ("spawnPlayersAfterDelay");		// To fix double instantiation of the player 
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	IEnumerator spawnPlayersAfterDelay()
	{
		yield return new WaitForSeconds (0.85f);
		spawnPlayers ();
	}

	/// <summary>
	/// This function will spawn the player and assigns appropriate values to player's components
	/// </summary>
	private void spawnPlayers()
	{
		GameObject player = PhotonNetwork.Instantiate (playerToBeSpawned.name, Vector3.zero, Quaternion.identity, 0) as GameObject;
		foreach(Transform spawnPoint in spawnPoints)
		{
			if(spawnPoint.name == player.GetComponent<PhotonView>().owner.ID.ToString())
			{
				player.transform.position = spawnPoint.position;
				break;
			}
		}

		if(player.GetComponent<PhotonView>().isMine)
		{
			player.transform.GetComponent<SMPlayerController> ().characterFocusedCamera = cameraPlayerShouldFollow;
			cameraPlayerShouldFollow.GetComponent<SMThirdPersonCamera> ().targetCameraShouldFocus = player.transform;
			cameraPlayerShouldFollow.GetComponent<SMThirdPersonCamera> ().rotationKnob = player.transform;

			player.transform.GetComponent<SMPlayerController> ().movementJoyStick = movementStick;
			player.transform.GetComponent<SMPlayerController> ().orientationJoyStick = orientationStick;

			player.transform.GetComponent<SMWeaponManager> ().pickUpButton = pickUpButton;
			pickUpButton.onClick.AddListener (player.GetComponent<SMWeaponManager> ().pickUpButtonPressed);
			pickUpButton.gameObject.SetActive (false);
			player.transform.GetComponent<SMWeaponManager> ().currentWeaponText = currentWeaponText;

			player.transform.GetComponent<SMPlayerFiring> ().ammoDetailsUI = ammoDetailsUI;
			player.transform.GetComponent<SMPlayerFiring> ().grenadeInput = grenadeButton;

			player.transform.GetComponent<SMPlayerController> ().leanButton = leanButtonUI;
			player.transform.GetComponent<SMPlayerController> ().hideOrUnhideButton = hideButtonUI;

			player.transform.GetComponent<SMPlayerHealth>().spawnManager = this;
			player.GetComponent<SMPlayerHealth>().playInteractableUI = playInteractableUI;

			gameRulesThatSpawned.localPlayer = player.GetComponent<SMPlayerIdentifier>();
			player.GetComponent<SMPlayerIdentifier>().gameType = gameRulesThatSpawned;
			gameRulesThatSpawned.localPlayer.setPlayerUIColor(player.GetComponent<PhotonView>());

			scoreManager.setGameType (gameRulesThatSpawned);
		}
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
		respawnHeading.gameObject.SetActive(true);
		respawnCounter.gameObject.SetActive(true);

		respawnCounter.text = "3";
		yield return new WaitForSeconds(1.0f);
		respawnCounter.text = "2";
		yield return new WaitForSeconds(1.0f);
		respawnCounter.text = "1";
		yield return new WaitForSeconds(1.0f);

		respawnHeading.gameObject.SetActive(false);
		respawnCounter.gameObject.SetActive(false);
		spawnSinglePlayer();
	}

	/// <summary>
	/// This function spawns single player in the room. Usually this function is called when player died and needs to respawn
	/// </summary>
	public void spawnSinglePlayer()
	{
		if(OnPlayerRespawned != null)
		{
			OnPlayerRespawned ();
		}
		GameObject player = PhotonNetwork.Instantiate(playerToBeSpawned.name, Vector3.zero, Quaternion.identity, 0) as GameObject;
		foreach (Transform spawnPoint in spawnPoints)
		{
			if (spawnPoint.name == player.GetComponent<PhotonView>().owner.ID.ToString())
			{
				player.transform.position = spawnPoint.position;
				break;
			}
		}

		if (player.GetComponent<PhotonView>().isMine)
		{
			player.transform.GetComponent<SMPlayerController>().characterFocusedCamera = cameraPlayerShouldFollow;
			cameraPlayerShouldFollow.GetComponent<SMThirdPersonCamera>().targetCameraShouldFocus = player.transform;
			cameraPlayerShouldFollow.GetComponent<SMThirdPersonCamera>().rotationKnob = player.transform;

			player.transform.GetComponent<SMPlayerController>().movementJoyStick = movementStick;
			player.transform.GetComponent<SMPlayerController>().orientationJoyStick = orientationStick;

			player.transform.GetComponent<SMWeaponManager>().pickUpButton = pickUpButton;
			pickUpButton.onClick.AddListener(player.GetComponent<SMWeaponManager>().pickUpButtonPressed);
			pickUpButton.gameObject.SetActive(false);
			player.transform.GetComponent<SMWeaponManager>().currentWeaponText = currentWeaponText;

			player.transform.GetComponent<SMPlayerFiring>().ammoDetailsUI = ammoDetailsUI;
			player.transform.GetComponent<SMPlayerFiring> ().grenadeInput = grenadeButton;

			player.transform.GetComponent<SMPlayerController>().leanButton = leanButtonUI;
			player.transform.GetComponent<SMPlayerController>().hideOrUnhideButton = hideButtonUI;

			player.transform.GetComponent<SMPlayerHealth>().spawnManager = this;
			player.GetComponent<SMPlayerHealth>().playInteractableUI = playInteractableUI;

			gameRulesThatSpawned.localPlayer = player.GetComponent<SMPlayerIdentifier>();
			player.GetComponent<SMPlayerIdentifier>().gameType = gameRulesThatSpawned;
			gameRulesThatSpawned.localPlayer.setPlayerUIColor(player.GetComponent<PhotonView>());
		}
		playInteractableUI.SetActive(true);
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
	}
}
