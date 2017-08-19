//
//  SMWeaponManager.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 09/02/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SandeepMattepu.Weapon;

namespace SandeepMattepu.Weapon
{
	/// <summary>
	/// This class manages the weapon pickup from ground to player, weapon drop from player to ground, reports current weapon to UI text and 
	/// show pickup button at appropriate time
	/// </summary>
	public class SMWeaponManager : MonoBehaviour 
	{
		/// <summary>
		/// This is the UI button shown and hidden when player walks in and out of the weapon collider
		/// </summary>
		public Button pickUpButton;
		/// <summary>
		/// This UI text will show current weapon player is holding
		/// </summary>
		public Text currentWeaponText;
		/// <summary>
		/// This value determines whether the character recieves input from joysticks or from Network
		/// </summary>
		[Tooltip("This value determines whether the character recieves input from joysticks or from Network")]
		public bool isUsingMultiplayer = false;

		/***********	Drag and drop all the prefabs which are present in while holding folder		***********/
		public GameObject july11HoldingPrefab;
		public GameObject greeneyeHoldingPrefab;
		public GameObject leoBlackDogHoldingPrefab;
		public GameObject mrozykHoldingPrefab;
		public GameObject blondeHoldingPrefab;
		public GameObject rainiHoldingPrefab;
		public GameObject smilereHoldingPrefab;
		public GameObject sniperHoldingPrefab;

		/***********	Drag and drop all the prefabs which are present in On floor folder		***********/
		public GameObject july11OnFloorPrefab;
		public GameObject greeneyeOnFloorPrefab;
		public GameObject leoBlackDogOnFloorPrefab;
		public GameObject mrozykOnFloorPrefab;
		public GameObject blondeOnFloorPrefab;
		public GameObject rainiOnFloorPrefab;
		public GameObject smilereOnFloorPrefab;
		public GameObject sniperOnFloorPrefab;

		/// <summary>
		/// Use this to transfer data from gun in hand to gun on floor when weapon swapping is done
		/// </summary>
		private SMGunAmmoDetails dataToBeTransferredToFloorGun;
		/// <summary>
		/// Use this to transfer data from gun on floor to gun in hand when weapon swapping is done
		/// </summary>
		private SMGunAmmoDetails dataToBeTransferredToHandGun;

		/// <summary>
		/// This will hold a reference to gun gameobject which the player is currently holding in his hand
		/// </summary>
		private GameObject currentHoldingGun;
		/// <summary>
		/// This will have some value if player is standing on some weapon otherwise it will be null
		/// </summary>
		private GameObject currentGunPlayerStandingOn;
		/// <summary>
		/// This will describe the weapon player is holding as enum
		/// </summary>
		private GUN_TYPE identifierOfWeaponHolding;
		/// <summary>
		/// Gets the identifier of weapon holding which describes the gun type player is holding.
		/// </summary>
		public GUN_TYPE IdentifierOfWeaponHolding 
		{
			get {	return identifierOfWeaponHolding;	}
		}

		/// <summary>
		/// This is responsible for handling firing from the gun
		/// </summary>
		private SMPlayerFiring playerFiring;

		/// <summary>
		/// This IK is used to control the player's left and right hand postion
		/// </summary>
		private IKControl ikControl;
		/// <summary>
		/// This is the photon component attached to the character
		/// </summary>
		private PhotonView photonViewComponent;
		// Use this for initialization
		void Start () 
		{
			ikControl = GetComponent<IKControl> ();
			playerFiring = GetComponent<SMPlayerFiring> ();
			photonViewComponent = GetComponent<PhotonView> ();
			findCurrentHoldingWeapon ();	// At the begining player will have some weapon
			if((isUsingMultiplayer && photonViewComponent.isMine) || !isUsingMultiplayer)
			{
				setGunBasedOnCustomizationData();
			}
		}

