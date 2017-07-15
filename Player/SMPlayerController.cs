//
//  SMPlayerController.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 31/01/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SandeepMattepu.Weapon;
using SandeepMattepu.MobileTouch;

namespace SandeepMattepu
{

	/// <summary>
	/// This class will control the player based on the input from the two joysticks(SMJoyStick type)
	/// </summary>
	public class SMPlayerController : MonoBehaviour 
	{
		/// <summary>
		/// This camera will influenze the direction of the character when player is not holding the joystick
		/// </summary>
		public Camera characterFocusedCamera = null;
		/// <summary>
		/// This value will determine whether camera is the deciding factor for player's rotation
		/// </summary>
		private bool cameraDictatesOrientation = true;
		/// <summary>
		/// This responsiveness will determine how fast the player should respond to the joystick movements
		/// </summary>
		private float playerResponsiveness = 0.98f;
		/// <summary>
		/// This angle will correct the character orientation if animations are slighly inclined in other angle.
		/// DONT MODIFY THIS VALUE FROM SCRIPT THIS VALUE IS INTENDED TO CHANGE ONLY IN INSPECTOR
		/// </summary>
		public float correctionAngleForModel = 45;
		/// <summary>
		/// This value determines whether the character recieves input from joysticks or from Network
		/// </summary>
		[Tooltip("This value determines whether the character recieves input from joysticks or from Network")]
		public bool isUsingMultiplayer = false;

		/// <summary>
		/// This timer will start ticking once the player makes new animation request
		/// </summary>
		private float timerForPlayerRequestedAnim = 0.0f;
		/// <summary>
		/// This stores the spped of the player from range 0 to 1
		/// </summary>
		private float playerSpeed = 0.0f;
		/// <summary>
		/// This value will get changed when user removes or presses the joystick. When user removes finger from joystick it will be set to false else it will
		/// be set to true
		/// </summary>
		private bool isUserTouchingMovementStick = false;

		/// <summary>
		/// This stick is used for player movement
		/// </summary>
		public SMJoyStick movementJoyStick;
		/// <summary>
		/// This stick is used for player orientation
		/// </summary>
		public SMJoyStick orientationJoyStick;
		/// <summary>
		/// The sensitivity of movement joystick.
		/// </summary>
		[Range(0.0f, 20.0f)]
		public float sensitivityOfMovement = 20.0f;

		/// <summary>
		/// This is used to report firing whenever player moves right stick beyond desired distance
		/// </summary>
		private SMPlayerFiring playerFiring;

		/// <summary>
		/// This animator controlls the animation of the player
		/// </summary>
		private Animator animator;
		/// <summary>
		/// The photon view component attached to the character.
		/// </summary>
		private PhotonView photonViewComponent;
		/// <summary>
		/// This will store the vector which will determine the movement of the player
		/// </summary>
		private Vector3 movementVector = Vector3.zero;
		/// <summary>
		/// This will store the vector which will determine the orientation of the player
		/// </summary>
		private Vector3 orientationVector = Vector3.zero;


		/// <summary>
		/// This will store the current player's animation state
		/// </summary>
		private PlayerAnimType currentAnimationState = PlayerAnimType.IDLE;
		/// <summary>
		/// This will store the player's requested animation state in previous frame
		/// </summary>
		private PlayerAnimType previousFramePlayerRequestedAnimState = PlayerAnimType.IDLE;
		/// <summary>
		/// This will store the player's requested animation state in current frame
		/// </summary>
		private PlayerAnimType currentFramePlayerRequestAnimState = PlayerAnimType.IDLE;

		#region Variables that calculate touch type

		/// <summary>
		/// This value will start ticking once single touch is made on the screen.
		/// </summary>
		private float singleTouchTimer = 0.0f;
		/// <summary>
		/// This value will start ticking once single touch is finished on the screen.
		/// </summary>
		private float timeAfterSingleTouch = 0.0f;
		/// <summary>
		/// This value describes whether previous touch is single tap or not.
		/// </summary>
		private bool prevIsSingleTap = false;

		#endregion

		#region Variables that calculate crouch

		/// <summary>
		/// The hide or unhide button of UI which when pressed will make the player hide or unhide.
		/// </summary>
		[Tooltip("The hide or unhide button of UI which when pressed will make the player hide or unhide.")]
		public Button hideOrUnhideButton;
		/// <summary>
		/// This bool value describes whether player can hide or not. It becomes true when player moves to hidable region and
		/// becomes false when player leaves hidable region
		/// </summary>
		private bool playerCanHide = false;
		/// <summary>
		/// The gun position while player is walking or running.
		/// </summary>
		private Vector3 gunPosWhileWalking;
		/// <summary>
		/// The gun position while player is hiding.
		/// </summary>
		[Tooltip("Done by play testing")]
		public Vector3 gunPosWhileHiding;
		/// <summary>
		/// The gun position while player is hiding and waling.
		/// </summary>
		[Tooltip("Done by play testing")]
		public Vector3 gunPosWhileHidingAndMoving;
		/// <summary>
		/// This value describes whether the player is hiding or not
		/// </summary>
		private bool isHiding = false;
		/// <summary>
		/// This value describes whether the player is hiding or not
		/// </summary>
		public bool IsHiding {
			get {
				return isHiding;
			}
		}

		#endregion

		#region Variables that calculate lean

		/// <summary>
		/// The lean button which will make the player lean when he is near the building or wall.
		/// </summary>
		[Tooltip("The lean button which will make the player lean when he is near the building or wall.")]
		public Button leanButton;
		/// <summary>
		/// This value will determine whether player can lean or not
		/// </summary>
		private bool canLean = false;
		/// <summary>
		/// This value will describe whether player is currently leaning or not
		/// </summary>
		private bool isLeaning = false;
		/// <summary>
		/// This value will describe whether the player is leaning towards left or not.
		/// </summary>
		private bool isLeaningLeft = false;
		/// <summary>
		/// This value will change to true when idle to lean animation is finished.
		/// </summary>
		private bool isIdleToLeanAnimationFinished = false;
		/// <summary>
		/// This value will change to true when lean to idle animation is finished.
		/// </summary>
		private bool isLeanToIdleAnimationFinished = true;
		/// <summary>
		/// The minimum distance from wall to make player eligible to lean.
		/// </summary>
		public float minDistanceFromWallForLean;
		/// <summary>
		/// This will store angle the player need to rotate after stand to lean animation made at every frame when player is
		/// near the wall.
		/// </summary>
		private float tempAnglePlayerHasToTurnAfterAnimation;
		/// <summary>
		/// This will store angle the character need to rotate after stand to lean animation made when lean button
		/// or double tap is made.
		/// </summary>
		private float finalAnglePlayerHasToTurnAfterAnimation;

