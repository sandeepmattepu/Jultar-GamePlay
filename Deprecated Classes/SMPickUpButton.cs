//
//  SMPickUpButton.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 08/02/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SandeepMattepu.Weapon;

/// <summary>
/// This class will show pick up button at appropriate times and handles the logic when pickup button is pressed. It also reports to UI text which weapon is selected
/// when pickup button is pressed
/// </summary>
[RequireComponent(typeof(Button))]
public class SMPickUpButton : MonoBehaviour 
{
	/// <summary>
	/// This text will show which weapon is selected when player presses pickup button
	/// </summary>
	public Text currentWeaponText;
	/// <summary>
	/// This is the pick up button UI
	/// </summary>
	public static Button pickUpButton;
	/// <summary>
	/// This stores the weapon game object when player stands on the weapon
	/// </summary>
	private static GameObject currentWeaponOnFloor;
	// Use this for initialization
	void Start () 
	{
		pickUpButton = GetComponent<Button> ();
		pickUpButton.gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	/// <summary>
	/// This function is called when PickUP button is pressed. It will destroy the weapon game object floating on floor and report to current weapon UI text
	/// 
	/// DO NOT CALL THIS FUNCTION MANUALLY UNTIL YOU ARE SURE
	/// </summary>
	public void pickUpButtonPressed()
	{
		if(currentWeaponText != null && currentWeaponOnFloor != null)
		{
			currentWeaponText.text = "Current Weapon : " + currentWeaponOnFloor.GetComponent<SMWeaponOnFloor> ().weaponName;
			Destroy (currentWeaponOnFloor);
			currentWeaponOnFloor = null;
			this.gameObject.SetActive (false);
		}
	}

	/// <summary>
	/// Call this function to assign the game object as currentWeaponOnFloor where the player is standing on
	/// </summary>
	/// <param name="weapon">Pass the weapon game object to assign it as the player standing on game object.</param>
	public static void assignCurrentWeapon(GameObject weapon)
	{
		currentWeaponOnFloor = weapon;
	}

	/// <summary>
	/// Call this function to reset currentWeaponOnFloor. When player moves away from weapon on the floor
	/// </summary>
	public static void resetCurrentWeapon()
	{
		currentWeaponOnFloor = null;
	}
}
