//
//  SMPhotonGameVersion.cs
//  Jultar
//
//  Created by Sandeep Yadav Mattepu on 12/10/17.
//  Copyright © 2017 Mattepu. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;

namespace SandeepMattepu.Multiplayer
{
	/// <summary>
	/// This class assigns game version for photon servers.
	/// </summary>
	public class SMPhotonGameVersion : MonoBehaviour 
	{
		[SerializeField]
		private ObscuredString gameVersion = "1.0";

		private void Start () 
		{
			PhotonNetwork.gameVersion = gameVersion;
		}
	}
}
