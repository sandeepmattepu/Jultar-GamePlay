//
//  ShowKillStyle.cs
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
/// This class is deprecated use AppreciatePlayer instead
/// </summary>
[RequireComponent(typeof(Animator))]
public class ShowKillStyle : MonoBehaviour 
{
	public enum KillStyle
	{
		HEAD_SHOT, DOUBLE_KILL, LONG_RANGE
	}

	private Text killStyleText;
	private Animator animator;
	private bool hasCompletedAnimating = true;

	private List<KillStyle> killStyleQueue = new List<KillStyle> ();

	// Use this for initialization
	void Start () 
	{
		killStyleText = GetComponent<Text> ();
		animator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		keepShowingKillReports ();
	}

	public string getKillStyleString(KillStyle killStyle)
	{
		switch(killStyle)
		{
		case KillStyle.DOUBLE_KILL:
			return "Double Kill";
		case KillStyle.HEAD_SHOT:
			return "HeadShot";
		case KillStyle.LONG_RANGE:
			return "Long Range Shot";
		}

		return "Unknown error";
	}

	public void reportKillStyle(KillStyle killStyle)
	{
		killStyleQueue.Add (killStyle);
	}

	public void animateShowKillStyle()
	{
		hasCompletedAnimating = false;
		animator.SetTrigger ("ShowKillStyle");
	}

	public void killStyleAnimationFinished()
	{
		hasCompletedAnimating = true;
	}

	public void keepShowingKillReports()
	{
		if(hasCompletedAnimating && killStyleQueue.Count != 0)
		{
			killStyleText.text = getKillStyleString (killStyleQueue [0]);
			killStyleQueue.RemoveAt (0);
			animateShowKillStyle ();
		}
	}
}
