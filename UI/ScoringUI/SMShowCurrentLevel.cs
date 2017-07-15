﻿//
//  SMShowCurrentLevel.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 30/01/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SandeepMattepu.Scoring;

namespace SandeepMattepu.UI
{
/// <summary>
/// This class will set the text UI based on player's level
/// </summary>
[RequireComponent(typeof(Text))]
public class SMShowCurrentLevel : MonoBehaviour 
{
	/// <summary>
	/// This is the UI which this script controlls
	/// </summary>
	private Text currentLevelUI;

	/// <summary>
	/// It contains player expeerience data encapsulated in it
	/// </summary>
	private PlayerExperience currentPlayerExperience;

	void Start () 
	{
		currentLevelUI = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		updateUI ();
	}

	/// <summary>
	/// This function will update the Text UI based on the player's level value present in ScoreManager.cs
	/// </summary>
	private void updateUI ()
	{
		currentPlayerExperience = SMScoreManager.getPlayerExperienceStatus ();
		currentLevelUI.text = "Level: " + currentPlayerExperience.currentLevel.ToString();
	}
}
}
