//
//  SMRocketBehaviour.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 24/07/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SandeepMattepu.Weapon
{
	public class SMRocketBehaviour : MonoBehaviour 
	{
		/// <summary>
		/// This tells whether the game that is multiplayer or not
		/// </summary>
		public bool isMultiplayer = true;
		/// <summary>
		/// The ID of local player.
		/// </summary>
		private int IDOfLocalPlayer = 0;
		/// <summary>
		/// The players present in grenade radius
		/// </summary>
		private List<SMPlayerHealth> players = new List<SMPlayerHealth>();
		/// <summary>
		/// The life span of rocket.
		/// </summary>
		[SerializeField]
		private float speedOfRocket = 2.5f;
		/// <summary>
		/// The rocket particle effect.
		/// </summary>
		[SerializeField]
		private GameObject rocketBlastParticleEffect;
		/// <summary>
		/// The blast radius.
		/// </summary>
		[SerializeField]
		private float blastRadius = 8.0f;
		/// <summary>
		/// The blast sound.
		/// </summary>
		[SerializeField]
		private AudioClip blastSound;
		/// <summary>
		/// The photon view component.
		/// </summary>
		private PhotonView photonViewComponent;
		/// <summary>
		/// The target position.
		/// </summary>
		public Vector3 targetPosition;
		/// <summary>
		/// The rocket can blast.
		/// </summary>
		private bool canBlast = false;
		private bool canLanch = false;
		[SerializeField]
		/// <summary>
		/// The delay before lauch.
		/// </summary>
		private float delayBeforeLauch = 2.5f;
		/// <summary>
		/// The tick delay.
		/// </summary>
		private float tickDelay = 0.0f;
		private AudioSource audioSourceComponent;
		// Use this for initialization
		void Start () 
		{
			IDOfLocalPlayer = PhotonNetwork.player.ID;
			photonViewComponent = GetComponent<PhotonView> ();
			GetComponent<Rigidbody> ().isKinematic = true;
			audioSourceComponent = GetComponent<AudioSource> ();
			StartCoroutine ("playRocketTravellingSound");
		}

		IEnumerator playRocketTravellingSound()
		{
			yield return new WaitForSeconds (delayBeforeLauch / 2.0f);

			audioSourceComponent.Play ();
		}

		void Update()
		{
			rocketMovement ();
		}

		/// <summary>
		/// This function determines the rocket movement
		/// </summary>
		private void rocketMovement()
		{
			if((isMultiplayer && photonViewComponent.isMine) || !isMultiplayer)
			{
				if(!canLanch)
				{
					tickDelay += Time.deltaTime;
					if(tickDelay >= delayBeforeLauch)
					{
						canLanch = true;
					}
				}
				if(!canBlast && canLanch)
				{
					Vector3 direction = transform.InverseTransformPoint (targetPosition);
					if(direction.magnitude <= 0.02f)
					{
						canBlast = true;
						audioSourceComponent.Play ();
						blastTheRocket ();
						if(isMultiplayer && photonViewComponent.isMine)
						{
							photonViewComponent.RPC ("blastTheRocket", PhotonTargets.Others, null);
						}
					}
					direction.Normalize();
					transform.Translate (direction * speedOfRocket * Time.deltaTime);
				}
			}
		}

		/// <summary>
		/// This function will blast the rocket
		/// </summary>
		[PunRPC]
		private void blastTheRocket()
		{
			Instantiate (rocketBlastParticleEffect, targetPosition, Quaternion.identity);
			AudioSource.PlayClipAtPoint (blastSound, targetPosition, 1.0f);

			//All game objects present in blast radius
			Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);
			foreach(Collider collider in colliders)
			{
				if(collider.tag == "Player")
				{
					players.Add (collider.gameObject.GetComponent<SMPlayerHealth>());
				}
			}

			// Now reduce the health based on the distance from grenade
			foreach(SMPlayerHealth player in players)
			{
				float distanceGrenadeAndPlayer = ((transform.position) - player.gameObject.transform.position).magnitude;
				if(distanceGrenadeAndPlayer <= (blastRadius/2.0f))
				{
					createDamageToPlayer (player, player.MaxHealth);
				}
				else if(distanceGrenadeAndPlayer >= blastRadius)
				{
					createDamageToPlayer (player, (player.MaxHealth * 0.15f));
				}
				else
				{
					float distancePercentage = findPercentage (blastRadius/2.0f, blastRadius, distanceGrenadeAndPlayer);
					float damageRate = 1 - distancePercentage;
					float damageTobeMade = findActualValueFromPercentage (0.15f, player.MaxHealth, damageRate);
					createDamageToPlayer (player, damageTobeMade);
				}
			}

			Destroy (this.gameObject);
		}

		void OnCollisionEnter(Collision collider)
		{
			canBlast = true;
			audioSourceComponent.Play ();
			blastTheRocket ();
			if(isMultiplayer && photonViewComponent.isMine)
			{
				photonViewComponent.RPC ("blastTheRocket", PhotonTargets.Others, null);
			}
		}

		/// <summary>
		/// Creates the damage to player.
		/// </summary>
		/// <param name="player">Player.</param>
		/// <param name="damage">Damage on player.</param>
		private void createDamageToPlayer(SMPlayerHealth player, float damage)
		{
			if(isMultiplayer)
			{
				int idOfThisPlayer = player.GetComponent<PhotonView> ().owner.ID;
				if(idOfThisPlayer == IDOfLocalPlayer)
				{
					player.reduceHealthPointsBy (damage, photonViewComponent.owner.ID);
				}
			}
			else
			{
				player.reduceHealthPointsBy (damage);
			}
		}

		/// <summary>
		/// Interpolates the values.
		/// </summary>
		/// <returns>percentage of the value.</returns>
		/// <param name="minValue">Minimum value.</param>
		/// <param name="maxValue">Max value.</param>
		/// <param name="actualValue">Actual value.</param>
		private float findPercentage(float minValue, float maxValue, float actualValue)
		{
			return (actualValue - minValue) / (maxValue - minValue);
		}

		/// <summary>
		/// Finds the actual value from percentage.
		/// </summary>
		/// <returns>The actual value from percentage.</returns>
		/// <param name="minValue">Minimum value.</param>
		/// <param name="maxValue">Max value.</param>
		/// <param name="percentage">Percentage.</param>
		private float findActualValueFromPercentage(float minValue, float maxValue, float percentage)
		{
			return ((percentage * (maxValue - minValue)) + minValue);
		}

		/// <summary>
		/// Extrapolates the value.
		/// </summary>
		/// <returns>The extrapolated minimum value.</returns>
		/// <param name="maxValue">Max value.</param>
		/// <param name="actualValue">Actual value.</param>
		/// <param name="percentage">Percentage.</param>
		private float extrapolateMinValue(float maxValue, float actualValue, float percentage)
		{
			return (((percentage * maxValue) - actualValue)/(percentage - 1));
		}
	}	
}
