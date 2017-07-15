//
//  SMFootSound.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 21/02/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SandeepMattepu;


/// <summary>
/// This class will produce foot sound when player is walking or running
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SMFootSound : MonoBehaviour 
{
	/// <summary>
	/// This is the audio sound made when player hits left leg on floor
	/// </summary>
	public AudioClip leftFootSound;
	/// <summary>
	/// This is the audio sound made when player hits right leg on floor
	/// </summary>
	public AudioClip rightFootSound;
	/// <summary>
	/// This is the audio source which generates the required sound
	/// </summary>
	private AudioSource audioSource;
	/// <summary>
	/// This holds reference to player controller script which can be used later used to retrive speed of the player 
	/// </summary>
	public SMPlayerController playerController;
	/// <summary>
	/// This will check whether the audio started
	/// </summary>
	private bool footAudioLoopStarted = false;
	private float speedOfThePlayer;
	// Use this for initialization
	void Start () 
	{
		audioSource = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		speedOfThePlayer = playerController.getSpeedOfThePlayer ();
		if(speedOfThePlayer >= 0.1f && !(footAudioLoopStarted))
		{
			if(!playerController.IsHiding)			// Produce sound only when player is not hiding
			{
				StartCoroutine("playAudioOfFootSound");
			}
		}
	}

	private IEnumerator playAudioOfFootSound()
	{
		footAudioLoopStarted = true;
		float delayTime = 0.0f;
		float gapTime = 0.0f;
		if(speedOfThePlayer < 0.65f)
		{
			delayTime = 0.18f;
			gapTime = 0.33f;
		}
		else if(speedOfThePlayer >= 0.65f)
		{
			delayTime = 0.11f;
			gapTime = 0.14f;
		}

		yield return new WaitForSeconds (delayTime);

		if(audioSource.isPlaying)
		{
			audioSource.Stop ();
		}
		audioSource.clip = leftFootSound;
		audioSource.Play ();

		yield return new WaitForSeconds (gapTime);

		if(audioSource.isPlaying)
		{
			audioSource.Stop ();
		}
		audioSource.clip = rightFootSound;
		audioSource.Play ();

		footAudioLoopStarted = false;
	}
}