		#endregion

		// Use this for initialization
		void Start () 
		{
			animator = GetComponent<Animator> ();
			photonViewComponent = GetComponent<PhotonView> ();
			playerFiring = GetComponent<SMPlayerFiring> ();

			if(hideOrUnhideButton != null)
			{
				hideOrUnhideButton.gameObject.SetActive (false);		// Make the button not visible at the start of the game
			}

			if(leanButton != null)
			{
				leanButton.gameObject.SetActive (false);				// Make the button not visible at the start of the game
			}
		}
		
		// Update is called once per frame
		void Update () 
		{
			if(isUsingMultiplayer)		// Handling single player, multiplayer (both player controlled and network controlled)
			{
				if(photonViewComponent.isMine)
				{
					recieveInputsFromOrientationJoyStick ();		// Player orientation should come first before analyzing player movement
					recieveInputsFromMovementJoyStick ();
//					calculateTouchTypeMadeOnScreen ();
//					if(!isHiding)				// Player will do lean only when he is not hiding
//					{
//						scanWallsNearBy ();
//					}
				}
			}
			else
			{
				recieveInputsFromOrientationJoyStick ();		// Player orientation should come first before analyzing player movement
				recieveInputsFromMovementJoyStick();
//				calculateTouchTypeMadeOnScreen ();
//				if(!isHiding)				// Player will do lean only when he is not hiding
//				{
//					scanWallsNearBy ();
//				}
			}
		}

//		void OnTriggerEnter(Collider collider)
//		{
//			if(collider.tag == "Hidable")
//			{
//				playerCanHide = true;
//				if(!isUsingMultiplayer)				// Hiding is not applicable in multiplayer
//				{
//					hideOrUnhideButton.gameObject.SetActive (true);
//				}
//			}
//		}

//		void OnTriggerExit(Collider collider)
//		{
//			if (collider.tag == "Hidable") 
//			{
//				playerCanHide = false;
//				if(!isUsingMultiplayer)			// Hiding is not applicable in multiplayer
//				{
//					if(isHiding)
//					{
//						hideOrUnhideButton.GetComponentInChildren<Text> ().text = "Hide";
//						hideOrUnhideButton.gameObject.SetActive (false);
//						animator.SetTrigger ("Idle");
//						currentAnimationState = PlayerAnimType.IDLE;
//						isHiding = false;
//
//						GameObject gunObject = transform.GetComponentInChildren<SMWeaponInHand> ().gameObject;
//						if(gunObject != null)
//						{
//							gunObject.transform.localPosition = gunPosWhileWalking;
//						}
//					}
//
//					hideOrUnhideButton.GetComponentInChildren<Text> ().text = "Hide";
//					hideOrUnhideButton.gameObject.SetActive (false);
//				}
//			}
//		}

		/// <summary>
		/// Wire this function to the Hide button UI to make player to hide or unhide from hidable region
		/// </summary>
		public void hideOrUnhideButtonPressed()
		{
//			if(!isHiding)
//			{
//				isHiding = true;
//				animator.SetTrigger ("Crouch");
//				currentAnimationState = PlayerAnimType.CROUCH_IDLE;
//				if(hideOrUnhideButton != null)
//				{
//					hideOrUnhideButton.GetComponentInChildren<Text>().text = "UnHide";
//				}
//
//				GameObject gunObject = transform.GetComponentInChildren<SMWeaponInHand> ().gameObject;
//				if(gunObject != null)
//				{
//					gunPosWhileWalking = gunObject.transform.localPosition;
//					gunObject.transform.localPosition = gunPosWhileHiding;
//				}
//			}
//			else
//			{
//				isHiding = false;
//				animator.SetTrigger ("Idle");
//				currentAnimationState = PlayerAnimType.IDLE;
//				if(hideOrUnhideButton != null)
//				{
//					hideOrUnhideButton.GetComponentInChildren<Text>().text = "Hide";
//				}
//
//				GameObject gunObject = transform.GetComponentInChildren<SMWeaponInHand> ().gameObject;
//				if(gunObject != null)
//				{
//					gunObject.transform.localPosition = gunPosWhileWalking;
//				}
//			}
		}

		/// <summary>
		/// This function makes player hide if he is in hidable region
		/// </summary>
		private void makePlayerHideOrUnHide()
		{
			if(playerCanHide)
			{
				if(!isHiding)
				{
					isHiding = true;
					animator.SetTrigger ("Crouch");
					currentAnimationState = PlayerAnimType.CROUCH_IDLE;
					if(hideOrUnhideButton != null)
					{
						hideOrUnhideButton.GetComponentInChildren<Text>().text = "UnHide";
					}

					GameObject gunObject = transform.GetComponentInChildren<SMWeaponInHand> ().gameObject;
					if(gunObject != null)
					{
						gunPosWhileWalking = gunObject.transform.localPosition;
						gunObject.transform.localPosition = gunPosWhileHiding;
					}
				}
				else
				{
					isHiding = false;
					animator.SetTrigger ("Idle");
					currentAnimationState = PlayerAnimType.IDLE;
					if(hideOrUnhideButton != null)
					{
						hideOrUnhideButton.GetComponentInChildren<Text>().text = "Hide";
					}

					GameObject gunObject = transform.GetComponentInChildren<SMWeaponInHand> ().gameObject;
					if(gunObject != null)
					{
						gunObject.transform.localPosition = gunPosWhileWalking;
					}
				}
			}
		}

		#region Functions help to calculate touch type

