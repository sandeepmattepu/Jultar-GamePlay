//
//  SMAppreciatePlayer.cs
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
/// Use this function to report a kill and show a message on screen
/// </summary>
[RequireComponent(typeof(Animator)),RequireComponent(typeof(Text))]
public class SMAppreciatePlayer : MonoBehaviour 
{
	/// <summary>
	/// Use this enum to report what appreciation to be shown to the user
	/// </summary>
	public enum AppreciationType
	{
		REVENGE, DOUBLE_KILL, LONG_RANGE, UNSTOPPABLE, FIRST_BLOOD
	}

	/// <summary>
	/// UI which shows the appreciation messages
	/// </summary>
	private Text killStyleText;

	/// <summary>
	/// Animator which makes appreciation animations for UI
	/// </summary>
	private Animator animator;

	/// <summary>
	///	Condition used to check whether animation is finished, so that other animation can be shown
	/// </summary>
	private bool hasCompletedAnimating = true; 

	/// <summary>
	///	Store all the appreciation messages which acts like a queue
	/// </summary>
	private static List<AppreciationType> killStyleQueue = new List<AppreciationType> ();

	// Use this for initialization
	void Start () 
	{
		killStyleText = GetComponent<Text> ();
		animator = GetComponent<Animator> ();
	}

	// Update is called once per frame
	void Update () 
	{	
		keepShowingKillReports ();		// Every frame it will try to show the kill style messages
	}

	/// <summary>
	/// This function will convert KillStyle enums to strings which can be used in UI
	/// </summary>
	/// <param name="killStyle">Pass KillStyle enum value</param>
	/// <returns>String message which can be used in UI</returns>
	public string getAppreciationMessageString(AppreciationType killStyle)
	{
		switch(killStyle)
		{
		case AppreciationType.DOUBLE_KILL:
			return "Double Kill";
		case AppreciationType.REVENGE:
			return "Revenge";
		case AppreciationType.LONG_RANGE:
			return "Long Range Shot";
		case AppreciationType.UNSTOPPABLE:
			return "Unstoppable";
		case AppreciationType.FIRST_BLOOD:
			return "First Blood";
		}

		return "Unknown error";
	}

	// Note :- You can use this function for reporting
	/// <summary>
	/// Use this function to add Appreciation to animation queue which will be animated after appropriate time
	/// </summary>
	/// <param name="killStyle">Pass AppreciationType enum value</param>
	public static void addAppreciationToQueue(AppreciationType killStyle)
	{
		killStyleQueue.Add (killStyle);
	}

	/// <summary>
	/// This function will start animating the UI
	/// </summary>
	private void animateAppreciationMessageUI()
	{
		hasCompletedAnimating = false;			// Animation started so set it to false
		animator.SetTrigger ("ShowKillStyle");
		StartCoroutine ("currentlyAnimating");	// To solve buggy animation event system
	}

	// TODO Animation event is buggy, if this bug persists use IEnumerator to hack from this bug
	// Either animation controller or IEnumerator should call this not both. Use only one
	/// <summary>
	///	This function will be called by animation event.
	/// DONT CALL THIS CODE MANUALLY UNTIL YOU ARE SURE
	/// </summary>
	/// <param name="killStyle">Pass AppreciationType enum value</param>
	// This function is called from animation event(at the end of the animation)
	public void appreciationMessageAnimationEnded()
	{
		hasCompletedAnimating = true;		// Animation ended so set it to true
	}
		
	/// <summary>
	///	This function will handle the logic of animating UI at proper time
	/// </summary>
	// This function will handle the logic to display all the appreciation message at appropriate times
	private void keepShowingKillReports()
	{
		if(hasCompletedAnimating && killStyleQueue.Count != 0)		// Checks whether animation is completed and queue is not empty
		{
			killStyleText.text = getAppreciationMessageString (killStyleQueue [0]);
			killStyleQueue.RemoveAt (0);			// Remove the message from the queue
			animateAppreciationMessageUI ();
		}
	}

	// Simulates animation event
	IEnumerator currentlyAnimating()
	{
		yield return new WaitForSeconds (0.44f);
		appreciationMessageAnimationEnded ();
	}
}
}
