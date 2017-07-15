//
//  SMShowXpMadeInstantly.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 30/01/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SandeepMattepu.UI
{
/// <summary>
/// This class will help the Text UI to appear on screen in zoom in style
/// </summary>
[RequireComponent(typeof(Animator)), RequireComponent(typeof(Text))]
public class SMShowXpMadeInstantly : MonoBehaviour 
{
	[Tooltip("Drag and drop game object which has UI audio source attached to it")]
	public AudioSource audioForScore;

	/// <summary>
	/// This is the Text UI this script controls
	/// </summary>
	private Text xpRecievedText;

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
	private static List<int> xpQueue = new List<int> ();
	
	void Awake()
	{
		xpQueue.Clear ();	
	}
	
	void Start () 
	{
		animator = GetComponent<Animator> ();
		xpRecievedText = GetComponent<Text> ();
	}

	// Update is called once per frame
	void Update () 
	{
		keepShowingScores ();		// This will try to keep on showing the animations
	}

	/// <summary>
	/// This function will start the animation of the Text
	/// </summary
	private void animateXPMadeUI()
	{
		hasCompletedAnimating = false;		// It is set to false because animation started
		animator.SetTrigger ("ShowXPMade");
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
			audioForScore.Stop ();			// Stop the sound
		}
	}

	/// <summary>
	/// Use this function to add Xp animation to queue, which will be later animated appropriately
	/// </summary>
	/// <param name="xpMade">Xp made by the player.</param>
	public static void addXPToQueue(int xpMade)
	{
			xpQueue.Add (xpMade);
	}

	/// <summary>
	///	This function will handle the logic of animating UI at proper time
	/// </summary>
	private void keepShowingScores()
	{
		if(xpRecievedText != null)
		{
			if (hasCompletedAnimating && xpQueue.Count != 0)
			{
				xpRecievedText.text = "+ " + xpQueue[0] + "XP";
				xpQueue.RemoveAt(0);
				animateXPMadeUI();
			}		
		}
	}
}
}