		/// <summary>
		/// This function calculates the touch type made on screen.
		/// </summary>
		private void calculateTouchTypeMadeOnScreen()
		{
			if(!prevIsSingleTap)
			{
				checkForSingleTouch ();
				return;
			}

			if(prevIsSingleTap)
			{
				checkForDoubleTap ();

				if(timeAfterSingleTouch > 0.15f)
				{
					makePlayerHideOrUnHide ();		// When single tap is made on screen the make player hide
					timeAfterSingleTouch = 0.0f;
					prevIsSingleTap = false;
				}
			}
		}

		/// <summary>
		/// This function checks whether single touch is made on screen or not.
		/// </summary>
		private void checkForSingleTouch()
		{
			if(Input.touchCount > 0)
			{
				Touch singleTouch = new Touch();
				int noOfGameTouches = 0;
				foreach(Touch touch in Input.touches)
				{
					if(SMTouchManager.processWhichTouchBelongsTo(touch) == TouchBelongs.GAME)
					{
						singleTouch = touch;
						noOfGameTouches++;
					}
				}

				if(noOfGameTouches == 1)		// Check for single tap
				{
					if(singleTouch.phase == TouchPhase.Began)
					{
						singleTouchTimer += Time.deltaTime;
					}
					else if(singleTouch.phase == TouchPhase.Moved || singleTouch.phase == TouchPhase.Stationary)
					{
						singleTouchTimer += Time.deltaTime;
					}
					else if(singleTouch.phase == TouchPhase.Ended)
					{
						if(singleTouchTimer <= 0.30f)
						{
							timeAfterSingleTouch += Time.deltaTime;
							prevIsSingleTap = true;
						}
						singleTouchTimer = 0.0f;
					}
				}
				else
				{
					singleTouchTimer = 0.0f;
				}
			}
		}

		/// <summary>
		/// This function checks whether double tap is made on screen or not.
		/// </summary>
		private void checkForDoubleTap()
		{
			if(Input.touchCount > 0)
			{
				timeAfterSingleTouch = 0.0f;
				Touch secondTouch = new Touch();
				int noOfGameTouches = 0;
				foreach(Touch touch in Input.touches)
				{
					if(SMTouchManager.processWhichTouchBelongsTo(touch) == TouchBelongs.GAME)
					{
						secondTouch = touch;
						noOfGameTouches++;
					}
				}

				if(noOfGameTouches == 1)		// Check for single tap
				{
					if(secondTouch.phase == TouchPhase.Began)
					{
						singleTouchTimer += Time.deltaTime;
					}
					else if(secondTouch.phase == TouchPhase.Moved || secondTouch.phase == TouchPhase.Stationary)
					{
						singleTouchTimer += Time.deltaTime;

						if(singleTouchTimer > 0.30f)
						{
							makePlayerHideOrUnHide ();		// When single tap is made on screen the make player hide
							prevIsSingleTap = false;
						}
					}
					else if(secondTouch.phase == TouchPhase.Ended)
					{
						if(singleTouchTimer <= 0.30f)
						{
							Debug.Log ("Double Tap");
							prevIsSingleTap = false;
						}
						singleTouchTimer = 0.0f;
					}
				}
				else
				{
					singleTouchTimer = 0.0f;
				}
			}
			else
			{
				timeAfterSingleTouch += Time.deltaTime;
			}
		}

		#endregion

		#region Functions help to make character lean

		/// <summary>
		/// This function will scan the for near by walls tagged as building. If there is one then it will calculate how much angle
		/// character need to rotate if player request for stand to lean animation
		/// </summary>
		private void scanWallsNearBy()
		{
			// Below code calculates player's facing vector
			Vector3 p1 = transform.position + (Vector3.up * 1.0f);
			Vector3 p2 = p1 + (Vector3.forward * minDistanceFromWallForLean);
			Vector3 result = p2 - p1;
			result = rodriguesFormulaToRotateVectors (Vector3.up, result, transform.rotation.eulerAngles.y - correctionAngleForModel);
			Debug.DrawRay (p1, result);		// Indicates player facing vector

			// Performs raycasting with the help of facing vector for wall detection
			Ray ray = new Ray (p1, result);
			RaycastHit hitInfo;
			Physics.Raycast (ray, out hitInfo, minDistanceFromWallForLean);
			if (hitInfo.collider != null)
			{
				if (hitInfo.collider.tag == "Building")
				{
					canLean = true;
					leanButton.gameObject.SetActive(true);

					// Below code finds the normal of the traingle where the raycast is been hit
					MeshCollider meshCollider = hitInfo.collider as MeshCollider;
					if(meshCollider != null)
					{
						Mesh mesh = meshCollider.sharedMesh;
						Vector3 point1 = mesh.vertices[mesh.triangles[(hitInfo.triangleIndex * 3) + 0]];
						Vector3 point2 = mesh.vertices[mesh.triangles[(hitInfo.triangleIndex * 3) + 1]];
						Vector3 point3 = mesh.vertices[mesh.triangles[(hitInfo.triangleIndex * 3) + 2]];
						Plane plane = new Plane(point1, point2, point3);

						Debug.DrawLine(hitInfo.point, (hitInfo.point + plane.normal), Color.red);               // Indicates the normal of the face at which player intersected
						float angleDiff = angleBtwVectorsInDegrees(result, plane.normal);
						// Below code is to find the vector angle in one direction since dot product always yields shortest angle but not angle in clockwise or anticlockwise
						Vector3 crossProduct = Vector3.Cross(result, plane.normal);
						float crossProductAngle = angleBtwVectorsInDegrees(Vector3.up, crossProduct);
						// Debug.Log (angleDiff1 + " " + angleDiff);
						if (crossProductAngle == 180.0f)   // angle difference is measured anticlockwise
						{
							tempAnglePlayerHasToTurnAfterAnimation = (180 - angleDiff);
						}
						else                    // angle difference is measured clockwise
						{
							tempAnglePlayerHasToTurnAfterAnimation = -(180 - angleDiff);
						}
					}
				}
				else
				{
					canLean = false;
					leanButton.gameObject.SetActive(false);
				}
			}
			else
			{
				canLean = false;
				if (leanButton != null)
				{
					leanButton.gameObject.SetActive(false);
				}
			}

			if(isLeaning)		// TO avoid cases when player leans but doesnt make raycast on wall
			{
				if (leanButton != null)
				{
					leanButton.gameObject.SetActive(true);
				}
			}
		}

