//
//  SMGunFiringTest.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 03/02/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SandeepMattepu.Testing
{
/// <summary>
/// This will simulate gun fire which can be used for testing
/// </summary>
public class SMGunFiringTest : MonoBehaviour 
{
	/// <summary>
	/// The player game object that you want to give health or armor from keyboard keys.
	/// </summary>
	[Tooltip("The player game object that you want to give health or armor from keyboard keys.")]
	public SMPlayerHealth player;
	/// <summary>
	/// The number of seconds per bullet to be fired.
	/// </summary>
	[Tooltip("The number of seconds per bullet to be fired.")]
	public float secondsPerBullet = 2.0f;

	private LineRenderer lineRenderer;
	private float timer = 0.0f;
	// Use this for initialization
	void Start () 
	{
		lineRenderer = GetComponent<LineRenderer> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(timer >= secondsPerBullet)
		{
			timer = 0.0f;
			producePhysicsRaycasts ();
		}
		else
		{
			timer += Time.deltaTime;
			lineRenderer.enabled = false;
		}

		if(Input.GetKeyDown(KeyCode.UpArrow))
		{
			player.giveArmorToPlayer ();
		}
		else if(Input.GetKeyDown(KeyCode.DownArrow))
		{
			player.removeArmorFromPlayer ();
		}
	}

	private void producePhysicsRaycasts()
	{
		Vector3 endPod = transform.localPosition + (Vector3.forward * 100.0f);
		endPod = transform.TransformPoint (endPod);
		// Create physics ray cast
		RaycastHit hitInfo;
		Ray ray = new Ray (transform.position, endPod);
		Physics.Raycast (ray, out hitInfo);
		if(hitInfo.collider != null)
		{
			endPod = hitInfo.point;
			if(hitInfo.collider.gameObject.tag == "Player")
			{
				// Check this when fire hits limbs or arms
				if (hitInfo.collider.gameObject.transform.root.GetComponent<SMPlayerHealth> ()) 
				{
					hitInfo.collider.gameObject.transform.root.GetComponent<SMPlayerHealth> ().reduceHealthPointsBy (20.0f);
				}
				else     // Check this if fire hits outer collider
				{
					hitInfo.collider.gameObject.GetComponent<SMPlayerHealth> ().reduceHealthPointsBy (20.0f);
				}

			}
		}

		// Show the line renderer in bullet path
		lineRenderer.SetPosition (0, transform.position);
		lineRenderer.SetPosition (1, endPod);
		lineRenderer.enabled = true;
	}
}
}