		/// <summary>
		/// This function will make sure that player is holding the appropriate gun based on what he choosed in customization panel
		/// </summary>
		private void setGunBasedOnCustomizationData()
		{
			GUN_TYPE playerChoosenType = (GUN_TYPE)((int)SMProductEquipper.INSTANCE.CurrentEquippedGunType);
			GameObject temp = null;

			switch(playerChoosenType)
			{
			case GUN_TYPE.GreenEye:
				temp = greeneyeHoldingPrefab;
				break;
			case GUN_TYPE.July11:
				temp = july11HoldingPrefab;
				break;
			case GUN_TYPE.LeoBlackDog:
				temp = leoBlackDogHoldingPrefab;
				break;
			case GUN_TYPE.Mrozyk:
				temp = mrozykHoldingPrefab;
				break;
			case GUN_TYPE.Blonde:
				temp = blondeHoldingPrefab;
				break;
			case GUN_TYPE.Raini:
				temp = rainiHoldingPrefab;
				break;
			case GUN_TYPE.Smilere:
				temp = smilereHoldingPrefab;
				break;
			case GUN_TYPE.Sniper:
				temp = sniperHoldingPrefab;
				break;
			}
			currentHoldingGun.transform.SetParent (null);			// Detach child before destroying to avoid bugs
			Destroy (currentHoldingGun);
			currentHoldingGun = null;
			Transform position = temp.transform;
			GameObject gunToHold = Instantiate (temp, transform) as GameObject;
			gunToHold.transform.localPosition = position.position;
			gunToHold.transform.localRotation = position.rotation;

			playerFiring.reportWeaponChange ();
			findCurrentHoldingWeapon ();
			placeIKAppropriately ();

			if(isUsingMultiplayer && photonViewComponent.isMine)		// Send weapon change message to other remote clients who represent this client
			{
				photonViewComponent.RPC ("setGunTypeOfRemoteBotPlayer", PhotonTargets.Others, identifierOfWeaponHolding.ToString ());
			}
		}

		/// <summary>
		/// This function will find out which weapon the player is holding and assigns currentHoldingGun when found the weapon in hand
		/// </summary>
		private void findCurrentHoldingWeapon()
		{
			bool foundGun = false;
			foreach(Transform child in transform)
			{
				if(child.tag == "Weapon")
				{
					foundGun = true;
					ikControl.placeHandsToAim();
					currentHoldingGun = child.gameObject;
					identifierOfWeaponHolding = currentHoldingGun.GetComponent<SMWeaponInHand>().weaponInHandIdentifier;
				}
			}

			if(isUsingMultiplayer)
			{
				if(photonViewComponent.isMine)
				{
					currentWeaponText.text = identifierOfWeaponHolding.ToString();
				}
			}
			else
			{
				currentWeaponText.text = identifierOfWeaponHolding.ToString();
				if (!foundGun) 
				{
					ikControl.placeHandsToSwing ();
				}
			}
		}

		/// <summary>
		/// This function will replace the weapon the player is currently holding in his hand with the weapon the player is standing on when pickup button is pressed
		/// (which is passed as parameter)
		/// </summary>
		/// <param name="replaceWith">Pass which gun to be instantiated at player's hand position</param>
		private void replaceCurrentHoldingWeapon(GameObject replaceWith)
		{
			// Retrive ammunation details of the gun in hand
			dataToBeTransferredToFloorGun = currentHoldingGun.GetComponent<SMWeaponInHand> ().getAmmunationDetails ();		// Get ammunation details from gun in hand
			currentHoldingGun.transform.SetParent (null);			// Detach child before destroying to avoid bugs
			Destroy (currentHoldingGun);
			currentHoldingGun = null;
			Transform position = replaceWith.transform;
			GameObject gunToHold = Instantiate (replaceWith, transform) as GameObject;
			gunToHold.transform.localPosition = position.position;
			gunToHold.transform.localRotation = position.rotation;
			gunToHold.GetComponent<SMWeaponInHand> ().setAmmunationDetails (dataToBeTransferredToHandGun);	// Set ammunation details to the gun in hand
		}

		private void setWeaponInHand(GameObject gunToPlace)
		{
			currentHoldingGun = null;
			Transform position = gunToPlace.transform;
			GameObject gunToHold = Instantiate (gunToPlace, transform) as GameObject;
			gunToHold.transform.localPosition = position.position;
			gunToHold.transform.localRotation = position.rotation;
			gunToHold.GetComponent<SMWeaponInHand> ().setAmmunationDetails (dataToBeTransferredToHandGun);	// Set ammunation details to the gun in hand
		}

