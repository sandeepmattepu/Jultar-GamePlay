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
	/// The color of each stripe.
	/// </summary>
	public Color colorOfLaser;
	/// <summary>
	/// The width of each strip.
	/// </summary>
	[Range(0.01f,0.5f)]
	public float widthOfLaser = 0.025f;
	/// <summary>
	/// The max distance laser can travel.
	/// </summary>
	public float maxDistanceOfLaser = 100.0f;
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
	private LineRenderer lineRenderer;
	/// <summary>
	/// The start position, end position and direction vector.
	/// </summary>
	private Vector3 startPosition, endPosition, direction;
	/// <summary>
	/// The layer mask to restrict collision.
	/// </summary>
	[SerializeField]
	private LayerMask layerMask;

	private void Start()
	{
		lineRenderer = GetComponent<LineRenderer> ();
		renderLaser ();
	}

	private void Update()
	{
		setEndLimitsOfLaser ();
	}

	/// <summary>
	/// Sets the end limits of laser.
	/// </summary>
	private void setEndLimitsOfLaser()
	{
		startPosition = transform.TransformPoint (lineRenderer.GetPosition (0));
		endPosition = transform.TransformPoint (lineRenderer.GetPosition (1));
		direction = endPosition - startPosition;
		direction.Normalize ();
		ray = new Ray (startPosition, direction);

		if(Physics.Raycast(ray, out hitInfo, maxDistanceOfLaser, layerMask.value, QueryTriggerInteraction.Ignore))
		{
			lineRenderer.SetPosition (1, transform.InverseTransformPoint (hitInfo.point));
		}
		else
		{
			lineRenderer.SetPosition (1, Vector3.forward * maxDistanceOfLaser);
		}
	}

	/// <summary>
	/// This function renders laser based on given attributes
	/// </summary>
	private void renderLaser()
	{
		lineRenderer.startColor = colorOfLaser;
		lineRenderer.endColor = colorOfLaser;

		lineRenderer.startWidth = widthOfLaser;
		lineRenderer.endWidth = widthOfLaser;

		lineRenderer.SetPosition (1, Vector3.forward * maxDistanceOfLaser);
	}
}
