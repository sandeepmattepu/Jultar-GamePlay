//
//  ScoreManagerExperimentel.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 29/01/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is deprecated use ScoreManager instead
/// </summary>
public class ScoreManagerExperimentel : MonoBehaviour 
{
	private static int score = 0;
	private static int experiencePoints = 0;
	private static int maxExp = 91;
	private static int killExp = 100;
	private static int currentExp = 1;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	public static int getScore()
	{
		return ScoreManagerExperimentel.score;
	}

	// Use this static method to report player score
	public static void addScore(int scoreToBeAdded, ShowScoreMade instantScore)
	{
		if(scoreToBeAdded > 0)
		{
			ScoreManagerExperimentel.score += scoreToBeAdded;
			if(scoreToBeAdded >= 100)
			{
				if(instantScore != null)
				{
					ShowScoreMade.addInstantScoreToQueue (scoreToBeAdded);
				}
				else
				{
					Debug.LogWarning ("Instatnt score is empty in inspector");
				}
			}
		}
	}

	public static void reportKill()
	{
		ScoreManagerExperimentel.experiencePoints += killExp;
		ScoreManagerExperimentel.checkPlayerEligibleToNextLevel ();
	}

	private static void checkPlayerEligibleToNextLevel()
	{
		int experienceRequiredForNextLevel = (int)Mathf.Pow (2.0f, (float)currentExp) * 1000;
		if (ScoreManagerExperimentel.experiencePoints >= experienceRequiredForNextLevel) 
		{
			if (currentExp < maxExp)
			{
				ScoreManagerExperimentel.currentExp += 1;
				ScoreManagerExperimentel.experiencePoints = (ScoreManagerExperimentel.experiencePoints - experienceRequiredForNextLevel);
			}
		}
	}

	public static PlayerExperienceExperimental getPlayerExperienceStatus()
	{
		PlayerExperienceExperimental playExp = new PlayerExperienceExperimental ();
		playExp.currentExperience = ScoreManagerExperimentel.currentExp;
		playExp.currentExperiencePonints = ScoreManagerExperimentel.currentExp;
		return playExp;
	}
}

public struct PlayerExperienceExperimental
{
	public int currentExperiencePonints;
	public int currentExperience;
	public int nextLevelExperiencePoints;
};