		/// <summary>
		/// This function is wired to UI button when player hits the button, character will perform lean animation if he is
		/// near to the wall
		/// </summary>
		public void leanButtonPressed()
		{
//			if(canLean && !isLeaning)
//			{
//				resetAllTriggers ();					// To avoid bugs sometimes that ocuur
//				animator.SetTrigger("StandToLean");
//				StartCoroutine ("standToLeanAnimationFinished");
//				// Below code is to capture the angle just when lean is pressed
//				finalAnglePlayerHasToTurnAfterAnimation = tempAnglePlayerHasToTurnAfterAnimation;
//
//				isLeaning = true;
//				isLeaningLeft = true;
//				isLeanToIdleAnimationFinished = false;
//				leanButton.GetComponentInChildren<Text>().text = "Stand";
//				currentAnimationState = currentFramePlayerRequestAnimState = PlayerAnimType.STAND_TO_LEAN;
//			}
//			else
//			{
//				animator.SetTrigger("Idle");
//				StartCoroutine ("leanToStandAnimationFinished");
//				isLeaning = false;
//				isLeaningLeft = false;
//				isIdleToLeanAnimationFinished = false;
//				currentAnimationState = PlayerAnimType.LEAN_TO_STAND;
//				leanButton.GetComponentInChildren<Text>().text = "Lean";
//			}
		}

		/// <summary>
		/// This function uses Rodrigues formula to rotate vectors.
		/// </summary>
		/// <returns>Vector which is rotated based on given parameters.</returns>
		/// <param name="axisOfRot">Axis of rotation vector.</param>
		/// <param name="vectToRot">Vector to be rotated.</param>
		/// <param name="angleToRotate">Angle to rotate in degrees.</param>
		private Vector3 rodriguesFormulaToRotateVectors(Vector3 axisOfRot, Vector3 vectToRot, float angleToRotate)
		{
			Vector3 unitAxis = (axisOfRot.normalized);
			Vector3 result = (vectToRot * (Mathf.Cos (angleToRotate * Mathf.Deg2Rad))) + (Vector3.Cross (unitAxis, vectToRot) * Mathf.Sin (angleToRotate * Mathf.Deg2Rad))
				+ (unitAxis * (Vector3.Dot (unitAxis, vectToRot)) * (1 - Mathf.Cos (angleToRotate * Mathf.Deg2Rad)));
			return result;
		}

		/// <summary>
		/// This function gives shortest angle between two vectors in degrees.
		/// </summary>
		/// <returns>The shortest angle between two vectors in degrees.</returns>
		/// <param name="v1">Vector1.</param>
		/// <param name="v2">Vector2.</param>
		private float angleBtwVectorsInDegrees(Vector3 v1, Vector3 v2)
		{
			float dotProduct = Vector3.Dot (v1, v2);
			float angleInRad = Mathf.Acos (dotProduct / ((v1.magnitude) * (v2.magnitude)));
			return (angleInRad * Mathf.Rad2Deg);
		}

		/// <summary>
		/// This enumerator will help in adjusting the character's orientation after stand to lean animation is performed
		/// </summary>
		IEnumerator standToLeanAnimationFinished()
		{
			yield return new WaitForSeconds (0.53f);	// 0.53 value is based on observing duration of stand to lean animation clip
			if(transform != null)		// Checking if when player is destroyed then to avoid bugs
			{
				transform.Rotate(new Vector3(0,finalAnglePlayerHasToTurnAfterAnimation,0));
				currentAnimationState = PlayerAnimType.LEAN_IDLE_LEFT;
				isIdleToLeanAnimationFinished = true;
				isLeanToIdleAnimationFinished = false;
			}
		}

		/// <summary>
		/// This enumerator will tell whether animation is complete, so that it can recieve inputs from joysticks
		/// </summary>
		IEnumerator leanToStandAnimationFinished()
		{
			yield return new WaitForSeconds (1.20f);
			if(transform != null)
			{
				isLeanToIdleAnimationFinished = true;
				currentAnimationState = PlayerAnimType.IDLE;
			}
		}

		#endregion

		/// <summary>
		/// This function will try to recieve inputs from the movement joystick which can be used for movement of player
		/// </summary>
		private void recieveInputsFromMovementJoyStick()
		{
			if(!isLeaning && isLeanToIdleAnimationFinished)
			{
				if(movementJoyStick != null)
				{
					movementVector = sensitivityOfMovement * movementJoyStick.getKnobVector ();
					setMovementAnimationParameter();
					//setCurrentRequestedAnimState (movementVector, orientationVector);
					//performAnimationForCharacter (movementVector, orientationVector);
				}
				else
				{
					movementVector = Vector3.zero;
					Debug.LogWarning("Character has no movement joystick assigned. Check the inspector");
				}
			}
			else if(isLeaning && isIdleToLeanAnimationFinished)
			{
				// Recieve input as lean input
				movementVector = movementJoyStick.getKnobVector();
				processInputWhileLeaning();
				performAnimationForCharacterWhileLeaning ();
			}
		}

		/// <summary>
		/// This function sets the movement's parameters based on joystick knobs.
		/// </summary>
		private void setMovementAnimationParameter()
		{
			float angleOfOrientStick = Mathf.Atan2(orientationVector.x, orientationVector.y) * Mathf.Rad2Deg;        // Unity uses clock-wise notation for angles
			float angleOfMovementStick = Mathf.Atan2(movementVector.x, movementVector.y) * Mathf.Rad2Deg;            // Unity uses clock-wise notation for angles
			float angleDifference = (angleOfMovementStick - angleOfOrientStick);
			angleDifference = angleDifference < 0 ? (angleDifference += 360) : angleDifference;
			/*
			 *  Angle 0 starts from top of the circle and angle increases in clockwise direction.
			 *  Animations are divided on this circle as Forward at top and ForwardRight, Right, BackwardRight, Backward, BackwardLeft, Left, ForwardLeft in clock-wise direction
			 */
			Vector3 final = new Vector3();
			// Converting angle difference to joystick reference angle
			float angleWithXY = ((-angleDifference) + 90.0f);
			angleWithXY = angleWithXY < 0 ? (angleWithXY += 360) : angleWithXY;
			final.x = (movementVector.magnitude * (Mathf.Cos(angleWithXY * Mathf.Deg2Rad)));
			final.y = (movementVector.magnitude * (Mathf.Sin(angleWithXY * Mathf.Deg2Rad)));

			animator.SetFloat("HorizontalInput", final.x);
			animator.SetFloat("HorizontalVertical", final.y);
			animator.SetFloat("Magnitude", final.magnitude);

			alignPlayerToCamera();
		}

