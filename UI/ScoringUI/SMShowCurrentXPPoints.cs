//
//  SMShowCurrentXPPoints.cs
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
/// This class will help the Text UI to show number of xp points present in Player's current level
/// </summary>
[RequireComponent(typeof(Text))]
public class SMShowCurrentXPPoints : MonoBehaviour 
{
	/// <summary>
	/// This is the Text UI this script controls
	/// </summary>
	private Text currentExperiencePoints;

	/// <summary>
	/// It contains player expeerience data encapsulated in it
	/// </summary>
	private PlayerExperience currentPlayerExperience;

	void Start () 
	{
		currentExperiencePoints = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		updateUI ();
	}

	/// <summary>
	/// This function will update the Text UI based on the player xp points available in current level
	/// </summary>
	private void updateUI ()
	{
		currentPlayerExperience = SMScoreManager.getPlayerExperienceStatus ();
		currentExperiencePoints.text = "Current Xp points: " + currentPlayerExperience.currentExperiencePonints.ToString();
	}
}
}