		void OnTriggerEnter(Collider collider)
		{
			if(isUsingMultiplayer)
			{
				if(photonViewComponent.isMine)
				{
					if(collider.tag == "Weapon")		// When player enters weapon collider
					{
						currentGunPlayerStandingOn = collider.gameObject;
						pickUpButton.gameObject.SetActive (true);
						// Retrive ammunation details from floor gun
						dataToBeTransferredToHandGun = collider.gameObject.GetComponent<SMWeaponOnFloor> ().getAmmunationDetails ();	// Get ammunation details from gun on floor
					}
				}
			}
			else
			{
				if(collider.tag == "Weapon")		// When player enters weapon collider
				{
					currentGunPlayerStandingOn = collider.gameObject;
					pickUpButton.gameObject.SetActive (true);
					// Retrive ammunation details from floor gun
					dataToBeTransferredToHandGun = collider.gameObject.GetComponent<SMWeaponOnFloor> ().getAmmunationDetails ();	// Get ammunation details from gun on floor
				}
			}
		}

		void OnTriggerExit(Collider collider)
		{
			if(isUsingMultiplayer)
			{
				if(photonViewComponent.isMine)
				{
					if(collider.tag == "Weapon")		// When player leaves weapon collider
					{
						currentGunPlayerStandingOn = null;
						pickUpButton.gameObject.SetActive (false);
					}
				}
			}
			else
			{
				if(collider.tag == "Weapon")		// When player leaves weapon collider
				{
					currentGunPlayerStandingOn = null;
					pickUpButton.gameObject.SetActive (false);
				}
			}
		}

		/// <summary>
		/// This function is called when player hits pickup button UI. This funtion will drop the currentHoldingGun to ground and picks the gun that is present on the floor
		/// </summary>
		public void pickUpButtonPressed()
		{
			if(currentHoldingGun != null && currentGunPlayerStandingOn != null)
			{
				playerFiring.setDataForSwapping ();
				GameObject gunToHold = calculateWhichPrefabPlayerWillHandle ();
				replaceCurrentHoldingWeapon (gunToHold);
				playerFiring.reportWeaponChange ();
				dropCurrentHoldingWeaponToFloor ();
				findCurrentHoldingWeapon ();
				placeIKAppropriately ();

				if(isUsingMultiplayer && photonViewComponent.isMine)		// Send weapon change message to other remote clients who represent this client
				{
					photonViewComponent.RPC ("setGunTypeOfRemoteBotPlayer", PhotonTargets.Others, identifierOfWeaponHolding.ToString ());
				}
			}
			else if(currentGunPlayerStandingOn != null)		// Previously player has no weapons
			{
				if(!isUsingMultiplayer)			// Below code is applicable only to campaing not multiplayer
				{
					GameObject gunToHold = calculateWhichPrefabPlayerWillHandle ();
					setWeaponInHand (gunToHold);
					playerFiring.reportWeaponChange ();
					findCurrentHoldingWeapon ();
					placeIKAppropriately ();
					Destroy (currentGunPlayerStandingOn);
				}
			}
			else
			{
				Debug.LogWarning ("UnKnown Error Please Debug the SMWeaponManager script");
			}
		}

		/// <summary>
		/// This function identifies on which gun, player is standing on and returns which prefab it needs to instantiate at hand position when player hits pickup button
		/// </summary>
		/// <returns>The prefab to instantiate at player's hand</returns>
		private GameObject calculateWhichPrefabPlayerWillHandle()
		{
			GUN_TYPE name = currentGunPlayerStandingOn.GetComponent<SMWeaponOnFloor> ().weaponName;

			switch(name)
			{
			case GUN_TYPE.GreenEye:
				return greeneyeHoldingPrefab;
			case GUN_TYPE.July11:
				return july11HoldingPrefab;
			case GUN_TYPE.LeoBlackDog:
				return leoBlackDogHoldingPrefab;
			case GUN_TYPE.Mrozyk:
				return mrozykHoldingPrefab;
			case GUN_TYPE.Blonde:
				return blondeHoldingPrefab;
			case GUN_TYPE.Raini:
				return rainiHoldingPrefab;
			case GUN_TYPE.Smilere:
				return smilereHoldingPrefab;
			case GUN_TYPE.Sniper:
				return sniperHoldingPrefab;
			}

			Debug.LogError("Weapon name or tag might have changed please debug and fix it");
			return null;
		}

