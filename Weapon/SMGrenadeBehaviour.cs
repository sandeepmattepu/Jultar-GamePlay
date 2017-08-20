//
//  SMGrenadeBehaviour.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 20/07/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;

namespace SandeepMattepu.Weapon
{
	/// <summary>
	/// This class defines how the grenade will behave in multiplayer and single player context
	/// </summary>
	public class SMGrenadeBehaviour : MonoBehaviour 
	{
		/// <summary>
		/// This tells whether the game that is multiplayer or not
		/// </summary>
		public ObscuredBool isMultiplayer = true;
		/// <summary>
		/// The ID of local player.
		/// </summary>
		private ObscuredInt IDOfLocalPlayer = 0;
		/// <summary>
		/// The players present in grenade radius
		/// </summary>
		private List<SMPlayerHealth> players = new List<SMPlayerHealth>();
		/// <summary>
		/// The life span of grenade.
		/// </summary>
		[SerializeField]
		private ObscuredFloat lifeSpanOfGrenade = 2.5f;
		/// <summary>
		/// The grenade particle effect.
		/// </summary>
		[SerializeField]
		private GameObject grenadeParticleEffect;
		/// <summary>
		/// The blast radius.
		/// </summary>
		[SerializeField]
		private ObscuredFloat blastRadius = 8.0f;
		/// <summary>
		/// The blast sound.
		/// </summary>
		[SerializeField]
		private AudioClip blastSound;
		/// <summary>
		/// The grenade floor hit sound.
		/// </summary>
		[SerializeField]
		private AudioClip grenadeFloorHitSound;
		/// <summary>
		/// The photon view component.
		/// </summary>
		private PhotonView photonViewComponent;
		// Use this for initialization
		void Start () 
		{
			IDOfLocalPlayer = PhotonNetwork.player.ID;
			photonViewComponent = GetComponent<PhotonView> ();
			if(isMultiplayer && !photonViewComponent.isMine)
			{
				gameObject.GetComponent<Rigidbody> ().isKinematic = true;
			}
			StartCoroutine ("waitUntilGreandeLife");
		}

		void OnCollisionEnter(Collision collider)
		{
			AudioSource.PlayClipAtPoint (grenadeFloorHitSound, transform.position);
		}

		IEnumerator waitUntilGreandeLife()
		{
			List<GameObject> inRangePlayers = new List<GameObject> ();
			yield return new WaitForSeconds (lifeSpanOfGrenade);
			Instantiate (grenadeParticleEffect, transform.position, Quaternion.identity);
			AudioSource.PlayClipAtPoint (blastSound, transform.position, 1.0f);

			//All game objects present in blast radius
			Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);
			foreach(Collider collider in colliders)
			{
				if(collider.tag == "Player")
				{
					inRangePlayers.Add (collider.gameObject);
				}
			}

			// Check if players are exposed to grenade. Sometimes they might be behind the wall then they should not feel it
			Vector3 direction, startPosition;
			foreach(GameObject inRangePlayer in inRangePlayers)
			{
				startPosition = inRangePlayer.transform.position;
				startPosition.y += 1.0f;
				direction = transform.position - startPosition;
				RaycastHit hitinfo;
				if(!Physics.Raycast(startPosition, direction, out hitinfo, (direction).magnitude))
				{
					players.Add (inRangePlayer.GetComponent<SMPlayerHealth> ());
				}
				else if(hitinfo.collider.gameObject == this.gameObject)
				{
					players.Add (inRangePlayer.GetComponent<SMPlayerHealth> ());
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
					player.reduceHealthPointsBy (damage, photonViewComponent.owner.ID, true);
				}
			}
			else
			{
				player.reduceHealthPointsBy (damage, true);
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
