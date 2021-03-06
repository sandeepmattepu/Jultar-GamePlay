﻿//
//  SMPlayerHealth.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 25/02/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using SandeepMattepu;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using SandeepMattepu.Weapon;

/// <summary>
/// This class will handle player health and armor and sets the UI accordingly based on those values
/// </summary>
public class SMPlayerHealth : MonoBehaviour, IPunObservable
{
	#region Public variables
	/// <summary>
	/// The max health player can have.
	/// </summary>
	public ObscuredFloat MaxHealth = 100.0f;
	/// <summary>
	/// The max armor player can carry.
	/// </summary>
	public ObscuredFloat MaxArmor = 1000.0f;
	/// <summary>
	/// The current health value of the player
	/// </summary>
	/// <value>The player health.</value>
	public ObscuredFloat PlayerHealth 
	{
		get {	return playerHealth;	}
	}
	/// <summary>
	/// The current armor value of the player
	/// </summary>
	/// <value>The armor points.</value>
	public ObscuredFloat ArmorPoints 
	{
		get {	return armorPoints;		}
	}
	/// <summary>
	/// Indicates whether the player is holding the armor or not
	/// </summary>
	/// <value><c>true</c> if armor available; otherwise, <c>false</c>.</value>
	public ObscuredBool armorAvailable 
	{
		get {	return _armorAvailable;	}
	}
	/// <summary>
	/// Gets a value indicating whether player is immune to gas bombs.
	/// </summary>
	public ObscuredBool IsPlayerImmuneToGasBombs 
	{
		get { return isPlayerImmuneToGasBombs; }
	}
	/// <summary>
	/// The value determines whether the player is used in multiplayer or single player
	/// </summary>
	public ObscuredBool isUsingMultiPlayer;
	/// <summary>
	/// This handles the spawning of the player when player is dead
	/// </summary>
	public SMPlayerSpawnerAndAssigner spawnManager;
	/// <summary>
	/// This UI shows the health and armor sliders at the top of the player
	/// </summary>
	public SMHealthUI uIToShowHealthAndArmor;
	/// <summary>
	/// This ragdoll will get instatiated at the place of the alive monio when player health becomes zero
	/// </summary>
	public GameObject ragdoll;
	/// <summary>
	/// This UI is the main UI where the player interacts with the game
	/// </summary>
	public GameObject[] playInteractableUI;
	/// <summary>
	/// Delegate when player is dead
	/// </summary>
	public delegate void onPlayerDead();
	/// <summary>
	/// Occurs when local player is dead in multiplayer context.
	/// </summary>
	public event onPlayerDead OnLocalPlayerDead;


	#endregion

	#region Private variables
	/// <summary>
	/// The current player health.
	/// </summary>
	private ObscuredFloat playerHealth = 0.0f;
	/// <summary>
	/// The current armor points.
	/// </summary>
	private ObscuredFloat armorPoints = 0.0f;
	/// <summary>
	/// This value explains whether the armor is available or not
	/// </summary>
	private ObscuredBool _armorAvailable = false;
	/// <summary>
	/// The photon view component attached to the game object.
	/// </summary>
	private PhotonView photonViewComponent;
	/// <summary>
	/// The player identifier componenet attached to the game object.
	/// </summary>
	private SMPlayerIdentifier playerIdentifier;
	/// <summary>
	/// The seconds to regenerate full health.
	/// </summary>
	[SerializeField]
	private ObscuredFloat secondsToRegenerateFullHealth = 8.0f;
	/// <summary>
	/// The seconds to wait before health regeneration.
	/// </summary>
	[SerializeField]
	private ObscuredFloat secondsToWaitBeforeHealthRegeneration = 3.0f;
	/// <summary>
	/// The timer that ticks for health regeneration waiting.
	/// </summary>
	private ObscuredFloat timerForHealthRegenWaiting = 0.0f;
	/// <summary>
	/// Is player immune to gas bombs.
	/// </summary>
	private ObscuredBool isPlayerImmuneToGasBombs = false;

