//
//  SMWeaponSpawnner.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 09/02/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SandeepMattepu.Weapon
{
	/// <summary>
	/// This class will spawn weapons in multiplayer area. Only master client will have responsibility to spawn weapons
	/// </summary>
	public class SMWeaponSpawnner : MonoBehaviour 
	{
		/// <summary>
		/// The guns to spawn in multiplayer area.
		/// </summary>
		[Tooltip("The guns to spawn in multiplayer area.")]
		public GameObject[] gunsToSpawn;
		/// <summary>
		/// This transform has several empty child game objects which represent weapon spawn location
		/// </summary>
		[Tooltip("The transform which has several child that act as spawn points")]
		public Transform spawnPoints;
		// Use this for initialization
		void Start () 
		{
			if(PhotonNetwork.isMasterClient)		// Check if he is the room creator
			{
				spawnWeaponsInArea ();
			}
		}

		/// <summary>
		/// This function handles the logic of spawnning the weapons based on the number of weapons to spawn and 
		/// number of locations to spawn
		/// </summary>
		private void spawnWeaponsInArea()
		{
			if(gunsToSpawn.Length >= spawnPoints.childCount)
			{
				List<GameObject> guns = new List<GameObject> ();
				foreach(GameObject gun in gunsToSpawn)
				{
					guns.Add (gun);							// Create a list of guns for better performance while working with random number
				}

				foreach(Transform spawnPoint in spawnPoints)
				{
					int randomIndex = Random.Range (0, guns.Count - 1);
					PhotonNetwork.InstantiateSceneObject (guns [randomIndex].name, spawnPoint.position, spawnPoint.rotation, 0, null);
					guns.RemoveAt (randomIndex);	// Deleting the point where gun is spawned and increasing the probability of other spawn points
				}
			}
			else
			{
				List<Transform> points = new List<Transform> ();
				foreach(Transform point in spawnPoints)
				{
					points.Add (point);					// Create a list of spawnn points for better performance while working with random number
				}
				foreach(GameObject gun in gunsToSpawn)
				{
					int randomIndex = Random.Range (0, points.Count - 1);
					PhotonNetwork.InstantiateSceneObject (gun.name, points[randomIndex].position, points[randomIndex].rotation, 0, null);
					points.RemoveAt (randomIndex);	// Deleting the point where gun is spawned and increasing the probability of other spawn points
				}
			}
		}
	}
}
