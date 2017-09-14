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
using SandeepMattepu.Multiplayer;

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
		private int localPlayerID;
		/// <summary>
		/// The number of enimeies in region.
		/// </summary>
		private int numberOfEnimeiesInRegion = 0;

		public int NumberOfEnimeiesInRegion {
			get {
				return numberOfEnimeiesInRegion;
			}
		}

		void Start()
		{
			localPlayerID = PhotonNetwork.player.ID;
			SMPlayerSpawnerAndAssigner.OnPlayerRespawned += checkEnemiesInTheArea;
		}

		void OnDestroy()
		{
			SMPlayerSpawnerAndAssigner.OnPlayerRespawned -= checkEnemiesInTheArea;
		}

		/// <summary>
		/// This function checks the enemies in the area.
		/// </summary>
		private void checkEnemiesInTheArea()
		{
			int layerMask = 1 << 8;		// 8 is for player layer
			Vector3 dimensions = transform.localScale / 2;
			Collider[] playersInRange = Physics.OverlapBox(transform.position, dimensions, transform.rotation, layerMask);
			if(SMPlayerSpawnerAndAssigner.gameType == SandeepMattepu.Multiplayer.MPGameTypes.FREE_FOR_ALL)
			{
				numberOfEnimeiesInRegion = 0;
				foreach(Collider collider in playersInRange)
				{
					int playerID = collider.gameObject.GetComponent<PhotonView> ().owner.ID;
					if(playerID != localPlayerID)
					{
						numberOfEnimeiesInRegion += 1;
					}
				}
			}
			else if(SMPlayerSpawnerAndAssigner.gameType == SandeepMattepu.Multiplayer.MPGameTypes.TEAM_DEATH_MATCH)
			{
				numberOfEnimeiesInRegion = 0;
				int localTeamID = SMTeamDeathMatch.LocalPlayerTeamIndex;
				foreach(Collider collider in playersInRange)
				{
					int playerID = collider.gameObject.GetComponent<PhotonView> ().owner.ID;
					int playersTeamIndexWhoEntered = 1;
					if(SMTeamDeathMatch.PlayerIdAndTeamIndex.TryGetValue
						(playerID, out playersTeamIndexWhoEntered))
					{
						if(localTeamID != playersTeamIndexWhoEntered)
						{
							numberOfEnimeiesInRegion += 1;
						}
					}
				}
			}
		}
	}	
}
