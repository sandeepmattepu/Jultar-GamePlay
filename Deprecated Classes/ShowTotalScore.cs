//
//  ShowTotalScore.cs
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
/// This class is deprecated
/// </summary>
[RequireComponent(typeof(Text))]
public class ShowTotalScore : MonoBehaviour 
{
	private Text scoreText;
	// Use this for initialization
	void Start () 
	{
		scoreText = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		scoreText.text = "Current player xp points: " + ScoreManagerExperimentel.getScore ().ToString ();
	}
}