	#endregion

	#region MonoBehaviour Calls

	// Use this for initialization
	void Start () 
	{
		photonViewComponent = GetComponent<PhotonView> ();
		playerHealth = MaxHealth;
		armorPoints = MaxArmor;
		setSliderValues ();
		playerIdentifier = GetComponent<SMPlayerIdentifier>();
	}

	void Update()
	{
		regenerateHealth ();
	}

	#endregion

	#region Events
	public delegate void localPlayerDied();
	/// <summary>
	/// Occurs when local player died.
	/// </summary>
	public static event localPlayerDied LocalPlayerDied;
	#endregion

	/// <summary>
	/// Call this function to set the amount of damage the player recieved
	/// </summary>
	/// <param name="value">Value.</param>
	public void reduceHealthPointsBy(float value)
	{
		if(_armorAvailable)			// If player has armor
		{
			if(armorPoints > 0)			// If player has armor
			{
				armorPoints -= value;
			}

			if(armorPoints <= 0)			// Handling the condition when armor becomes negative
			{
				playerHealth -= armorPoints;
				_armorAvailable = false;
			}
		} 
		else 					// If player won't have any armor
		{
			playerHealth -= value;
			if(playerHealth <= 0)
			{
				playerHealth = 0.0f;
				// remove hands ik, deactivate collider of player perform death and destroy weapon in hand drop the weapon in hand to floor
			}
		}
		if(isUsingMultiPlayer)
		{
			if(photonViewComponent.isMine)
			{
				setSliderValues ();

				if(playerHealth <= 0.0f)
				{
					if(LocalPlayerDied != null)
					{
						LocalPlayerDied ();
					}
					setSliderValues ();
					hidePlayerInteractiveUI();
					photonViewComponent.RPC("sendDeathMessage", PhotonTargets.Others, (int)SMProductEquipper.INSTANCE.CurrentHelmet);
					createDeadBody(true, true, SMProductEquipper.INSTANCE.CurrentHelmet);
					Destroy(this.gameObject);
				}
			}
		}
		else
		{
			setSliderValues ();
			if (playerHealth <= 0.0f)
			{
				if(LocalPlayerDied != null)
				{
					LocalPlayerDied ();
				}
				hidePlayerInteractiveUI();
				createDeadBody (false, true, SMProductEquipper.INSTANCE.CurrentHelmet);
				Destroy(this.gameObject);
			}
		}
	}

	/// <summary>
	/// Call this function to set the amount of damage the player recieved from gas related weapons
	/// </summary>
	/// <param name="value">Value.</param>
	/// <param name="isGasWeapon">Is the weapon creating damage is gas bomb.</param>
	public void reduceHealthPointsBy(float value, bool isGasWeapon)
	{
		if(isGasWeapon && isPlayerImmuneToGasBombs)
		{
			return;
		}
		if(_armorAvailable)			// If player has armor
		{
			if(armorPoints > 0)			// If player has armor
			{
				armorPoints -= value;
			}

			if(armorPoints <= 0)			// Handling the condition when armor becomes negative
			{
				playerHealth -= armorPoints;
				_armorAvailable = false;
			}
		} 
		else 					// If player won't have any armor
		{
			playerHealth -= value;
			if(playerHealth <= 0)
			{
				playerHealth = 0.0f;
				// remove hands ik, deactivate collider of player perform death and destroy weapon in hand drop the weapon in hand to floor
			}
		}
		if(isUsingMultiPlayer)
		{
			if(photonViewComponent.isMine)
			{
				setSliderValues ();

				if(playerHealth <= 0.0f)
				{
					if(LocalPlayerDied != null)
					{
						LocalPlayerDied ();
					}
					setSliderValues ();
					hidePlayerInteractiveUI();
					photonViewComponent.RPC("sendDeathMessage", PhotonTargets.Others, (int)SMProductEquipper.INSTANCE.CurrentHelmet);
					createDeadBody(true, true, SMProductEquipper.INSTANCE.CurrentHelmet);
					Destroy(this.gameObject);
				}
			}
		}
		else
		{
			setSliderValues ();
			if (playerHealth <= 0.0f)
			{
				if(LocalPlayerDied != null)
				{
					LocalPlayerDied ();
				}
				hidePlayerInteractiveUI();
				createDeadBody(false, true, SMProductEquipper.INSTANCE.CurrentHelmet);
				Destroy(this.gameObject);
			}
		}
	}