		/// <summary>
		/// This function aligns the player animation movement according to the camera facing direction
		/// </summary>
		private void alignPlayerToCamera()
		{
			if(cameraDictatesOrientation)
			{
				Vector3 minInput = new Vector3(0.1f, 0.0f, 0.0f);
				if (movementVector.magnitude > minInput.magnitude)
				{
					Vector3 newOrientation = transform.rotation.eulerAngles;
					newOrientation.y = characterFocusedCamera.transform.rotation.eulerAngles.y + correctionAngleForModel /*This is to fix the charater's animation deviation*/;
					transform.rotation = Quaternion.Euler(newOrientation);
				}
			}
		}

		/// <summary>
		/// This function will try to recieve inputs from the orientation joystick which can be used for orientation of player
		/// </summary>
		private void recieveInputsFromOrientationJoyStick()
		{
			if(!isLeaning && isLeanToIdleAnimationFinished)			// Player will perform firing and rotation when he is not leaning
			{
				if (orientationJoyStick != null) 
				{
					if (orientationJoyStick.isJoyStickUnderTouching())
					{
						cameraDictatesOrientation = false;			// When joystick input is given then camera input is ignored
						orientationVector = orientationJoyStick.getKnobVector ();
						makePlayerRotate (orientationVector, characterFocusedCamera.transform.rotation.eulerAngles);
						checkForFiring (orientationVector);
					}
					else
					{
						playerFiring.stopFiring ();
						cameraDictatesOrientation = true;					// When joystick input is not given then camera input is considered
						orientationVector = new Vector2(0.0f, 0.25f);		// Creating arbitary vector whose angle will be 0 in our joystick circle
					}
				}
				else
				{
					orientationVector = Vector3.zero;
					Debug.LogWarning("Character has no orientation joystick assigned. Check the inspector");
				}
			}
			else
			{
				orientationVector = Vector3.zero;
			}
		}

		/// <summary>
		/// This function will processes the input from left joystick while leaning.
		/// </summary>
		private void processInputWhileLeaning()
		{
			// Dont recieve input if animations are in transition from stand to lean or lean to stand
			if(currentAnimationState != PlayerAnimType.STAND_TO_LEAN && currentAnimationState != PlayerAnimType.LEAN_TO_STAND)
			{
				float angleOfOrientStick = Mathf.Atan2(orientationVector.x , orientationVector.y) * Mathf.Rad2Deg;		// Unity uses clock-wise notation for angles
				float angleOfMovementStick = Mathf.Atan2(movementVector.x , movementVector.y) * Mathf.Rad2Deg;			// Unity uses clock-wise notation for angles
				float angleDifference = (angleOfMovementStick - angleOfOrientStick);
				if(angleDifference < 0)						// Making range of angleDifference as (0 to 360)
				{
					angleDifference += 360;
				}
				if((angleDifference <= 112.5f && angleDifference > 67.5f) && movementVector.magnitude >= 0.65f)
				{
					// Move left
					if(isLeaningLeft)
					{
						currentFramePlayerRequestAnimState = PlayerAnimType.MOVE_LEFT;
					}
					else
					{
						currentFramePlayerRequestAnimState = PlayerAnimType.TURN_LEFT;
						isLeaningLeft = true;
					}
				}
				else if(angleDifference <= 292.5f && angleDifference > 247.5f)
				{
					// move right
					if(isLeaningLeft)
					{
						currentFramePlayerRequestAnimState = PlayerAnimType.TURN_RIGHT;
						isLeaningLeft = false;
					}
					else
					{
						currentFramePlayerRequestAnimState = PlayerAnimType.MOVE_RIGHT;
					}
				}
				else
				{
					//left idle or right idle
					if(isLeaningLeft)
					{
						currentFramePlayerRequestAnimState = PlayerAnimType.LEAN_IDLE_LEFT;
					}
					else
					{
						currentFramePlayerRequestAnimState = PlayerAnimType.LEAN_IDLE_RIGHT;
					}
				}
			}
		}

		/// <summary>
		/// This function will rotate the player based on joystick and camera facing vectors
		/// </summary>
		/// <param name="joyStickInput">Pass the vector recieved from orientation joystick</param>
		/// <param name="cameraFacingOrientation">Pass the vector which describes the camera orientation</param>
		private void makePlayerRotate(Vector3 joyStickInput, Vector3 cameraFacingOrientation)
		{
			float angleInDegrees = Mathf.Atan2(joyStickInput.x , joyStickInput.y) * Mathf.Rad2Deg;			// Unity uses clock-wise notation for angle
			transform.rotation = Quaternion.Euler(0,angleInDegrees + cameraFacingOrientation.y + correctionAngleForModel /*Player orientation should be based on camera facing at*/,0);
		}

		/// <summary>
		/// This function will check the vector and initiats the firing process if it crosses 0.65f magnitude.
		/// </summary>
		/// <param name="joyStickInput">Pass the vector which will be used to determine firing</param>
		private void checkForFiring(Vector3 joyStickInput)
		{
			if(!isHiding)
			{
				if(joyStickInput.magnitude >= 0.65)
				{
					playerFiring.performFiring ();
				}
				else
				{
					playerFiring.stopFiring ();
				}
			}
		
		}

