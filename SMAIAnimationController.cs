//
//  SMAIAnimationController.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 21/02/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SandeepMattepu.AI
{
	/// <summary>
	/// This class will make the AI to animate and perform transitions from one state to other
	/// </summary>
	[RequireComponent(typeof(Animator))]
	public class SMAIAnimationController : MonoBehaviour 
	{
		#region Gun in hand prefabs

		public GameObject jully11InHand;
		public GameObject greenEyeInHand;
		public GameObject leoBlackDogInHand;
		public GameObject mrozykInHand;
		public GameObject blondeInHand;

		#endregion
		/// <summary>
		/// This animator is responsible for performing animation transitions
		/// </summary>
		private Animator animator;
		/// <summary>
		/// The audio source component attached to the game object.
		/// </summary>
		private AudioSource audioSourceComponent;
		/// <summary>
		/// The ik control component attached to the game object.
		/// </summary>
		private IKControl ikControlComponent;
		/// <summary>
		/// The state of the currrent player animation.
		/// </summary>
		private PlayerAnimType _currrentPlayerAnimationState = PlayerAnimType.IDLE;
		/// <summary>
		/// The gun player is holding at present.
		/// </summary>
		private GameObject gunPlayerIsHolding = null;
		/// <summary>
		/// The tip of the gun the player is holding.
		/// </summary>
		private GameObject gunTip = null;
		/// <summary>
		/// The line renderer component attached to the tip of the gun.
		/// </summary>
		private LineRenderer lineRenderer;
		/// <summary>
		/// The ammo details of gun in hand.
		/// </summary>
		private SMGunAmmoDetails ammoDetailsOfGun;
		/// <summary>
		/// The state of the current player animation.
		/// </summary>
		public PlayerAnimType currentPlayerAnimationState
		{
			get{ 	return _currrentPlayerAnimationState;	}	
		}
		/// <summary>
		/// The boolean tells whether the player is firing or not
		/// </summary>
		private bool isAIFiring = false;
		/// <summary>
		/// The timer which records time after each bullet.
		/// </summary>
		private float timer = 0.0f;
		/// <summary>
		/// The seconds gap required after firing each bullet.
		/// </summary>
		private float secondsPerBullet = 0.0f;
		// Use this for initialization
		void Start () 
		{
			animator = GetComponent<Animator> ();
			ikControlComponent = GetComponent<IKControl> ();
			audioSourceComponent = GetComponent<AudioSource> ();
		}

		void Update()
		{
			if(isAIFiring)
			{
				performFiring ();
			}
		}

		#region Public Methods

		/// <summary>
		/// Call this method if you want to change AI character from one animation state to another state
		/// </summary>
		/// <param name="playerAnimationType">Animation type you wish the AI to animate.</param>
		/// <param name="speedOfPlayer">Speed range is 0 to 1 which will decide whether to walk or run</param>
		public void transitionToAnimation(PlayerAnimType aiAnimationType, float speedOfPlayer)
		{
			speedOfPlayer = speedOfPlayer < 0 ? 0 : speedOfPlayer;
			speedOfPlayer = speedOfPlayer > 1 ? 1 : speedOfPlayer;
			animator.SetFloat ("speed", speedOfPlayer);

			string animationName = convertAnimEnumToString (aiAnimationType);
			animator.SetTrigger (animationName);
		}

		/// <summary>
		/// Call this function to assign a particula gun to AI
		/// </summary>
		/// <param name="gunType">Gun type the AI should hold.</param>
		public void assignGunToAI(GUN_TYPE gunType)
		{
			if(gunPlayerIsHolding)			// If player is already holding a gun in hand
			{
				gunPlayerIsHolding.transform.SetParent (null);
				Destroy (gunPlayerIsHolding);
				gunPlayerIsHolding = null;
			}

			GameObject gunToBeInstantiated = null;

			switch(gunType)
			{
			case GUN_TYPE.Blonde:
				gunToBeInstantiated = blondeInHand;
				break;
			case GUN_TYPE.GreenEye:
				gunToBeInstantiated = greenEyeInHand;
				break;
			case GUN_TYPE.July11:
				gunToBeInstantiated = jully11InHand;
				break;
			case GUN_TYPE.LeoBlackDog:
				gunToBeInstantiated = leoBlackDogInHand;
				break;
			case GUN_TYPE.Mrozyk:
				gunToBeInstantiated = mrozykInHand;
				break;
			}

			if(gunToBeInstantiated != null)
			{
				GameObject gunAfterInstantiation = Instantiate (gunToBeInstantiated, transform) as GameObject;
				gunAfterInstantiation.transform.localPosition = gunToBeInstantiated.transform.position;
				gunAfterInstantiation.transform.localRotation = gunToBeInstantiated.transform.rotation;
				gunPlayerIsHolding = gunAfterInstantiation;
				placeIKAppropriately ();
			}
			else
			{
				ikControlComponent.ikActive = false;
				Debug.LogWarning ("Gun prefabs are empty check the inspector");
			}
		}

		/// <summary>
		/// Performs the firing of AI.
		/// </summary>
		public void requestForFiring()
		{
			if(gunPlayerIsHolding != null)
			{
				ammoDetailsOfGun = gunPlayerIsHolding.GetComponent<SMWeaponInHand> ().getAmmunationDetails();
				audioSourceComponent.clip = gunPlayerIsHolding.GetComponent<SMWeaponInHand> ().getGunSound ();
				secondsPerBullet = gunPlayerIsHolding.GetComponent<SMWeaponInHand> ().getSecondsPerBullet ();
				isAIFiring = true;
				foreach(Transform child in gunPlayerIsHolding.transform)
				{
					if(child.name == "GunTip")
					{
						gunTip = child.gameObject;
						lineRenderer = gunTip.GetComponent<LineRenderer> ();
					}
				}
			}
		}

		/// <summary>
		/// Use this function to send AI message to stop firing
		/// </summary>
		public void requestToStopFiring()
		{
			isAIFiring = false;

			if(gunPlayerIsHolding != null)
			{
				stopAllComponenets ();
			}
		}

		#endregion

		/// <summary>
		/// This function actually performs the firing from AI.
		/// </summary>
		private void performFiring()
		{
			if(timer >= secondsPerBullet)
			{
				timer = 0.0f;
				// play firing audio
				if(!audioSourceComponent.isPlaying)
				{
					audioSourceComponent.Stop ();
				}
				audioSourceComponent.Play ();

				// Show particle effects
				if(gunTip.GetComponent<ParticleSystem>().isPlaying)
				{
					gunTip.GetComponent<ParticleSystem> ().Stop ();
				}
				gunTip.GetComponent<ParticleSystem> ().Play ();

				// Show line renderer and cast physics raycast
				createPhysicsRayCastAndLineRenderer();
			}
			else
			{
				timer += Time.deltaTime;
				// Make bullet path line renderer look more realistic, we will make them stop
				lineRenderer.SetPosition(0,gunTip.transform.position);
				lineRenderer.SetPosition (1, gunTip.transform.position);
				lineRenderer.enabled = true;
			}
		}

		/// <summary>
		/// This function will stop all the firing related events
		/// </summary>
		private void stopAllComponenets()
		{
			// stop firing audio
			if(audioSourceComponent.isPlaying)
			{
				audioSourceComponent.Stop ();
			}
			//stop particle effects
			if(gunTip.GetComponent<ParticleSystem>().isPlaying)
			{
				gunTip.GetComponent<ParticleSystem> ().Stop ();
			}

			// Stop rendering the bullet path
			lineRenderer.SetPosition(0,gunTip.transform.position);
			lineRenderer.SetPosition (1, gunTip.transform.position);
			lineRenderer.enabled = true;
		}

		/// <summary>
		/// This function will create a physics raycast along the line of the bullet path and also renders the bullet path when player fires
		/// </summary>
		private void createPhysicsRayCastAndLineRenderer()
		{
			Vector3 endPod = new Vector3 (gunTip.transform.localPosition.x + 1000.0f, gunTip.transform.localPosition.y, gunTip.transform.localPosition.z);
			endPod = gunTip.transform.TransformPoint (endPod);
			// Create physics ray cast
			RaycastHit hitInfo;
			Ray ray = new Ray (gunTip.transform.position, endPod);
			Physics.Raycast (ray, out hitInfo);
			if(hitInfo.collider != null)
			{
				endPod = hitInfo.point;
				// TODO process the ray cast result
				if(hitInfo.collider.gameObject.tag == "Player")
				{
					hitInfo.collider.gameObject.GetComponent<SMPlayerHealth> ().reduceHealthPointsBy (ammoDetailsOfGun.damageMade);
				}
			}

			// Show the line renderer in bullet path
			lineRenderer.SetPosition (0, gunTip.transform.position);
			lineRenderer.SetPosition (1, endPod);
			lineRenderer.enabled = true;
		}

		/// <summary>
		/// Call this function to set the position of the left and right hand of the AI at the weapon
		/// </summary>
		private void placeIKAppropriately()
		{
			ikControlComponent.ikActive = true;
			Transform leftHandHolder = null;
			Transform rightHandHolder = null;
			Transform tempPos = null;
			foreach(Transform handle in gunPlayerIsHolding.transform)
			{
				if(handle.gameObject.name == "LeftHandle")
				{
					leftHandHolder = handle;
				}
				else if(handle.gameObject.name == "RightHandle")
				{
					rightHandHolder = handle;
				}
				else if(handle.gameObject.name == "TempPos")
				{
					tempPos = handle;
				}
			}

			ikControlComponent.leftHandObj = leftHandHolder;
			ikControlComponent.rightHandObj = rightHandHolder;
			ikControlComponent.tempPos = tempPos;
		}

		/// <summary>
		/// Converts the animation enum to string value.
		/// </summary>
		/// <returns>The animation enum to string value.</returns>
		/// <param name="animationType">Animation type to be converted.</param>
		private string convertAnimEnumToString(PlayerAnimType aiAnimationType)
		{
			switch(aiAnimationType)
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
			}

			return "IDLE";
		}
	}
}