	/// <summary>
	/// Regenerates the health.
	/// </summary>
	private void regenerateHealth()
	{
		if(playerHealth < MaxHealth)
		{
			if(timerForHealthRegenWaiting > secondsToWaitBeforeHealthRegeneration)
			{
				if(isUsingMultiPlayer && photonViewComponent.isMine)
				{
					playerHealth += ((MaxHealth / secondsToRegenerateFullHealth) * Time.deltaTime);
				}
				else if(!isUsingMultiPlayer)
				{
					playerHealth += ((MaxHealth / secondsToRegenerateFullHealth) * Time.deltaTime);
				}
				if(playerHealth >= MaxHealth)
				{
					timerForHealthRegenWaiting = 0.0f;
					playerHealth = MaxHealth;
				}
				setSliderValues ();
			}
			else
			{
				timerForHealthRegenWaiting += Time.deltaTime;
			}
		}
	}

	/// <summary>
	/// This function creates ragdoll at the player's position and sets the joints as current player's orientation
	/// </summary>
	/// <param name="isMultiplayer">Value which indicates to produce ragdoll for multiplayer game or single player game</param>
	/// <param name="canListenAudio">Value which indicates whether ragdoll can hear sounds are not</param>
	/// <param name="helmetType">Helmet type that dead body should have</param>
	private void createDeadBody(bool isMultiplayer, bool canListenAudio, int helmetType)
	{
		if(isMultiplayer && photonViewComponent.isMine)
		{
			if(OnLocalPlayerDead != null)
			{
				OnLocalPlayerDead ();
			}
			transform.GetComponent<SMWeaponManager> ().dropHoldingWeaponToFloorWhenDead ();
		}
		GameObject ragdollBody = Instantiate(ragdoll, transform.position, transform.rotation) as GameObject;
		SMDestroyRagdoll destroyRagdoll = ragdollBody.GetComponent<SMDestroyRagdoll>();
		destroyRagdoll.isMultiplayer = isMultiplayer;
		destroyRagdoll.canListenToAudio = canListenAudio;
		ragdollBody.GetComponent<SMPlayerEquipper> ().assignHelmetToDeadBody (helmetType);
		Camera gameCamera = transform.GetComponent<SMPlayerController>().characterFocusedCamera;
		if(gameCamera != null)
		{
			gameCamera.GetComponent<SMThirdPersonCamera>().targetCameraShouldFocus = ragdollBody.transform;
			gameCamera.GetComponent<SMThirdPersonCamera>().rotationKnob = ragdollBody.transform;
		}
		Transform[] aliveBody = transform.GetComponentsInChildren<Transform>();
		Transform[] deadBody = ragdollBody.GetComponentsInChildren<Transform>();

		for (int i = 0; i < aliveBody.Length; i++)
		{
			for (int j = 0; j < deadBody.Length; j++)
			{
				if (aliveBody[i].gameObject.name == deadBody[j].gameObject.name)
				{
					deadBody[j].position = aliveBody[i].position;
					deadBody[j].rotation = aliveBody[i].rotation;
					break;
				}
			}
		}
	}

