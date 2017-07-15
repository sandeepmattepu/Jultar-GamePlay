//
//  SMLaserBehaviour.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 21/06/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class will make laser render based on many factors like number of stripes, each stripe length etc.
/// </summary>
public class SMLaserBehaviour : MonoBehaviour 
{
	/// <summary>
	/// This is the laser which has line renderer attached to it
	/// </summary>
	public GameObject laserRenderer;
	/// <summary>
	/// The number of stripes of laser
	/// </summary>
	public int numberOfStripes;
	/// <summary>
	/// The length of each stripe.
	/// </summary>
	public float lengthOfEachStripe;
	/// <summary>
	/// The distance between each stripe.
	/// </summary>
	public float distanceBetweenEachStripe;
	/// <summary>
	/// The color of each stripe.
	/// </summary>
	public Color colorOfEachStripe;
	// Use this for initialization
	void Start () 
	{
		renderLaser ();
	}

	/// <summary>
	/// This function renders the laser based on above parameters
	/// </summary>
	private void renderLaser()
	{
		Vector3 positionOfLastStripe = Vector3.zero;
		for(int i = 1; i <= numberOfStripes; i++)
		{
			GameObject instantaitedLaserStripe = Instantiate (laserRenderer, transform) as GameObject;

			if(transform.childCount == 1)		// Only sample is child
			{
				LineRenderer lineRendererOfInstance = instantaitedLaserStripe.GetComponent<LineRenderer> ();
				lineRendererOfInstance.SetPosition (0, Vector3.zero);
				lineRendererOfInstance.SetPosition(1, (Vector3.forward * lengthOfEachStripe));
				lineRendererOfInstance.startColor = colorOfEachStripe;
				lineRendererOfInstance.endColor = colorOfEachStripe;

				positionOfLastStripe = lineRendererOfInstance.GetPosition (1);
				positionOfLastStripe.z += distanceBetweenEachStripe;
			}
			else
			{
				LineRenderer lineRendererOfInstance = instantaitedLaserStripe.GetComponent<LineRenderer> ();
				lineRendererOfInstance.SetPosition (0, positionOfLastStripe);
				lineRendererOfInstance.SetPosition(1, positionOfLastStripe + (Vector3.forward * lengthOfEachStripe));
				lineRendererOfInstance.startColor = colorOfEachStripe;
				lineRendererOfInstance.endColor = colorOfEachStripe;

				positionOfLastStripe = lineRendererOfInstance.GetPosition (1);
				positionOfLastStripe.z += distanceBetweenEachStripe;
			}
		}

		laserRenderer.SetActive (false);
	}
}
