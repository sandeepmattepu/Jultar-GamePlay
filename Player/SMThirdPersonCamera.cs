//
//  SMThirdPersonCamera.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 12/02/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SandeepMattepu.MobileTouch;
using CodeStage.AntiCheat.ObscuredTypes;

/// <summary>
/// This class will act as third person camera to the player.
/// </summary>
[RequireComponent(typeof(Camera))]
public class SMThirdPersonCamera : MonoBehaviour 
{
	/// <summary>
	/// This angle will determine how much the camera should rotate around the player along the verical axis passing through player
	/// </summary>
	[Tooltip("This angle will determine how much the camera should rotate around the player along the verical axis passing through player")]
	public ObscuredFloat angleMadeWithVerticalFromPlayer = 45;
	/// <summary>
	/// This angle will determine how much the camera should rotate up and down from player along the horizontal axis passing through player 
	/// </summary>
	[Tooltip("This angle will determine how much the camera should rotate up and down from player along the horizontal axis passing through player")]
	public ObscuredFloat angleMadeWithHorizontalFromPlayer = 57;
	/// <summary>
	/// The distance in magnitude from camera to target where camera is focussed on
	/// </summary>
	[Tooltip("The distance in magnitude from camera to target where camera is focussed on")]
	public ObscuredFloat distanceFromTarget;
	/// <summary>
	/// This value will determine how faster the camera should rotate around vertical axis passing through the charaacter, while player touch and drags on the screen
	/// </summary>
	[Tooltip("This value will determine how faster the camera should rotate around vertical axis passing through the charaacter, while player touch and drags on the screen")]
	public float cameraRotationOfVerticalAxisSensitivity;
	/// <summary>
	/// This is the target the camera should focus on at all the times
	/// </summary>
	[Tooltip("This is the target the camera should focus on at all the times")]
	public Transform targetCameraShouldFocus;
	/// <summary>
	/// This will determine how fast the camera should move to the desired targeted position
	/// </summary>
	[Tooltip("This will determine how fast the camera should move to the desired targeted position")]
	public float cameraTransitionSpeed = 1;
	/// <summary>
	/// This will indicate the postion of the camera
	/// </summary>
	private Vector3 cameraPosition;
	/// <summary>
	/// This will indicate the orientaion of the camera in Quaternion
	/// </summary>
	private Quaternion cameraOrientation;
	/// <summary>
	/// This transform will always be present at nuetral camera position even if actual camera is moved. This can be used to raycast accuratly
	/// </summary>
	public Transform virtualCameraTransform;
	/// <summary>
	/// This will store the finger id of a touch
	/// </summary>
	private int touchId;
	/// <summary>
	/// This will hold reference to previous touch
	/// </summary>
	private Touch lastTouch;
	/// <summary>
	/// This specifies based on which position the camera should rotate. This point will specify both horizontal and vertical axis for camera rotations
	/// </summary>
	public Transform rotationKnob;
	/// <summary>
	/// This ray passes from target to camera
	/// </summary>
	private Ray rayFromTargetToCamera;
	/// <summary>
	/// This will store the result of the raycast made from player to camera
	/// </summary>
	private RaycastHit hitInfo;
    /// <summary>
    /// This variable hold reference to the mesh renderer to make it appear or dissapear at appropriate times
    /// </summary>
    private MeshRenderer meshRenderer = null;

	#if UNITY_EDITOR

	/// <summary>
	/// This stores the mouse position when user clicks on the screen
	/// </summary>
	private Vector3 previousMousePosition = new Vector3(0,0,0);
	/// <summary>
	/// This value indicates whether the mouse is clicked or not
	/// </summary>
	private bool mouseClicked = false;
	/// <summary>
	/// This touch manager is used to identify where touch/mouse click belongs to
	/// </summary>
	public SMTouchManager touchManager;

	#endif
	// Use this for initialization

	void Awake()
	{
		if (targetCameraShouldFocus == null) 
		{
			targetCameraShouldFocus = transform;
		}
	}

