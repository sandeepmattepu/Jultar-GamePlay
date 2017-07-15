//
//  SMTestScoreAndKill.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 30/01/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SandeepMattepu.Scoring;
using SandeepMattepu.UI;

namespace SandeepMattepu.Testing
{
/// <summary>
/// This class is used for testing player appreciation messages on screen by pressing some keys in keyboard
/// </summary>
public class SMTestScoreAndKill : MonoBehaviour 
{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{

		if(Input.GetKeyDown(KeyCode.Space))
		{
			SMScoreManager.reportKill ();
		}
		else if(Input.GetKeyDown(KeyCode.Q))
		{
			SMAppreciatePlayer.addAppreciationToQueue (SMAppreciatePlayer.AppreciationType.DOUBLE_KILL);
		}
		else if(Input.GetKeyDown(KeyCode.W))
		{
			SMAppreciatePlayer.addAppreciationToQueue (SMAppreciatePlayer.AppreciationType.FIRST_BLOOD);	
		}
		else if(Input.GetKeyDown(KeyCode.E))
		{
			SMAppreciatePlayer.addAppreciationToQueue (SMAppreciatePlayer.AppreciationType.LONG_RANGE);
		}
		else if(Input.GetKeyDown(KeyCode.R))
		{
			SMAppreciatePlayer.addAppreciationToQueue(SMAppreciatePlayer.AppreciationType.REVENGE);
		}
		else if(Input.GetKeyDown(KeyCode.T))
		{
			SMAppreciatePlayer.addAppreciationToQueue (SMAppreciatePlayer.AppreciationType.UNSTOPPABLE);
		}
	}
}
}
