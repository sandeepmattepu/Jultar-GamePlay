//
//  SMTimeCounter.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 29/01/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class will load, save and count the Time played by the player
/// </summary>
public class SMTimeCounter : MonoBehaviour 
{
	/// <summary>
	/// This is the key used to save amount of time player played
	/// </summary>
	private const string countTimeKey = "TIME_PLAYED";
	/// <summary>
	/// This stores total amount of time player played the game in seconds
	/// </summary>
	private static float secondsPlayed;
	/// <summary>
	/// This UI Text will show amount of time user played the game
	/// </summary>
	public Text playedTimeText;

	/*These values store time played by the game in format*/
	private int seconds;
	private int minutes;
	private int hours;
	private int days;
	// Use this for initialization
	void Start () 
	{
		secondsPlayed = loadPlayTime ();
		setTimeValuesForFormat ((int)secondsPlayed);
	}
	
	// Update is called once per frame
	void Update () 
	{
		secondsPlayed += Time.deltaTime;
		setTimeValuesForFormat ((int)secondsPlayed);
		if(playedTimeText != null)
		{
			playedTimeText.text = "Played Time is: " + days + " Days " + hours + " Hours " + minutes + " Minutes " + seconds + " Seconds ";
		}
		else
		{
			Debug.Log ("Days " + days + " Hours " + hours + " Minutes " + minutes + "Seconds " + seconds);
		}
	}

	/// <summary>
	/// This function will load the played time value if there is any
	/// </summary>
	/// <returns>Play time in seconds.</returns>
	private int loadPlayTime()
	{
		if(PlayerPrefs.HasKey(countTimeKey))
		{
			int playTime = PlayerPrefs.GetInt (countTimeKey);
			return playTime;
		}
		return 0;
	}

	/// <summary>
	/// This function will save the amount of time player played the game
	/// </summary>
	public void savePlayTime()
	{
		PlayerPrefs.SetInt (countTimeKey,(int)secondsPlayed);
	}

	/// <summary>
	/// This function will calculate number of given seconds and breaks them into format like days,hours,minutes,seconds
	/// </summary>
	/// <param name="Number of seconds user played the game">Seconds played.</param>
	private void setTimeValuesForFormat(int secondsPlayed)
	{
		seconds = secondsPlayed;

		minutes = (seconds / 60);
		seconds = (seconds % 60);

		hours = (minutes / 60);
		minutes = (minutes % 60);

		days = (hours / 24);
		hours = (hours % 24);
	}
}
