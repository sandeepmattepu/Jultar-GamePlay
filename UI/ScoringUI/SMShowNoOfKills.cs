//
//  SMShowNoOfKills.cs
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
/// This class will set Text UI to display Total no of kills made based on the value present in ScoreManager.cs
/// </summary>
[RequireComponent(typeof(Text))]
public class SMShowNoOfKills : MonoBehaviour 
{
	/// <summary>
	/// This is the UI this script will control
	/// </summary>
	private Text noOfKills;
	// Use this for initialization
	void Start () 
	{
		noOfKills = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		noOfKills.text = "No of kills: " + SMScoreManager.getNoOfKills ().ToString ();		// Update Text UI
	}
}
}
