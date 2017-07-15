//
//  SMPlayerFiring.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 09/02/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SandeepMattepu.Weapon
{
	/// <summary>
	/// This class is responsible for performing gun fire based on number of bullets and number of extra clips are present in gun
	/// </summary>
	public class SMPlayerFiring : MonoBehaviour , IPunObservable
	{
		/// <summary>
		/// This is the position where the player has left pocket which can be used to pick bullet clips from it
		/// </summary>
		[Tooltip("This is the position where the player has left pocket which can be used to pick bullet clips from it")]
		public Transform leftPocket;
		/// <summary>
		/// This UI text will show ammunation details of the gun that is present in hand
		/// </summary>
		[Tooltip("This UI text will show ammunation details of the gun that is present in hand")]
		public Text ammoDetailsUI;
		/// <summary>
		/// This value determines whether the character recieves input from joysticks or from Network
		/// </summary>
		[Tooltip("This value determines whether the character recieves input from joysticks or from Network")]
		public bool isUsingMultiplayer = false;
		[Tooltip("The blood particle effect created when it hits the player")]
		public GameObject bloodSpatter;
		[Tooltip("The spark particle effect created when it hits the wall")]
		public GameObject wallSpark;
		[Tooltip("This value dictates amount of kickback happens while firing")]
		public float amountOfGunKickBackFiring = 1.0f;
		/// <summary>
		/// This is the gun tip of the gun that is present in the hand of the player
		/// </summary>
		private GameObject gunTip;
		/// <summary>
		/// This line renderer is used to render the bullet path when it is fired
		/// </summary>
		private LineRenderer lineRenderer;
		/// <summary>
		/// This will hold reference to gun game object which is present in player's hand
		/// </summary>
		private GameObject gunPlayerIsHolding;
		/// <summary>
		/// This struct will hold all the ammunation details of gun in hand 
		/// </summary>
		private SMGunAmmoDetails ammoDetails;
		/// <summary>
		/// The total number of bullets available to the player.
		/// </summary>
		private int totalNumberOfBullets;
		/// <summary>
		/// This holds reference to SMWeaponInHand component that is present in current gun player is holding
		/// </summary>
		private SMWeaponInHand weaponInHandComponent;
		/// <summary>
		/// This describes the gun type player is holding
		/// </summary>
		private GUN_TYPE guntype;
		/// <summary>
		/// The initial position of gun before firing.
		/// </summary>
		private Vector3 initialPosOfGun;

		/// <summary>
		/// This will determine the firing speed of the gun
		/// </summary>
		private float secondsPerBullet = 0.2f;
		/// <summary>
		/// Use this as reference to check whether timer has crossed fireRate, if it crossed then perform firing
		/// </summary>
		private float timer = 0.0f;
		/// <summary>
		/// This shows whether reloading of the gun is finished or not
		/// </summary>
		private bool isReloadingFinished = true;
		/// <summary>
		/// This will show whether hand needs to reach pocket
		/// </summary>
		private bool handNeededToReachPocket = false;
		/// <summary>
		/// This will show whether hand needs to reach gun
		/// </summary>
		private bool handNeededToReachGun = false;

		#region Network Related Variables
		/// <summary>
		/// This value indicates whether owner client has began firing.
		/// </summary>
		///
		private bool ownerClientBeganFiring = false;
		#endregion

		/// <summary>
		/// This will play or stop the audio clip
		/// </summary>
		private AudioSource audioSource;
		/// <summary>
		/// This will hold reference to IKControl which can be used to change left hand while reloading
		/// </summary>
		private IKControl ikControl;
		/// <summary>
		/// The photon view component attached to the character.
		/// </summary>
		private PhotonView photonViewComponent;
		/// <summary>
		/// This will store the current position of the left hand
		/// </summary>
		private Transform leftHandPos;
		/// <summary>
		/// This will hold reference to sound that gun makes while firing
		/// </summary>
		private AudioClip audioClip;
		/// <summary>
		/// This is the light created when gun is fired
		/// </summary>
		private Light lightFromGunFire;
		// Use this for initialization
		void Start () 
		{
			ikControl = GetComponent<IKControl> ();
			leftHandPos = ikControl.tempPos;
			audioSource = GetComponent<AudioSource>();
			photonViewComponent = GetComponent<PhotonView> ();
			reportWeaponChange ();			// We can call this at start
		}
		
		// Update is called once per frame
		void Update () 
		{
			if(photonViewComponent.isMine)
			{
				setAmmoDetailsUI ();
			}
			else if(!isUsingMultiplayer)
			{
				setAmmoDetailsUI ();
			}

			if(isUsingMultiplayer && !(photonViewComponent.isMine))		// Recieve firing related events from network
			{
				if(ownerClientBeganFiring)
				{
					makeremoteClientFire ();
				}
				else
				{
					timer = 0.0f;
					stopAllComponenets ();
				}
			}

			//TODO To remove this code later after testing
			if(Input.GetKeyDown(KeyCode.Space))
			{
				checkAndPerformReloading ();
			}
		}

		/// <summary>
		/// This function will find out which weapon the player is holding and assigns gunPlayerIsHolding when the weapon is found in hand
		/// </summary>
		private void findCurrentHoldingWeapon()
		{
			foreach(Transform child in transform)
			{
				if(child.tag == "Weapon")
				{
					gunPlayerIsHolding = child.gameObject;
					foreach(Transform childInChild in child)
					{
						if(childInChild.name == "GunTip")
						{
							gunTip = childInChild.gameObject;
							lineRenderer = gunTip.GetComponent<LineRenderer> ();
							lightFromGunFire = gunTip.GetComponent<Light> ();
						}
					}
					return;
				}
			}
		}

		/// <summary>
		/// This function will perform firing, reloading based on no of bullets and no of clips present with player and also reduces ammo after successful firing.
		/// DONOT CALL THIS METHOD MANUALLY UNTIL YOU ARE SURE. ONLY WIRE THIS FUNCTION TO INPUT RECIEVING CODE
		/// </summary>
		public void performFiring()
		{
			if(ammoDetails.bulletsLeft > 0 && isReloadingFinished)
			{
				if(timer >= secondsPerBullet)
				{
					ownerClientBeganFiring = true;
					ammoDetails.bulletsLeft -= 1;
					totalNumberOfBullets -= 1;
					timer = 0.0f;
					// play firing audio
					if(audioClip != null)
					{
						if(!audioSource.isPlaying)
						{
							audioSource.Stop ();
						}
						audioSource.Play ();

					}
					// Show particle effects
					if(gunTip.GetComponent<ParticleSystem>().isPlaying)
					{
						gunTip.GetComponent<ParticleSystem> ().Stop ();
					}
					gunTip.GetComponent<ParticleSystem> ().Play ();

					// Show line renderer and cast physics raycast
					createPhysicsRayCastAndLineRenderer();
					// Show light when gun is fired
					lightFromGunFire.enabled = true;
					setGunPositionWhileFiring (amountOfGunKickBackFiring, true);	// Move the gun back
				}
				else
				{
					timer += Time.deltaTime;
					// Make bullet path line renderer look more realistic, we will make them stop
					lineRenderer.SetPosition(0,gunTip.transform.position);
					lineRenderer.SetPosition (1, gunTip.transform.position);
					lineRenderer.enabled = true;
					// Make gun fire look more real
					lightFromGunFire.enabled = false;
					setGunPositionWhileFiring (amountOfGunKickBackFiring, false);	// Move the gun forward
				}
			}
			else if(ammoDetails.extraClipsLeft > 0)
			{
				ownerClientBeganFiring = false;
				performReloading();
				stopAllComponenets ();
			}
			else
			{
				ownerClientBeganFiring = false;
				stopAllComponenets ();
			}
		}

		/// <summary>
		/// This function makes remote client player to fire in multiplayer game
		/// </summary>
		private void makeremoteClientFire()
		{
			if(timer >= secondsPerBullet)
			{
				timer = 0.0f;
				audioSource.volume = 0.2f;
				// play firing audio
				if(audioClip != null)
				{
					if(!audioSource.isPlaying)
					{
						audioSource.Stop ();
					}
					audioSource.Play ();

				}
				// Show particle effects
				if(gunTip.GetComponent<ParticleSystem>().isPlaying)
				{
					gunTip.GetComponent<ParticleSystem> ().Stop ();
				}
				gunTip.GetComponent<ParticleSystem> ().Play ();

				// Show line renderer and cast physics raycast
				createPhysicsRayCastAndLineRenderer();
				// Show light when gun is fired
				lightFromGunFire.enabled = true;
				setGunPositionWhileFiring (amountOfGunKickBackFiring, true);	// Move the gun backward
			}
			else
			{
				timer += Time.deltaTime;
				// Make bullet path line renderer look more realistic, we will make them stop
				lineRenderer.SetPosition(0,gunTip.transform.position);
				lineRenderer.SetPosition (1, gunTip.transform.position);
				lineRenderer.enabled = true;
				// Make gun fire look more real
				lightFromGunFire.enabled = false;
				setGunPositionWhileFiring (amountOfGunKickBackFiring, false);	// Move the gun forward
			}
		}

		/// <summary>
		/// This function sets the gun position based on firing. Giving the feel of kickback while firing
		/// </summary>
		/// <param name="amount">Amount of kick back when bullet is fired.</param>
		/// <param name="isFiring">If set to <c>true</c>then it will set gun position a bit backwards else it will restore the position.</param>
		private void setGunPositionWhileFiring(float amount, bool isFiring)
		{
			if(isFiring)
			{
				Vector3 localPosOfGun = gunPlayerIsHolding.transform.localPosition;
				localPosOfGun += -(Vector3.forward * amount);
				localPosOfGun = transform.TransformPoint (localPosOfGun);
				gunPlayerIsHolding.transform.position = localPosOfGun;
			}
			else
			{
				gunPlayerIsHolding.transform.localPosition = initialPosOfGun;
			}

		}

		/// <summary>
		/// This function will stop firing. Call this function to stop all firing related events
		/// </summary>
		public void stopFiring()
		{
			ownerClientBeganFiring = false;
			stopAllComponenets ();
		}

		/// <summary>
		/// This function return no of bullets each gun clip will have based on the gun type which is passed as enum
		/// </summary>
		/// <returns>The number of bullets in clip for given gun type</returns>
		/// <param name="gunType">Gun type enum value.</param>
		private int noOfBulletsInClip(GUN_TYPE gunType)
		{
			switch(gunType)
			{
			case GUN_TYPE.GreenEye:
				return 25;
			case GUN_TYPE.July11:
				return 30;
			case GUN_TYPE.LeoBlackDog:
				return 35;
			case GUN_TYPE.Mrozyk:
				return 25;
			case GUN_TYPE.Blonde:
				return 20;
			case GUN_TYPE.Raini:
				return 25;
			case GUN_TYPE.Smilere:
				return 30;
			case GUN_TYPE.Sniper:
				return 5;
			}

			Debug.LogError ("All possible gun types are not handled refer SMPlayerFiring.cs");
			return 0;
		}

		/// <summary>
		/// Call this method before performing weapon swapping from weapon in hand to weapon in floor
		/// </summary>
		public void setDataForSwapping()
		{
			ownerClientBeganFiring = false;
			ammoDetails.extraClipsLeft = (totalNumberOfBullets / noOfBulletsInClip(guntype));
			ammoDetails.bulletsLeft = (totalNumberOfBullets % noOfBulletsInClip (guntype));
			weaponInHandComponent.setAmmunationDetails (ammoDetails);;
		}

		/// <summary>
		/// Call this function after there is successful swapping of the weapon for making necessary changes
		/// </summary>
		public void reportWeaponChange()
		{
			findCurrentHoldingWeapon ();

			if(gunPlayerIsHolding != null)
			{
				weaponInHandComponent = gunPlayerIsHolding.GetComponent<SMWeaponInHand> ();
				ammoDetails = weaponInHandComponent.getAmmunationDetails ();
				guntype = weaponInHandComponent.getGunType ();
				totalNumberOfBullets = (ammoDetails.bulletsLeft + (noOfBulletsInClip(guntype) * ammoDetails.extraClipsLeft));
				// Get gun sound and assign to audio source
				audioClip = weaponInHandComponent.getGunSound ();
				audioSource.clip = audioClip;
				// Assigning fire rate
				secondsPerBullet = weaponInHandComponent.getSecondsPerBullet();
				initialPosOfGun = gunPlayerIsHolding.transform.localPosition;
			}
		}

		/// <summary>
		/// This function will stop all the firing related events
		/// </summary>
		private void stopAllComponenets()
		{
			if(gunTip != null)
			{
				// stop firing audio
				if(audioSource.isPlaying)
				{
					audioSource.Stop ();
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

				// stop light from gun tip which is shown while firing
				lightFromGunFire.enabled = false;
				setGunPositionWhileFiring (amountOfGunKickBackFiring, false);	// Move the gun forward
			}
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
					//Instantiate (bloodSpatter, endPod, Quaternion.identity);
					if(isUsingMultiplayer)
					{
						/*
						 * We will recieve damage only from remote client's fire only when they hit local player.
						 * This procedure is adopted to make fair gameplay even if there is a lag player in room
						*/
						if(!photonViewComponent.isMine)
						{
							if(hitInfo.collider.gameObject.GetComponent<PhotonView>().isMine)
							{
								hitInfo.collider.gameObject.GetComponent<SMPlayerHealth> ().reduceHealthPointsBy 
								       (ammoDetails.damageMade, photonViewComponent.owner.ID);
							}
						}
					}
					else
					{
						hitInfo.collider.gameObject.GetComponent<SMPlayerHealth> ().reduceHealthPointsBy (ammoDetails.damageMade);
					}
				}
				else if(hitInfo.collider.transform.root.tag == "explode")
				{
					Instantiate (wallSpark, endPod, Quaternion.identity);
				}
			}

			// Show the line renderer in bullet path
			lineRenderer.SetPosition (0, gunTip.transform.position);
			lineRenderer.SetPosition (1, endPod);
			lineRenderer.enabled = true;
		}

		/// <summary>
		/// This function will set the UI text which shows the ammunation details of the gun
		/// </summary>
		private void setAmmoDetailsUI()
		{
			if(ammoDetailsUI != null)
			{
				ammoDetailsUI.text = ammoDetails.bulletsLeft + "/" + (totalNumberOfBullets - ammoDetails.bulletsLeft);
			}
			else
			{
				Debug.LogWarning ("Ammo Details UI is not assigned check the inspector of SMPlayerFiring component");
			}
		}

		/// <summary>
		/// This function checks whether current gun configuration is eligible for reloading. If it is then it will call performReloading()
		/// </summary>
		private void checkAndPerformReloading()
		{
			if(ammoDetails.bulletsLeft != noOfBulletsInClip(guntype) && (totalNumberOfBullets - ammoDetails.bulletsLeft > 0))
			{
				performReloading ();
			}
		}

		/// <summary>
		/// This function is called when player needs to perform reloading
		/// </summary>
		private void performReloading()
		{
			// clip goes down
			// hand goes back and comes forward then perform firing
			if(isReloadingFinished)		// Newly reloading
			{
				isReloadingFinished = false;
				handNeededToReachPocket = true;
				handNeededToReachGun = false;
				ikControl.tempPos.transform.position = Vector3.Lerp (leftHandPos.position, leftPocket.position, 0.5f);
				ikControl.tempPos.transform.rotation = Quaternion.Slerp (leftHandPos.rotation, leftPocket.rotation, 0.5f);
				StartCoroutine ("handMovesTowardsPocket");
			}
			else if(!(isReloadingFinished) && handNeededToReachPocket && !(handNeededToReachGun))	// Reloading is under progress hand goes backwards
			{
				ikControl.tempPos.transform.position = Vector3.Lerp (leftHandPos.position, leftPocket.position, 0.5f);
				ikControl.tempPos.transform.rotation = Quaternion.Slerp (leftHandPos.rotation, leftPocket.rotation, 0.5f);
			}
			else if(!(isReloadingFinished) && !(handNeededToReachPocket) && !(handNeededToReachGun))		// Hand is at the pocket
			{
				StartCoroutine ("handMovesTowardsGun");
				handNeededToReachGun = true;
			}
			else if(!(isReloadingFinished) && handNeededToReachGun)		// left hand returns back to gun
			{
				ikControl.tempPos.position = ikControl.leftHandObj.position;
				ikControl.tempPos.rotation = ikControl.leftHandObj.rotation;
			}
		}

		/// <summary>
		/// Call this function after both swapping and ik of hands are placed successfully
		/// </summary>
		public void updateHandPos()
		{
			leftHandPos = ikControl.tempPos.transform;
		}

		private IEnumerator handMovesTowardsPocket()
		{
			yield return new WaitForSeconds (0.5f/*This value is based on play testing ie time taken by the hand to reach the pocket*/);	// This duration is when hand goes to pocket
			handNeededToReachPocket = false;
			yield return null;
		}

		private IEnumerator handMovesTowardsGun()
		{
			yield return new WaitForSeconds (ammoDetails.reloadTime);
			isReloadingFinished = true;
			handNeededToReachGun = false;
			int deficientBullets = noOfBulletsInClip (guntype) - ammoDetails.bulletsLeft;
			if((totalNumberOfBullets - ammoDetails.bulletsLeft) > deficientBullets)
			{
				ammoDetails.bulletsLeft = noOfBulletsInClip (guntype);
			}
			else
			{
				int allExtraBullets = totalNumberOfBullets - ammoDetails.bulletsLeft;
				ammoDetails.bulletsLeft += allExtraBullets;
			}
			int extraBullets = totalNumberOfBullets - ammoDetails.bulletsLeft;
			ammoDetails.extraClipsLeft = (extraBullets / noOfBulletsInClip (guntype));
			if((extraBullets % noOfBulletsInClip(guntype)) != 0)
			{
				ammoDetails.extraClipsLeft += 1;
			}
		}

		#region IPunObservable implementation

		public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
		{
			if(stream.isWriting)
			{
				stream.SendNext (ownerClientBeganFiring);
			}
			else
			{
				ownerClientBeganFiring = (bool)stream.ReceiveNext ();
			}
		}

		#endregion
	}

	public struct GunClip
	{
		/// <summary>
		/// The bullets in clip.
		/// </summary>
		private int bulletsInClip;

		public int BulletsInClip
		{
			get{ 	return bulletsInClip;	}
		}

		public GunClip(int bullets)
		{
			bulletsInClip = bullets;
		}

		public void transferBullets(int bullets)
		{
			if(bullets > 0)
			{
				bulletsInClip = bullets > BulletsInClip ? 0 : (BulletsInClip - bullets);
			}
		}
	}
}
