//
//  SMAreaObserver.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 06/08/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SandeepMattepu
{
	/// <summary>
	/// This class will observes area and counts how many enemies are present in the area
	/// </summary>
	public class SMAreaObserver : MonoBehaviour 
	{
		/// <summary>
		/// The local player ID.
		/// </summary>
		private int localPlayerID = PhotonNetwork.player.ID;
		/// <summary>
		/// The number of enimeies in region.
		/// </summary>
		private int numberOfEnimeiesInRegion = 0;

		public int NumberOfEnimeiesInRegion {
			get {
				return numberOfEnimeiesInRegion;
			}
		}

		void OnTriggerEnter(Collider playerCollider)
		{
			if(playerCollider.gameObject.tag == "Player")
			{
				if(SMPlayerSpawnerAndAssigner.gameType == SandeepMattepu.Multiplayer.MPGameTypes.FREE_FOR_ALL)
				{
					Debug.Log ("Someone entered my region");
					if(playerCollider.gameObject.GetComponent<PhotonView>().owner.ID != localPlayerID)
					{
						numberOfEnimeiesInRegion += 1;
					}
				}
			}
		}

		void OnTriggerExit(Collider playerCollider)
		{
			if(playerCollider.gameObject.tag == "Player")
			{
				if(SMPlayerSpawnerAndAssigner.gameType == SandeepMattepu.Multiplayer.MPGameTypes.FREE_FOR_ALL)
				{
					Debug.Log ("Someone left my region");
					if(playerCollider.gameObject.GetComponent<PhotonView>().owner.ID != localPlayerID)
					{
						numberOfEnimeiesInRegion -= 1;
					}
				}
			}
		}
	}	
}