	void Start () 
	{
		// Setup camera postion and rotation based on inspector values
		transform.eulerAngles = new Vector3 (angleMadeWithVerticalFromPlayer, angleMadeWithHorizontalFromPlayer, 0);
		transform.position = (transform.forward * -distanceFromTarget);
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void LateUpdate ()
	{
		DragCamera();
		setVirtualCamera ();
		setCameraAppropriately ();

	}

	/// <summary>
	/// This function checks wether player made a touch. If he touch and dragged on the screen it will make the screen to rotate around the vertical axis passing
	/// through player
	/// </summary>
	private void DragCamera ()
	{
		foreach (Touch touch in Input.touches) 
		{
			if (SMTouchManager.processWhichTouchBelongsTo (touch) == TouchBelongs.GAME)		// Drag the camera only when the player touches in non UI region
			{
				if (touch.phase == TouchPhase.Began) 
				{
					lastTouch = touch;
					touchId = touch.fingerId;
				}
				else if (touch.phase == TouchPhase.Moved && touch.fingerId == touchId) 
				{
					if (lastTouch.position.x - touch.position.x > 0) {
						// Rotate camera to right
						angleMadeWithVerticalFromPlayer += 1 * cameraRotationOfVerticalAxisSensitivity;
					} else if (lastTouch.position.x - touch.position.x < 0) {
						angleMadeWithVerticalFromPlayer -= 1 * cameraRotationOfVerticalAxisSensitivity;
					}
					lastTouch = touch;
				} 
			}
		}

		#if UNITY_EDITOR

		if(Input.GetMouseButtonDown(0))
		{
			previousMousePosition = Input.mousePosition;
			int layer = touchManager.layerTouchMadeOn(previousMousePosition);
			mouseClicked = (layer != 5);
		}
		else if(Input.GetMouseButtonUp(0))
		{
			mouseClicked = false;
		}
		else if(mouseClicked)
		{
			if(previousMousePosition.x - Input.mousePosition.x > 0)
			{
				// Rotate camera to right
				angleMadeWithVerticalFromPlayer += 1 * cameraRotationOfVerticalAxisSensitivity;
			}
			else if(previousMousePosition.x - Input.mousePosition.x < 0)
			{
				angleMadeWithVerticalFromPlayer -= 1 * cameraRotationOfVerticalAxisSensitivity;
			}
			previousMousePosition = Input.mousePosition;
		}

		#endif
	}

	/// <summary>
	/// This function will set the camera position at desired place if there is obstacle in between camera and target camera is focused at
	/// </summary>
	private void setCameraAppropriately()
	{
		if(targetCameraShouldFocus != this.transform && targetCameraShouldFocus != null)
		{
//			Vector3 directionVector = virtualCameraTransform.position - targetCameraShouldFocus.position;
//			rayFromTargetToCamera = new Ray (targetCameraShouldFocus.position, directionVector);
//			Debug.DrawRay (targetCameraShouldFocus.position, directionVector);
//			Physics.Raycast (rayFromTargetToCamera, out hitInfo, distanceFromTarget);
//			if(hitInfo.collider != null && hitInfo.collider.tag == "Building")
//			{
//				transform.position = Vector3.Lerp(transform.position, hitInfo.point, Time.deltaTime * cameraTransitionSpeed);
//				transform.LookAt (targetCameraShouldFocus);
//				if(meshRenderer != null)
//				{
//					meshRenderer.enabled = true;
//					meshRenderer = null;
//				}
//			}
//			else if(hitInfo.collider != null && hitInfo.collider.tag == "HideMesh")
//			{
//				meshRenderer = hitInfo.collider.gameObject.GetComponent<MeshRenderer>();
//				if(meshRenderer != null)
//				{
//					meshRenderer.enabled = false;
//				}
//			}
//			else
//			{
				cameraOrientation = Quaternion.Euler(angleMadeWithHorizontalFromPlayer, -angleMadeWithVerticalFromPlayer, 0);
				cameraPosition = cameraOrientation * new Vector3(0, 0, -distanceFromTarget) + targetCameraShouldFocus.position;
				transform.rotation = Quaternion.Slerp(transform.rotation, cameraOrientation, Time.deltaTime * cameraTransitionSpeed);
				transform.position = Vector3.Lerp(transform.position, cameraPosition, Time.deltaTime * cameraTransitionSpeed);
				if (meshRenderer != null)
				{
					meshRenderer.enabled = true;
					meshRenderer = null;
				}
			//}
		}
	}

	/// <summary>
	/// This will set the virtual camera position and orientations first, so that raycasting will be done successfully
	/// </summary>
	private void setVirtualCamera()
	{
		if (targetCameraShouldFocus != this.transform && targetCameraShouldFocus != null) 
		{
			cameraOrientation = Quaternion.Euler(angleMadeWithHorizontalFromPlayer, -angleMadeWithVerticalFromPlayer, 0);
			cameraPosition = cameraOrientation * new Vector3(0, 0, -distanceFromTarget) + targetCameraShouldFocus.position;
			virtualCameraTransform.rotation = Quaternion.Slerp(transform.rotation, cameraOrientation, Time.deltaTime * cameraTransitionSpeed);
			virtualCameraTransform.position = Vector3.Lerp(transform.position, cameraPosition, Time.deltaTime * cameraTransitionSpeed);
		}
	}
}