		/// <summary>
		/// This function will drop the currently holding weapon to floor by instatiating appropriate floor weapon prefab
		/// </summary>
		private void dropCurrentHoldingWeaponToFloor()
		{
			float yPos = currentGunPlayerStandingOn.transform.position.y;
			if(isUsingMultiplayer)
			{
				if(PhotonNetwork.connectedAndReady && PhotonNetwork.isMasterClient)
				{
					PhotonNetwork.Destroy (currentGunPlayerStandingOn);
				}
				else
				{
					currentGunPlayerStandingOn.GetComponent<SMWeaponOnFloor> ().requestToDestroySelfInRemoteClients ();
				}
			}
			else
			{
				Destroy (currentGunPlayerStandingOn);
			}
			GameObject weaponToInstantiate = null;

			switch(identifierOfWeaponHolding)
			{
			case GUN_TYPE.GreenEye:
				weaponToInstantiate = greeneyeOnFloorPrefab;
				break;
			case GUN_TYPE.July11:
				weaponToInstantiate = july11OnFloorPrefab;
				break;
			case GUN_TYPE.LeoBlackDog:
				weaponToInstantiate = leoBlackDogOnFloorPrefab;
				break;
			case GUN_TYPE.Mrozyk:
				weaponToInstantiate = mrozykOnFloorPrefab;
				break;
			case GUN_TYPE.Blonde:
				weaponToInstantiate = blondeOnFloorPrefab;
				break;
			case GUN_TYPE.Raini:
				weaponToInstantiate = rainiOnFloorPrefab;
				break;
			case GUN_TYPE.Smilere:
				weaponToInstantiate = smilereOnFloorPrefab;
				break;
			case GUN_TYPE.Sniper:
				weaponToInstantiate = sniperOnFloorPrefab;
				break;
			}

			if(weaponToInstantiate != null)
			{
				if(isUsingMultiplayer)
				{
					if(PhotonNetwork.connectedAndReady && PhotonNetwork.isMasterClient)
					{
						Vector3 pos = new Vector3 (transform.position.x, yPos, transform.position.z);
						// Building ammo data to be transferred
						int bulletsLeftData = dataToBeTransferredToFloorGun.bulletsLeft;
						int extraClipsLeftData = dataToBeTransferredToFloorGun.extraClipsLeft;
						float reloadTimeData = dataToBeTransferredToFloorGun.reloadTime;
						float damageMadedata = dataToBeTransferredToFloorGun.damageMade;

						object[] data = {	bulletsLeftData,
											extraClipsLeftData,
											reloadTimeData,
											damageMadedata
										};
						PhotonNetwork.InstantiateSceneObject (weaponToInstantiate.name, pos, Quaternion.identity, 0, data);
					}
					else
					{
						// send request to master to instantiate a prefab there
						Vector3 pos = new Vector3 (transform.position.x, yPos, transform.position.z);
						int bulletsLeftData = dataToBeTransferredToFloorGun.bulletsLeft;
						int extraClipsLeftData = dataToBeTransferredToFloorGun.extraClipsLeft;
						float reloadTimeData = dataToBeTransferredToFloorGun.reloadTime;
						float damageMadedata = dataToBeTransferredToFloorGun.damageMade;

						object[] data = {	bulletsLeftData,
											extraClipsLeftData,
											reloadTimeData,
											damageMadedata
										};
						photonViewComponent.RPC ("sendRequestToInstantiateGun", PhotonTargets.MasterClient, weaponToInstantiate.name, pos, data);
					}
				}
				else
				{
					GameObject afterInstantiation = Instantiate (weaponToInstantiate) as GameObject;
					Vector3 pos = new Vector3 (transform.position.x, yPos, transform.position.z);
					afterInstantiation.transform.position = pos;
					afterInstantiation.GetComponent<SMWeaponOnFloor> ().setAmmunationDetails (dataToBeTransferredToFloorGun);	// Set ammunation details to floor
				}
			}
			else
			{
				Debug.LogWarning ("Can't drop weapons on floor please check whether you have renamed the prefabs or their identifiers");
			}

			currentGunPlayerStandingOn = null;
		}

