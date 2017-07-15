//
//  ShowInstantScore.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 30/01/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is deprecated use ShowXpMadeInstantly instead
/// </summary>
[RequireComponent(typeof(Animator)), RequireComponent(typeof(Animator))]
public class ShowScoreMade : MonoBehaviour 
{
	[Tooltip("Drag and drop game object which has UI audio source attached to it")]
	public AudioSource audioForScore;

	/// <summary>
	/// This is the Text UI this script controls
	/// </summary>
	private Text scoreText;

	/// <summary>
	/// This is the animator which controls the animation of the text
	/// </summary>
	private Animator animator;

	/// <summary>
	/// This helps us to determine whether Text UI is animating or finished
	/// </summary>
	private bool hasCompletedAnimating = true;

	/// <summary>
	/// This will maintain the List/queue of all the messages to be shown to the player
	/// </summary>
	private static List<int> scoreQueue = new List<int> ();

	void Start () 
	{
		animator = GetComponent<Animator> ();
		scoreText = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		keepShowingScores ();		// This will try to keep on showing the animations
	}

	/// <summary>
	/// This function will start the animation of the Text
	/// </summary>
	private void animateShowScore()
	{
		hasCompletedAnimating = false;		// It is set to false because animation started
		animator.SetTrigger ("ShowScore");
		audioForScore.Play ();				// Play the sound
	}

	/// <summary>
	/// This function will be called by the animation event when animation is about to finish.
	/// DONT CALL THIS FUNCTION UNTIL YOU ARE SURE
	/// </summary>
	public void animationFinished()
	{
		hasCompletedAnimating = true;
		if(audioForScore.isPlaying)
		{
			audioForScore.Stop ();		
		}
	}

	// Note :- You can use this function to add Xp animation to queue
	/// <summary>
	/// Use this function to add Xp animation to queue, which will be later animated appropriately
	/// </summary>
	/// <param name="scoreToBeAdded">Pass the amount of xp to be shown on UI</param>
	public static void addInstantScoreToQueue(int scoreToBeAdded)
	{
		scoreQueue.Add (scoreToBeAdded);
	}

	/// <summary>
	///	This function will handle the logic of animating UI at proper time
	/// </summary>
	private void keepShowingScores()
	{
		if(hasCompletedAnimating && scoreQueue.Count != 0)
		{
			scoreText.text = "+" + scoreQueue [0].ToString ();
			scoreQueue.RemoveAt (0);
			animateShowScore ();
		}
	}
}
