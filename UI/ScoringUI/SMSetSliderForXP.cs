//
//  SMSetSliderForXP.cs
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
/// This class will help the slider to position itself based on the Player's experirnce
/// </summary>
[RequireComponent(typeof(Slider))]
public class SMSetSliderForXP : MonoBehaviour 
{
	/// <summary>
	/// This is the slider UI this script controls
	/// </summary>
	private Slider sliderXp;

	/// <summary>
	/// It contains player expeerience data encapsulated in it
	/// </summary>
	private PlayerExperience currentPlayerExperience;

	void Start () 
	{
		sliderXp = GetComponent<Slider> ();
	}
	void Update () 
	{
		updateUI ();
	}

	/// <summary>
	/// This function will update the slider based on the player experirnce value present in ScoreManager.cs
	/// </summary>
	private void updateUI()
	{
		currentPlayerExperience = SMScoreManager.getPlayerExperienceStatus ();
		float maxValue = currentPlayerExperience.nextLevelExperiencePoints;
		float currentValue = currentPlayerExperience.currentExperiencePonints;;
		float fraction = (currentValue / maxValue);
		sliderXp.value = fraction;
	}
}
}