		/// <summary>
		/// This function set the player requested animation state in current frame request variable based on the joystick movements
		/// </summary>
		/// <param name="movVector">Pass the vector which is obtained from movement joystick</param>
		/// <param name="orientVector">Pass the vector which is obtained from orientation joystick</param>
		private void setCurrentRequestedAnimState(Vector3 movVector, Vector3 orientVector)
		{
			float angleOfOrientStick = Mathf.Atan2(orientVector.x , orientVector.y) * Mathf.Rad2Deg;		// Unity uses clock-wise notation for angles
			float angleOfMovementStick = Mathf.Atan2(movVector.x , movVector.y) * Mathf.Rad2Deg;			// Unity uses clock-wise notation for angles
			float angleDifference = (angleOfMovementStick - angleOfOrientStick);
			if(angleDifference < 0)						// Making range of angleDifference as (0 to 360)
			{
				angleDifference += 360;
			}
			/*
			 * 	Angle 0 starts from top of the circle and angle increases in clockwise direction.
			 * 	Animations are divided on this circle as Forward at top and ForwardRight, Right, BackwardRight, Backward, BackwardLeft, Left, ForwardLeft in clock-wise direction
			 */
			if(movementVector.magnitude <= 0.1)
			{
				currentFramePlayerRequestAnimState = PlayerAnimType.IDLE;

				if(isHiding)
				{
					currentFramePlayerRequestAnimState = PlayerAnimType.CROUCH_IDLE;
				}
			}
			else
			{
				string requestedState = calculateWhichAnimationToPlay (angleDifference);
				switch (requestedState) {
				case "Forward":
					currentFramePlayerRequestAnimState = PlayerAnimType.FORWARD;
					break;
				case "ForwardRight":
					currentFramePlayerRequestAnimState = PlayerAnimType.FORWARD_RIGHT;
					break;
				case "Right":
					currentFramePlayerRequestAnimState = PlayerAnimType.RIGHT;
					break;
				case "BackwardRight":
					currentFramePlayerRequestAnimState = PlayerAnimType.BACKWARD_RIGHT;
					break;
				case "Backward":
					currentFramePlayerRequestAnimState = PlayerAnimType.BACKWARD;
					break;
				case "BackwardLeft":
					currentFramePlayerRequestAnimState = PlayerAnimType.BACKWARD_LEFT;
					break;
				case "Left":
					currentFramePlayerRequestAnimState = PlayerAnimType.LEFT;
					break;
				case "ForwardLeft":
					currentFramePlayerRequestAnimState = PlayerAnimType.FORWARD_LEFT;
					break;
				case "CrouchForward":
					currentFramePlayerRequestAnimState = PlayerAnimType.CROUCH_FORWARD;
					break;
				case "CrouchForwardRight":
					currentFramePlayerRequestAnimState = PlayerAnimType.CROUCH_FORWARD_RIGHT;
					break;
				case "CrouchRight":
					currentFramePlayerRequestAnimState = PlayerAnimType.CROUCH_RIGHT;
					break;
				case "CrouchBackwardRight":
					currentFramePlayerRequestAnimState = PlayerAnimType.CROUCH_BACKWARD_RIGHT;
					break;
				case "CrouchBackward":
					currentFramePlayerRequestAnimState = PlayerAnimType.CROUCH_BACKWARD;
					break;
				case "CrouchBackwardLeft":
					currentFramePlayerRequestAnimState = PlayerAnimType.CROUCH_BACKWARD_LEFT;
					break;
				case "CrouchLeft":
					currentFramePlayerRequestAnimState = PlayerAnimType.CROUCH_LEFT;
					break;
				case "CrouchForwardLeft":
					currentFramePlayerRequestAnimState = PlayerAnimType.CROUCH_FORWARD_LEFT;
					break;
				case "StandToLean":
					currentFramePlayerRequestAnimState = PlayerAnimType.STAND_TO_LEAN;
					break;
				case "LeanIdleLeft":
					currentFramePlayerRequestAnimState = PlayerAnimType.LEAN_IDLE_LEFT;
					break;
				case "LeanIdleRight":
					currentFramePlayerRequestAnimState = PlayerAnimType.LEAN_IDLE_RIGHT;
					break;
				case "MoveLeft":
					currentFramePlayerRequestAnimState = PlayerAnimType.MOVE_LEFT;
					break;
				case "MoveRight":
					currentFramePlayerRequestAnimState = PlayerAnimType.MOVE_RIGHT;
					break;
				case "TurnLeft":
					currentFramePlayerRequestAnimState = PlayerAnimType.TURN_LEFT;
					break;
				case "TurnRight":
					currentFramePlayerRequestAnimState = PlayerAnimType.TURN_RIGHT;
					break;
				case "LeanToStand":
					currentFramePlayerRequestAnimState = PlayerAnimType.LEAN_TO_STAND;
					break;
				}
			}
		}

