//
//  SMWeaponDetails.cs
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
/// Use this struct to describe how much ammo is left for a gun, reload time and damage the gun can make
/// </summary>
public struct SMGunAmmoDetails
{
	public ObscuredInt bulletsLeft;
	public ObscuredInt extraClipsLeft;
	public ObscuredFloat reloadTime;
	public ObscuredFloat damageMade;
}

/// <summary>
/// This enum describes the gun type
/// </summary>
public enum GUN_TYPE
{
	July11,
	GreenEye,
	LeoBlackDog,
	Mrozyk,
	Blonde,
	Raini,
	Smilere,
	Sniper
}

/// <summary>
/// This enum describes the fire rate of the weapon
/// </summary>
public enum FIRE_RATE
{
	SLOW,
	MEDIUM,
	FAST
}