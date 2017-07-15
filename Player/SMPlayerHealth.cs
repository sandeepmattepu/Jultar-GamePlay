//
//  SMPlayerHealth.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 25/02/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using SandeepMattepu;
using UnityEngine;
/// <summary>
/// This class will handle player health and armor and sets the UI accordingly based on those values
/// </summary>
public class SMPlayerHealth : MonoBehaviour, IPunObservable
{
	#region Public variables
	/// <summary>
	/// The max health player can have.
	/// </summary>
	public float MaxHealth = 100.0f;
	/// <summary>
	/// The max armor player can carry.
	/// </summary>
	public float MaxArmor = 100.0f;
	/// <summary>
	/// The current health value of the player
	/// </summary>
	/// <value>The player health.</value>
	public float PlayerHealth 
	{
		get {	return playerHealth;	}
	}
	/// <summary>
	/// The current armor value of the player
	/// </summary>
	/// <value>The armor points.</value>
	public float ArmorPoints 
	{
		get {	return armorPoints;		}
	}
	/// <summary>
	/// Indicates whether the player is holding the armor or not
	/// </summary>
	/// <value><c>true</c> if armor available; otherwise, <c>false</c>.</value>
	public bool armorAvailable 
	{
		get {	return _armorAvailable;	}
	}
	/// <summary>
	/// The value determines whether the player is used in multiplayer or single player
	/// </summary>
	public bool isUsingMultiPlayer;
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
	public GameObject playInteractableUI;


	#endregion

	#region Private variables
	/// <summary>
	/// The current player health.
	/// </summary>
	private float playerHealth = 0.0f;
	/// <summary>
	/// The current armor points.
	/// </summary>
	private float armorPoints = 0.0f;
	/// <summary>
	/// This value explains whether the armor is available or not
	/// </summary>
	private bool _armorAvailable = false;
	/// <summary>
	/// The photon view component attached to the game object.
	/// </summary>
	private PhotonView photonViewComponent;
	/// <summary>
	/// The player identifier componenet attached to the game object.
	/// </summary>
	private SMPlayerIdentifier playerIdentifier;

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
					setSliderValues ();
					playInteractableUI.SetActive(false);
					photonViewComponent.RPC("sendDeathMessage", PhotonTargets.Others);
					Destroy(this.gameObject);
					createDeadBody(true);
				}
			}
		}
		else
		{
			setSliderValues ();
			if (playerHealth <= 0.0f)
			{
				playInteractableUI.SetActive(false);
				createDeadBody(false);
				Destroy(this.gameObject);
			}
		}
	}

	/// <summary>
	/// This function creates ragdoll at the player's position and sets the joints as current player's orientation
	/// </summary>
	/// <param name="isMultiplayer">Value which indicates to produce ragdoll for multiplayer game or single player game</param>
	private void createDeadBody(bool isMultiplayer)
	{
		GameObject ragdollBody = Instantiate(ragdoll, transform.position, transform.rotation) as GameObject;
		ragdollBody.GetComponent<SMDestroyRagdoll>().isMultiplayer = isMultiplayer;
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
					if(ID >= 0)				// Sometimes ID will have -ve value due to death traps
					{
						playerIdentifier.reportScore(ID);
					}
					setSliderValues();
					spawnManager.spawnAfterDeath();
					playInteractableUI.SetActive(false);
					photonViewComponent.RPC("sendDeathMessage", PhotonTargets.Others);
					Destroy(this.gameObject);
					createDeadBody(true);
				}
			}
		}
		else
		{
			setSliderValues();
			if(playerHealth <= 0.0f)
			{
				playInteractableUI.SetActive(false);
				createDeadBody(false);
				Destroy(this.gameObject);
			}
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
		byValue = byValue < 0.0f ? 0.0f : byValue;
		byValue = byValue > MaxHealth ? MaxHealth : byValue;
		playerHealth += byValue;
		playerHealth = playerHealth > MaxHealth ? MaxHealth : playerHealth;
		setSliderValues ();
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
	/// This function will send this client representers to die
	/// </summary>
	[PunRPC]
	public void sendDeathMessage()
	{
		Destroy(this.gameObject);
		createDeadBody(true);
	}

	#region IPunObservable implementation

	void IPunObservable.OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		if(stream.isWriting)
		{
			stream.SendNext (PlayerHealth);
			stream.SendNext (ArmorPoints);
		}
		else
		{
			playerHealth = (float)stream.ReceiveNext ();
			armorPoints = (float)stream.ReceiveNext ();
			setSliderValues ();
		}
	}

	#endregion
}
