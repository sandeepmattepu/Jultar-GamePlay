//
//  SMWeaponOnFloor.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 08/02/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SandeepMattepu.Weapon
{
	/// <summary>
	/// This class is responsible to play appropriate animation of the weapon on the floor and also helps us to identify and store weapon details
	/// </summary>
	[RequireComponent(typeof(Animator))]
	public class SMWeaponOnFloor : MonoBehaviour 
	{
		/// <summary>
		/// This describes the weapon floating on the ground
		/// </summary>
		public GUN_TYPE weaponName;

		[Tooltip("Type no of bullets that each gun clip can carry")]
		[SerializeField]
		private int noOfBulletsInClip;

		[Tooltip("Type no of clips this gun can have excluding the one present in gun")]
		[SerializeField]
		private int extraNoOfClips;

		[Tooltip("Type the speed at which reload happens")]
		[SerializeField]
		private float reloadTime = 2.0f;

		/// <summary>
		/// The damage made by the gun bullets to the player.
		/// </summary>
		[Tooltip("The damage made by the gun bullets to the player.")]
		[SerializeField]
		private float damageMade;

		/// <summary>
		/// This holds the ammo details of the gun as struct
		/// </summary>
		private SMGunAmmoDetails ammoDetails;

		private PhotonView photonViewComponent;
		// Use this for initialization
		void Start () 
		{
			photonViewComponent = GetComponent<PhotonView> ();
			playAppropriateAnimation ();

			ammoDetails = new SMGunAmmoDetails ();

			if(photonViewComponent.instantiationData != null)		// If this game object is used in mp context then data is recieved from photon
			{
				noOfBulletsInClip = (int)photonViewComponent.instantiationData [0];
				extraNoOfClips = (int)photonViewComponent.instantiationData [1];
				reloadTime = (float)photonViewComponent.instantiationData [2];
				damageMade = (float)photonViewComponent.instantiationData [3];
			}

			ammoDetails.bulletsLeft = noOfBulletsInClip;
			ammoDetails.extraClipsLeft = extraNoOfClips;
			ammoDetails.reloadTime = reloadTime;
			ammoDetails.damageMade = damageMade;
		}
		
		// Update is called once per frame
		void Update () 
		{
		}

		/// <summary>
		/// Use this function to retrive ammunation details from the gun present on floor
		/// </summary>
		/// <returns>Ammunation Details.</returns>
		public SMGunAmmoDetails getAmmunationDetails()
		{
			ammoDetails = new SMGunAmmoDetails ();
			ammoDetails.bulletsLeft = noOfBulletsInClip;
			ammoDetails.extraClipsLeft = extraNoOfClips;
			ammoDetails.reloadTime = reloadTime;
			ammoDetails.damageMade = damageMade;
			return ammoDetails;
		}

		/// <summary>
		/// Use this function to set the ammunation details to gun on floor
		/// </summary>
		/// <param name="gunAmmoDetails">Pass gun details struct</param>
		public void setAmmunationDetails(SMGunAmmoDetails gunAmmoDetails)
		{
			ammoDetails = gunAmmoDetails;
			noOfBulletsInClip = gunAmmoDetails.bulletsLeft;
			extraNoOfClips = gunAmmoDetails.extraClipsLeft;
			reloadTime = gunAmmoDetails.reloadTime;
			damageMade = gunAmmoDetails.damageMade;
		}

		/// <summary>
		/// This function will analyze the weapon name and plays appropraite animation
		/// </summary>
		private void playAppropriateAnimation()
		{
			// Maybe useful later
//			switch (weaponName) 
//			{
//			case GUN_TYPE.July11:
//				// do nothing because default state is july11 animation
//				break;
//			case GUN_TYPE.GreenEye:
//				animator.SetTrigger ("Greeneye");
//				break;
//			case GUN_TYPE.LeoBlackDog:
//				animator.SetTrigger ("LeoBlackDog");
//				break;
//			case GUN_TYPE.Mrozyk:
//				animator.SetTrigger ("Mrozyk");
//				break;
//			case GUN_TYPE.Blonde:
//				animator.SetTrigger ("Blonde");
//				break;
//			default:
//				Debug.LogError ("The weaponName is wrong check the inspector");
//				break;
//			}
		}

		/// <summary>
		/// Call this method to destroy this gun on floor in all remote clients. CALL THIS FUNCTION ONLY UNDER NETWORK CONTEXT
		/// </summary>
		public void requestToDestroySelfInRemoteClients()
		{
			photonViewComponent.RPC ("destroyGunInAllRemoteClients", PhotonTargets.All, null);
		}

		/// <summary>
		/// This function will actually delete the self game object
		/// </summary>
		[PunRPC]
		private void destroyGunInAllRemoteClients()
		{
			Destroy (this.gameObject);
		}
	}
}