	/// <summary>
	/// Call this function to set the amount of damage the player recieved.
	/// USE THIS FUNCTION ONLY IN MULTIPLAYER CONTEXT.  
	/// </summary>
	/// <param name="value">Value.</param>
	/// <param name="ID">Id of the player who made the damage.</param>
	public void reduceHealthPointsBy(float value, int ID)
	{
		if (_armorAvailable)            // If player has armor
		{
			if (armorPoints > 0)            // If player has armor
			{
				armorPoints -= value;
			}

			if (armorPoints <= 0)           // Handling the condition when armor becomes negative
			{
				playerHealth -= armorPoints;
				_armorAvailable = false;
			}
		}
		else                    // If player won't have any armor
		{
			playerHealth -= value;
			if (playerHealth <= 0)
			{
				playerHealth = 0.0f;
				// remove hands ik, deactivate collider of player perform death and destroy weapon in hand drop the weapon in hand to floor
			}
		}
		if (isUsingMultiPlayer)
		{
			if (photonViewComponent.isMine)
			{
				setSliderValues();

				if (playerHealth <= 0.0f)
				{
					if(LocalPlayerDied != null)
					{
						LocalPlayerDied ();
					}
					if(ID >= 0)				// Sometimes ID will have -ve value due to death traps
					{
						playerIdentifier.reportScore(ID);
					}
					setSliderValues();
					spawnManager.spawnAfterDeath();
					hidePlayerInteractiveUI();
					photonViewComponent.RPC("sendDeathMessage", PhotonTargets.Others, (int)SMProductEquipper.INSTANCE.CurrentHelmet);
					createDeadBody(true, true, SMProductEquipper.INSTANCE.CurrentHelmet);
					Destroy(this.gameObject);
				}
			}
		}
		else
		{
			setSliderValues();
			if(playerHealth <= 0.0f)
			{
				if(LocalPlayerDied != null)
				{
					LocalPlayerDied ();
				}
				hidePlayerInteractiveUI();
				createDeadBody(false, true, SMProductEquipper.INSTANCE.CurrentHelmet);
				Destroy(this.gameObject);
			}
		}
	}

	/// <summary>
	/// Call this function to set the amount of damage the player recieved by gas related weapons.
	/// USE THIS FUNCTION ONLY IN MULTIPLAYER CONTEXT.  
	/// </summary>
	/// <param name="value">Value.</param>
	/// <param name="ID">Id of the player who made the damage.</param>
	/// <param name="isGasBomb">Is death due to gas bomb.</param>
	public void reduceHealthPointsBy(float value, int ID, bool isGasBomb)
	{
		if(isGasBomb && isPlayerImmuneToGasBombs)
		{
			return;
		}
		if (_armorAvailable)            // If player has armor
		{
			if (armorPoints > 0)            // If player has armor
			{
				armorPoints -= value;
			}

			if (armorPoints <= 0)           // Handling the condition when armor becomes negative
			{
				playerHealth -= armorPoints;
				_armorAvailable = false;
			}
		}
		else                    // If player won't have any armor
		{
			playerHealth -= value;
			if (playerHealth <= 0)
			{
				playerHealth = 0.0f;
				// remove hands ik, deactivate collider of player perform death and destroy weapon in hand drop the weapon in hand to floor
			}
		}
		if (isUsingMultiPlayer)
		{
			if (photonViewComponent.isMine)
			{
				setSliderValues();

				if (playerHealth <= 0.0f)
				{
					if(LocalPlayerDied != null)
					{
						LocalPlayerDied ();
					}
					if(ID >= 0)				// Sometimes ID will have -ve value due to death traps
					{
						playerIdentifier.reportScore(ID);
					}
					setSliderValues();
					spawnManager.spawnAfterDeath();
					hidePlayerInteractiveUI();
					photonViewComponent.RPC("sendDeathMessage", PhotonTargets.Others, (int)SMProductEquipper.INSTANCE.CurrentHelmet);
					createDeadBody(true, true, SMProductEquipper.INSTANCE.CurrentHelmet);
					Destroy(this.gameObject);
				}
			}
		}
		else
		{
			setSliderValues();
			if(playerHealth <= 0.0f)
			{
				if(LocalPlayerDied != null)
				{
					LocalPlayerDied ();
				}
				hidePlayerInteractiveUI();
				createDeadBody(false, true, SMProductEquipper.INSTANCE.CurrentHelmet);
				Destroy(this.gameObject);
			}
		}
	}

