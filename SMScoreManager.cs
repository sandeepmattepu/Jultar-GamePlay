//
//  SMScoreManager.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 29/01/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SandeepMattepu.UI;

namespace SandeepMattepu.Scoring
{
/// <summary>
/// This class will handle all the model layer data like Kills made, experience acquired, current level
/// </summary>
public class SMScoreManager : MonoBehaviour 
{
	/************** Change these values if you want to change the game rules  **************/
	/// <summary>
	/// No of kills made
	/// </summary>
	private static int killsMade = 0;

	/// <summary>
	/// This value resets to 0 when player moves to next level. It also helps us to determine how many points are required for next level
	/// </summary>
	private static int experiencePoints = 0;
	private static int maxLevel = 91;

	/// <summary>
	/// Xp recieved for each kill
	/// </summary>
	private static int killExpPoints = 100;
	private static int currentLevel = 1;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	// Note :- You can use this function to know how many kills are made
	/// <summary>
	/// Use this function to retrive no of kills which can be used for UI or leaderboard
	/// </summary>
	public static int getNoOfKills()
	{
		return SMScoreManager.killsMade;
	}

	// Note :- You can use this function to report a kill
	/// <summary>
	/// Use this function to report a kill
	/// </summary>
	public static void reportKill()
	{
		SMScoreManager.killsMade += 1;
		SMScoreManager.experiencePoints += killExpPoints;
		SMScoreManager.checkPlayerEligibleToNextLevel ();
		SMShowXpMadeInstantly.addXPToQueue (1);
	}

	/// <summary>
	/// This function checks the no of xp points in experiencePoints and compare it with points required for next level and promots the player
	/// to next level if necessary.
	/// </summary>
	private static void checkPlayerEligibleToNextLevel()
	{
		int experienceRequiredForNextLevel = (int)Mathf.Pow (2.0f, (float)currentLevel) * 1000;
		if (SMScoreManager.experiencePoints >= experienceRequiredForNextLevel) 
		{
			if (SMScoreManager.currentLevel < SMScoreManager.maxLevel)
			{
				SMScoreManager.currentLevel += 1;
				SMScoreManager.experiencePoints = (SMScoreManager.experiencePoints - experienceRequiredForNextLevel);
			}
		}
	}

	// Note :- You can use this function to get Player Experience Status
	/// <summary>
	/// Use this function to retrive Player Experience status which is encapsulated inside PlayerExperience struct
	/// </summary>
	/// <returns>PlayerExperience struct which has all the experience details of player</returns>
	public static PlayerExperience getPlayerExperienceStatus()
	{
		PlayerExperience playExp = new PlayerExperience ();
		playExp.currentLevel = SMScoreManager.currentLevel;
		playExp.currentExperiencePonints = SMScoreManager.experiencePoints;

		int experienceRequiredForNextLevel = (int)Mathf.Pow (2.0f, (float)currentLevel) * 1000;
		playExp.nextLevelExperiencePoints = experienceRequiredForNextLevel;
		return playExp;
	}

	public static byte LevelFromExperience(int Experience)
		{
			return Experience < 2000 ? (byte) 1 : (byte) Mathf.Ceil(((Mathf.Log10((float)Experience) - 3) / Mathf.Log10(2f)));
		}
}

/// <summary>
/// This struct encapsulates all the data required for Player Experience
/// </summary>
public struct PlayerExperience
{
	public int currentExperiencePonints;
	public int currentLevel;
	public int nextLevelExperiencePoints;
};

}
