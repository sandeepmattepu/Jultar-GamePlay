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
	/// <summary>
	/// The width of each strip.
	/// </summary>
	[Range(0.01f,0.5f)]
	public float widthOfEachStrip = 0.025f;
	/// <summary>
	/// The collection of stripes.
	/// </summary>
	private List<GameObject> collectionOfStripes = new List<GameObject>();
	/// <summary>
	/// The total length of laser.
	/// </summary>
	private float totalLengthOfLaser = 0.0f;
	/// <summary>
	/// The start position of laser.
	/// </summary>
	private Vector3 startPosOfLaser;
	private Vector3 endPositionOfLaser;
	/// <summary>
	/// The collision point when raycasted.
	/// </summary>
	private Vector3 collisionPoint;
	/// <summary>
	/// The ray which has same length and direction of laser.
	/// </summary>
	private Ray ray;
	/// <summary>
	/// The hit info stored in it after raycasting.
	/// </summary>
	private RaycastHit hitInfo;
	/// <summary>
	/// The line rendered of strip.
	/// </summary>
	private LineRenderer lineRenderedOfStrip;

	// Use this for initialization
	void Start () 
	{
		renderLaser ();
	}

	void Update()
	{
		setEndLaserLimits ();
	}

	private void setEndLaserLimits()
	{
		lineRenderedOfStrip = collectionOfStripes [0].GetComponent<LineRenderer> ();
		startPosOfLaser = collectionOfStripes [0].transform.TransformPoint (lineRenderedOfStrip.GetPosition (0));

		lineRenderedOfStrip = collectionOfStripes [collectionOfStripes.Count - 1].GetComponent<LineRenderer> ();
		endPositionOfLaser = collectionOfStripes [collectionOfStripes.Count - 1].transform.
			TransformPoint (lineRenderedOfStrip.GetPosition (1));
		
		Vector3 direction = endPositionOfLaser - startPosOfLaser;
		direction.Normalize ();
		ray = new Ray (startPosOfLaser, direction);
		if(Physics.Raycast(ray, out hitInfo, totalLengthOfLaser/2, (1 << 0)/*Default Layer*/, QueryTriggerInteraction.Ignore))
		{
			collisionPoint = hitInfo.point;
			float permittedDistance = hitInfo.distance;
			Debug.DrawRay (ray.origin, ray.direction, Color.white);
			int index = (int)(permittedDistance / (lengthOfEachStripe + distanceBetweenEachStripe));
			collisionPoint = collectionOfStripes [index].transform.InverseTransformPoint (collisionPoint);
			collectionOfStripes [index].GetComponent<LineRenderer> ().SetPosition (1, collisionPoint);
			for(int i = index + 1; i < collectionOfStripes.Count; i++)
			{
				collectionOfStripes [i].SetActive (false);
			}
		}
		else
		{
			foreach(GameObject stripe in collectionOfStripes)
			{
				stripe.SetActive (true);
			}
			resetAllStripes ();
		}
	}

	private void resetAllStripes()
	{
		Vector3 positionOfLastStripe = Vector3.zero;

		foreach (GameObject stripe in collectionOfStripes) 
		{
			lineRenderedOfStrip = stripe.GetComponent<LineRenderer> ();
			lineRenderedOfStrip.SetPosition (0, positionOfLastStripe);
			lineRenderedOfStrip.SetPosition (1, positionOfLastStripe + (Vector3.forward * lengthOfEachStripe));
			lineRenderedOfStrip.startColor = colorOfEachStripe;
			lineRenderedOfStrip.endColor = colorOfEachStripe;
			lineRenderedOfStrip.startWidth = widthOfEachStrip;
			lineRenderedOfStrip.endWidth = widthOfEachStrip;

			positionOfLastStripe = lineRenderedOfStrip.GetPosition (1);
			positionOfLastStripe.z += distanceBetweenEachStripe;
		}
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
				collectionOfStripes.Add (instantaitedLaserStripe);
				LineRenderer lineRendererOfInstance = instantaitedLaserStripe.GetComponent<LineRenderer> ();
				lineRendererOfInstance.SetPosition (0, Vector3.zero);
				startPosOfLaser = instantaitedLaserStripe.transform.TransformPoint (Vector3.zero);
				lineRendererOfInstance.SetPosition(1, (Vector3.forward * lengthOfEachStripe));
				lineRendererOfInstance.startColor = colorOfEachStripe;
				lineRendererOfInstance.endColor = colorOfEachStripe;
				lineRendererOfInstance.startWidth = widthOfEachStrip;
				lineRendererOfInstance.endWidth = widthOfEachStrip;

				positionOfLastStripe = lineRendererOfInstance.GetPosition (1);
				positionOfLastStripe.z += distanceBetweenEachStripe;
			}
			else
			{
				collectionOfStripes.Add (instantaitedLaserStripe);
				LineRenderer lineRendererOfInstance = instantaitedLaserStripe.GetComponent<LineRenderer> ();
				lineRendererOfInstance.SetPosition (0, positionOfLastStripe);
				lineRendererOfInstance.SetPosition(1, positionOfLastStripe + (Vector3.forward * lengthOfEachStripe));
				endPositionOfLaser = instantaitedLaserStripe.transform.TransformPoint (lineRendererOfInstance.GetPosition (1));
				lineRendererOfInstance.startColor = colorOfEachStripe;
				lineRendererOfInstance.endColor = colorOfEachStripe;
				lineRendererOfInstance.startWidth = widthOfEachStrip;
				lineRendererOfInstance.endWidth = widthOfEachStrip;

				positionOfLastStripe = lineRendererOfInstance.GetPosition (1);
				positionOfLastStripe.z += distanceBetweenEachStripe;
			}
		}
		totalLengthOfLaser = (endPositionOfLaser - startPosOfLaser).magnitude;
		laserRenderer.SetActive (false);
		positionOfLastStripe.z -= distanceBetweenEachStripe;
	}
}
