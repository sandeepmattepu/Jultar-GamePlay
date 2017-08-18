//
//  SMWeaponInHand.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 09/02/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;
/// <summary>
/// This class will help to identify the gun that is present in player's hand as different 
/// </summary>
public class SMWeaponInHand : MonoBehaviour 
{
	/// <summary>
	/// This will dictate how fast the reloading should happen
	/// </summary>
	[Tooltip("This will dictate how fast the reloading should happen")]
	public ObscuredFloat reloadTime = 2.0f;
	/// <summary>
	/// This describes the weapon in hand
	/// </summary>
	public GUN_TYPE weaponInHandIdentifier;
	/// <summary>
	/// This will determine the firing speed of the gun
	/// </summary>
	public ObscuredFloat secondsPerBullet = 0.2f;
	/// <summary>
	/// This holds reference to audio clip that this gun makes while firing
	/// </summary>
	[SerializeField]
	private AudioClip gunSoundMadeByGun;
	/// <summary>
	/// The reload sound made by gun.
	/// </summary>
	[SerializeField]
	private AudioClip reloadSoundMadeByGun;
	/// <summary>
	/// The reload sound made by gun.
	/// </summary>
	public AudioClip ReloadSoundMadeByGun
	{
		get{ 	return reloadSoundMadeByGun;	}
	}

	[Tooltip("Type no of bullets that each gun clip can carry")]
	[SerializeField]
	private ObscuredInt noOfBulletsInClip;

	[Tooltip("Type no of clips this gun can have excluding the one present in gun")]
	[SerializeField]
	private ObscuredInt extraNoOfClips;

	/// <summary>
	/// The damage made by the gun bullets to the player.
	/// </summary>
	[Tooltip("The damage made by the gun bullets to the player.")]
	[SerializeField]
	private ObscuredFloat damageMade;

	/// <summary>
	/// This holds the ammo details of the gun as struct
	/// </summary>
	private SMGunAmmoDetails ammoDetails;

	void Start()
	{
		ammoDetails = new SMGunAmmoDetails ();
		ammoDetails.bulletsLeft = noOfBulletsInClip;
		ammoDetails.extraClipsLeft = extraNoOfClips;
		ammoDetails.reloadTime = reloadTime;
		ammoDetails.damageMade = damageMade;
	}

	/// <summary>
	/// Use this function to retrive ammunation details from the gun present in hand
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
	/// Use this function to set the ammunation details to gun in hand
	/// </summary>
	/// <param name="gunAmmoDetails">Pass gun details struct</param>
	public void setAmmunationDetails(SMGunAmmoDetails gunAmmoDetails)
	{
		ammoDetails = gunAmmoDetails;
		noOfBulletsInClip = ammoDetails.bulletsLeft;
		extraNoOfClips = ammoDetails.extraClipsLeft;
		reloadTime = ammoDetails.reloadTime;
		damageMade = ammoDetails.damageMade;
	}

	/// <summary>
	/// Use this function to retrive which gun type the player is holding in his hand
	/// </summary>
	/// <returns>The gun type as enum.</returns>
	public GUN_TYPE getGunType()
	{
		return weaponInHandIdentifier;
	}

	/// <summary>
	/// Use this function to retrive the audio sound made by the gun
	/// </summary>
	/// <returns>The sound clip made by the gun.</returns>
	public AudioClip getGunSound()
	{
		if(gunSoundMadeByGun != null)
		{
			return gunSoundMadeByGun;
		}
		else
		{
			Debug.LogError ("Audio clip is not assigned check the inspector");
			return null;
		}
	}

	/// <summary>
	/// Use this function to retrive seconds per bullet the gun in hand fires
	/// </summary>
	/// <returns>The number of seconds per bullet.</returns>
	public float getSecondsPerBullet()
	{
		return secondsPerBullet;
	}
}