		/// <summary>
		/// This function will perform the animation based on player's movement and orientation.
		/// </summary>
		/// <param name="movVector">Pass the vector which is obtained from movement joystick</param>
		/// <param name="orientVector">Pass the vector which is obtained from orientation joystick</param>
		private void performAnimationForCharacter(Vector3 movVector, Vector3 orientVector)
		{
			resetAllTriggers ();
			animator.SetFloat ("speed", movVector.magnitude);
			playerSpeed = movVector.magnitude;
			if(currentAnimationState == currentFramePlayerRequestAnimState)
			{
				// do nothing since player is not making any new request for animation
				previousFramePlayerRequestedAnimState = currentAnimationState;
				timerForPlayerRequestedAnim = 0.0f;

				// Check if camera is input for character orientation and player is not idle, if it satisfies then orient the character in camera direction
				if(cameraDictatesOrientation && (currentAnimationState != PlayerAnimType.IDLE && currentAnimationState != PlayerAnimType.CROUCH_IDLE))
				{
					Vector3 newOrientation = transform.rotation.eulerAngles;
					newOrientation.y = characterFocusedCamera.transform.rotation.eulerAngles.y + correctionAngleForModel /*This is to fix the charater's animation deviation*/;
					transform.rotation = Quaternion.Euler (newOrientation);
				}
			}
			else if(previousFramePlayerRequestedAnimState == currentFramePlayerRequestAnimState)
			{
				// player already made a request in previous frame for new animation state. Tick the timer
				timerForPlayerRequestedAnim += Time.deltaTime;
			
				if(timerForPlayerRequestedAnim >= (1 - playerResponsiveness))				// Change the animation state only when player has requested for change for long enough time
				{
					// Check if camera is input for character orientation, if it is then orient the character in camera direction
					if(cameraDictatesOrientation)
					{
						Vector3 newOrientation = transform.rotation.eulerAngles;
						newOrientation.y = characterFocusedCamera.transform.rotation.eulerAngles.y + correctionAngleForModel /*This is to fix the charater's animation deviation*/;
						transform.rotation = Quaternion.Euler (newOrientation);
					}

					string triggerName = convertEnumToString (currentFramePlayerRequestAnimState);
					animator.SetTrigger(triggerName);
				
					currentAnimationState = currentFramePlayerRequestAnimState;
					timerForPlayerRequestedAnim = 0.0f;
				}
				previousFramePlayerRequestedAnimState = currentFramePlayerRequestAnimState;
			}
			else if(previousFramePlayerRequestedAnimState != currentFramePlayerRequestAnimState)
			{
				// player made a new request for animation state change
				timerForPlayerRequestedAnim = 0.0f;
				previousFramePlayerRequestedAnimState = currentFramePlayerRequestAnimState;

				// Check if camera is input for character orientation and player is not idle, if it satisfies then orient the character in camera direction
				if(cameraDictatesOrientation && (currentAnimationState != PlayerAnimType.IDLE && currentAnimationState != PlayerAnimType.CROUCH_IDLE))
				{
					Vector3 newOrientation = transform.rotation.eulerAngles;
					newOrientation.y = characterFocusedCamera.transform.rotation.eulerAngles.y + correctionAngleForModel /*This is to fix the charater's animation deviation*/;
					transform.rotation = Quaternion.Euler (newOrientation);
				}
			}

			if(isHiding)                   // While hiding set the gun pos appropriatly
			{
				GameObject gunObject = transform.GetComponentInChildren<SMWeaponInHand> ().gameObject;
				if(gunObject != null && playerSpeed >= 0.1f)
				{
					gunObject.transform.localPosition = gunPosWhileHidingAndMoving;
				}
				else if(playerSpeed < 0.1f && gunObject != null)
				{
					gunObject.transform.localPosition = gunPosWhileHiding;
				}
			}
		}

		/// <summary>
		/// This function will perform the animation when the player is leaning
		/// </summary>
		private void performAnimationForCharacterWhileLeaning()
		{
			resetAllTriggers ();
			// When requested for turn left or turn right process immediatly
			if(currentFramePlayerRequestAnimState == PlayerAnimType.MOVE_RIGHT || currentFramePlayerRequestAnimState == PlayerAnimType.MOVE_LEFT)
			{
				string triggerName = convertEnumToString (currentFramePlayerRequestAnimState);
				animator.SetTrigger (triggerName);
				currentAnimationState = currentFramePlayerRequestAnimState;
				previousFramePlayerRequestedAnimState = currentAnimationState;
				timeAfterSingleTouch = 0.0f;
			}
			else if(currentFramePlayerRequestAnimState == PlayerAnimType.TURN_LEFT || currentFramePlayerRequestAnimState == PlayerAnimType.TURN_RIGHT)
			{
				currentAnimationState = currentFramePlayerRequestAnimState;
				previousFramePlayerRequestedAnimState = currentAnimationState;
				timerForPlayerRequestedAnim = 0.0f;

				string triggerName = convertEnumToString (currentFramePlayerRequestAnimState);
				animator.SetTrigger(triggerName);
			}
			else if(currentAnimationState == currentFramePlayerRequestAnimState)
			{
				// do nothing since player is not making any new request for animation
				previousFramePlayerRequestedAnimState = currentAnimationState;
				timerForPlayerRequestedAnim = 0.0f;
			}
			else if(previousFramePlayerRequestedAnimState == currentFramePlayerRequestAnimState)
			{
				// player already made a request in previous frame for new animation state. Tick the timer
				timerForPlayerRequestedAnim += Time.deltaTime;

				if(timerForPlayerRequestedAnim >= (1 - playerResponsiveness))				// Change the animation state only when player has requested for change for long enough time
				{
					string triggerName = convertEnumToString (currentFramePlayerRequestAnimState);
					animator.SetTrigger(triggerName);

					currentAnimationState = currentFramePlayerRequestAnimState;
					timerForPlayerRequestedAnim = 0.0f;
				}
				previousFramePlayerRequestedAnimState = currentFramePlayerRequestAnimState;
			}
			else if(previousFramePlayerRequestedAnimState != currentFramePlayerRequestAnimState)
			{
				// player made a new request for animation state change
				timerForPlayerRequestedAnim = 0.0f;
				previousFramePlayerRequestedAnimState = currentFramePlayerRequestAnimState;
			}
		}

		/// <summary>
		/// Resets all triggers if any of them are previously present. By doing this we can avoid many bugs
		/// </summary>
		private void resetAllTriggers()
		{
			animator.ResetTrigger("Idle");
			animator.ResetTrigger("Forward");
			animator.ResetTrigger("ForwardRight");
			animator.ResetTrigger("Right");
			animator.ResetTrigger("BackwardRight");
			animator.ResetTrigger("Backward");
			animator.ResetTrigger("BackwardLeft");
			animator.ResetTrigger("Left");
			animator.ResetTrigger("ForwardLeft");
			animator.ResetTrigger("Crouch");
			animator.ResetTrigger("CrouchForward");
			animator.ResetTrigger("CrouchForwardRight");
			animator.ResetTrigger("CrouchRight");
			animator.ResetTrigger("CrouchBackwardRight");
			animator.ResetTrigger("CrouchBackward");
			animator.ResetTrigger("CrouchBackwardLeft");
			animator.ResetTrigger("CrouchLeft");
			animator.ResetTrigger("CrouchForwardLeft");
			animator.ResetTrigger("StandToLean");
			animator.ResetTrigger("LeanIdleLeft");
			animator.ResetTrigger("LeanIdleRight");
			animator.ResetTrigger("MoveLeft");
			animator.ResetTrigger("MoveRight");
			animator.ResetTrigger("TurnLeft");
			animator.ResetTrigger("TurnRight");
			animator.ResetTrigger("LeanToStand");
		}

