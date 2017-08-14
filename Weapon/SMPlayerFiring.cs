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
using SandeepMattepu.MobileTouch;
using SandeepMattepu.UI;
using CodeStage.AntiCheat.ObscuredTypes;

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
		public ObscuredBool isUsingMultiplayer = false;
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
		/// The pin removed sound.
		/// </summary>
		public AudioClip pinRemovedSound;
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
		private ObscuredInt totalNumberOfBullets;
		/// <summary>
		/// This holds reference to SMWeaponInHand component that is present in current gun player is holding
		/// </summary>
		private SMWeaponInHand weaponInHandComponent;

		public SMWeaponInHand WeaponInHandComponent {
			get {
				return weaponInHandComponent;
			}
		}

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
		private ObscuredFloat secondsPerBullet = 0.2f;
		/// <summary>
		/// Use this as reference to check whether timer has crossed fireRate, if it crossed then perform firing
		/// </summary>
		private ObscuredFloat timer = 0.0f;
		/// <summary>
		/// This shows whether reloading of the gun is finished or not
		/// </summary>
		private ObscuredBool isReloadingFinished = true;
		/// <summary>
		/// The total number of grenads the player has
		/// </summary>
		[SerializeField]
		private ObscuredInt numberOfGrenadeBombs = 2;
		/// <summary>
		/// Does throwing a grenade is finsihed
		/// </summary>
		private ObscuredBool isThrowingGrenadeFinished = true;
		/// <summary>
		/// The UI button for grenade input.
		/// </summary>
		public SMGrenadeInput grenadeInput;

		#region Network Related Variables
		/// <summary>
		/// This value indicates whether owner client has began firing.
		/// </summary>
		///
		private ObscuredBool ownerClientBeganFiring = false;
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
		/// This will hold reference to sound that gun makes while firing
		/// </summary>
		private AudioClip audioClip;
		/// <summary>
		/// This is the light created when gun is fired
		/// </summary>
		private Light lightFromGunFire;
		/// <summary>
		/// The grenade prefab.
		/// </summary>
		public GameObject grenadePrefab;
		/// <summary>
		/// The right hand of the player.
		/// </summary>
		public GameObject rightHand;
		public ObscuredFloat maxUpwardForceForGranade = 0.0f;
		public ObscuredFloat maxForwardForceForGrenade = 0.0f;
		private ObscuredFloat intensityOfThrow = 0.0f;
		// Use this for initialization
		void Start () 
		{
			ikControl = GetComponent<IKControl> ();
			audioSource = GetComponent<AudioSource>();
			photonViewComponent = GetComponent<PhotonView> ();
			reportWeaponChange ();			// We can call this at start
	
			SMJoyStick firingStick = gameObject.GetComponent<SMPlayerController> ().orientationJoyStick;

			if((isUsingMultiplayer && photonViewComponent.isMine) || (!isUsingMultiplayer))
			{
				ikControl.OnReloadFinished += onReloadingFinished;
				ikControl.OnGrenadeRelease += onGrenadeRelease;
				ikControl.OnGrenadeThrowFinished += onGrenadeThrowFinsihed;
				ikControl.OnGrenadePinRemoved += onGrenadePinRemoved;
				firingStick.DoubleTapEvent += checkAndPerformReloading;
				grenadeInput.OnPressIntensity += setIntensityOfThrow;
				grenadeInput.OnTouchDownHandler += throwGrenadeInputHandler;
			}
			else if(isUsingMultiplayer)
			{
				ikControl.OnClientGrenadePinRemoved += onGrenadePinRemoved;
				ikControl.OnClientReloadFinished += OnClientReloadAnimationFinished;
			}
		}

		void OnDestroy()
		{
			SMJoyStick firingStick = gameObject.GetComponent<SMPlayerController> ().orientationJoyStick;

			if((isUsingMultiplayer && photonViewComponent.isMine) || (!isUsingMultiplayer))
			{
				ikControl.OnReloadFinished -= onReloadingFinished;
				ikControl.OnGrenadeRelease -= onGrenadeRelease;
				ikControl.OnGrenadeThrowFinished -= onGrenadeThrowFinsihed;
				ikControl.OnGrenadePinRemoved -= onGrenadePinRemoved;
				if(firingStick != null)
				{
					firingStick.DoubleTapEvent -= checkAndPerformReloading;
				}
				if(grenadeInput != null)
				{
					grenadeInput.OnPressIntensity -= setIntensityOfThrow;
					grenadeInput.OnTouchDownHandler -= throwGrenadeInputHandler;
				}
			}
			else if(isUsingMultiplayer)
			{
				ikControl.OnClientGrenadePinRemoved -= onGrenadePinRemoved;
				ikControl.OnClientReloadFinished -= OnClientReloadAnimationFinished;
			}
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
		}

		/// <summary>
		/// This function will throws grenade.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="touchMade">Touch made.</param>
		public void throwGrenadeInputHandler()
		{
			if((isReloadingFinished /*&& isusingpowerups*/) && isThrowingGrenadeFinished)
			{
				if(numberOfGrenadeBombs > 0)
				{
					ikControl.requestGrenadeThrow ();
					isThrowingGrenadeFinished = false;
				}
			}
		}

		/// <summary>
		/// Call this function when the grenade pin removed.
		/// </summary>
		private void onGrenadePinRemoved()
		{
			if(pinRemovedSound != null)
			{
				AudioSource.PlayClipAtPoint (pinRemovedSound, transform.position);
			}
		}

		/// <summary>
		/// This function is called when the hand throw is finished in the animation
		/// </summary>
		private void onGrenadeRelease()
		{
			//TODO Instantiate stuff
			if(isUsingMultiplayer && photonViewComponent.isMine)
			{
				GameObject grenade = PhotonNetwork.Instantiate (grenadePrefab.name, 
					rightHand.transform.position, Quaternion.identity, 0) as GameObject;
				Vector3 grenadeThrowDirection = Vector3.forward;
				grenadeThrowDirection.z *= (maxForwardForceForGrenade * intensityOfThrow);
				grenadeThrowDirection.y *= (maxUpwardForceForGranade * intensityOfThrow);
				grenadeThrowDirection = transform.TransformDirection (grenadeThrowDirection);
				grenade.GetComponent<Rigidbody> ().AddForce (grenadeThrowDirection, ForceMode.Force);
				numberOfGrenadeBombs -= 1;
			}
			else if(!isUsingMultiplayer)
			{
				GameObject grenade = Instantiate (grenadePrefab, rightHand.transform.position, Quaternion.identity);
				Vector3 grenadeThrowDirection = Vector3.forward;
				grenadeThrowDirection.z *= (maxForwardForceForGrenade * intensityOfThrow);
				grenadeThrowDirection.y *= (maxUpwardForceForGranade * intensityOfThrow);
				grenadeThrowDirection = transform.TransformDirection (grenadeThrowDirection);
				grenade.GetComponent<Rigidbody> ().AddForce (grenadeThrowDirection, ForceMode.Force);
				grenade.GetComponent<SMGrenadeBehaviour> ().isMultiplayer = false;
				numberOfGrenadeBombs -= 1;
			}
		}

		private void setIntensityOfThrow(float intensity)
		{
			intensityOfThrow = intensity;
		}

		/// <summary>
		/// This function is called when the entire grenade throw animation is finsihed
		/// </summary>
		private void onGrenadeThrowFinsihed()
		{
			isThrowingGrenadeFinished = true;
			intensityOfThrow = 0.0f;
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
			if(ammoDetails.bulletsLeft > 0 && (isReloadingFinished && isThrowingGrenadeFinished))
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
				if(isReloadingFinished && isThrowingGrenadeFinished)
				{
					ikControl.requestReloading ();
					playReloadSound ();
				}
				isReloadingFinished = false;
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
				if(audioSource.isPlaying && isReloadingFinished)
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
			Vector3 direction = endPod - gunTip.transform.position;
			Ray ray = new Ray (gunTip.transform.position, direction);
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
		private void checkAndPerformReloading(object sender, JoyStickType stickType)
		{
			if(stickType == JoyStickType.AIMING)
			{
				if(ammoDetails.bulletsLeft != noOfBulletsInClip(guntype) && (totalNumberOfBullets - ammoDetails.bulletsLeft > 0))
				{
					if(isReloadingFinished && isThrowingGrenadeFinished)
					{
						ikControl.requestReloading ();
						playReloadSound ();
					}
					isReloadingFinished = false;
				}
			}
		}

		/// <summary>
		/// This function is called when reloading is finished
		/// </summary>
		private void onReloadingFinished()
		{
			isReloadingFinished = true;
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

			if(weaponInHandComponent != null)
			{
				if(audioSource.isPlaying)
				{
					audioSource.Stop ();
				}
				audioSource.clip = weaponInHandComponent.getGunSound ();
			}
		}

		private void OnClientReloadAnimationFinished()
		{
			if(audioSource.isPlaying)
			{
				audioSource.Stop ();
			}
			audioSource.clip = weaponInHandComponent.getGunSound ();
		}

		/// <summary>
		/// This function plays reloading sound when gun is reloading
		/// </summary>
		private void playReloadSound()
		{
			AudioClip clip = weaponInHandComponent.ReloadSoundMadeByGun;
			audioSource.clip = clip;
			if(audioSource.isPlaying)
			{
				audioSource.Stop ();
			}
			audioSource.Play ();
		}

		#region IPunObservable implementation

		public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
		{
			if(stream.isWriting)
			{
				bool dataToTransfer = ownerClientBeganFiring;
				stream.SendNext (dataToTransfer);
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
