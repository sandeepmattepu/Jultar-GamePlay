//
//  SMHealthUI.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 25/02/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// This class will make health bar and armor bar always stay with the player. It also controls the sliders of both health and armor
/// </summary>
public class SMHealthUI : MonoBehaviour 
{
	#region Public Variables

	/// <summary>
	/// The camera to which the UI should always face
	/// </summary>
	public Camera faceTowardsCamera;
	/// <summary>
	/// The player this UI should always follow.
	/// </summary>
	public Transform playerToFollow;
	/// <summary>
	/// The height offset for UI from player.
	/// </summary>
	[Tooltip("The height offset for UI from player.")]
	public float heightOffsetFromPlayer = 0.0f;

	#endregion

	#region Private Variables

	/// <summary>
	/// This slider represents health of the player
	/// </summary>
	private Slider healthSlider;
	/// <summary>
	/// This slider represents armor of the player
	/// </summary>
	private Slider armorSlider;
	/// <summary>
	/// This image represents the armor
	/// </summary>
	private Image imageOfArmor;

	#endregion

	#region MonoBehaviour Calls

	// Use this for initialization
	void Start () 
	{
		findSlidersInChild ();
		faceTowardsCamera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.rotation = faceTowardsCamera.transform.rotation;
		transform.position = playerToFollow.position + (Vector3.up * heightOffsetFromPlayer);
	}

	#endregion
	/// <summary>
	/// This function will find both health and armor sliders present in the child of the transform
	/// </summary>
	private void findSlidersInChild()
	{
		foreach(Transform child in transform)
		{
			if(child.name == "Health_Slider")
			{
				healthSlider = child.GetComponent<Slider> ();
			}
			else if(child.name == "Armor")
			{
				armorSlider = child.GetComponent<Slider> ();
			}
			else if(child.name == "Armor_Icon")
			{
				imageOfArmor = child.GetComponent<Image> ();
			}
		}
	}

	/// <summary>
	/// Call this function to set the health slider of the player
	/// </summary>
	/// <param name="minValue">Minimum value of health.</param>
	/// <param name="maxValue">Max value of health.</param>
	/// <param name="actualValue">Actual value of health player is having.</param>
	public void setHealthSlider(float minValue, float maxValue, float actualValue)
	{
		float fraction = (actualValue - minValue) / (maxValue - minValue);
		if(healthSlider != null)
		{
			healthSlider.value = fraction;
			Color resultColor = Color.Lerp (Color.red, Color.green, fraction);
			if(fraction == 0.0f)			// This is to fix the slider bug
			{
				resultColor = Color.white;
			}
			healthSlider.fillRect.GetComponent<Image> ().color = resultColor;
		}
	}

	/// <summary>
	/// Call this function to set the armor slider of the player
	/// </summary>
	/// <param name="minValue">Minimum value of health.</param>
	/// <param name="maxValue">Max value of health.</param>
	/// <param name="actualValue">Actual value of health player is having.</param>
	public void setArmorSlider(float minValue, float maxValue, float actualValue)
	{
		if(armorSlider != null)
		{
			float fraction = (actualValue - minValue) / (maxValue - minValue);
			if(fraction > 0.0f)
			{
				armorSlider.gameObject.SetActive (true);
				imageOfArmor.gameObject.SetActive (true);
			}
			else if( fraction <= 0.0f)
			{
				armorSlider.gameObject.SetActive (false);
				imageOfArmor.gameObject.SetActive (false);
			}
			armorSlider.value = fraction;
		}
	}
}