	/// <summary>
	/// This function hides player interactble UI when he dies
	/// </summary>
	private void hidePlayerInteractiveUI()
	{
		foreach(GameObject uiGameObject in playInteractableUI)
		{
			uiGameObject.SetActive (false);
		}
	}

	/// <summary>
	/// Use this function to set the slider values of both health and armor
	/// </summary>
	private void setSliderValues()
	{
		uIToShowHealthAndArmor.setHealthSlider (0.0f, MaxHealth, playerHealth);
		if(_armorAvailable)
		{
			uIToShowHealthAndArmor.setArmorSlider (0.0f, MaxArmor, armorPoints);
		}
		else
		{
			uIToShowHealthAndArmor.setArmorSlider (0.0f, MaxArmor, 0.0f);
		}
	}

	/// <summary>
	/// Call this function if you wish to give armor to the player
	/// </summary>
	public void giveArmorToPlayer()
	{
		_armorAvailable = true;
		armorPoints = MaxArmor;
		setSliderValues ();
	}

	/// <summary>
	/// Call this function if you wish to remove armor from the player
	/// </summary>
	public void removeArmorFromPlayer()
	{
		_armorAvailable = false;
		armorPoints = 0.0f;
		setSliderValues ();
	}

	/// <summary>
	/// Use this function to increase the health of the player
	/// </summary>
	/// <param name="byValue">Increase health of the player by value</param>
	public void restoreHealthOfThePlayer(float byValue)
	{
		float maxHealthFromObscured = MaxHealth;
		byValue = byValue < 0.0f ? 0.0f : byValue;
		byValue = byValue > maxHealthFromObscured ? maxHealthFromObscured : byValue;
		playerHealth += byValue;
		playerHealth = playerHealth > MaxHealth ? MaxHealth : playerHealth;
		setSliderValues ();
	}

	/// <summary>
	/// Adds the health points to max health by.
	/// </summary>
	/// <param name="byValue">By value.</param>
	public void addPointsToMaxHealthBy(float byValue)
	{
		MaxHealth += byValue;
		playerHealth = MaxHealth;
	}

	/// <summary>
	/// Assigns the health regeneration rate.
	/// </summary>
	/// <param name="secondsToFillHealth">Seconds to fill health.</param>
	public void assignHealthRegenerationRate(float secondsToFillHealth)
	{
		secondsToFillHealth = secondsToFillHealth < 0.5f? 0.5f : secondsToFillHealth;
		secondsToRegenerateFullHealth = secondsToFillHealth;
	}

	/// <summary>
	/// Call this function when player is respawned and needs health
	/// </summary>
	public void restoreHealthOfThePlayer()
	{
		playerHealth = MaxHealth;
		setSliderValues ();
	}

	/// <summary>
	/// Sets the player immunity towards gas bombs.
	/// </summary>
	/// <param name="isImmune">If player is immune.</param>
	public void setPlayerImmunityTowardsGasBombs(bool isImmune)
	{
		isPlayerImmuneToGasBombs = isImmune;
	}

	/// <summary>
	/// This function will send this client representers to die
	/// </summary>
	[PunRPC]
	public void sendDeathMessage(int helmetValue)
	{
		createDeadBody(true, false, helmetValue);
		Destroy(this.gameObject);
	}

	#region IPunObservable implementation

	void IPunObservable.OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		if(stream.isWriting)
		{
			float playerHealthDataToTransfer = PlayerHealth;
			float armorDataToTransfer = ArmorPoints;
			bool armorAvaialableData = _armorAvailable;
			stream.SendNext (playerHealthDataToTransfer);
			stream.SendNext (armorDataToTransfer);
			stream.SendNext (armorAvaialableData);
		}
		else
		{
			playerHealth = (float)stream.ReceiveNext ();
			armorPoints = (float)stream.ReceiveNext ();
			_armorAvailable = (bool)stream.ReceiveNext ();
			setSliderValues ();
		}
	}

	#endregion
}
