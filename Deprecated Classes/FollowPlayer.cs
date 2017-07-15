//
//  FollowPlayer.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 31/01/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SandeepMattepu;

/// <summary>
/// This class will make the camera follow the player/character who has SMPlayerController attached to it
/// </summary>
[RequireComponent(typeof(Camera))]
public class FollowPlayer : MonoBehaviour 
{
	[Tooltip("Drag and drop the game object which has SMPlayerController script attached")]
	public SMPlayerController player;

	/// <summary>
	/// This vector will store the difference from the player and camera
	/// </summary>
	private Vector3 offSetFromPlayer;
	// Use this for initialization
	void Start () 
	{
		if(player != null)
		{
			offSetFromPlayer = (transform.position - player.transform.position);
		}
		else
		{
			offSetFromPlayer = Vector3.zero;
			Debug.LogWarning ("Camera can't follow player check the inspector of camera");
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(offSetFromPlayer != Vector3.zero)
		{
			Vector3 camPos = player.transform.position + offSetFromPlayer;
			transform.position = camPos;
		}
	}
}
