//
//  SMDeathTrap.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 25/02/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SandeepMattepu
{
	/// <summary>
	/// This class will kill players who enters no-go zones
	/// </summary>
	public class SMDeathTrap : MonoBehaviour 
	{
		void OnTriggerEnter(Collider collidedObject)
		{
			if(collidedObject.tag == "Player")
			{
				GameObject player = collidedObject.gameObject;
				SMPlayerHealth playerHealth = player.GetComponent<SMPlayerHealth> ();
				float maxHitPoints = (playerHealth.MaxArmor + playerHealth.MaxHealth);
				if(playerHealth.isUsingMultiPlayer)
				{
					playerHealth.reduceHealthPointsBy (maxHitPoints, -10000);		// -ve value is sent saying that trap is not the player
				}
				else
				{
					playerHealth.reduceHealthPointsBy (maxHitPoints);
				}
			}
		}
	}	
}