		/// <summary>
		/// This function will convert the enum into appropriate string which can be used to send these values to triggers
		/// </summary>
		/// <param name="animationType">Pass the enum value which you want to make it to string</param>
		/// <returns>String value which is converted from enum value</returns>
		private string convertEnumToString(PlayerAnimType animationType)
		{
			switch (animationType) 
			{
			case PlayerAnimType.IDLE:
				return "Idle";
			case PlayerAnimType.FORWARD:
				return "Forward";
			case PlayerAnimType.FORWARD_RIGHT:
				return "ForwardRight";
			case PlayerAnimType.RIGHT:
				return "Right";
			case PlayerAnimType.BACKWARD_RIGHT:
				return "BackwardRight";
			case PlayerAnimType.BACKWARD:
				return "Backward";
			case PlayerAnimType.BACKWARD_LEFT:
				return "BackwardLeft";
			case PlayerAnimType.LEFT:
				return "Left";
			case PlayerAnimType.FORWARD_LEFT:
				return "ForwardLeft";
			case PlayerAnimType.CROUCH_IDLE:
				return "Crouch";
			case PlayerAnimType.CROUCH_FORWARD:
				return "CrouchForward";
			case PlayerAnimType.CROUCH_FORWARD_RIGHT:
				return "CrouchForwardRight";
			case PlayerAnimType.CROUCH_RIGHT:
				return "CrouchRight";
			case PlayerAnimType.CROUCH_BACKWARD_RIGHT:
				return "CrouchBackwardRight";
			case PlayerAnimType.CROUCH_BACKWARD:
				return "CrouchBackward";
			case PlayerAnimType.CROUCH_BACKWARD_LEFT:
				return "CrouchBackwardLeft";
			case PlayerAnimType.CROUCH_LEFT:
				return "CrouchLeft";
			case PlayerAnimType.CROUCH_FORWARD_LEFT:
				return "CrouchForwardLeft";
			case PlayerAnimType.STAND_TO_LEAN:
				return "StandToLean";
			case PlayerAnimType.LEAN_IDLE_LEFT:
				return "LeanIdleLeft";
			case PlayerAnimType.LEAN_IDLE_RIGHT:
				return "LeanIdleRight";
			case PlayerAnimType.MOVE_LEFT:
				return "MoveLeft";
			case PlayerAnimType.MOVE_RIGHT:
				return "MoveRight";
			case PlayerAnimType.TURN_LEFT:
				return "TurnLeft";
			case PlayerAnimType.TURN_RIGHT:
				return "TurnRight";
			case PlayerAnimType.LEAN_TO_STAND:
				return "LeanToStand";
			}

			return "Idle";
		}

		/// <summary>
		/// This function is to fix the bug when user suddenly changes the direction. Animation will played as jerky way. To fix this we will avoid animation to
		/// go to idle state when user is still touching the joystick. To achieve this we need to set the movement vector manually so that it will have atleast
		/// 0.125f magnitude when actual joystick magnitude is less thatn 0.1f
		/// </summary>
		private void makePlayerMoveWhenJoyStickIsStillTouched()
		{
			if(movementVector.magnitude < 0.1f && isUserTouchingMovementStick)
			{
				Vector2 manualVector = new Vector2 (0.0f, 0.15f);
				movementVector = manualVector;
			}
		}

		/// <summary>
		/// This function will determine which trigger animation to be played based on the angle difference
		/// </summary>
		/// <param name="angleDifference">Pass angle difference between movement joystick and orientation joystick</param>
		/// <returns>String value which says what trigger to be played</returns>
		private string calculateWhichAnimationToPlay(float angleDifference)
		{
			if(angleDifference <= 22.5f || angleDifference > 337.5f)
			{
				if(isHiding)
				{
					return "CrouchForward";
				}
				return "Forward";
			}
			else if(angleDifference <= 67.5f && angleDifference > 22.5f)
			{
				if(isHiding)
				{
					return "CrouchForwardRight";
				}
				return "ForwardRight";
			}
			else if(angleDifference <= 112.5f && angleDifference > 67.5f)
			{
				if(isHiding)
				{
					return "CrouchRight";
				}
				return "Right";
			}
			else if(angleDifference <= 157.5f && angleDifference > 112.5f)
			{
				if(isHiding)
				{
					return "CrouchBackwardRight";
				}
				return "BackwardRight";
			}
			else if(angleDifference <= 202.5f && angleDifference > 157.5f)
			{
				if(isHiding)
				{
					return "CrouchBackward";
				}
				return "Backward";
			}
			else if(angleDifference <= 247.5f && angleDifference > 202.5f)
			{
				if(isHiding)
				{
					return "CrouchBackwardLeft";
				}
				return "BackwardLeft";
			}
			else if(angleDifference <= 292.5f && angleDifference > 247.5f)
			{
				if(isHiding)
				{
					return "CrouchLeft";
				}
				return "Left";
			}
			else if(angleDifference <= 337.5f && angleDifference > 292.5f)
			{
				if(isHiding)
				{
					return "CrouchForwardLeft";
				}
				return "ForwardLeft";
			}

			return "Forward";
		}

		/// <summary>
		/// Gets the speed of the player. This value ranges from 0 to 1
		/// </summary>
		/// <returns>The speed of the player.</returns>
		public float getSpeedOfThePlayer()
		{
			return playerSpeed;
		}
	}

	/// <summary>
	/// This enum describes the animation state of the player
	/// </summary>
	public enum PlayerAnimType
	{
		IDLE,
		FORWARD,
		FORWARD_RIGHT,
		RIGHT,
		BACKWARD_RIGHT,
		BACKWARD,
		BACKWARD_LEFT,
		LEFT,
		FORWARD_LEFT,
		CROUCH_IDLE,
		CROUCH_FORWARD,
		CROUCH_FORWARD_RIGHT,
		CROUCH_RIGHT,
		CROUCH_BACKWARD_RIGHT,
		CROUCH_BACKWARD,
		CROUCH_BACKWARD_LEFT,
		CROUCH_LEFT,
		CROUCH_FORWARD_LEFT,
		STAND_TO_LEAN,
		LEAN_IDLE_LEFT,
		LEAN_IDLE_RIGHT,
		MOVE_LEFT,
		MOVE_RIGHT,
		TURN_LEFT,
		TURN_RIGHT,
		LEAN_TO_STAND
	}

}