		/// <summary>
		/// This function is used to place the left and right hand position of the player at proper places for currentHoldingGun
		/// </summary>
		private void placeIKAppropriately()
		{
			Transform leftHandHolder = null;
			Transform rightHandHolder = null;
			Transform tempPos = null;
			foreach(Transform handle in currentHoldingGun.transform)
			{
				if(handle.gameObject.name == "LeftHandle")
				{
					leftHandHolder = handle;
				}
				else if(handle.gameObject.name == "RightHandle")
				{
					rightHandHolder = handle;
				}
				else if(handle.gameObject.name == "TempPos")
				{
					tempPos = handle;
				}
			}

			ikControl.leftHandObj = leftHandHolder;
			ikControl.rightHandObj = rightHandHolder;
			ikControl.tempPos = tempPos;
		}

		/// <summary>
		/// Sets the gun type of remote bot player based on the string value passsed to it.
		/// </summary>
		/// <param name="gunType">This string should be exactly as <see cref="GUN_TYPE"/> enum values.To avoid errors pass <see cref="GUN_TYPE.value.ToString()"/></param>
		[PunRPC]
		public void setGunTypeOfRemoteBotPlayer(string gunType)
		{
			GameObject gunToInstantiate = null;

			switch (gunType)
			{
			case "Blonde":
				gunToInstantiate = blondeHoldingPrefab;
				break;
			case "GreenEye":
				gunToInstantiate = greeneyeHoldingPrefab;
				break;
			case "July11":
				gunToInstantiate = july11HoldingPrefab;
				break;
			case "LeoBlackDog":
				gunToInstantiate = leoBlackDogHoldingPrefab;
				break;
			case "Mrozyk":
				gunToInstantiate = mrozykHoldingPrefab;
				break;
			case "Raini":
				gunToInstantiate = rainiHoldingPrefab;
				break;
			case "Smilere":
				gunToInstantiate = smilereHoldingPrefab;
				break;
			case "Sniper":
				gunToInstantiate = sniperHoldingPrefab;
				break;
			}

			if(gunToInstantiate != null)	// Perform instantiation if there is change in weapon
			{
				// Below code is to avoid null error
				ikControl = GetComponent<IKControl> ();
				playerFiring = GetComponent<SMPlayerFiring> ();
				photonViewComponent = GetComponent<PhotonView> ();
				if(currentHoldingGun == null)
				{
					findCurrentHoldingWeapon ();
				}
				Transform posAndRot = currentHoldingGun.transform;
				currentHoldingGun.transform.SetParent (null);
				Destroy (currentHoldingGun);
				currentHoldingGun = null;

				GameObject gunToHold = Instantiate (gunToInstantiate, transform) as GameObject;
				gunToHold.transform.SetParent (transform);
				gunToHold.transform.localPosition = gunToInstantiate.transform.position;
				gunToHold.transform.localRotation = gunToInstantiate.transform.rotation;

				findCurrentHoldingWeapon ();
				placeIKAppropriately ();

				playerFiring.reportWeaponChange ();
			}
		}

		/// <summary>
		/// Sends the request to master client to instantiate gun.
		/// </summary>
		/// <param name="nameOfTheGun">Name of the gun.</param>
		/// <param name="pos">Position to instantiate.</param>
		[PunRPC]
		private void sendRequestToInstantiateGun(string nameOfTheGun, Vector3 pos, object[] data)
		{
			PhotonNetwork.InstantiateSceneObject (nameOfTheGun, pos, Quaternion.identity, 0, data);
		}
	}
}